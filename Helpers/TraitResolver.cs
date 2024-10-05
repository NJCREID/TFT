using AutoMapper;
using TFT_API.Interfaces;
using TFT_API.Models.FetchedTFTData;
using TFT_API.Models.Unit;

namespace TFT_API.Helpers
{
    public class TraitResolver(ITraitDataAccess traitRepo) : IValueResolver<Champion, PersistedUnit, List<UnitTrait>>
    {
        private readonly ITraitDataAccess _traitRepo = traitRepo;

        // Resolves the list of UnitTrait objects based on Champion
        public List<UnitTrait> Resolve(Champion source, PersistedUnit destination, List<UnitTrait> destMember, ResolutionContext context)
        {
            var unitTraits = new List<UnitTrait>();

            foreach (var traitKey in source.Traits)
            {
                // Fetch the trait using the trait repository 
                var trait = _traitRepo.GetTraitByKeyAsync(traitKey).Result;
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
}
