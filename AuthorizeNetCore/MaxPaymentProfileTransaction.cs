using AuthorizeNetCore.PaymentProfileModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AuthorizeNetCore
{
    public class MaxPaymentProfileTransaction
    {
		private readonly string _authorizeNetUrl;
		private readonly string _apiLoginId;
		private readonly string _transactionKey;
		private string _profileId;
		private string _paymentProfileId;
		private LineItems _lineItems;
		private string _transactionType;
		private string _amount;
		private string _invoiceNumber;
		private string _invoiceDescription;
		private int _receiptId;
		private int _autoBillingDraftId;
		private ChargePaymentProfileTransactionRequest _transactionRequest;
		public MaxPaymentProfileTransaction(string authorizeNetUrl, string apiLoginId, string transactionKey)
		{
			_authorizeNetUrl = authorizeNetUrl;
			_apiLoginId = apiLoginId;
			_transactionKey = transactionKey;
		}

		public string ProfileId
        {
			set { _profileId = value; }
		}
		public string PaymentProfileId
		{
			set { _paymentProfileId = value; }
		}

		public LineItems LineItems
		{
			set { _lineItems = value; }
		}
		public string TransactionType
		{
			set { _transactionType = value; }
		}
		public string Amount
		{
			set { _amount = value; }
		}

		public int ReceiptId
		{
			set { _receiptId = value; }
		}
		public int AutoBillingDraftId
		{
			set { _autoBillingDraftId = value; }
		}

		public string InvoiceNumber
		{
			set { _invoiceNumber = value; }
		}
		public string InvoiceDescription
		{
			set { _invoiceDescription = value; }
		}

		public string CreatePaymentRequest()
        {
			var merchantAuthentication = new MerchantAuthentication()
			{
				Name = _apiLoginId,
				TransactionKey = _transactionKey
			};

			Profile profile = new Profile();
			PaymentProfile paymentProfile = new PaymentProfile();

			paymentProfile.PaymentProfileId = _paymentProfileId;

			profile.PaymentProfile = paymentProfile;
			profile.CustomerProfileId = _profileId;

			Order order = new Order();

			order.InvoiceNumber = _invoiceNumber;
			order.Description = _invoiceDescription;

			var transactionRequest = new TransactionRequest
			{
				TransactionType = "authCaptureTransaction",
				Amount = _amount,
				Profile = profile,
				Order = order,
				LineItems = _lineItems
			};

			ChargePaymentProfileTransactionRequest chargePaymentRequest = new ChargePaymentProfileTransactionRequest();

			chargePaymentRequest.CreateTransactionRequest = new ChargeProfileTransactionRequest { 
																				MerchantAuthentication = merchantAuthentication, 
																				RefId = $"{_receiptId}-{_autoBillingDraftId}", 
																				TransactionRequest = transactionRequest 
															};
			_transactionRequest = chargePaymentRequest;
			return (JsonConvert.SerializeObject(_transactionRequest));
		}

		public async Task<PaymentTransactionResponce> ProcessPayment()
        {
			var stringContent = new StringContent(JsonConvert.SerializeObject(_transactionRequest), Encoding.UTF8, "application/json");


			// Connect to Authorize.net
			var httpClient = new HttpClient();
			var response = await httpClient.PostAsync(_authorizeNetUrl, stringContent);


			// If response is not successful, return appropriate transaction response
			if (!response.IsSuccessStatusCode)
			{
				// store results
				var resultMessage = new ResultMessage
				{
					Code = response.StatusCode.ToString(),
					Text = response.ReasonPhrase
				};

				var resultMessages = new ResultMessage[1];
				resultMessages[0] = resultMessage;

				var responseType = typeof(PaymentTransactionResponce);
				var responseDTO = (PaymentTransactionResponce)Activator.CreateInstance(responseType);

				// set the Result Property of TResponse
				var resultProp = responseType.GetProperty("Messages");
				resultProp.SetValue(responseDTO, new Results
				{
					ResultCode = "Error",
					ResultMessages = resultMessages
				});

				return responseDTO;
			}
			var json = await response.Content.ReadAsStringAsync();
			PaymentTransactionResponce paymentResponse = JsonConvert.DeserializeObject<PaymentTransactionResponce>(json);
			// Deserialize the response content
			//var json = await response.Content.ReadAsStringAsync();
			//return json;
			return paymentResponse;
		}
	}
}
