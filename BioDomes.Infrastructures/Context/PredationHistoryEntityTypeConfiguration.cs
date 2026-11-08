using BioDomes.Infrastructures.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BioDomes.Infrastructures.Context;

public class PredationHistoryEntityTypeConfiguration : IEntityTypeConfiguration<PredationHistory>
{
    public void Configure(EntityTypeBuilder<PredationHistory> builder)
    {
        builder.HasKey(h => h.Id);

        builder.Property(h => h.Action)
            .IsRequired();

        builder.Property(h => h.Timestamp)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // 🔗 Biome
        builder.HasOne(h => h.Biome)
            .WithMany() // tu peux ajouter une collection si tu veux
            .HasForeignKey(h => h.BiomeId)
            .OnDelete(DeleteBehavior.Cascade);

        // 🔗 Predator
        builder.HasOne(h => h.Predator)
            .WithMany()
            .HasForeignKey(h => h.PredatorId)
            .OnDelete(DeleteBehavior.Restrict);

        // 🔗 Prey
        builder.HasOne(h => h.Prey)
            .WithMany()
            .HasForeignKey(h => h.PreyId)
            .OnDelete(DeleteBehavior.Restrict);

        // ⚡ Index critique
        builder.HasIndex(h => new { h.BiomeId, h.Timestamp });

        // ⚡ Optionnel mais très utile
        builder.HasIndex(h => new { h.BiomeId, h.PredatorId, h.PreyId });
    }
}