using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class DocumentTagModel
    {
        public int DocumentTagId { get; set; }
        public int? DocumentObjectId { get; set; }
        public int? TagId { get; set; }
        public string TagName { get; set; }
        public string TagValue { get; set; }
        public TagModel Tag { get; set; }

    }
}
