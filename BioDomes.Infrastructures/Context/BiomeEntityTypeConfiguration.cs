using BioDomes.Domains;
using BioDomes.Infrastructures.Authentification;
using BioDomes.Infrastructures.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BioDomes.Infrastructures.Context;

public class BiomeEntityTypeConfiguration : IEntityTypeConfiguration<Biome>
{
    public void Configure(EntityTypeBuilder<Biome> builder)
    {
        builder.HasKey(b => b.BiomeId);

        builder.Property(b => b.Name).IsRequired().HasMaxLength(100);
        builder.Property(b => b.Temperature).IsRequired().HasColumnType("TEXT").HasPrecision(5, 2);
        builder.Property(b => b.Humidity).IsRequired().HasColumnType("TEXT").HasPrecision(5, 2);

        // Relation Biome → User (N:1)
        builder.HasOne(b => b.User)
            .WithMany(u => u.Biomes)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relation Biome → BiomeSpecies (1:N)
        builder.HasMany(b => b.BiomeSpecies)
            .WithOne(bs => bs.Biome)
            .HasForeignKey(bs => bs.BiomeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new Biome
            {
                BiomeId = 1,
                Slug = "Experimental Ecosystem".ToKebabCase(),
                Name = "Experimental Ecosystem",
                Temperature = 20.0m,
                Humidity = 12.0m,
                UserId = BioDomeUserConfiguration.SystemId
            },
            new Biome { BiomeId = 2, Slug = "Désert".ToKebabCase(), Name = "Désert", Temperature = 38.0m, Humidity = 5.0m, UserId = BioDomeUserConfiguration.SystemId },
            new Biome { BiomeId = 3, Slug = "Campagne".ToKebabCase(), Name = "Campagne", Temperature = 18.0m, Humidity = 10.0m, UserId = BioDomeUserConfiguration.SystemId },
            new Biome { BiomeId = 4, Slug = "Forêt tempérée".ToKebabCase(), Name = "Forêt tempérée", Temperature = 12.0m, Humidity = 9.0m, UserId = BioDomeUserConfiguration.SystemId },
            new Biome { BiomeId = 5, Slug = "Forêt tropicale".ToKebabCase(), Name = "Forêt tropicale", Temperature = 27.5m, Humidity = 22.0m, UserId = BioDomeUserConfiguration.SystemId },
            new Biome { BiomeId = 6, Slug = "Montagne".ToKebabCase(), Name = "Montagne", Temperature = 5.0m, Humidity = 5.0m, UserId = BioDomeUserConfiguration.SystemId }
            
            /*new Biome { BiomeId = 7, Slug = "Arctique".ToKebabCase(), Name = "Arctique", Temperature = -15.0m, Humidity = 1.0m, UserId = BioDomeUserConfiguration.SystemId },
            new Biome { BiomeId = 8, Slug = "Océan".ToKebabCase(), Name = "Océan", Temperature = 10.0m, Humidity = 9.0m, UserId = BioDomeUserConfiguration.SystemId },
            new Biome { BiomeId = 9, Slug = "Eau douce".ToKebabCase(), Name = "Eau douce", Temperature = 12.0m, Humidity = 8.0m, UserId = BioDomeUserConfiguration.SystemId },
            new Biome { BiomeId = 10, Slug = "Savane".ToKebabCase(), Name = "Savane", Temperature = 30.0m, Humidity = 12.0m, UserId = BioDomeUserConfiguration.SystemId },
            new Biome { BiomeId = 11, Slug = "Marais".ToKebabCase(), Name = "Marais", Temperature = 20.0m, Humidity = 17.0m, UserId = BioDomeUserConfiguration.SystemId }*/
        );
    }
}