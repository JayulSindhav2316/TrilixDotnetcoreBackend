using Max.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Max.Core;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Max.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Max.Services
{
    public class StaffSearchHistoryService : IStaffSearchHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public StaffSearchHistoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> DeleteSearchHistory(string text, int id)
        {
            var entity =  _unitOfWork.StaffSearchHistories.Find(x=>x.SearchText==text && x.StaffUserId==id).FirstOrDefault();
            _unitOfWork.StaffSearchHistories.Remove(entity);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<List<string>> GetSearchHistory(int userId)
        {
            var data = await _unitOfWork.StaffSearchHistories.GetSearchHistory(userId);
            var res = data.ToList().OrderByDescending(x => x.UpdatedAt).Select(x => x.SearchText).ToList();
            return res;
        }

        public async Task<bool> SaveSearchText(string text, int userId)
        {
            try
            {
                var searchResult = await _unitOfWork.StaffSearchHistories.GetSearchHistory(userId);

                //to save only ten entries.
                if (searchResult.Count() < 10)
                {
                    if (searchResult.Any(x => x.SearchText == text && x.StaffUserId == userId))
                    {
                        //update updatedAt
                        var data = searchResult.Where(x => x.SearchText == text).FirstOrDefault();
                        data.UpdatedAt = DateTime.Now;
                        _unitOfWork.StaffSearchHistories.Update(data);
                        await _unitOfWork.CommitAsync();
                    }
                    else
                    {
                        var res = await _unitOfWork.StaffSearchHistories.SaveSearchText(userId, text);
                        await _unitOfWork.CommitAsync();
                        return res;
                    }

                }
                else
                {
                    if (searchResult.Any(x => x.SearchText == text && x.StaffUserId == userId))
                    {
                        var data = searchResult.Where(x => x.SearchText == text).FirstOrDefault();
                        data.UpdatedAt = DateTime.Now;
                        _unitOfWork.StaffSearchHistories.Update(data);
                        await _unitOfWork.CommitAsync();
                    }
                    else
                    {
                        var lastRes = searchResult.OrderBy(x => x.UpdatedAt).FirstOrDefault();
                        _unitOfWork.StaffSearchHistories.Remove(lastRes);
                        var res = await _unitOfWork.StaffSearchHistories.SaveSearchText(userId, text);
                        await _unitOfWork.CommitAsync();
                        return res;
                    }
                }
                return true;


            }
            catch (Exception ex)
            {

                throw;
            }

        }




    }
}
