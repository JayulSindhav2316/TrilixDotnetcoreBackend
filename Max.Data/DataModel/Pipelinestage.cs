using System;
using System.Collections.Generic;

namespace Max.Data.DataModel
{
    public partial class Pipelinestage
    {
        public int StageId { get; set; }
        public int? PipelineId { get; set; }
        public string StageName { get; set; }
        public int? Probability { get; set; }
        public int? StageIndex { get; set; }
        public int? Status { get; set; }

        public virtual Opportunitypipeline Pipeline { get; set; }
    }
}
