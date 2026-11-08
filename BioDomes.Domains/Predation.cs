using BioDomes.Domains.SpeciesDom;

namespace BioDomes.Domains;

public record Predation(
    Species Predator,
    Species Prey
);