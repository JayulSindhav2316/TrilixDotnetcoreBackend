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
namespace Max.Services
{
    public class MembershipCategoryService : IMembershipCategoryService
    {

        private readonly IUnitOfWork _unitOfWork;
        public MembershipCategoryService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Membershipcategory>> GetAllMembershipCategories()
        {
            return await _unitOfWork.MembershipCategories
                .GetAllMembershipCategoriesAsync();
        }

        public async Task<Membershipcategory> GetMembershipCategoryById(int id)
        {
            return await _unitOfWork.MembershipCategories
                .GetMembershipCategoryByIdAsync(id);
        }

        public async Task<Membershipcategory> CreateMembershipCategory(MembershipCategoryModel model)
        {
            Membershipcategory membershipCategory = new Membershipcategory();
            var isValid = ValidMembershipCategory(model);
            if (isValid)
            {
                //Map Model Data
                membershipCategory.Name = model.Name;
                membershipCategory.Status = model.Status;

                await _unitOfWork.MembershipCategories.AddAsync(membershipCategory);
                await _unitOfWork.CommitAsync();
            }
            return membershipCategory;
        }
        public async Task<List<SelectListModel>> GetSelectList()
        {
            var categories = await _unitOfWork.MembershipCategories.GetAllMembershipCategoriesAsync();

            List<SelectListModel> selectList = new List<SelectListModel>();
            foreach (var category in categories)
            {
                SelectListModel selectListItem = new SelectListModel();
                selectListItem.code = category.MembershipCategoryId.ToString();
                selectListItem.name = category.Name;
                selectList.Add(selectListItem);
            }

            return selectList;
        }

        private bool ValidMembershipCategory(MembershipCategoryModel model)
        {
            //Validate  Name
            if (model.Name == null)
            {
                throw new NullReferenceException($"Membership Category Name can not be NULL.");
            }

            if (model.Name.Trim() == String.Empty)
            {
                throw new InvalidOperationException($"Membership Category Name can not be empty.");
            }

            return true;
        }

    }
}
