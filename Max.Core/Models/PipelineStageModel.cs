using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Core.Models
{
    public class PipelineStageModel
    {
        public int StageId { get; set; }
        public int? PipelineId { get; set; }
        public string StageName { get; set; }
        public int? Probability { get; set; }
        public int? StageIndex { get; set; }
        public int? Status { get; set; }

        public  OpportunityPipelineModel Pipeline { get; set; }
    }
}
