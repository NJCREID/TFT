using AutoMapper;
using TFT_API.Interfaces;
using TFT_API.Models.FetchedTFTData;
using TFT_API.Models.Unit;

namespace TFT_API.Helpers
{
    public class TraitResolver(ITraitDataAccess traitRepo) : IValueResolver<Champion, PersistedUnit, List<UnitTrait>>
    {
        private readonly ITraitDataAccess _traitRepo = traitRepo;

        public List<UnitTrait> Resolve(Champion source, PersistedUnit destination, List<UnitTrait> destMember, ResolutionContext context)
        {
            var unitTraits = new List<UnitTrait>();

            foreach (var traitKey in source.Traits)
            {
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
