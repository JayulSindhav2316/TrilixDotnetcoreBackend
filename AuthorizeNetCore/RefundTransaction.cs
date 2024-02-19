using AuthorizeNetCore.RefundModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AuthorizeNetCore
{
    public class RefundTransaction
    {
		private readonly string _authorizeNetUrl;
		private readonly string _apiLoginId;
		private readonly string _transactionKey;

		private string _transactionId;
		private string _creditCardNumber;
		private string _amount;
		private int _receiptId;
		private int _receiptDetailId;
		private string _refundTransactionId;

		private VoidTransactionRequest _refundRequest;

		public RefundTransaction(string authorizeNetUrl, string apiLoginId, string transactionKey)
		{
			_authorizeNetUrl = authorizeNetUrl;
			_apiLoginId = apiLoginId;
			_transactionKey = transactionKey;
		}

		public string TransactionId
		{
			set { _transactionId = value; }
		}

		public string CreditCardNumber
		{
			set { _creditCardNumber = value; }
		}

		public string Amount
		{
			set { _amount = value; }
		}
		public int ReceiptId
		{
			set { _receiptId = value; }
		}
		public int ReceiptDetailId
		{
			set { _receiptDetailId = value; }
		}
		public string RefundTransactionId
		{
			set { _refundTransactionId = value; }
		}

		public string CreateVoidRequest()
		{
			var merchantAuthentication = new MerchantAuthentication()
			{
				Name = _apiLoginId,
				TransactionKey = _transactionKey
			};


			var transactionRequest = new TransactionRequest
			{
				TransactionType = "voidTransaction",
				RefTransId = _refundTransactionId
			};

			VoidTransactionRequest refundRequest = new VoidTransactionRequest();

			refundRequest.CreateTransactionRequest = new CreateTransactionRequest
			{
				MerchantAuthentication = merchantAuthentication,
				RefId = $"{_receiptId}",
				TransactionRequest = transactionRequest
			};

			_refundRequest = refundRequest;
			return (JsonConvert.SerializeObject(refundRequest));
		}

		public string CreateRefundRequest()
		{
			var merchantAuthentication = new MerchantAuthentication()
			{
				Name = _apiLoginId,
				TransactionKey = _transactionKey
			};

			RefundModels.CreditCard creditCard = new RefundModels.CreditCard();
			creditCard.CardNumber = _creditCardNumber;
			creditCard.ExpirationDate = "xxxx";

			var payment = new Payment();
			payment.CreditCard = creditCard;

			var transactionRequest = new TransactionRequest
			{
				TransactionType = "refundTransaction",
				Amount = _amount,
				Payment = payment,
				RefTransId = _refundTransactionId
			};

			VoidTransactionRequest refundRequest = new VoidTransactionRequest();

			refundRequest.CreateTransactionRequest = new CreateTransactionRequest
			{
				MerchantAuthentication = merchantAuthentication,
				RefId = $"{_receiptId}-{_receiptDetailId}",
				TransactionRequest = transactionRequest
			};

			_refundRequest = refundRequest;
			return (JsonConvert.SerializeObject(refundRequest));
		}
		public async Task<string> ProcessRefund()
		{
			var stringContent = new StringContent(JsonConvert.SerializeObject(_refundRequest), Encoding.UTF8, "application/json");


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
					Description = response.ReasonPhrase
				};

				var resultMessages = new ResultMessage[1];
				resultMessages[0] = resultMessage;

				var responseType = typeof(VoidResponseModel);
				var responseDTO = (VoidResponseModel)Activator.CreateInstance(responseType);

				// set the Result Property of TResponse
				var resultProp = responseType.GetProperty("Messages");
				resultProp.SetValue(responseDTO, new Message
				{
					Code = "",
					Text = ""
				});

				return (JsonConvert.SerializeObject(responseDTO));
			}

			// Deserialize the response content
			var json = await response.Content.ReadAsStringAsync();
			return json;
		}
	}
}
