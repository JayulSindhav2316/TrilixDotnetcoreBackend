using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Session
    {
        public Session()
        {
            Answertoquestions = new HashSet<Answertoquestion>();
            Eventregisterquestions = new HashSet<Eventregisterquestion>();
            Eventregistersessions = new HashSet<Eventregistersession>();
            Questionlinks = new HashSet<Questionlink>();
            Sessionleaderlinks = new HashSet<Sessionleaderlink>();
            Sessionregistrationgrouppricings = new HashSet<Sessionregistrationgrouppricing>();
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
        public decimal? MemberPrice { get; set; }
        public decimal? NonMemberPrice { get; set; }

        public virtual Event Event { get; set; }
        public virtual Glaccount GlAccount { get; set; }
        public virtual ICollection<Answertoquestion> Answertoquestions { get; set; }
        public virtual ICollection<Eventregisterquestion> Eventregisterquestions { get; set; }
        public virtual ICollection<Eventregistersession> Eventregistersessions { get; set; }
        public virtual ICollection<Questionlink> Questionlinks { get; set; }
        public virtual ICollection<Sessionleaderlink> Sessionleaderlinks { get; set; }
        public virtual ICollection<Sessionregistrationgrouppricing> Sessionregistrationgrouppricings { get; set; }
    }
}
