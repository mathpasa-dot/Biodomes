namespace BioDomes.Domains.SpeciesDom;

public enum SpeciesRegime
{
    PhotoSynthesis = 1,
    Herbivore,
    Carnivore,
    Omnivore,
    Coprophages, // nourrit de matières fécales
    Limivores, // se nourrit de matières organiques contenues dans la vase
    Detritivores, //des bactéries, champignons et invertébrés lato sensu, qui se nourrissent de débris animaux, végétaux ou fongiques qui sont des excrétats, excréments, sont en décomposition, ou font partie de la nécromasse.
    Heterotrophie // L'hétérotrophie est le mode de nutrition des organismes vivants qui se nourrissent de matière organique préexistante
}

public static class SpeciesRegimeRegimeExtensions
{
    public static string ToDisplayString(this SpeciesRegime type)
    {
        return type switch
        {
            SpeciesRegime.PhotoSynthesis => "Photosynthèse",
            SpeciesRegime.Herbivore => "Herbivore",
            SpeciesRegime.Carnivore => "Carnivore",
            SpeciesRegime.Omnivore => "Omnivore",
            SpeciesRegime.Coprophages => "Coprophages",
            SpeciesRegime.Limivores => "Limivores",
            SpeciesRegime.Detritivores => "Detritivores",
            SpeciesRegime.Heterotrophie => "Heterotrophie",
            _ => type.ToString()
        };
    }
}