using AutoMapper;
using TFT_API.Data;
using TFT_API.Models.UserGuides;

namespace TFT_API.Helper
{
    public class UserGuideResolver(TFTContext context) : IValueResolver<UserGuideRequest, UserGuide, List<GuideTrait>>
    {
        private readonly TFTContext _context = context;

        public List<GuideTrait> Resolve(UserGuideRequest source, UserGuide destination, List<GuideTrait> destMember, ResolutionContext context)
        {
            var traits = new List<GuideTrait>();

            foreach (var traitDto in source.Traits)
            {
                var existingTrait = _context.Traits.FirstOrDefault(t => t.Key == traitDto.Trait.Key);
                if (existingTrait != null)
                {
                    var userGuideTrait = new GuideTrait
                    {
                        Trait = existingTrait,
                        Value = traitDto.Value,
                        Tier = traitDto.Tier,
                    };
                    traits.Add(userGuideTrait);
                }
            }

            return traits;
        }
    }
}
