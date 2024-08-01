using System.Text.Json.Serialization;
using TFT_API.Models.Trait;

namespace TFT_API.Models.Unit
{
    public class UnitTrait
    {
        public int UnitId { get; set; }
        public PersistedUnit Unit { get; set; } = new PersistedUnit();
        public int TraitId { get; set; }
        public PersistedTrait Trait { get; set; } = new PersistedTrait();
    }
}
