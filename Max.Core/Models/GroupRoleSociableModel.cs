using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class GroupRoleSociableModel
    {
        public string GroupRoleName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Status { get; set; }
    }
}
