using BioDomes.Infrastructures.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BioDomes.Infrastructures.Context;

public class SpeciesTypeEntityTypeConfiguration : IEntityTypeConfiguration<SpeciesType>
{
    public void Configure(EntityTypeBuilder<SpeciesType> builder)
    {
        builder.HasKey(c => c.TypeId);

        builder.Property(c => c.Label).IsRequired();

        builder.HasData(
            new SpeciesType { TypeId = 1, Label = "Mammifères" },
            new SpeciesType { TypeId = 2, Label = "Oiseaux" },
            new SpeciesType { TypeId = 3, Label = "Poissons" },
            new SpeciesType { TypeId = 4, Label = "Reptiles" },
            new SpeciesType { TypeId = 5, Label = "Amphibiens" },
            new SpeciesType { TypeId = 6, Label = "Crustacés" },
            new SpeciesType { TypeId = 7, Label = "Arachnides" },
            new SpeciesType { TypeId = 8, Label = "Insectes" },
            new SpeciesType { TypeId = 9, Label = "Myriapodes" },
            new SpeciesType { TypeId = 10, Label = "Mollusques" },
            new SpeciesType { TypeId = 11, Label = "Echinodermes" },
            new SpeciesType { TypeId = 12, Label = "Anélides" },
            new SpeciesType { TypeId = 13, Label = "Cnidaires" },
            new SpeciesType { TypeId = 14, Label = "Plathelminthes" },
            new SpeciesType { TypeId = 15, Label = "Porifères" },
            new SpeciesType { TypeId = 16, Label = "Plantes" },
            new SpeciesType { TypeId = 17, Label = "Mousses" },
            new SpeciesType { TypeId = 18, Label = "Algues" },
            new SpeciesType { TypeId = 19, Label = "Lichen" },
            new SpeciesType { TypeId = 20, Label = "Champignons" });
    }
}