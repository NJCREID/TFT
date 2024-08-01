using TFT_API.Models.Unit;

namespace TFT_API.Interfaces
{
    public interface IUnitDataAccess
    {
        List<PersistedUnit> GetUnits();
        PersistedUnit? GetUnitByKey(string key);
        PersistedUnit AddUnit(PersistedUnit unit);
    }
}
