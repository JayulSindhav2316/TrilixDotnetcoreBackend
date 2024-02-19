using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Core;
using Max.Core.Models;
using Max.Core.Services;

namespace Max.Services
{
    public class StaffUserService : IStaffUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        public StaffUserService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<Staffuser> CreateStaffUser(Staffuser staffuser)
        {
            await _unitOfWork.Staffusers.AddAsync(staffuser);
            await _unitOfWork.CommitAsync();
            return staffuser;
        }

        public async Task DeleteMusic(Staffuser staffuser)
        {
            _unitOfWork.Staffusers.Remove(staffuser);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<Staffuser>> GetAllStaffUsers()
        {
            return await _unitOfWork.Staffusers
                .GetAllStaffUsersAsync();
        }

        public async Task<Staffuser> GetStaffUserById(int id)
        {
            return await _unitOfWork.Staffusers
                .GetStaffUserByIdAsync(id);
        }

        public async Task UpdateStaffUser(Staffuser staffuserToUpdate, Staffuser staffuser)
        {
            staffuserToUpdate.FirstName = staffuser.FirstName;
            staffuserToUpdate.LastName = staffuser.LastName;

            await _unitOfWork.CommitAsync();
        }
    }
}