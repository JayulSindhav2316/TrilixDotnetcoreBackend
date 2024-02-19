using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IItemRepository : IRepository<Item>
    {
        Task<IEnumerable<Item>> GetAllItemsAsync();
        Task<IEnumerable<Item>> GetItemsByCodeAsync(string code);
        Task<IEnumerable<Item>> GetItemsByNameAsync(string code);
        Task<Item> GetItemDetailByIdAsync(int id);
        Task<Item> GetItemByIdAsync(int id);
        Task<Item> GetItemsByTypeAsync(int typeId);
        Task<bool> UpdateItem(Item input);
    }
}
