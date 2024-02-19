﻿using Newtonsoft.Json;

namespace AuthorizeNetCore.Models
{
	public class DeletePaymentProfileTransactionRequest
	{
		[JsonProperty(PropertyName = "merchantAuthentication")]
		public MerchantAuthentication MerchantAuthentication { get; set; }

		[JsonProperty(PropertyName = "customerProfileId")]
		public string CustomerProfileId { get; set; }

		[JsonProperty(PropertyName = "customerPaymentProfileId")]
		public string CustomerPaymentProfileId { get; set; }

	}
}