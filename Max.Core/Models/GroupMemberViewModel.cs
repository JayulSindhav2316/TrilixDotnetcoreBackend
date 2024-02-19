using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class GroupMemberViewModel
    {
        public int GroupMemberId { get; set; }
        public int? EntityId { get; set; }
        public int? GroupId { get; set; }
        public int? Status { get; set; }
        public string CellPhoneNumber { get; set; }
        public string Email { get; set; }
        public string EntityName { get; set; }
        public string RoleName { get; set; }
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
        public string FormattedPhoneNumber
        {
            get
            {
                return CellPhoneNumber.FormatPhoneNumber();
            }
        }
    }
}
