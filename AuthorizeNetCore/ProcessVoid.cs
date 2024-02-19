using AuthorizeNetCore.VoidModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AuthorizeNetCore
{
	public class VoidTransaction
	{
		private readonly string _authorizeNetUrl;
		private readonly string _apiLoginId;
		private readonly string _transactionKey;

		private string _transactionId;
		private int _receiptId;
		private string _referenceTransactionId;

		private VoidTransactionRequest _voidRequest;

		public VoidTransaction(string authorizeNetUrl, string apiLoginId, string transactionKey)
		{
			_authorizeNetUrl = authorizeNetUrl;
			_apiLoginId = apiLoginId;
			_transactionKey = transactionKey;
		}

		public string TransactionId
		{
			set { _transactionId = value; }
		}
		public int ReceiptId
		{
			set { _receiptId = value; }
		}
		public string ReferenceransactionId
		{
			set { _referenceTransactionId = value; }
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
				RefTransId = _referenceTransactionId
			};

			VoidTransactionRequest voidRequest = new VoidTransactionRequest();

			voidRequest.CreateTransactionRequest = new CreateTransactionRequest
			{
				MerchantAuthentication = merchantAuthentication,
				RefId = $"{_receiptId}",
				TransactionRequest = transactionRequest
			};

			_voidRequest = voidRequest;
			return (JsonConvert.SerializeObject(voidRequest));
		}

		public async Task<string> ProcessVoid()
		{
			var stringContent = new StringContent(JsonConvert.SerializeObject(_voidRequest), Encoding.UTF8, "application/json");


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
