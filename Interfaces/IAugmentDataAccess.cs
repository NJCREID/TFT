using TFT_API.Models.Augments;
using TFT_API.Models.Item;

namespace TFT_API.Interfaces
{
    public interface IAugmentDataAccess
    {
        List<PersistedAugment> GetAugments();
        PersistedAugment? GetAugmentByKey(string key);
        PersistedAugment AddAugment(PersistedAugment item);
        List<PersistedAugment> GetAugmentsByTier(int tier);
    }
}
