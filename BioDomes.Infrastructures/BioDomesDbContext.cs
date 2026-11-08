using BioDomes.Infrastructures.Authentification;
using BioDomes.Infrastructures.Context;
using BioDomes.Infrastructures.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BioDomes.Infrastructures;

public class BioDomesDbContext : IdentityDbContext<User>
{
    public BioDomesDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<Biome> Biomes { get; set; }
    public DbSet<BiomeSpecies> BiomeSpecies { get; set; }
    
    public DbSet<BiomeEquipment> BiomeEquipments { get; set; }
    
    public DbSet<Equipment> Equipments { get; set; }
    public DbSet<EquipmentMaterial> EquipmentMaterials { get; set; }
    public DbSet<Material> Materials { get; set; }
    public DbSet<Species> Species { get; set; }
    public DbSet<SpeciesRegime> SpeciesRegimes { get; set; }
    public DbSet<SpeciesType> SpeciesType { get; set; }

    public DbSet<Predation> Predations { get; set; }
    
    public DbSet<BiomeSpeciesHistory> BiomeHistory { get; set; }
    
    public DbSet<PredationHistory> PredationHistory { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new MaterialEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new SpeciesRegimeEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new SpeciesTypeEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PredationEntityTypeConfiguration());

        modelBuilder.ApplyConfiguration(new SpeciesEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new BiomeEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new EquipmentEntityTypeConfiguration());

        modelBuilder.ApplyConfiguration(new BiomeSpeciesEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new EquipmentMaterialEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new BiomeEquipmentTypeConfiguration());
        modelBuilder.ApplyConfiguration(new BiomeSpeciesHistoryEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PredationHistoryEntityTypeConfiguration());

        modelBuilder.ApplyConfiguration(new IdentityRoleConfiguration());
        modelBuilder.ApplyConfiguration(new BioDomeUserConfiguration());
        modelBuilder.Entity<IdentityUserRole<string>>().HasData
        (
            new IdentityUserRole<string>
                { RoleId = IdentityRoleConfiguration.EmployeeId, UserId = BioDomeUserConfiguration.SystemId }
        );
    }
}