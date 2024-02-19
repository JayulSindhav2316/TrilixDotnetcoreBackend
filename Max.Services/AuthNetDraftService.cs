using AutoMapper;
using Max.Core;
using Max.Core.Models;
using Max.Data.Interfaces;
using Max.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static Max.Core.Constants;

namespace Max.Services
{
    public class AuthNetDraftService : IAuthNetDraftService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPaymentTransactionService _paymentTransactionService;
        private readonly ITransactionService _transactionService;

        public AuthNetDraftService(IUnitOfWork unitOfWork,
                                IMapper mapper,
                                IPaymentTransactionService paymentTransactionService,
                                IPaymentProcessorService paymentProcessorService,
                                ITransactionService transactionService
                                )
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._paymentTransactionService = paymentTransactionService;
            this._transactionService = transactionService;
        }
        public async Task<AuthNetPaymentResponseModel> ChargePaymentProfile(AuthNetChargePaymentProfileRequestModel model)
        {/*
            var merchantAuthentication = new AuthorizeNetCore.ProfileTransaction.MerchantAuthentication()
            {
                name= model.MerchantLoginId,
                transactionKey = model.TransactionKey
            };

            // Add line Items
            LineItems lineItems = new LineItems();
            lineItems.lineItem = new LineItem[model.InvoiceDetails.Count()];
            int i = 0;
            foreach (var item in model.InvoiceDetails)
            {
                LineItem lineItem = new LineItem();

                lineItem.ItemId = item.InvoiceDetailId.ToString();
                lineItem.Name = item.Description;
                lineItem.Quantity = item.Quantity.ToString();
                lineItem.UnitPrice = item.Price.ToString();
                lineItems.lineItem[i++] = lineItem;
            }

            AuthorizeNetCore.ProfileTransaction.Profile profile = new AuthorizeNetCore.ProfileTransaction.Profile();
            AuthorizeNetCore.ProfileTransaction.PaymentProfile paymentProfile = new AuthorizeNetCore.ProfileTransaction.PaymentProfile();

            paymentProfile.paymentProfileId = model.PaymentProfileId;

            profile.paymentProfile = paymentProfile;
            profile.customerProfileId = model.ProfileId;


            var transactionRequest = new AuthorizeNetCore.ProfileTransaction.TransactionRequest
            {
                transactionType = "authCaptureTransaction",
                amount = model.InvoiceDetails.Sum(x => x.Price).ToString(),
                profile = profile,
                lineItems = lineItems
            };

            // Create PaymentRecord

            PaymentTransactionModel paymentTransaction = new PaymentTransactionModel();
            paymentTransaction.TransactionDate = DateTime.Now;
            paymentTransaction.ReceiptId = model.ReceiptId;
            paymentTransaction.PersonId = model.PersonId;
            paymentTransaction.Status = (int)PaymentTransactionStatus.Created;
            paymentTransaction.Amount = model.InvoiceDetails.Sum(x => x.Price);
            paymentTransaction.PaymentType = PaymentType.CREDITCARD;
            ChargePaymentProfileTransactionRequest chargePaymentRequest = new ChargePaymentProfileTransactionRequest();

            chargePaymentRequest.createTransactionRequest = new AuthorizeNetCore.ProfileTransaction.ChargeProfileTransactionRequest { merchantAuthentication = merchantAuthentication, refId = model.ReceiptId.ToString() + "-" + model.DraftId.ToString(), transactionRequest = transactionRequest };
            var request = chargePaymentRequest;

            //var profileTransaction = new AuthorizeNetCore.PaymentProfile(model.AuthNetUrl, merchantAuthentication.name, merchantAuthentication.transactionKey);
            // get the response from the service (errors contained if any)
            paymentTransaction.Status = (int)PaymentTransactionStatus.Submitted;
            // Serialize the object
            var stringContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");


            // Connect to Authorize.net
            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(model.AuthNetUrl, stringContent);


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
            }

            // Deserialize the response content
            var json = await response.Content.ReadAsStringAsync();
            */
            return new AuthNetPaymentResponseModel();
        }
    }
}
