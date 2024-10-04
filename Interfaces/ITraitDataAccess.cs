using TFT_API.Models.Trait;

namespace TFT_API.Interfaces
{
    public interface ITraitDataAccess
    {
        Task<List<FullTraitDto>> GetTraitsAsync();
        Task<PersistedTrait?> GetTraitByKeyAsync(string key);
    }
}
