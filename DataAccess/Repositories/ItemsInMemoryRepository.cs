using Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;

namespace DataAccess.Repositories
{
    public class ItemsInMemoryRepository : IItemsRepository
    {
        private readonly IMemoryCache _memoryCache;
        private const string StorageKey = "TempBulkItems";

        public ItemsInMemoryRepository(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        //Get items from memory cache
        public List<IItemValidating> Get()
        {
            if (_memoryCache.TryGetValue(StorageKey, out List<IItemValidating> cachedItems))
            {
                return cachedItems;
            }
            return new List<IItemValidating>();
        }

        //Save items temporarily to memory cache
        public void Save(List<IItemValidating> itemsList)
        {
            _memoryCache.Set(StorageKey, itemsList);
        }

        //Clear the cache
        //will be used after saving to the database
        public void Clear()
        {
            _memoryCache.Remove(StorageKey);
        }
    }
}