using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.Repositories;
using Max.Data.Interfaces;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;
using Max.Core;
using Max.Data;
using Max.Services;
using System.Linq;
using AutoMapper;
using System.Text;

namespace Max.Services
{
    public class ItemService : IItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ItemService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        }

        public async Task<List<ItemModel>> GetAllItems(int status)
        {
            var items = await _unitOfWork.Items.GetAllItemsAsync();
            var itemList = new List<ItemModel>();
            foreach(var item in items)
            {
                var model = new ItemModel();
                model.ItemId = item.ItemId;
                model.Name = item.Name;
                model.ItemCode = item.ItemCode;
                model.Description = item.Description;
                model.ItemType = item.ItemType;
                model.UnitRate = item.UnitRate;
                model.Status = item.Status;
                model.ItemGlAccount = item.ItemGlAccount;
                model.ItemTypeName = item.ItemTypeNavigation.Name;
                model.GlAccountCode = item.ItemGlAccountNavigation.Code;
                model.StockCount = item.StockCount ?? 0;
                model.EnableMemberPortal = item.EnableMemberPortal ?? 0;
                model.TotalStock = item.TotalStock ?? 0;
                model.EnableStock = item.EnableStock ?? 0;
                itemList.Add(model);
            }

            if (status == 1 || status == 0)
            {
                return itemList.Where(x => x.Status == status).ToList();
            }
            else
            {
                return itemList;
            }
        }

        public async Task<List<ItemModel>> GetItemsByCode(string code, int status)
        {
            var items = await _unitOfWork.Items.GetItemsByCodeAsync(code);
            var itemList = new List<ItemModel>();
            foreach (var item in items)
            {
                var model = new ItemModel();
                model.ItemId = item.ItemId;
                model.Name = item.Name;
                model.ItemCode = item.ItemCode;
                model.Description = item.Description;
                model.ItemType = item.ItemType;
                model.UnitRate = item.UnitRate;
                model.Status = item.Status;
                model.ItemGlAccount = item.ItemGlAccount;
                model.ItemTypeName = item.ItemTypeNavigation.Name;
                model.GlAccountCode = item.ItemGlAccountNavigation.Code;
                model.StockCount = item.StockCount ?? 0;
                model.EnableMemberPortal = item.EnableMemberPortal ?? 0;
                model.EnableStock = item.EnableStock ?? 0;
                itemList.Add(model);
            }

            if(status == 1 || status == 0)
            {
                return itemList.Where(x => x.Status == status).ToList();
            }
            else
            {
                return itemList;
            }
            
        }
        public async Task<List<ItemModel>> GetItemsByName(string name, int status)
        {
            var items = await _unitOfWork.Items.GetItemsByNameAsync(name);
            var itemList = new List<ItemModel>();
            foreach (var item in items)
            {
                var model = new ItemModel();
                model.ItemId = item.ItemId;
                model.Name = item.Name;
                model.ItemCode = item.ItemCode;
                model.Description = item.Description;
                model.ItemType = item.ItemType;
                model.UnitRate = item.UnitRate;
                model.Status = item.Status;
                model.ItemGlAccount = item.ItemGlAccount;
                model.ItemTypeName = item.ItemTypeNavigation.Name;
                model.GlAccountCode = item.ItemGlAccountNavigation.Code;
                model.StockCount = item.StockCount??0;
                model.EnableMemberPortal = item.EnableMemberPortal??0;
                model.EnableStock = item.EnableStock ?? 0;
                itemList.Add(model);
            }

            if (status == 1 || status == 0)
            {
                return itemList.Where(x => x.Status == status).ToList();
            }
            else
            {
                return itemList;
            }
        }
        public async Task<ItemModel> GetItemById(int id)
        {
            ItemModel model = new ItemModel();
            var item = await _unitOfWork.Items.GetByIdAsync(id);
            model = _mapper.Map<ItemModel>(item);

            return model;
        }

        public async Task<Item> CreateItem(ItemModel model)
        {
            Item item = new Item();
            var isValid = await ValidItem(model);
            if (isValid)
            {
                //Map Model Data
                item.Name = model.Name.Trim();
                item.ItemCode = model.ItemCode.Trim();
                item.Description = model.Description;
                item.ItemType = model.ItemType;
                item.UnitRate = model.UnitRate;
                item.Status = model.Status;
                item.ItemGlAccount = model.ItemGlAccount;
                item.EnableMemberPortal = model.EnableMemberPortal;
                item.EnableStock = model.EnableStock;

                if(model.EnableStock == 1)
                {
                    item.StockCount = model.TotalStock;
                    item.TotalStock = model.TotalStock;
                }

                await _unitOfWork.Items.AddAsync(item);
                await _unitOfWork.CommitAsync();
            }
            return item;
        }

        public async Task<Item> UpdateItem(ItemModel model)
        {
            Item item = await _unitOfWork.Items.GetByIdAsync(model.ItemId);

            if (item == null)
            {
                throw new InvalidOperationException($"Item: {model.ItemId} not found.");
            }
            
            var isValid = await ValidItem(model);
            if (isValid)
            {
                //Map Model Data
                item.Name = model.Name.Trim();
                item.ItemCode = model.ItemCode.Trim();
                item.Description = model.Description;
                item.ItemType = model.ItemType;
                item.UnitRate = model.UnitRate;
                item.Status = model.Status;
                item.ItemGlAccount = model.ItemGlAccount;
                item.EnableMemberPortal = model.EnableMemberPortal;
                item.EnableStock = model.EnableStock;

                if (model.EnableStock == 1)
                { 
                    if(model.TotalStock > item.TotalStock)
                    {
                        int stockDifference = model.TotalStock - item.TotalStock??0;
                        item.TotalStock = model.TotalStock;
                        item.StockCount += stockDifference;
                    }

                    if (model.TotalStock < item.TotalStock)
                    {
                        int soldStock = Convert.ToInt32(item.TotalStock) - Convert.ToInt32(item.StockCount);

                        if (soldStock > model.TotalStock)
                        {
                            throw new InvalidOperationException($"{soldStock} quantity have been already sold for this item.");
                        }

                        int stockDifference = item.TotalStock - model.TotalStock ?? 0;
                        item.TotalStock = model.TotalStock;
                        item.StockCount -= stockDifference;
                    }
                }
                else
                {
                    item.StockCount = 0;
                    item.TotalStock = 0;
                }

                try
                {
                    _unitOfWork.Items.Update(item);
                    await _unitOfWork.CommitAsync();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Unable to Update Item.");
                }
            }
            return item;
        }

        public async Task<bool> DeleteItem(int itemId)
        {
            Item item = await _unitOfWork.Items.GetItemDetailByIdAsync(itemId);

            if(item.Invoicedetails.Count > 0)
            {
                throw new InvalidOperationException($"Cannot delete this item as there are invoice(s) attached.");
            }

            //TODO: AKS - Add validation to check if any Invoice exists
            if (item != null)
            {
                _unitOfWork.Items.Remove(item);
                await _unitOfWork.CommitAsync();
                return true;

            }
            throw new InvalidOperationException($"Item: {item} not found.");

        }
        public async Task<List<SelectListModel>> GetSelectList()
        {
            var itemTypes = await _unitOfWork.ItemTypes.GetAllItemTypesAsync();

            List<SelectListModel> selectList = new List<SelectListModel>();
            foreach (var itemType in itemTypes)
            {
                SelectListModel selectListItem = new SelectListModel();
                selectListItem.code = itemType.ItemTypeId.ToString();
                selectListItem.name = itemType.Name;
                selectList.Add(selectListItem);
            }
            return selectList;
        }
        private async Task<bool> ValidItem(ItemModel model)
        {
            //Validate  Name
            if (model.Name.IsNullOrEmpty())
            {
                throw new NullReferenceException($"Item Name can not be NULL.");
            }

            if (model.ItemCode.IsNullOrEmpty())
            {
                throw new NullReferenceException($"Item Code can not be NULL.");
            }

            if (model.Description.IsNullOrEmpty())
            {
                throw new NullReferenceException($"Item Description Code can not be NULL.");
            }

            if (model.Description.IsNullOrEmpty())
            {
                throw new NullReferenceException($"Item Description Code can not be NULL.");
            }

            // Check if there is an existing  code
            var items = await _unitOfWork.Items.GetAllAsync();
            if (items.Where(x => x.ItemCode == model.ItemCode.Trim() && x.ItemId != model.ItemId).Any())
            {
                throw new InvalidOperationException($"An Item already exists with Code:{model.ItemCode}.");
            }

            return true;
        }
    }
}
