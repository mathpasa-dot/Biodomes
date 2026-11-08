using BioDomes.Infrastructures.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BioDomes.Infrastructures.Context;

public class BiomeSpeciesEntityTypeConfiguration : IEntityTypeConfiguration<BiomeSpecies>
{
    public void Configure(EntityTypeBuilder<BiomeSpecies> builder)
    {
        builder.HasKey(bs => new { bs.BiomeId, bs.SpeciesId });

        builder.Property(bs => bs.Count)
            .IsRequired()
            .HasDefaultValue(0);

        // Relation BiomeSpecies → Biome (N:1)
        builder.HasOne(bs => bs.Biome)
            .WithMany(b => b.BiomeSpecies)
            .HasForeignKey(bs => bs.BiomeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relation BiomeSpecies → Species (N:1)
        builder.HasOne(bs => bs.Species)
            .WithMany(s => s.BiomeSpecies)
            .HasForeignKey(bs => bs.SpeciesId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new BiomeSpecies { BiomeId = 1, SpeciesId = 3, Count = 120 }, // Hêtre
            new BiomeSpecies { BiomeId = 1, SpeciesId = 4, Count = 100 }, // Chêne
            new BiomeSpecies { BiomeId = 1, SpeciesId = 9, Count = 899000 }, // Pâturin
            new BiomeSpecies { BiomeId = 1, SpeciesId = 8, Count = 150 }, // Lièvre
            new BiomeSpecies { BiomeId = 1, SpeciesId = 10, Count = 11960 }, // Souris
            new BiomeSpecies { BiomeId = 1, SpeciesId = 6, Count = 474 }, // Araignée
            new BiomeSpecies { BiomeId = 1, SpeciesId = 7, Count = 1000 }, // Hirondelle
            new BiomeSpecies { BiomeId = 1, SpeciesId = 2, Count = 23 }, // Boa
            new BiomeSpecies { BiomeId = 1, SpeciesId = 1, Count = 2 }, // Lion
            new BiomeSpecies { BiomeId = 1, SpeciesId = 5, Count = 90000 }, // Mouche
            
            // 🌵 Désert (2)
            new BiomeSpecies { BiomeId = 2, SpeciesId = 15, Count = 300 }, // Cactus
            new BiomeSpecies { BiomeId = 2, SpeciesId = 13, Count = 120 }, // Gerboise
            new BiomeSpecies { BiomeId = 2, SpeciesId = 11, Count = 80 },  // Scorpion
            new BiomeSpecies { BiomeId = 2, SpeciesId = 14, Count = 25 },  // Serpent
            new BiomeSpecies { BiomeId = 2, SpeciesId = 12, Count = 10 },  // Fennec

            // 🌾 Campagne (3)
            new BiomeSpecies { BiomeId = 3, SpeciesId = 19, Count = 30000 }, // Blé
            new BiomeSpecies { BiomeId = 3, SpeciesId = 10, Count = 2400 },  // Souris
            new BiomeSpecies { BiomeId = 3, SpeciesId = 17, Count = 30000 },  // Ver de terre
            new BiomeSpecies { BiomeId = 3, SpeciesId = 18, Count = 30 },   // Poule
            new BiomeSpecies { BiomeId = 3, SpeciesId = 16, Count = 10 },   // Renard

            // 🌲 Forêt tempérée (4)
            new BiomeSpecies { BiomeId = 4, SpeciesId = 4, Count = 200 },  // Chêne
            new BiomeSpecies { BiomeId = 4, SpeciesId = 20, Count = 80 },  // Cerf
            new BiomeSpecies { BiomeId = 4, SpeciesId = 22, Count = 150 }, // Champignons
            new BiomeSpecies { BiomeId = 4, SpeciesId = 21, Count = 20 },  // Loup

            // 🌴 Forêt tropicale (5)
            new BiomeSpecies { BiomeId = 5, SpeciesId = 26, Count = 1334 }, // Banane
            new BiomeSpecies { BiomeId = 5, SpeciesId = 24, Count = 10 }, // Singe
            new BiomeSpecies { BiomeId = 5, SpeciesId = 5, Count = 100000 },  // Mouche
            new BiomeSpecies { BiomeId = 5, SpeciesId = 25, Count = 200 },  // Grenouille
            new BiomeSpecies { BiomeId = 5, SpeciesId = 23, Count = 1 },  // Jaguar

            // 🏔️ Montagne (6)
            new BiomeSpecies { BiomeId = 6, SpeciesId = 29, Count = 400 }, // Herbes alpines
            new BiomeSpecies { BiomeId = 6, SpeciesId = 28, Count = 60 },  // Chamois
            new BiomeSpecies { BiomeId = 6, SpeciesId = 27, Count = 12 } // Aigle
                
            // ❄️ Arctique (7)
            /*new BiomeSpecies { BiomeId = 7, SpeciesId = 32, Count = 200 }, // Poisson arctique
            new BiomeSpecies { BiomeId = 7, SpeciesId = 31, Count = 40 },  // Phoque
            new BiomeSpecies { BiomeId = 7, SpeciesId = 30, Count = 8 },   // Ours polaire

            // 🌊 Océan (8)
            new BiomeSpecies { BiomeId = 8, SpeciesId = 35, Count = 5000 }, // Plancton
            new BiomeSpecies { BiomeId = 8, SpeciesId = 36, Count = 300 },  // Crabe
            new BiomeSpecies { BiomeId = 8, SpeciesId = 34, Count = 80 },   // Thon
            new BiomeSpecies { BiomeId = 8, SpeciesId = 33, Count = 10 },   // Requin

            // 🐟 Eau douce (9)
            new BiomeSpecies { BiomeId = 9, SpeciesId = 35, Count = 2000 }, // Algues / plancton
            new BiomeSpecies { BiomeId = 9, SpeciesId = 38, Count = 120 },  // Carpe
            new BiomeSpecies { BiomeId = 9, SpeciesId = 37, Count = 25 },   // Brochet

            // 🪨 Savane (10)
            new BiomeSpecies { BiomeId = 10, SpeciesId = 41, Count = 250 }, // Acacia
            new BiomeSpecies { BiomeId = 10, SpeciesId = 39, Count = 90 },  // Zèbre
            new BiomeSpecies { BiomeId = 10, SpeciesId = 1, Count = 25 },   // Lion
            new BiomeSpecies { BiomeId = 10, SpeciesId = 40, Count = 30 },  // Hyène

            // 🍄 Marais (11)
            new BiomeSpecies { BiomeId = 11, SpeciesId = 44, Count = 300 }, // Roseau
            new BiomeSpecies { BiomeId = 11, SpeciesId = 43, Count = 200 }, // Escargot
            new BiomeSpecies { BiomeId = 11, SpeciesId = 42, Count = 60 },  // Canard
            new BiomeSpecies { BiomeId = 11, SpeciesId = 17, Count = 150 }  // Détritivores
            */
        );
    }
}