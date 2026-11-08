using BioDomes.Domains;
using BioDomes.Domains.Dto;
using BioDomes.Domains.EquipmentsDom;
using BioDomes.Infrastructures.Model;
using Microsoft.EntityFrameworkCore;
using Equipment = BioDomes.Domains.EquipmentsDom.Equipment;
using Material = BioDomes.Domains.EquipmentsDom.Material;

namespace BioDomes.Infrastructures;

public class EquipmentRepository : IEquipmentRepository
{
    private readonly BioDomesDbContext _context;

    public EquipmentRepository(BioDomesDbContext context)
    {
        _context = context;
    }
    
    public int Count()
    {
        return _context.Equipments.Count();
    }

    public IList<Equipment> GetAll()
    {
        return _context.Equipments.AsNoTracking().Include(e => e.EquipmentMaterials)
            .ThenInclude(em => em.Material).ToList().Select(ToDomain).ToList();
    }

    public IList<Equipment> GetAvailableAndOwn(string userId)
    {
        return _context.Equipments.AsNoTracking().Include(e => e.EquipmentMaterials)
            .ThenInclude(em => em.Material).Where(e => e.IsAvailable || e.UserId == userId).ToList().Select(ToDomain).ToList();
    }
    
    public Equipment GetTargetEquipment(string slug)
    {
        var target = _context.Equipments.AsNoTracking().
            Include(e => e.EquipmentMaterials)
            .ThenInclude(em => em.Material)
            .FirstOrDefault(s => s.Slug == slug);

        return ToDomain(target ?? throw new InvalidOperationException("Équipement inconnu ou données incomplètes."));
    }
    
    public Dictionary<Equipment, int> GetEquipmentForBiome(Biome biomeInfra)
    {
        var equipment = new Dictionary<Equipment, int>();

        foreach (var biomeEquip in biomeInfra.BiomeEquipments)
        {
            var equipInfra = _context.Equipments.AsNoTracking()
                .Include(e => e.EquipmentMaterials)
                .ThenInclude(em => em.Material)
                .FirstOrDefault(s => s.EquipmentId == biomeEquip.EquipmentId);
            if (equipInfra != null)
            {
                var equipDomain = ToDomain(equipInfra);
                equipment[equipDomain] = biomeEquip.Count;
            }
        }

        return equipment;
    }

    public void AddOne(Equipment equipment, string userId)
    {
        if(EquipExists(equipment))
            throw new InvalidOperationException($"L'équipement '{equipment.Name}' existe déjà.");

        
        _context.Equipments.Add(ToInfrastructure(equipment, userId));
        _context.SaveChanges();
    }
    
    public void UpdateOne(Equipment oldEquip, Equipment newEquip)
    {
        if (_context.Equipments
            .Any(e => e.Slug == newEquip.Name.ToKebabCase() 
                      && e.Slug != oldEquip.Name.ToKebabCase()))
            throw new InvalidOperationException($"L'équipement '{newEquip.Name}' existe déjà.");
        
        var equipInfra = _context.Equipments
            .Include(e => e.EquipmentMaterials)
            .ThenInclude(em => em.Material)
            .FirstOrDefault(e => e.Slug == oldEquip.Name.ToKebabCase());

        if (equipInfra is null) 
            throw new InvalidOperationException($"La référence existante n'a pas pu être trouvée.");
        
        equipInfra.Name = newEquip.Name;
        equipInfra.Slug = newEquip.Name.ToKebabCase();
        equipInfra.ImagePath = newEquip.FilePath;
        
        _context.EquipmentMaterials.RemoveRange(equipInfra.EquipmentMaterials);

        var newMaterials = new List<EquipmentMaterial>();

        // Fonction locale pour éviter duplication
        void AddMaterials(IEnumerable<Material> materials, MaterialFlowType flowType)
        {
            foreach (var mat in materials)
            {
                var infraMat = _context.Materials
                    .FirstOrDefault(m => m.Slug == mat.Name.ToKebabCase());

                if (infraMat == null)
                {
                    throw new InvalidOperationException("L'un des matériaux renseignés n'existe pas.");
                }

                newMaterials.Add(new EquipmentMaterial
                {
                    Equipment = equipInfra,
                    Material = infraMat,
                    FlowType = flowType
                });
            }
        }

        AddMaterials(newEquip.Consumed, MaterialFlowType.Consumed);
        AddMaterials(newEquip.Produced, MaterialFlowType.Produced);

        equipInfra.EquipmentMaterials = newMaterials;

        _context.SaveChanges();
    }

