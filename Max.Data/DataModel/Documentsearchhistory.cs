using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Documentsearchhistory
    {
        public int DocumentSearchId { get; set; }
        public int? EntityId { get; set; }
        public string SearchText { get; set; }
        public DateTime? SearchDate { get; set; }
        public string IpAddress { get; set; }
    }
}
