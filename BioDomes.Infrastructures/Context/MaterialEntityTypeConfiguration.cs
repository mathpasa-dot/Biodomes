using BioDomes.Domains;
using BioDomes.Infrastructures.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BioDomes.Infrastructures.Context;

public class MaterialEntityTypeConfiguration : IEntityTypeConfiguration<Material>
{
    public void Configure(EntityTypeBuilder<Material> builder)
    {
        builder.HasKey(e => e.MaterialId);

        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);

        // Relation Material → EquipmentMaterial (1:N)
        builder.HasMany(m => m.EquipmentMaterials)
            .WithOne(em => em.Material)
            .HasForeignKey(em => em.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new { MaterialId = 1, Slug = "Lumière".ToKebabCase(), Name = "Lumière" },
            new { MaterialId = 2, Slug = "Eau".ToKebabCase(), Name = "Eau" },
            new { MaterialId = 3, Slug = "Azote".ToKebabCase(), Name = "Azote" },
            new { MaterialId = 4, Slug = "Hydrogène".ToKebabCase(), Name = "Hydrogène" },
            new { MaterialId = 5, Slug = "Électricité".ToKebabCase(), Name = "Électricité" },
            new { MaterialId = 6, Slug = "Chaleur".ToKebabCase(), Name = "Chaleur" });
    }
}