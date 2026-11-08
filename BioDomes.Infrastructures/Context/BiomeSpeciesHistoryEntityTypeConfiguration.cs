using BioDomes.Infrastructures.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BioDomes.Infrastructures.Context;

public class BiomeSpeciesHistoryEntityTypeConfiguration: IEntityTypeConfiguration<BiomeSpeciesHistory>
{
    public void Configure(EntityTypeBuilder<BiomeSpeciesHistory> builder)
    {
        builder.HasKey(h => h.Id);

        builder.Property(h => h.Delta)
            .IsRequired();

        builder.Property(h => h.NewCount)
            .IsRequired();

        builder.Property(h => h.Timestamp)
            .IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");

        // 🔗 Relation → Biome
        builder.HasOne(h => h.Biome)
            .WithMany(b => b.BiomeSpeciesHistory)
            .HasForeignKey(h => h.BiomeId)
            .OnDelete(DeleteBehavior.Cascade);

        // 🔗 Relation → Species
        builder.HasOne(h => h.Species)
            .WithMany(s => s.BiomeSpeciesHistory)
            .HasForeignKey(h => h.SpeciesId)
            .OnDelete(DeleteBehavior.Cascade);

        // ⚡ Index critique pour les perfs
        builder.HasIndex(h => new { h.BiomeId, h.SpeciesId, h.Timestamp });
    }
}