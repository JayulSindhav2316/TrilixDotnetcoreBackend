using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class SessionModel
    {
        public SessionModel()
        {
            SessionQuestions = new List<QuestionBankModel>();
            SessionLeaders = new List<SessionLeaderLinkModel>();
            GroupPricing = new List<LinkEventGroupModel>();
            LinkedFeeTypes = new List<LinkEventFeeTypeModel>();
        }
        public int SessionId { get; set; }
        public int? EventId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime? StartDatetime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string Location { get; set; }
        public int? EnableMaxCapacity { get; set; }
        public int? MaxCapacity { get; set; }
        public int? GlAccountId { get; set; }
        public int? EnableSamePrice { get; set; }
        public int? EnableTax { get; set; }
        public decimal? Tax { get; set; }
        public int? EnableCeu { get; set; }
        public int? Status { get; set; }
        public int RegisteredSessions { get; set; } = 0;
        public EventModel Event { get; set; }

        public List<QuestionBankModel> SessionQuestions { get; set; }
        public List<SessionLeaderLinkModel> SessionLeaders { get; set; }
        public List<LinkEventGroupModel> GroupPricing { get; set; }
        public List<LinkEventFeeTypeModel> LinkedFeeTypes { get; set; }
    }
}
