using System;
using System.Collections.Generic;

namespace Max.Data.DataModel
{
    public partial class Opportunitypipeline
    {
        public Opportunitypipeline()
        {
            Pipelineproducts = new HashSet<Pipelineproduct>();
            Pipelinestages = new HashSet<Pipelinestage>();
            Opportunities = new HashSet<Opportunity>();

        }

        public int PipelineId { get; set; }
        public string Name { get; set; }
        public int? Status { get; set; }

        public virtual ICollection<Pipelineproduct> Pipelineproducts { get; set; }
        public virtual ICollection<Pipelinestage> Pipelinestages { get; set; }
        public virtual ICollection<Opportunity> Opportunities { get; set; }
    }
}
