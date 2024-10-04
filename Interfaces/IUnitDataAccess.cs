using TFT_API.Models.Unit;

namespace TFT_API.Interfaces
{
    public interface IUnitDataAccess
    {
        Task<List<UnitDto>> GetFullUnitsAsync();
        Task<List<PartialUnitDto>> GetPartialUnitsAsync();
        Task<UnitDto?> GetUnitByKeyAsync(string key);
    }
}
