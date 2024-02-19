using System;
using System.Collections.Generic;

namespace Max.Core.Models
{
    public partial class StaffUserModel
    {
        public StaffUserModel()
        {
            Roles = new List<RoleModel>();
            SelectedRoles = new int[] { };
        }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string DepartmentName { get; set; }
        public int DepartmentId { get; set; }
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string AccountName { get; set; }
        public int Status { get; set; }
        public DateTime LastAccessed { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        //public List<RoleModel> Roles { get; set; }
        public int[] SelectedRoles { get; set; }
        public List<RoleModel> Roles { get; set; }
        public string[] RoleNames { get; set; }
        public int CartId { get; set; }
        public string CellPhoneNumber { get; set; }
        public int Locked { get; set; }
        public int FailedAttempts { get; set; }
        public DepartmentModel Department { get; set; }
        public OrganizationModel Organization { get; set; }
        public int SociableUserId { get; set; }
        public int SociableProfileId { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsBirthdayRequired { get; set; }
        public int IsDisplayUser { get; set; }
        public bool IsActive
        {
            get
            {
                return Status == 1 ? true : false;
            }
            set
            {
                Status = value ? 1 : 0;
            }
        }
        public bool IsLocked
        {
            get
            {
                return Locked == 1 ? true : false;
            }
            set
            {
                Locked = value ? 1 : 0;
            }
        }

        public string FormattedPhoneNumber
        {
            get
            {
                return CellPhoneNumber.FormatPhoneNumber();
            }
        }
        public string Name
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
    }
}
