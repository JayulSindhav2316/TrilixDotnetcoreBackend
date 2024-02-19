using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface IItemService
    {
        Task<List<ItemModel>> GetAllItems(int status);
        Task<List<ItemModel>> GetItemsByCode( string code, int status);
        Task<List<ItemModel>> GetItemsByName(string name, int status);
        Task<ItemModel> GetItemById(int id);
        Task<List<SelectListModel>> GetSelectList();
        Task<Item> CreateItem(ItemModel itemModel);
        Task<Item> UpdateItem(ItemModel itemModel);
        Task<bool> DeleteItem(int itemId);
    }
}