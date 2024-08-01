using TFT_API.Models.Item;

namespace TFT_API.Models.UserGuides
{
    public class HexItem
    {
        public int Id { get; set; }
        public PersistedItem Item { get; set; } = new PersistedItem();
        public int HexId { get; set; }
    }
}
