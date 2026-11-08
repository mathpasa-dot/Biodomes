using BioDomes.Infrastructures.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BioDomes.Infrastructures.Context;

public class EquipmentMaterialEntityTypeConfiguration : IEntityTypeConfiguration<EquipmentMaterial>
{
    public void Configure(EntityTypeBuilder<EquipmentMaterial> builder)
    {
        builder.HasKey(em => new { em.EquipmentId, em.MaterialId, em.FlowType });

        // Conversion enum → string en DB
        builder.Property(em => em.FlowType)
            .IsRequired()
            .HasConversion<string>();

        // Relation EquipmentMaterial → Equipment (N:1)
        builder.HasOne(em => em.Equipment)
            .WithMany(e => e.EquipmentMaterials)
            .HasForeignKey(em => em.EquipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relation EquipmentMaterial → Material (N:1)
        builder.HasOne(em => em.Material)
            .WithMany(m => m.EquipmentMaterials)
            .HasForeignKey(em => em.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new { EquipmentId = 1, MaterialId = 5, FlowType = MaterialFlowType.Consumed },
            new { EquipmentId = 1, MaterialId = 1, FlowType = MaterialFlowType.Produced },
            new { EquipmentId = 1, MaterialId = 6, FlowType = MaterialFlowType.Produced },
            new { EquipmentId = 2, MaterialId = 5, FlowType = MaterialFlowType.Consumed },
            new { EquipmentId = 2, MaterialId = 2, FlowType = MaterialFlowType.Consumed },
            new { EquipmentId = 2, MaterialId = 2, FlowType = MaterialFlowType.Produced },
            new { EquipmentId = 2, MaterialId = 6, FlowType = MaterialFlowType.Produced },
            new { EquipmentId = 3, MaterialId = 5, FlowType = MaterialFlowType.Consumed },
            new { EquipmentId = 3, MaterialId = 3, FlowType = MaterialFlowType.Produced },
            new { EquipmentId = 4, MaterialId = 5, FlowType = MaterialFlowType.Consumed },
            new { EquipmentId = 4, MaterialId = 4, FlowType = MaterialFlowType.Produced });
    }
}