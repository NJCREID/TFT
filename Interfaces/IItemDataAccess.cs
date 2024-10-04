using TFT_API.Models.Item;

namespace TFT_API.Interfaces
{
    public interface IItemDataAccess
    {
        Task<List<ItemDto>> GetFullItemsAsync();
        Task<List<PartialItemDto>> GetPartialItemsAsync();
        Task<ItemDto?> GetItemByKeyAsync(string key);
        Task<List<ItemDto>> GetComponentsAsync();
    }
}
