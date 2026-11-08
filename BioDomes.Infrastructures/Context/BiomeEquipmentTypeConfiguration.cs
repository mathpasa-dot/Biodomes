using BioDomes.Infrastructures.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BioDomes.Infrastructures.Context;

public class BiomeEquipmentTypeConfiguration : IEntityTypeConfiguration<BiomeEquipment>
{
    public void Configure(EntityTypeBuilder<BiomeEquipment> builder)
    {
        builder.HasKey(bs => new { bs.BiomeId, bs.EquipmentId });

        builder.Property(bs => bs.Count)
            .IsRequired()
            .HasDefaultValue(0);

        // Relation BiomeSpecies → Biome (N:1)
        builder.HasOne(bs => bs.Biome)
            .WithMany(b => b.BiomeEquipments)
            .HasForeignKey(bs => bs.BiomeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relation BiomeSpecies → Species (N:1)
        builder.HasOne(bs => bs.Equipment)
            .WithMany(s => s.BiomeEquipments)
            .HasForeignKey(bs => bs.EquipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            // Tropical Rainforest (1) → Frog, Oak Tree
            new BiomeEquipment { BiomeId = 1, EquipmentId = 1, Count = 10 }, // Lamp

            // Boreal Forest (3) → Oak Tree, Deer, Wolf, Bear
            new BiomeEquipment { BiomeId = 3, EquipmentId = 1, Count = 10 }, // Lamp

            // Temperate Grassland (4) → Deer, Wolf, Bear, Quercus acerifolia
            new BiomeEquipment { BiomeId = 4, EquipmentId = 1, Count = 10 }, // Lamp

            // Arctic Tundra (5) → Bear uniquement
            new BiomeEquipment { BiomeId = 5, EquipmentId = 1, Count = 10 }, // Lamp

            // Mangrove Swamp (6) → Frog, Oak Tree
            new BiomeEquipment { BiomeId = 6, EquipmentId = 1, Count = 10 } // Lamp

            // Mediterranean Scrub (7) → Quercus acerifolia, Deer, Wolf
           // new BiomeEquipment { BiomeId = 7, EquipmentId = 1, Count = 10 } // Lamp
        );
    }
}