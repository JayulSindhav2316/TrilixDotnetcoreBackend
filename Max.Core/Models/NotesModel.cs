using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class NotesModel
    {
        public int NoteId { get; set; }
        public int EntityId { get; set; }
        public string Notes { get; set; }
        public string Severity { get; set; }
        public int DisplayOnProfile { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public int Status { get; set; }
    }
}
