using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Core.Models
{
    public class PipelineProductModel
    {
        public int ProductId { get; set; }
        public int? PipelineId { get; set; }
        public string ProductName { get; set; }
        public int? ProductIndex { get; set; }
        public int? Status { get; set; }

        public OpportunityPipelineModel Pipeline { get; set; }
    }
}
