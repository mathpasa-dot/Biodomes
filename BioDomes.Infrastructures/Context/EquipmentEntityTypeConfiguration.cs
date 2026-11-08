using BioDomes.Domains;
using BioDomes.Infrastructures.Authentification;
using BioDomes.Infrastructures.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BioDomes.Infrastructures.Context;

public class EquipmentEntityTypeConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.HasKey(e => e.EquipmentId);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Car propriétés calculées
        builder.Ignore(e => e.ConsumedMaterials);
        builder.Ignore(e => e.ProducedMaterials);

        // Relation Equipment → User (N:1)
        builder.HasOne(e => e.User)
            .WithMany(u => u.Equipments)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relation Equipment → EquipmentMaterial (1:N)
        builder.HasMany(e => e.EquipmentMaterials)
            .WithOne(em => em.Equipment)
            .HasForeignKey(em => em.EquipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new { EquipmentId = 1, Slug = "Lampe".ToKebabCase(), Name = "Lampe", UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true, ImagePath = "/images/equipments/lampe.jpg" },
            new { EquipmentId = 2, Slug = "Pompe".ToKebabCase(), Name = "Pompe", UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true, ImagePath = "/images/equipments/pompe.jpg" },
            new
            {
                EquipmentId = 3, Slug = "Générateur azote".ToKebabCase(), Name = "Générateur azote", UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true, ImagePath = "/images/equipments/generateur-azote.jpg"
            },
            new
            {
                EquipmentId = 4, Slug = "Générateur hydrogène".ToKebabCase(), Name = "Générateur hydrogène", UserId = BioDomeUserConfiguration.SystemId,
                IsAvailable = true, ImagePath = "/images/equipments/generateur-hydrogene.jpg"
            });
    }
}