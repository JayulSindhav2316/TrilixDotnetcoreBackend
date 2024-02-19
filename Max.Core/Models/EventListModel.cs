using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Core.Models
{
    public class EventListModel
    {
        public int EventId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? EventTypeId { get; set; }
        public string EventType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? RegStartDate { get; set; }
        public DateTime? RegEndDate { get; set; }
        public int? TimeZoneId { get; set; }
        public string TimeZone { get; set; }
        public int? OrganizationId { get; set; }
        public string Location { get; set; }
        public string Area { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Zip { get; set; }
        public int? ContactPersonEntityId { get; set; }
        public int? Status { get; set; }
        public string Summary { get; set; }
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
        public bool IsDueDateDialogSaveForAll { get; set; }
    }
}
