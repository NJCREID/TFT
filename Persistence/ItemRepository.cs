using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TFT_API.Data;
using TFT_API.Interfaces;
using TFT_API.Models.Item;

namespace TFT_API.Persistence
{
    public class ItemRepository: IItemDataAccess
    {
        private readonly TFTContext _context;

        public ItemRepository(TFTContext context)
        {
            _context = context;
        }
        public PersistedItem AddItem(PersistedItem item)
        {
            _context.Items.Add(item);
            return Save(item);
        }

        public PersistedItem? GetItemByKey(string key)
        {
            return _context.Items.AsNoTracking().FirstOrDefault(t => t.Key == key);
        }

        public List<PersistedItem> GetItems()
        {
            var items = _context.Items.AsNoTracking().ToList();
            return items;
        }

        public List<PersistedItem> GetComponents()
        {
            var components = _context.Items
                .AsNoTracking()
                .Where(item => item.IsComponent == true)
                .ToList();

            return components;
        }
        public PersistedItem Save(PersistedItem item)
        {
            _context.SaveChanges();
            return item;
        }
    }
}
