using BioDomes.Domains;
using BioDomes.Domains.EquipmentsDom;
using BioDomes.Domains.SpeciesDom;

namespace BioDomes.Web.Services;

public class FileService
{
    private readonly IWebHostEnvironment _environment;
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly ISpeciesRepository _speciesRepository;

    public FileService(IWebHostEnvironment environment, IEquipmentRepository equipmentRepository, 
        ISpeciesRepository speciesRepository)
    {
        _environment = environment;
        _equipmentRepository = equipmentRepository;
        _speciesRepository = speciesRepository;
    }
    
    // dir equipments/
    public string? ComputeNewFileName(IFormFile? imageFile, string name, string directory)
    {
        if (imageFile == null) return null;
        
        var extension = Path.GetExtension(imageFile.FileName);
        var fileName = $"{name.ToKebabCase()}{extension}";
        return $"/images/{directory}/{fileName}";
    }

    // dir equipments/
    public async Task SaveImage(IFormFile file, string name, string? existingImagePath, string directory)
    {
        if (file.Length == 0) return;

        if (!string.IsNullOrEmpty(existingImagePath))
        {
            var existingFilePath = Path.Combine(_environment.WebRootPath, existingImagePath.TrimStart('/'));
            if (File.Exists(existingFilePath)) File.Delete(existingFilePath);
        }

        var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", directory);
        Directory.CreateDirectory(uploadsFolder);

        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{name.ToKebabCase()}{extension}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);
    }
    
    public void DeleteSpeciesImage(string slug)
    {
        var target = _speciesRepository.GetTargetSpecies(slug);

        if (target.FilePath is null) return;

        var existingFilePath = Path.Combine(_environment.WebRootPath, target.FilePath.TrimStart('/'));
        if (File.Exists(existingFilePath)) File.Delete(existingFilePath);
    }

    public void DeleteEquipImage(string slug)
    {
        var target = _equipmentRepository.GetTargetEquipment(slug);

        if (target.FilePath is null) return;

        var existingFilePath = Path.Combine(_environment.WebRootPath, target.FilePath.TrimStart('/'));
        if (File.Exists(existingFilePath)) File.Delete(existingFilePath);
    }
}