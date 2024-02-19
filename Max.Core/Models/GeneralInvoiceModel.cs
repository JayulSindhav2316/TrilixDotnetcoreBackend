using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class GeneralInvoiceModel
    {
            public GeneralInvoiceModel()
            {
                InvoiceItems = new List<ItemModel>();
            }

            public int InvoiceId { get; set; }
            public DateTime Date { get; set; }
            public DateTime DueDate { get; set; }
            public string BillingType { get; set; }
            public string InvoiceType { get; set; }
            public int EntityId { get; set; }
            public int BillableEntityId { get; set; }
            public string Notes { get; set; }
            public int UserId { get; set; }
            public int organizationId { get; set; }

            [JsonProperty(PropertyName = "items")]
            public List<ItemModel> InvoiceItems { get; set; }
    }
}
