using BioDomes.Infrastructures.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BioDomes.Infrastructures.Context;

public class PredationEntityTypeConfiguration : IEntityTypeConfiguration<Predation>
{
    public void Configure(EntityTypeBuilder<Predation> builder)
    {
        builder.HasKey(p => new { p.PredatorId, p.PreyId, p.BiomeId });

        // Relation Predation → Species (prédateur)
        builder.HasOne(p => p.Predator)
            .WithMany(s => s.AsPredator)
            .HasForeignKey(p => p.PredatorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relation Predation → Species (proie)
        builder.HasOne(p => p.Prey)
            .WithMany(s => s.AsPrey)
            .HasForeignKey(p => p.PreyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relation Predation → Biome
        builder.HasOne(p => p.Biome)
            .WithMany(b => b.Predations)
            .HasForeignKey(p => p.BiomeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            // Biome 1 — Experimental Ecosystem
            new Predation { PredatorId = 1, PreyId = 2,  BiomeId = 1 }, // Lion → Boa
            new Predation { PredatorId = 6, PreyId = 5,  BiomeId = 1 }, // Araignée → Mouche
            new Predation { PredatorId = 7, PreyId = 5, BiomeId = 1 }, // Hirondelle → Mouche
            new Predation { PredatorId = 7, PreyId = 6,  BiomeId = 1 }, // Hirondelle → Araignée
            new Predation { PredatorId = 1, PreyId = 8,  BiomeId = 1 }, // Lion → Lièvre
            new Predation { PredatorId = 2, PreyId = 8,  BiomeId = 1 }, // Boa → Lièvre
            new Predation { PredatorId = 8, PreyId = 9,  BiomeId = 1 }, // Lièvre → Paturin
            new Predation { PredatorId = 10, PreyId = 9,  BiomeId = 1 }, // Souris → Paturin
            new Predation { PredatorId = 2, PreyId = 10,  BiomeId = 1 }, // Boa → Souris

            // Biome 2 — Désert
            new Predation { PredatorId = 12, PreyId = 11, BiomeId = 2 }, // Fennec → Scorpion
            new Predation { PredatorId = 12, PreyId = 13, BiomeId = 2 }, // Fennec → Gerboise
            new Predation { PredatorId = 14, PreyId = 13, BiomeId = 2 }, // Serpent → Gerboise

            // Biome 3 — Campagne
            new Predation { PredatorId = 16, PreyId = 10, BiomeId = 3 }, // Renard → Souris
            new Predation { PredatorId = 16, PreyId = 18, BiomeId = 3 }, // Renard → Poule
            new Predation { PredatorId = 18, PreyId = 17, BiomeId = 3 }, // Poule → Ver de terre
            new Predation { PredatorId = 10, PreyId = 19, BiomeId = 3 }, // Souris → Blé
            new Predation { PredatorId = 17, PreyId = 19, BiomeId = 3 }, // Ver de terre → Blé
            new Predation { PredatorId = 18, PreyId = 19, BiomeId = 3 }, // Poule → Blé
            
            // Biome 4 — Forêt tempérée
            new Predation { PredatorId = 21, PreyId = 20, BiomeId = 4 }, // Loup → Cerf

            // Biome 5 — Forêt tropicale
            new Predation { PredatorId = 23, PreyId = 24, BiomeId = 5 }, // Jaguar → Singe
            new Predation { PredatorId = 24, PreyId = 25, BiomeId = 5 }, // Singe → Grenouille
            new Predation { PredatorId = 25, PreyId = 5,  BiomeId = 5 }, // Grenouille → Mouche
            new Predation { PredatorId = 25, PreyId = 26, BiomeId = 5 }, // Grenouille → Banane
            new Predation { PredatorId = 24, PreyId = 26, BiomeId = 5 }, // Singe → Banane
            
            // Biome 6 — Montagne
            new Predation { PredatorId = 27, PreyId = 28, BiomeId = 6 }  // Aigle → Chamois
        );
    }
}