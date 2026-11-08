using BioDomes.Domains;
using BioDomes.Infrastructures.Authentification;
using BioDomes.Infrastructures.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BioDomes.Infrastructures.Context;

public class SpeciesEntityTypeConfiguration : IEntityTypeConfiguration<Species>
{
    public void Configure(EntityTypeBuilder<Species> builder)
    {
        builder.HasKey(g => g.SpeciesId);

        builder.Property(g => g.Name).IsRequired().HasMaxLength(100);
        builder.Property(g => g.Weight).IsRequired();
        builder.Property(g => g.Height).IsRequired();

        // Relation Species → SpeciesRegime (N:1)
        builder.HasOne(s => s.Regime)
            .WithMany(r => r.Species)
            .HasForeignKey(s => s.RegimeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relation Species → SpeciesType (N:1)
        builder.HasOne(s => s.Type)
            .WithMany(t => t.Species)
            .HasForeignKey(s => s.TypeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relation Species → User (N:1)
        builder.HasOne(s => s.User)
            .WithMany(u => u.Species)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relation Species → BiomeSpecies (1:N)
        builder.HasMany(s => s.BiomeSpecies)
            .WithOne(bs => bs.Species)
            .HasForeignKey(bs => bs.SpeciesId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            // OBLIGATOIRE
            new Species
            {
                SpeciesId = 1, RegimeId = 3, TypeId = 1, UserId = BioDomeUserConfiguration.SystemId, Slug = "Lion".ToKebabCase(), Name = "Lion",
                Weight = 150.0, Height = 180, IsAvailable = true, ImagePath = "/images/species/lion.jpg"
            },
            new Species
            {
                SpeciesId = 2, RegimeId = 3, TypeId = 4, UserId = BioDomeUserConfiguration.SystemId, Slug = "Boa".ToKebabCase(), Name = "Boa",
                Weight = 13.0, Height = 225, IsAvailable = true, ImagePath = "/images/species/boa.png"
            },
            new Species
            {
                SpeciesId = 3, RegimeId = 1, TypeId = 16, UserId = BioDomeUserConfiguration.SystemId, Slug = "Hetre".ToKebabCase(), Name = "Hetre",
                Weight = 830.0, Height = 600, IsAvailable = true, ImagePath = "/images/species/hetre.jpg"
            },
            new Species
            {
                SpeciesId = 4, RegimeId = 1, TypeId = 16, UserId = BioDomeUserConfiguration.SystemId, Slug = "Chene".ToKebabCase(), Name = "Chene",
                Weight = 2100.0, Height = 2500, IsAvailable = true, ImagePath = "/images/species/chene.jpg"
            },
            new Species
            {
                SpeciesId = 5, RegimeId = 5, TypeId = 8, UserId = BioDomeUserConfiguration.SystemId, Slug = "Mouche".ToKebabCase(), Name = "Mouche",
                Weight = 0.001, Height = 1, IsAvailable = true, ImagePath = "/images/species/mouche.jpg"
            },
            new Species
            {
                SpeciesId = 6, RegimeId = 3, TypeId = 7, UserId = BioDomeUserConfiguration.SystemId,
                Slug = "Araignée".ToKebabCase(), Name = "Araignée", Weight = 0.095, Height = 4, IsAvailable = true, ImagePath = "/images/species/araignee.jpg"
            },
            new Species
            {
                SpeciesId = 7, RegimeId = 3, TypeId = 2, UserId = BioDomeUserConfiguration.SystemId,
                Slug = "Hirondelle".ToKebabCase(), Name = "Hirondelle", Weight = 0.045, Height = 18, IsAvailable = true, ImagePath = "/images/species/hirondelle.jpg"
            },
            new Species
            {
                SpeciesId = 8, RegimeId = 2, TypeId = 1, UserId = BioDomeUserConfiguration.SystemId,
                Slug = "Lièvre".ToKebabCase(), Name = "Lièvre", Weight = 4.0, Height = 50, IsAvailable = true, ImagePath = "/images/species/lievre.jpg"
            },new Species
            {
                SpeciesId = 9, RegimeId = 1, TypeId = 16, UserId = BioDomeUserConfiguration.SystemId,
                Slug = "Pâturin des prés".ToKebabCase(), Name = "Pâturin des prés", Weight = 0.001, Height = 23, IsAvailable = true, ImagePath = "/images/species/paturin-des-pres.png"
            },new Species
            {
                SpeciesId = 10, RegimeId = 4, TypeId = 1, UserId = BioDomeUserConfiguration.SystemId,
                Slug = "Souris".ToKebabCase(), Name = "Souris", Weight = 0.025, Height = 9, IsAvailable = true, ImagePath = "/images/species/souris.jpg"
            },
            
            new Species { SpeciesId = 11, Slug = "Scorpion".ToKebabCase(), Name = "Scorpion", TypeId = 7, RegimeId = 3, Weight = 0.02, Height = 5, UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true, ImagePath = "/images/species/scorpion.jpg" },
            new Species { SpeciesId = 12, Slug = "Fennec".ToKebabCase(), Name = "Fennec", TypeId = 1, RegimeId = 4, Weight = 1.5, Height = 40, UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true, ImagePath = "/images/species/fennec.jpg" },
            new Species { SpeciesId = 13, Slug = "Gerboise".ToKebabCase(), Name = "Gerboise", TypeId = 1, RegimeId = 2, Weight = 0.1, Height = 10, UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true, ImagePath = "/images/species/gerboise.jpg" },
            new Species { SpeciesId = 14, Slug = "Serpent à sonnette".ToKebabCase(), Name = "Serpent à sonnette", TypeId = 4, RegimeId = 3, Weight = 2.0, Height = 120, UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true, ImagePath = "/images/species/serpent-a-sonnette.jpg" },
            new Species { SpeciesId = 15, Slug = "Cactus".ToKebabCase(), Name = "Cactus", TypeId = 16, RegimeId = 1, Weight = 500.0, Height = 200, UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true, ImagePath = "/images/species/cactus.jpg" },

            new Species { SpeciesId = 16, Slug = "Renard".ToKebabCase(), Name = "Renard", TypeId = 1, RegimeId = 3, Weight = 6.0, Height = 40, UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true, ImagePath = "/images/species/renard.jpg" },
            new Species { SpeciesId = 17, Slug = "Ver de terre".ToKebabCase(), Name = "Ver de terre", TypeId = 12, RegimeId = 7, Weight = 0.002, Height = 10, UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true, ImagePath = "/images/species/ver-de-terre.jpg" },
            new Species { SpeciesId = 18, Slug = "Poule".ToKebabCase(), Name = "Poule", TypeId = 2, RegimeId = 4, Weight = 2.0, Height = 45, UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true, ImagePath = "/images/species/poule.jpg" },
            new Species { SpeciesId = 19, Slug = "Blé".ToKebabCase(), Name = "Blé", TypeId = 16, RegimeId = 1, Weight = 0.002, Height = 100, UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true, ImagePath = "/images/species/ble.jpg" },

            new Species { SpeciesId = 20, Slug = "Cerf".ToKebabCase(), Name = "Cerf", TypeId = 1, RegimeId = 2, Weight = 150.0, Height = 150, UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true, ImagePath = "/images/species/cerf.jpg" },
            new Species { SpeciesId = 21, Slug = "Loup".ToKebabCase(), Name = "Loup", TypeId = 1, RegimeId = 3, Weight = 40.0, Height = 80, UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true, ImagePath = "/images/species/loup.jpg" },
            new Species { SpeciesId = 22, Slug = "Champignon forestier".ToKebabCase(), Name = "Champignon forestier", TypeId = 20, RegimeId = 7, Weight = 0.5, Height = 10, UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true, ImagePath = "/images/species/champignon-forestier.jpg" },

            new Species { SpeciesId = 23, Slug = "Jaguar".ToKebabCase(), Name = "Jaguar", TypeId = 1, RegimeId = 3, Weight = 100.0, Height = 70, UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true, ImagePath = "/images/species/jaguar.jpg" },
            new Species { SpeciesId = 24, Slug = "Singe".ToKebabCase(), Name = "Singe", TypeId = 1, RegimeId = 4, Weight = 10.0, Height = 60, UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true, ImagePath = "/images/species/singe.jpg" },
            new Species { SpeciesId = 25, Slug = "Grenouille".ToKebabCase(), Name = "Grenouille", TypeId = 5, RegimeId = 3, Weight = 0.5, Height = 10, UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true, ImagePath = "/images/species/grenouille.jpg" },
            new Species { SpeciesId = 26, Slug = "Banane".ToKebabCase(), Name = "Banane", TypeId = 16, RegimeId = 1, Weight = 0.15, Height = 20, UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true, ImagePath = "/images/species/banane.jpg" },

            new Species { SpeciesId = 27, Slug = "Aigle".ToKebabCase(), Name = "Aigle", TypeId = 2, RegimeId = 3, Weight = 5.0, Height = 70, UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true, ImagePath = "/images/species/aigle.jpg" },
            new Species { SpeciesId = 28, Slug = "Chamois".ToKebabCase(), Name = "Chamois", TypeId = 1, RegimeId = 2, Weight = 50.0, Height = 90, UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true, ImagePath = "/images/species/chamois.jpg" },
            new Species { SpeciesId = 29, Slug = "Herbes alpines".ToKebabCase(), Name = "Herbes alpines", TypeId = 16, RegimeId = 1, Weight = 0.5, Height = 30, UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true, ImagePath = "/images/species/herbes-alpines.jpg" }

            /*new Species { SpeciesId = 30, Slug = "Ours polaire".ToKebabCase(), Name = "Ours polaire", TypeId = 1, RegimeId = 3, Weight = 450.0, Height = 160, UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true },
            new Species { SpeciesId = 31, Slug = "Phoque".ToKebabCase(), Name = "Phoque", TypeId = 1, RegimeId = 3, Weight = 80.0, Height = 120, UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true },
            new Species { SpeciesId = 32, Slug = "Poisson arctique".ToKebabCase(), Name = "Poisson arctique", TypeId = 3, RegimeId = 3, Weight = 2.0, Height = 50, UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true },

            new Species { SpeciesId = 33, Slug = "Requin".ToKebabCase(), Name = "Requin", TypeId = 3, RegimeId = 3, Weight = 500.0, Height = 400, UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true },
            new Species { SpeciesId = 34, Slug = "Thon".ToKebabCase(), Name = "Thon", TypeId = 3, RegimeId = 3, Weight = 200.0, Height = 250, UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true },
            new Species { SpeciesId = 35, Slug = "Plancton".ToKebabCase(), Name = "Plancton", TypeId = 18, RegimeId = 7, Weight = 0.0001, Height = 1, UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true },
            new Species { SpeciesId = 36, Slug = "Crabe".ToKebabCase(), Name = "Crabe", TypeId = 6, RegimeId = 4, Weight = 1.0, Height = 20, UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true },

            new Species { SpeciesId = 37, Slug = "Brochet".ToKebabCase(), Name = "Brochet", TypeId = 3, RegimeId = 3, Weight = 10.0, Height = 100, UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true },
            new Species { SpeciesId = 38, Slug = "Carpe".ToKebabCase(), Name = "Carpe", TypeId = 3, RegimeId = 4, Weight = 5.0, Height = 80, UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true },

            new Species { SpeciesId = 39, Slug = "Zèbre".ToKebabCase(), Name = "Zèbre", TypeId = 1, RegimeId = 2, Weight = 300.0, Height = 140, UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true },
            new Species { SpeciesId = 40, Slug = "Hyène".ToKebabCase(), Name = "Hyène", TypeId = 1, RegimeId = 3, Weight = 60.0, Height = 80, UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true },
            new Species { SpeciesId = 41, Slug = "Acacia".ToKebabCase(), Name = "Acacia", TypeId = 16, RegimeId = 1, Weight = 1000.0, Height = 500, UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true },

            new Species { SpeciesId = 42, Slug = "Canard".ToKebabCase(), Name = "Canard", TypeId = 2, RegimeId = 4, Weight = 2.0, Height = 50, UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true },
            new Species { SpeciesId = 43, Slug = "Escargot".ToKebabCase(), Name = "Escargot", TypeId = 10, RegimeId = 2, Weight = 0.05, Height = 5, UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true },
            new Species { SpeciesId = 44, Slug = "Roseau".ToKebabCase(), Name = "Roseau", TypeId = 16, RegimeId = 1, Weight = 1.0, Height = 150, UserId = BioDomeUserConfiguration.SystemId, IsAvailable = true }*/
            );
    }
}