using System;
using System.Collections.Generic;

namespace Max.Data.DataModel
{
    public partial class Pipelineproduct
    {
        public int ProductId { get; set; }
        public int? PipelineId { get; set; }
        public string ProductName { get; set; }
        public int? ProductIndex { get; set; }
        public int? Status { get; set; }

        public virtual Opportunitypipeline Pipeline { get; set; }
    }
}
