using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Resetpassword
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string IpAddress { get; set; }
        public DateTime? RequestDate { get; set; }
        public string Token { get; set; }
        public int? Status { get; set; }
        public int UserId { get; set; }
        public int EntityId { get; set; }
    }
}