    public void RemoveOne(string slug)
    {
        var infra = _context.Equipments
            .FirstOrDefault(e => e.Slug == slug);
        
        if (infra is null)
            throw new InvalidOperationException("Équipement inconnu.");

        _context.Equipments.Remove(infra);
        _context.SaveChanges();
    }

    public void MakeAvailable(string slug)
    {
        var equipment = _context.Equipments
            .FirstOrDefault(e => e.Slug == slug);

        if (equipment is null)
            throw new InvalidOperationException("Équipement inconnu.");

        equipment.IsAvailable = true;
        _context.SaveChanges();
    }
    
    private Equipment ToDomain(Model.Equipment e)
    {
        var produced = new List<Material>();
        var consumed = new List<Material>();

        foreach (var consumedM in e.ConsumedMaterials) consumed.Add(new Material(consumedM.Material.Name));
        foreach (var producedM in e.ProducedMaterials) produced.Add(new Material(producedM.Material.Name));

        return new Equipment(e.Name, produced, consumed, e.ImagePath)
        {
            IsAvailable = e.IsAvailable,
            OwnerId = e.UserId
        };
    }

    private Model.Equipment ToInfrastructure(Equipment e, string userId)
    {
        var infraEquipment = new Model.Equipment
        {
            Name = e.Name,
            Slug = e.Name.ToKebabCase(),
            UserId = userId,
            IsAvailable = false,

            EquipmentMaterials = new List<EquipmentMaterial>(),
            ImagePath = e.FilePath
        };

        foreach (var material in e.Produced)
            infraEquipment.EquipmentMaterials.Add(new EquipmentMaterial
            {
                MaterialId = MaterialIdByName(material.Name),
                FlowType = MaterialFlowType.Produced
            });

        foreach (var material in e.Consumed)
            infraEquipment.EquipmentMaterials.Add(new EquipmentMaterial
            {
                MaterialId = MaterialIdByName(material.Name),
                FlowType = MaterialFlowType.Consumed
            });

        return infraEquipment;
    }
    
    public List<BiomeImpactDto> GetBiomesForEquipment(string slug)
    {
        var equipment = _context.Equipments
            .FirstOrDefault(s => s.Slug == slug);

        if (equipment == null)
            return new List<BiomeImpactDto>();
        
        var equipmentId = equipment.EquipmentId;

        return _context.BiomeEquipments
            .Where(be => be.EquipmentId == equipmentId)
            .Select(be => new BiomeImpactDto
            {
                BiomeName = be.Biome.Name,
                PresenceCount = be.Count
            })
            .ToList();
    }

    public IList<Material> GetAllMaterial()
    {
        return _context.Materials.AsNoTracking().ToList().Select(ToDomain).ToList();
    }

    public void AddOneMaterial(string name)
    {
        var created = new Model.Material { Name = name.Trim(),  Slug = name.ToKebabCase() };

        if (MaterialExists(created))
            throw new InvalidOperationException($"Le matériel '{created.Name}' existe déjà.");
        
        _context.Materials.Add(created);
        _context.SaveChanges();
    }
    
    private Material ToDomain(Model.Material m)
    {
        return new Material(m.Name);
    }

    private int MaterialIdByName(string name)
    {
        var existing = _context.Materials
            .FirstOrDefault(m => m.Slug == name.ToKebabCase());

        return existing!.MaterialId;
    }
    
    private bool EquipExists(Equipment eq)
    {
        return _context.Equipments.Any(e => e.Slug.Equals(eq.Name.ToKebabCase()));
    }
    
    private bool MaterialExists(Model.Material mat)
    {
        return _context.Materials.Any(m => m.Slug.Equals(mat.Name.ToKebabCase()));
    }
}