using TFT_API.Models.Item;

namespace TFT_API.Interfaces
{
    public interface IItemDataAccess
    {
        List<PersistedItem> GetItems();
        PersistedItem? GetItemByKey(string key);
        PersistedItem AddItem(PersistedItem item);
        List<PersistedItem> GetComponents();
    }
}
