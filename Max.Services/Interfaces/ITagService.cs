using Max.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Max.Services.Interfaces
{
    public interface ITagService
    {
        Task<List<TagModel>> GetAllTags();
        Task<TagModel> GetTagById(int id);
        Task<TagModel> CreateTag(TagModel model);
        Task<bool> UpdateTag(TagModel model);
        Task<bool> DeleteTag(int tagId);
        Task<List<SelectListModel>> GetTagList();
    }
}
