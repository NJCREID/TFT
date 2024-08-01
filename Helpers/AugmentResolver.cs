using AutoMapper;
using TFT_API.Data;
using TFT_API.Models.UserGuides;

namespace TFT_API.Helper
{
    public class AugmentResolver : IValueResolver<UserGuideRequest, UserGuide, List<GuideAugment>>
    {
        private readonly TFTContext _context;

        public AugmentResolver(TFTContext context)
        {
            _context = context;
        }

        public List<GuideAugment> Resolve(UserGuideRequest source, UserGuide destination, List<GuideAugment> destMember, ResolutionContext context)
        {
            var augments = new List<GuideAugment>();

            foreach (var augmentDto in source.Augments)
            {
                var existingAugment = _context.Augments.FirstOrDefault(t => t.Key == augmentDto.Key);
                if (existingAugment != null)
                {
                    var userGuideAugment = new GuideAugment
                    {
                        Augment = existingAugment,
                    };
                    augments.Add(userGuideAugment);
                }
            }

            return augments;
        }
    }
}
