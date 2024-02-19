using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Staffusersearchhistory
    {
        public int Id { get; set; }
        public int? StaffUserId { get; set; }
        public string SearchText { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
