using AutoMapper;
using TFT_API.Models.UserGuides;
using TFT_API.Data;

namespace TFT_API.Helper
{
    public class HexResolver(TFTContext context) : IValueResolver<UserGuideRequest, UserGuide, List<Hex>>
    {
        private readonly TFTContext _context = context;

        public List<Hex> Resolve(UserGuideRequest source, UserGuide destination, List<Hex> destMember, ResolutionContext context)
        {
            var hexes = new List<Hex>();

            foreach (var hexRequest in source.Hexes)
            {
                var hex = new Hex
                {
                    Coordinates = hexRequest.Coordinates,
                    IsStarred = hexRequest.IsStarred
                };

                var existingUnit = _context.Units.FirstOrDefault(u => u.InGameKey == hexRequest.Unit.InGameKey);
                if (existingUnit != null)
                {
                    hex.Unit = existingUnit;
                }

                var existingItems = new List<HexItem>();
                foreach (var itemRequest in hexRequest.CurrentItems)
                {
                    var existingItem = _context.Items.FirstOrDefault(i => i.InGameKey == itemRequest.InGameKey);
                    if (existingItem != null)
                    {
                        var hexItem = new HexItem
                        {
                            Item = existingItem,
                        };
                        existingItems.Add(hexItem);
                    }
                }
                hex.CurrentItems = existingItems;

                hexes.Add(hex);
            }

            return hexes;
        }
    }
}
