using AutoMapper;
using Max.Core;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Max.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Max.Services
{
    public class TagService : ITagService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TagService> _logger;
        public TagService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TagService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._logger = logger;
        }


        public async Task<List<TagModel>> GetAllTags()
        {
            var tags =  await _unitOfWork.Tags.GetAllAsync();
            return _mapper.Map<List<TagModel>>(tags);
        }

        public async Task<TagModel> GetTagById(int id)
        {
            var tag =  await _unitOfWork.Tags.GetTagByIdAsync(id);
            return _mapper.Map<TagModel>(tag);
        }
        public async Task<TagModel> CreateTag(TagModel model)
        {
            Tag tag = new Tag();
               
            tag.TagName = model.TagName;

            //check if it already exists

            var existingTag = await _unitOfWork.Tags.GetTagByNameAsync(tag.TagName);
            if(existingTag == null)
            {
                try
                {
                    await _unitOfWork.Tags.AddAsync(tag);
                    await _unitOfWork.CommitAsync();
                    return _mapper.Map<TagModel>(tag);
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                    throw new Exception("Unable to add Tag.");
                }
            }
            else
            {
                throw new Exception("A Tag already exists with this name.");
            }
     
        }

        public async Task<bool> UpdateTag(TagModel model)
        {
            var tag = await _unitOfWork.Tags.GetByIdAsync(model.TagId);
            if (tag != null)
            {
                //Map Model Data
                tag.TagName = model.TagName;

                try
                {
                    _unitOfWork.Tags.Update(tag);
                    await _unitOfWork.CommitAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                    throw new Exception("Unable to update Tag.");
                }
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteTag(int id)
        {

            var tag = await _unitOfWork.Tags.GetByIdAsync(id);
            if (tag != null)
            {
                var documentTags = await _unitOfWork.DocumentObjectTags.GetDocumentTagsByTagIdAsync(id);
                if (documentTags.IsNullOrEmpty())
                {
                    _unitOfWork.Tags.Remove(tag);
                    await _unitOfWork.CommitAsync();
                    return true;
                }
                throw new InvalidOperationException("This tag can not be deleted as it has referenced documents.");
            }
            return false;

        }
        public async Task<List<SelectListModel>> GetTagList()
        {
            var tags = await _unitOfWork.Tags.GetAllTagsAsync();

            List<SelectListModel> selectList = new List<SelectListModel>();
            foreach (var tag in tags)
            {
                SelectListModel selectListItem = new SelectListModel();
                selectListItem.code = tag.TagId.ToString();
                selectListItem.name = tag.TagName;
                selectList.Add(selectListItem);
            }
            return selectList;
        }

    }
}
