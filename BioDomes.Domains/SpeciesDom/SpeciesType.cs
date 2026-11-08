namespace BioDomes.Domains.SpeciesDom;

public enum SpeciesType
{
    Mammal = 1,
    Bird,
    Fish,
    Reptile,
    Amphibian,
    Crustacean,
    Arachnid,
    Insect,
    Myriapods,
    Mollusc,
    Echinoderms,
    Anelids,
    Cnidarians,
    Flatworms,
    Porifera,
    Plant,
    Moss,
    Algae,
    Lichen, // champignon
    Mushroom
}

public static class SpeciesTypeExtensions
{
    public static string ToDisplayString(this SpeciesType type)
    {
        return type switch
        {
            SpeciesType.Mammal => "Mammifères",
            SpeciesType.Bird => "Oiseaux",
            SpeciesType.Fish => "Poissons",
            SpeciesType.Reptile => "Reptiles",
            SpeciesType.Amphibian => "Amphibiens",
            SpeciesType.Crustacean => "Crustacés",
            SpeciesType.Arachnid => "Arachnides",
            SpeciesType.Insect => "Insectes",
            SpeciesType.Myriapods => "Myriapodes",
            SpeciesType.Mollusc => "Mollusques",
            SpeciesType.Echinoderms => "Echinodermes",
            SpeciesType.Anelids => "Anélides",
            SpeciesType.Cnidarians => "Cnidaires",
            SpeciesType.Flatworms => "Plathelminthes",
            SpeciesType.Porifera => "Porifères",
            SpeciesType.Plant => "Plantes",
            SpeciesType.Moss => "Mousses",
            SpeciesType.Algae => "Algues",
            SpeciesType.Lichen => "Lichen",
            SpeciesType.Mushroom => "Champignons",
            _ => type.ToString()
        };
    }
}