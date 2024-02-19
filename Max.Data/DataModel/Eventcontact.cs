using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Eventcontact
    {
        public int EventContactId { get; set; }
        public int? EventId { get; set; }
        public int? StaffId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int? HidePhoneNumber { get; set; }

        public virtual Event Event { get; set; }
        public virtual Staffuser Staff { get; set; }
    }
}
