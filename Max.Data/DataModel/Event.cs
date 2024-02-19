using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Event
    {
        public Event()
        {
            Answertoquestions = new HashSet<Answertoquestion>();
            Eventcontacts = new HashSet<Eventcontact>();
            Eventregisterquestions = new HashSet<Eventregisterquestion>();
            Eventregisters = new HashSet<Eventregister>();
            Eventregistrationsettings = new HashSet<Eventregistrationsetting>();
            Invoices = new HashSet<Invoice>();
            Linkeventfeetypes = new HashSet<Linkeventfeetype>();
            Linkeventgroups = new HashSet<Linkeventgroup>();
            Questionlinks = new HashSet<Questionlink>();
            Sessions = new HashSet<Session>();
        }

        public int EventId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? EventTypeId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? RegStartDate { get; set; }
        public DateTime? RegEndDate { get; set; }
        public int? TimeZoneId { get; set; }
        public int? OrganizationId { get; set; }
        public string Location { get; set; }
        public string Area { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Zip { get; set; }
        public int? Status { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public int? EventImageId { get; set; }
        public int? EventBannerImageId { get; set; }
        public string WebinarLiveLink { get; set; }
        public string WebinarRecordedLink { get; set; }
        public int? MaxCapacity { get; set; }
        public int? ShowEventCode { get; set; }
        public int? AllowNonMember { get; set; }
        public int? AllowWaitlist { get; set; }
        public int? AllowMultipleRegistration { get; set; }
        public DateTime? DueDate { get; set; }

        public virtual Eventtype EventType { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual Timezone TimeZone { get; set; }
        public virtual ICollection<Answertoquestion> Answertoquestions { get; set; }
        public virtual ICollection<Eventcontact> Eventcontacts { get; set; }
        public virtual ICollection<Eventregisterquestion> Eventregisterquestions { get; set; }
        public virtual ICollection<Eventregister> Eventregisters { get; set; }
        public virtual ICollection<Eventregistrationsetting> Eventregistrationsettings { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual ICollection<Linkeventfeetype> Linkeventfeetypes { get; set; }
        public virtual ICollection<Linkeventgroup> Linkeventgroups { get; set; }
        public virtual ICollection<Questionlink> Questionlinks { get; set; }
        public virtual ICollection<Session> Sessions { get; set; }
    }
}
