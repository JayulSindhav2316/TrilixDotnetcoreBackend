using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Core.Models
{
    public class OpportunityStageGroupModel
    {
        public OpportunityStageGroupModel()
        {
            Opportunities = new List<OpportunityModel>();
        }
        public int stageId { get; set; }
        public string stageName { get; set; }
        public List<OpportunityModel> Opportunities { get; set; }
        public int OpportunityCount
        {
            get
            {
                return Opportunities.Count();
            }
        }
        public decimal TotalPotential
        {
            get
            {
                return Opportunities.Select(x => x.Potential).Sum();
            }
        }
    }
}