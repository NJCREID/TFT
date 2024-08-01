using AutoMapper;
using System.Collections.Generic;
using TFT_API.Interfaces;
using TFT_API.Models.Unit;

public class TraitResolver : IValueResolver<UnitRequest, PersistedUnit, List<UnitTrait>>
{
    private readonly ITraitDataAccess _traitRepo;

    public TraitResolver(ITraitDataAccess traitRepo)
    {
        _traitRepo = traitRepo;
    }

    public List<UnitTrait> Resolve(UnitRequest source, PersistedUnit destination, List<UnitTrait> destMember, ResolutionContext context)
    {
        var unitTraits = new List<UnitTrait>();

        foreach (var traitKey in source.Traits)
        {
            var trait = _traitRepo.GetTraitByKey(traitKey);
            if (trait == null)
            {
                continue; 
            }

            var unitTrait = new UnitTrait
            {
                Unit = destination,
                Trait = trait
            };
            unitTraits.Add(unitTrait);
        }

        return unitTraits;
    }
}
