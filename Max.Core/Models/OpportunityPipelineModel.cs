using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Core.Models
{
    public class OpportunityPipelineModel
    {
        public OpportunityPipelineModel()
        {
            PipelineProducts = new List<PipelineProductModel>();
            PipelineStages = new List<PipelineStageModel>();
        }

        public int PipelineId { get; set; }
        public string Name { get; set; }
        public int? Status { get; set; }

        public List<PipelineProductModel> PipelineProducts { get; set; }
        public List<PipelineStageModel> PipelineStages { get; set; }
    }
}
