using TFT_API.Models.Item;
using TFT_API.Models.Unit;

namespace TFT_API.Models.UserGuides
{
    public class HexDto
    {
        public GuideUnitDto Unit { get; set; } = new GuideUnitDto();
        public List<GuideItemDto> CurrentItems { get; set; } = [];
        public bool IsStarred { get; set; }
        public int Coordinates { get; set; }
    }
}
