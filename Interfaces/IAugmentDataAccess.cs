using TFT_API.Models.Augments;

namespace TFT_API.Interfaces
{
    public interface IAugmentDataAccess
    {
        Task<List<AugmentDto>> GetAugmentsAsync();
        Task<AugmentDto?> GetAugmentByKeyAsync(string key);
        Task<List<AugmentDto>> GetAugmentsByTierAsync(int tier);
    }
}
