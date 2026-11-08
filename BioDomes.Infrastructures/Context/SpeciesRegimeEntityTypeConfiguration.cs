using BioDomes.Infrastructures.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BioDomes.Infrastructures.Context;

public class SpeciesRegimeEntityTypeConfiguration : IEntityTypeConfiguration<SpeciesRegime>
{
    public void Configure(EntityTypeBuilder<SpeciesRegime> builder)
    {
        builder.HasKey(c => c.RegimeId);

        builder.Property(c => c.Label).IsRequired();

        builder.HasData(
            new SpeciesRegime { RegimeId = 1, Label = "Photosynthèse" },
            new SpeciesRegime { RegimeId = 2, Label = "Herbivore" },
            new SpeciesRegime { RegimeId = 3, Label = "Carnivore" },
            new SpeciesRegime { RegimeId = 4, Label = "Omnivore" },
            new SpeciesRegime { RegimeId = 5, Label = "Coprophages" },
            new SpeciesRegime { RegimeId = 6, Label = "Limivores" },
            new SpeciesRegime { RegimeId = 7, Label = "Detritivores" },
            new SpeciesRegime { RegimeId = 8, Label = "Heterotrophie" });
    }
}