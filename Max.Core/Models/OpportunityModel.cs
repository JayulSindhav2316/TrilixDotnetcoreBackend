using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Core.Models
{
    public class OpportunityModel
    {
        public int OpportunityId { get; set; }
        public int CompanyId { get; set; }
        public int PipelineId { get; set; }
        public int ProductId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EstimatedCloseDate { get; set; }
        public decimal Potential { get; set; }
        public int StaffUserId { get; set; }
        public int? AccountContactId { get; set; }
        public int StageId { get; set; }
        public int Probability { get; set; }
        public string Notes { get; set; }
        public string PipelineName { get; set; }
        public string ProductName { get; set; }
        public string StageName { get; set; }
        public string OwnerName { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public CompanyModel Company { get; set; }
        public string EstimatedClose
        {
            get
            {
                return EstimatedCloseDate.ToString("MM/dd/yyyy");
            }
        }
    }
}