﻿using AutoMapper;
using TFT_API.Data;
using TFT_API.Models.UserGuides;

namespace TFT_API.Helper
{
    public class AugmentResolver(TFTContext context) : IValueResolver<UserGuideRequest, UserGuide, List<GuideAugment>>
    {
        private readonly TFTContext _context = context;

        // Resolves the augments for the user guide from the request
        public List<GuideAugment> Resolve(UserGuideRequest source, UserGuide destination, List<GuideAugment> destMember, ResolutionContext context)
        {
            var augments = new List<GuideAugment>();

            foreach (var augmentDto in source.Augments)
            {
                // Find an existing augment in the database by its InGameKey
                var existingAugment = _context.Augments.FirstOrDefault(t => t.InGameKey == augmentDto.InGameKey);
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
