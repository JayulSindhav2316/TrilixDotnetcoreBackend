using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorizeNetCore.PaymentProfileModels
{
	public class Order
	{
		[JsonProperty(PropertyName = "invoiceNumber")]
		public string InvoiceNumber { get; set; }

		[JsonProperty(PropertyName = "description")]
		public string Description { get; set; }
	}
}
