using TFT_API.Models.Item;
using TFT_API.Models.Unit;

namespace TFT_API.Models.UserGuides
{
    public class HexDto
    {
        public PartialUnitDto Unit { get; set; } = new PartialUnitDto();
        public List<PartialItemDto> CurrentItems { get; set; } = [];
        public bool IsStarred { get; set; }
        public int Coordinates { get; set; }
    }
}
