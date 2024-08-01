using TFT_API.Models.Trait;

namespace TFT_API.Interfaces
{
    public interface ITraitDataAccess
    {
        List<PersistedTrait> GetTraits();
        PersistedTrait? GetTraitByKey(string key);
        PersistedTrait AddTrait(PersistedTrait trait);
    }
}
