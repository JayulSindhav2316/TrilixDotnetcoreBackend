
using AuthorizeNetCore;
using AuthorizeNetCore.Models;
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
using Serilog;
using Serilog.Events;
using Max.Data.DataModel;

namespace Max.Services
{
    /// <summary>
    /// This implementation has been done using .Net core implementation
    /// based on https://github.com/biBERK/AuthorizeNet-dotnetcore
    /// </summary>
    public class AuthNetService : IAuthNetService
    {
        static readonly ILogger _logger = Serilog.Log.ForContext<AuthNetService>();
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPaymentTransactionService _paymentTransactionService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IPaymentProcessorService _paymentProcessorService;
        private readonly IPaymentProfileService _paymentProfileService;
        private readonly IPersonService _personService;
        private readonly ITransactionService _transactionService;
        private readonly IOrganizationService _organizationService;

        public AuthNetService(IUnitOfWork unitOfWork,
                                IMapper mapper,
                                IPaymentTransactionService paymentTransactionService,
                                IShoppingCartService shoppingCartService,
                                IPaymentProcessorService paymentProcessorService,
                                IPaymentProfileService paymentProfileService,
                                IPersonService personService,
                                ITransactionService transactionService,
                                IOrganizationService organizationService
                                )
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._paymentTransactionService = paymentTransactionService;
            this._shoppingCartService = shoppingCartService;
            this._paymentProcessorService = paymentProcessorService;
            this._paymentProfileService = paymentProfileService;
            this._personService = personService;
            this._transactionService = transactionService;
            this._organizationService = organizationService;
        }
        public async Task<AuthNetPaymentResponseModel> ProcessAcceptPayment(AuthNetSecureDataModel model)
        {
            string primaryEMail = string.Empty;

            // Get processor details
            var paymentprocessor = await _paymentProcessorService.GetPaymentProcessorByOrganizationId(model.OrganizationId);

            // Get shopping Cart Details

            var cart = await _unitOfWork.ShoppingCarts.GetShoppingCartByIdAsync(model.CartId);

            // define the merchant information (authentication / transaction id)
            var merchantAuthentication = new MerchantAuthentication()
            {
                LoginId = paymentprocessor.LoginId,
                TransactionKey = paymentprocessor.TransactionKey
            };

            var opaqueData = new OpaqueData
            {
                NonceValue = model.DataValue
            };
            var billingAddress = new CustomerContact();
            var entity = await _unitOfWork.Entities.GetEntityByIdAsync(cart.EntityId ?? 0);
            if (entity.CompanyId != null)
            {
                var company = await _unitOfWork.Companies.GetCompanyByIdAsync(entity.CompanyId ?? 0);

                billingAddress.FirstName = company.CompanyName.Truncate(45);
                billingAddress.LastName = string.Empty;
                //billingAddress.Address = company.StreetAddress.Truncate(55);
                //billingAddress.City = company.City.Truncate(35);
                //billingAddress.State = company.State.Truncate(35);
                //billingAddress.Zip = company.Zip.Truncate(19);
            }
            else if (entity.PersonId != null)
            {
                var person = await _unitOfWork.Persons.GetPersonByIdAsync(entity.PersonId ?? 0);
                PersonModel personModel = _mapper.Map<PersonModel>(person);
                var primaryAddress = personModel.Addresses.GetPrimaryAddress();
                primaryEMail = personModel.Emails.GetPrimaryEmail();

                billingAddress.FirstName = person.FirstName.Truncate(45);
                billingAddress.LastName = person.LastName.Truncate(45);
                billingAddress.Address = primaryAddress.StreetAddress.Truncate(55);
                billingAddress.City = primaryAddress.City.Truncate(35);
                billingAddress.State = primaryAddress.State.Truncate(35);
                billingAddress.Zip = primaryAddress.Zip.Truncate(19);
            }

            //standard api call to retrieve response
            var paymentType = new AuthorizeNetCore.Models.Payment { OpaqueData = opaqueData };

            // Add line Items
            LineItems lineItems = new LineItems();
            lineItems.LineItem = new LineItem[cart.Shoppingcartdetails.Count()];
            int i = 0;
            foreach (var item in cart.Shoppingcartdetails)
            {
                LineItem lineItem = new LineItem();

                lineItem.ItemId = item.ItemId.ToString();
                lineItem.Name = item.Description.Truncate(25);
                lineItem.Quantity = item.Quantity == 0? "1" : item.Quantity.ToString();
                lineItem.UnitPrice = item.Price.ToString();
                lineItem.Description = item.Description.Truncate(25);
                lineItems.LineItem[i++] = lineItem;
            }

            decimal creditBalance = 0;
            decimal paymentAmount = cart.Shoppingcartdetails.Sum(x => x.Amount);

            if (cart.UseCreditBalance > 0)
            {
                creditBalance = await _unitOfWork.CreditTransactions.GetCreditBalanceByEntityIdAsync(entity.EntityId);
                paymentAmount = paymentAmount - creditBalance;
            }

            var transactionRequest = new TransactionRequest
            {
                Amount = paymentAmount.ToString(),
                Customer = new Customer { Id = "MemberMax" },
                CustomerIP = Helpers.Helper.GetLocalIPAddress(),
                Duty = new Duty(),
                Payment = paymentType,
                BillTo = billingAddress,
                LineItems = lineItems,
                Order = new Order
                {
                    InvoiceNumber = cart.ReceiptId.ToString()
                },
                Shipping = new Shipping(),
                Tax = new Tax()
            };

            // Create PaymentRecord

            PaymentTransactionModel paymentTransaction = new PaymentTransactionModel();
            paymentTransaction.TransactionDate = DateTime.Now;
            paymentTransaction.ReceiptId = cart.ReceiptId;
            paymentTransaction.EntityId = cart.EntityId;
            paymentTransaction.ShoppingCartId = cart.ShoppingCartId;
            paymentTransaction.Status = (int)PaymentTransactionStatus.Created;
            paymentTransaction.Amount = paymentAmount;
            paymentTransaction.CreditBalanceUsed = creditBalance;
            paymentTransaction.PaymentType = model.PaymentMode;

            paymentTransaction.AccountHolderName = model.PaymentMode == PaymentType.ECHECK ? model.AccountHolderName : string.Empty;
            paymentTransaction.BankName = model.PaymentMode == PaymentType.ECHECK ? model.BankName : string.Empty;
            paymentTransaction.AccountType = model.PaymentMode == PaymentType.ECHECK ? model.AccountType : string.Empty;
            paymentTransaction.RoutingNumber = model.PaymentMode == PaymentType.ECHECK ? model.RoutingNumber : string.Empty;
            paymentTransaction.NickName = model.PaymentMode == PaymentType.ECHECK ? model.NickName : string.Empty;

            var request = new CreateTransactionRequest { MerchantAuthentication = merchantAuthentication, ReferenceId = cart.EntityId.ToString(), TransactionRequest = transactionRequest };

            //Chcek for Test mode
            var processorUrl = string.Empty;

            if (paymentprocessor.TransactionMode == (int)PaymentTransactionMode.Live)
            {
                processorUrl = paymentprocessor.LiveUrl;
            }
            else
            {
                processorUrl = paymentprocessor.TestUrl;
            }

            var creditCard = new AuthorizeNetCore.CreditCard(processorUrl, merchantAuthentication.LoginId, merchantAuthentication.TransactionKey);
            // get the response from the service (errors contained if any)
            paymentTransaction.Status = (int)PaymentTransactionStatus.Submitted;
            var response = await creditCard.ChargeAsync(request);
            _logger.Information($"creditCard.ChargeAsync - EntityId: {cart.EntityId} Response: {JsonConvert.SerializeObject(response.TransactionResponse)}");
            AuthNetPaymentResponseModel responseModel = new AuthNetPaymentResponseModel();
            if (response != null)
            {
                paymentTransaction.Result = response.IsSuccessful ? 1 : 0;
                if (response.IsSuccessful)
                {
                    // Update PaymentTransaction Model
                    if (response.TransactionResponse.ResponseCode == "1")
                    {
                        paymentTransaction.Status = (int)PaymentTransactionStatus.Approved;
                    }
                    else
                    {
                        paymentTransaction.Status = (int)PaymentTransactionStatus.Declined;
                    }

                    paymentTransaction.TransactionId = response.TransactionResponse.TransId;
                    paymentTransaction.ResponseDetails = JsonConvert.SerializeObject(response.TransactionResponse);
                    paymentTransaction.MessageDetails = JsonConvert.SerializeObject(response.Results);
                    paymentTransaction.ResponseCode = response.TransactionResponse.ResponseCode;
                    paymentTransaction.AuthCode = response.TransactionResponse.AuthCode;
                    paymentTransaction.AccountNumber = response.TransactionResponse.AccountNumber;
                    paymentTransaction.CardType = response.TransactionResponse.AccountType;
                }
                else
                {
                    paymentTransaction.MessageDetails = JsonConvert.SerializeObject(response.Results);
                    paymentTransaction.Status = (int)PaymentTransactionStatus.Declined;
                    if (response.TransactionResponse != null && response.TransactionResponse.Errors != null)
                    {
                        paymentTransaction.ResponseDetails = JsonConvert.SerializeObject(response.TransactionResponse);
                    }
                }
                //Update PaymentTransaction with Response

                await _paymentTransactionService.CreatePaymentTransaction(paymentTransaction);
                var updateshoppingCartResult = await _shoppingCartService.UpdateShoppingCartPaymentStatus(cart.UserId ?? 0, cart.ShoppingCartId, paymentTransaction.Status ?? 0, creditBalance, paymentTransaction.PaymentType);
                if (paymentTransaction.Status == (int)PaymentTransactionStatus.Approved)
                {
                    //Create GL Entries
                    await _transactionService.UpdateTransactionStatus(paymentTransaction);
                }

                //Check if need to create payment profile

                if (model.SavePaymentProfile == (int)Status.Active)
                {

                    var currentCustomerProfiles = await _paymentProfileService.GetPaymentProfileByEntityId(paymentTransaction.EntityId ?? 0);
                    var organization = await _organizationService.GetOrganizationById(model.OrganizationId);
                    var profileDescription = $"{organization.Name}-Payment Profile";
                    if (!currentCustomerProfiles.Any(x=>x.Status==(int)Status.Active))
                    {
                        //Create Payment profile

                        var customerProfile = new CustomerProfile(processorUrl, paymentprocessor.LoginId, paymentprocessor.TransactionKey);

                        var profileResponse = await customerProfile.CreateAsync(paymentTransaction.EntityId.ToString(), paymentTransaction.EntityId.ToString(), primaryEMail, profileDescription, response.TransactionResponse.TransId);

                        PaymentProfileModel authNetpaymentProfile = new PaymentProfileModel();
                        AuthNetPaymentProfileResponseModel profileResponseModel = new AuthNetPaymentProfileResponseModel();
                        _logger.Information($"customerProfile.CreateAsync -EntityId: {cart.EntityId} Response: {JsonConvert.SerializeObject(profileResponse)}");
                        if (profileResponse.IsSuccessful)
                        {
                            authNetpaymentProfile.EntityId = paymentTransaction.EntityId ?? 0;
                            authNetpaymentProfile.ProfileId = profileResponse.CustomerProfileId;
                            if (model.PaymentMode == PaymentType.CREDITCARD)
                            {
                                CreditCardPaymentProfile creditCardProfile = new CreditCardPaymentProfile();
                                creditCardProfile.AuthNetPaymentProfileId = profileResponse.CustomerPaymentProfileIds[0];
                                creditCardProfile.CardNumber = response.TransactionResponse.AccountNumber;
                                creditCardProfile.CardType = response.TransactionResponse.AccountType;
                                creditCardProfile.CardHolderName = model.AccountHolderName;
                                authNetpaymentProfile.CreditCards.Add(creditCardProfile);
                            }
                            else
                            {
                                BankAccountPaymentProfile bankAccountProfile = new BankAccountPaymentProfile();
                                bankAccountProfile.AuthNetPaymentProfileId = profileResponse.CustomerPaymentProfileIds[0];
                                bankAccountProfile.AccountNumber = response.TransactionResponse.AccountNumber;
                                bankAccountProfile.AccountType = response.TransactionResponse.AccountType;
                                bankAccountProfile.NickName = paymentTransaction.NickName;
                                authNetpaymentProfile.BankAccounts.Add(bankAccountProfile);
                            }
                            var profile = await _paymentProfileService.CreatePaymentProfile(authNetpaymentProfile);
                        }
                    }
                }

                if (response.IsSuccessful)
                {
                    if (response.TransactionResponse.ResponseCode == "1")
                    {
                        responseModel.TransactionStatus = (int)PaymentTransactionStatus.Approved;
                    }
                    else
                    {
                        paymentTransaction.Status = (int)PaymentTransactionStatus.Declined;
                    }

                    if (response.TransactionResponse.Messages != null)
                    {
                        responseModel.TransactionId = response.TransactionResponse.TransId;
                        responseModel.ResponseCode = response.TransactionResponse.ResponseCode;
                        responseModel.MessageCode = response.TransactionResponse.Messages[0].Code;
                        responseModel.MessageDescription = response.TransactionResponse.Messages[0].Description;
                        responseModel.AuthCode = response.TransactionResponse.AuthCode;
                    }
                    else
                    {
                        if (response.TransactionResponse.Errors != null)
                        {
                            responseModel.ErrorCode = response.TransactionResponse.Errors[0].Code;
                            responseModel.ErrorMessage = response.TransactionResponse.Errors[0].Text;
                        }
                    }
                }
                else
                {

                    if (response.TransactionResponse != null && response.TransactionResponse.Errors != null)
                    {
                        responseModel.ErrorCode = response.TransactionResponse.Errors[0].Code;
                        responseModel.ErrorMessage = response.TransactionResponse.Errors[0].Text;
                    }
                    else
                    {
                        responseModel.ErrorCode = response.Results.ResultMessages[0].Code;
                        responseModel.ErrorMessage = response.Results.ResultMessages[0].Text;
                    }
                }
            }
            else
            {
                //May be it times out.
                paymentTransaction.Status = (int)PaymentTransactionStatus.NoResponse;
                await _paymentTransactionService.CreatePaymentTransaction(paymentTransaction);
                responseModel.ErrorCode = "No Response";
                responseModel.MessageDescription = "No Response Received from Authorize.Net";
            }

            return responseModel;
        }

        public async Task<AuthNetPaymentProfileResponseModel> ProcessPaymentProfile(AuthNetSecureDataModel model)
        {
            // Get processor details
            var paymentProcessor = await _paymentProcessorService.GetPaymentProcessorByOrganizationId(model.OrganizationId);


            // define the merchant information (authentication / transaction id)
            var merchantAuthentication = new MerchantAuthentication()
            {
                LoginId = paymentProcessor.LoginId,
                TransactionKey = paymentProcessor.TransactionKey
            };

            var opaqueData = new OpaqueData
            {
                NonceValue = model.DataValue
            };

            var billingAddress = new CustomerContact();
            var entity = await _unitOfWork.Entities.GetEntityByIdAsync(model.BillableEntityId > 0 ? model.BillableEntityId : model.EntityId);
            string primaryEmail = string.Empty;
            if (entity.CompanyId != null)
            {
                var company = await _unitOfWork.Companies.GetCompanyByIdAsync(entity.CompanyId ?? 0);

                billingAddress.FirstName = company.CompanyName.Truncate(45);
                billingAddress.LastName = string.Empty;
                //billingAddress.Address = company.StreetAddress.Truncate(55);
                //billingAddress.City = company.City.Truncate(35);
                //billingAddress.State = company.State.Truncate(35);
                //billingAddress.Zip = company.Zip.Truncate(19);
                primaryEmail = company.Email;
            }
            else if (entity.PersonId != null)
            {
                var person = await _unitOfWork.Persons.GetPersonByIdAsync(entity.PersonId ?? 0);
                PersonModel personModel = _mapper.Map<PersonModel>(person);
                var primaryAddress = personModel.Addresses.GetPrimaryAddress();

                billingAddress.FirstName = person.FirstName.Truncate(45);
                billingAddress.LastName = person.LastName.Truncate(45);
                billingAddress.Address = primaryAddress.StreetAddress.Truncate(55);
                //billingAddress.City = primaryAddress.City.Truncate(35);
                //billingAddress.State = primaryAddress.State.Truncate(35);
                billingAddress.Zip = primaryAddress.Zip.Truncate(19);
                primaryEmail = personModel.Emails.GetPrimaryEmail();
            }
            if(!string.IsNullOrEmpty(model.FullName))
            {
                billingAddress.FirstName = model.FullName;
                billingAddress.LastName = string.Empty;
            }
            if(!string.IsNullOrEmpty(model.StreetAddress))
            {
                billingAddress.Address = model.StreetAddress;
            }
            if (!string.IsNullOrEmpty(model.Zip))
            {
                billingAddress.Zip = model.Zip;
            }
            //Chcek for Test mode
            var processorUrl = string.Empty;

            if (paymentProcessor.TransactionMode == (int)PaymentTransactionMode.Live)
            {
                processorUrl = paymentProcessor.LiveUrl;
            }
            else
            {
                processorUrl = paymentProcessor.TestUrl;
            }
            var organization = await _organizationService.GetOrganizationById(model.OrganizationId);
            var profileDescription = $"{organization.Name}-Payment Profile";

            //Check if entity has a custom profile

            var currentCustomerProfiles = await _paymentProfileService.GetPaymentProfileByEntityId(model.EntityId);
            string profileId = string.Empty;
            AuthNetPaymentProfileResponseModel responseModel = new AuthNetPaymentProfileResponseModel();
            if (currentCustomerProfiles.IsNullOrEmpty())
            {
                var customerProfile = new CustomerProfile(processorUrl, paymentProcessor.LoginId, paymentProcessor.TransactionKey);

                var response = await customerProfile.CreateAsync(model.EntityId.ToString(), model.EntityId.ToString(), primaryEmail, profileDescription);
                _logger.Information($"customerProfile.CreateAsync - EntityId: {model.EntityId} Response: {JsonConvert.SerializeObject(response)}");
                if (!response.IsSuccessful)
                {
                    responseModel.EntityId = model.EntityId;
                    responseModel.ProfileId = string.Empty;
                    responseModel.AuthNetPaymentProfileId = string.Empty;
                    responseModel.ErrorMessage = response.Results.ResultMessages[0].Text;
                    return responseModel;
                }
                profileId = response.CustomerProfileId;
            }
            else
            {
                profileId = currentCustomerProfiles.Select(x => x.ProfileId).FirstOrDefault();

            }
            //Create Customer Payment Profile

            var paymentProfile = new CreateCustomerPaymentProfile
            {
                BillTo = billingAddress,
                Payment = new AuthorizeNetCore.Models.Payment { OpaqueData = opaqueData },
                DefaultPaymentProfile = true
            };
            var paymentProfileRequest = new CreateCustomerPaymentProfileTransactionRequest
            {
                MerchantAuthentication = merchantAuthentication,
                ReferenceId = model.EntityId.ToString(),
                CustomerProfileId = profileId,
                CustomerPaymentProfile = paymentProfile
            };

            var request = new CreateCustomerPaymentProfileRequest();

            request.CustomerPaymentProfileTransactionRequest = paymentProfileRequest;

            var customerPaymentProfile = new CustomerPaymentProfile(processorUrl, paymentProcessor.LoginId, paymentProcessor.TransactionKey);

            var paymentProfileResponse = await customerPaymentProfile.CreateAsync(request);
            _logger.Information($"customerPaymentProfile.CreateAsync - EntityId: {model.EntityId} Response: {JsonConvert.SerializeObject(paymentProfileResponse)}");
            PaymentProfileModel authNetpaymentProfile = new PaymentProfileModel();


            if (paymentProfileResponse.IsSuccessful)
            {
                authNetpaymentProfile.EntityId = model.EntityId;
                authNetpaymentProfile.ProfileId = profileId;
                if (model.PaymentMode == "CreditCard")
                {
                    CreditCardPaymentProfile creditcard = new CreditCardPaymentProfile();
                    creditcard.AuthNetPaymentProfileId = paymentProfileResponse.CustomerPaymentProfileId;
                    authNetpaymentProfile.CreditCards.Add(creditcard);
                }
                else
                {
                    BankAccountPaymentProfile bankAccount = new BankAccountPaymentProfile();
                    bankAccount.AuthNetPaymentProfileId = paymentProfileResponse.CustomerPaymentProfileId;
                    bankAccount.NickName = model.NickName;
                    authNetpaymentProfile.BankAccounts.Add(bankAccount);
                }

                var profile = await _paymentProfileService.CreatePaymentProfile(authNetpaymentProfile);
                responseModel.EntityId = profile.EntityId;
                responseModel.ProfileId = profile.ProfileId;
                responseModel.AuthNetPaymentProfileId = profile.AuthNetPaymentProfileId;
                responseModel.ErrorMessage = String.Empty;
            }
            else
            {
                responseModel.EntityId = model.EntityId;
                responseModel.ProfileId = String.Empty;
                responseModel.AuthNetPaymentProfileId = String.Empty;
                responseModel.ErrorMessage = paymentProfileResponse.Results.ResultMessages[0].Text;
            }
            return responseModel;
        }

        public async Task<PaymentProfileModel> GetPaymentProfile(AuthNetPaymentProfileRequestModel model)
        {
            PaymentProfileModel paymentProfile = new PaymentProfileModel();
            // Get processor details
            var paymentProcessor = await _paymentProcessorService.GetPaymentProcessorByOrganizationId(model.OrganizationId);

            // define the merchant information (authentication / transaction id)
            var merchantAuthentication = new MerchantAuthentication()
            {
                LoginId = paymentProcessor.LoginId,
                TransactionKey = paymentProcessor.TransactionKey
            };
            var profiles = await _unitOfWork.PaymentProfiles.GetPaymentProfileByEntityIdAsync(model.EntityId);
            var profile = profiles.Where(x => x.ProfileId != null).FirstOrDefault();
            if (profile == null)
            {
                return paymentProfile;
            }
            var getCustomerProfileRequest = new GetCustomerProfileRequest
            {
                GetProfileTransactionRequest = new GetProfileTransactionRequest
                {
                    MerchantAuthentication = new MerchantAuthentication
                    {
                        LoginId = paymentProcessor.LoginId,
                        TransactionKey = paymentProcessor.TransactionKey
                    },
                    CustomerProfileId = profile.ProfileId,
                    UnmaskExpirationDate = true,
                    IncludeIssuerInfo = true
                }
            };

            // Serialize the object
            var stringContent = new StringContent(JsonConvert.SerializeObject(getCustomerProfileRequest), Encoding.UTF8, "application/json");

            //Chcek for Test mode
            var processorUrl = string.Empty;

            if (paymentProcessor.TransactionMode == (int)PaymentTransactionMode.Live)
            {
                processorUrl = paymentProcessor.LiveUrl;
            }
            else
            {
                processorUrl = paymentProcessor.TestUrl;
            }

            // Connect to Authorize.net
            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(processorUrl, stringContent);
            var responseJson = await response.Content.ReadAsStringAsync();
            _logger.Information($"GetPaymentProfile - Entity Id:{model.EntityId} - Response: {responseJson}");
            GetPaymentProfileResponse authNetResponse = JsonConvert.DeserializeObject<GetPaymentProfileResponse>(responseJson);

            if (authNetResponse != null)
            {
                if (authNetResponse.profile.paymentProfiles != null)
                {
                    foreach (var authNetPaymentProfile in authNetResponse.profile.paymentProfiles)
                    {
                        paymentProfile.ProfileId = authNetResponse.profile.customerProfileId;
                        paymentProfile.EntityId = int.Parse(authNetResponse.profile.merchantCustomerId);

                        if (authNetPaymentProfile.payment.creditCard != null)
                        {
                            CreditCardPaymentProfile creditCardProfile = new CreditCardPaymentProfile();
                            creditCardProfile.CardHolderName = $"{authNetPaymentProfile.billTo.firstName} {authNetPaymentProfile.billTo.lastName}";
                            creditCardProfile.CardNumber = authNetPaymentProfile.payment.creditCard.cardNumber;
                            creditCardProfile.CardType = authNetPaymentProfile.payment.creditCard.cardType;
                            creditCardProfile.ExpirationDate = authNetPaymentProfile.payment.creditCard.expirationDate;
                            creditCardProfile.PreferredPaymentMethod = authNetPaymentProfile.defaultPaymentProfile ? 1 : 0;
                            creditCardProfile.AuthNetPaymentProfileId = authNetPaymentProfile.customerPaymentProfileId;
                            paymentProfile.CreditCards.Add(creditCardProfile);
                        }
                        else
                        {
                            BankAccountPaymentProfile bankAccountProfile = new BankAccountPaymentProfile();
                            bankAccountProfile.NameOnAccount = authNetPaymentProfile.payment.bankAccount.nameOnAccount;
                            bankAccountProfile.AccountType = authNetPaymentProfile.payment.bankAccount.accountType;
                            bankAccountProfile.AccountNumber = authNetPaymentProfile.payment.bankAccount.accountNumber;
                            bankAccountProfile.RoutingNumber = authNetPaymentProfile.payment.bankAccount.routingNumber;
                            bankAccountProfile.PreferredPaymentMethod = authNetPaymentProfile.defaultPaymentProfile ? 1 : 0;
                            bankAccountProfile.AuthNetPaymentProfileId = authNetPaymentProfile.customerPaymentProfileId;
                            paymentProfile.BankAccounts.Add(bankAccountProfile);
                        }
                    }
                }
            }
            var updatedProfiles = await _paymentProfileService.UpdatePaymentProfile(paymentProfile);
            return updatedProfiles;
        }

        public async Task<AuthNetVoidModel> ProcessCreditCardVoid(CreditCardVoidModel model)
        {
            var resModel = new AuthNetVoidModel();

            // Get processor details
            var paymentProcessor = await _paymentProcessorService.GetPaymentProcessorByOrganizationId(model.OrganizationId);

            VoidTransaction voidTransaction = new VoidTransaction(paymentProcessor.TransactionMode == (int)PaymentTransactionMode.Live ? paymentProcessor.LiveUrl : paymentProcessor.TestUrl, paymentProcessor.LoginId, paymentProcessor.TransactionKey);
            voidTransaction.ReferenceransactionId = model.ReferenceTransactionId;
            voidTransaction.ReceiptId = model.ReceiptId;

            var request = voidTransaction.CreateVoidRequest();

            // Create PaymentRecord
            PaymentTransactionModel paymentTransaction = new PaymentTransactionModel();
            paymentTransaction.TransactionDate = DateTime.Now;
            paymentTransaction.ReceiptId = model.ReceiptId;
            paymentTransaction.EntityId = model.EntityId;
            paymentTransaction.Status = (int)PaymentTransactionStatus.Created;
            paymentTransaction.Amount = model.VoidAmount;
            paymentTransaction.PaymentType = model.PaymentMode;
            paymentTransaction.TransactionType = (int)PaymentTransactionType.Void;

            var response = await voidTransaction.ProcessVoid();
            _logger.Information($"ProcessCreditCardVoid - Entity Id:{model.EntityId} - Response: {response}");
            dynamic responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject(response);

            if (responseObject.transactionResponse != null)
            {
                var txResponse = responseObject.transactionResponse;
                var responseMessages = (IEnumerable<dynamic>)txResponse.messages;
                if (responseMessages != null && responseMessages.Count()>0)
                {
                    var responseMessage = responseMessages.FirstOrDefault();
                    if (responseMessage.code == 310)
                    {
                        resModel.IsPaymentAlreadyVoided = true;
                        resModel.PaymentVoidMessage= responseMessage.description;
                    }
                }
                if (txResponse.responseCode == "1")
                {
                    paymentTransaction.Status = (int)PaymentTransactionStatus.Approved;
                    paymentTransaction.TransactionId = txResponse.transId;
                    paymentTransaction.ResponseDetails = response;
                    paymentTransaction.MessageDetails = JsonConvert.SerializeObject(responseObject.messages);
                    paymentTransaction.ResponseCode = txResponse.responseCode;
                    paymentTransaction.AuthCode = txResponse.authCode;
                    paymentTransaction.AccountNumber = txResponse.accountNumber;
                    paymentTransaction.CardType = txResponse.accountType;
                    paymentTransaction.Result = (int)ReceiptStatus.Active;
                    paymentTransaction.ReferenceTransactionId = model.ReferenceTransactionId;
                }
                else
                {
                    paymentTransaction.ResponseCode = txResponse.responseCode;
                    paymentTransaction.Status = (int)PaymentTransactionStatus.Declined;
                    paymentTransaction.Result = (int)ReceiptStatus.Created;
                    paymentTransaction.MessageDetails = JsonConvert.SerializeObject(responseObject.messages);
                    if (txResponse != null && txResponse.errors != null)
                    {
                        paymentTransaction.ResponseDetails = response;
                    }
                }
            }
            else
            {
                paymentTransaction.Status = (int)PaymentTransactionStatus.NoResponse;
                paymentTransaction.Result = (int)ReceiptStatus.Created;
                paymentTransaction.MessageDetails = JsonConvert.SerializeObject(responseObject.messages);
            }
            //Update PaymentTransaction with Response
            var paymentResult = await _paymentTransactionService.CreatePaymentTransaction(paymentTransaction);

            resModel.PaymentTransactionId = paymentResult.PaymentTransactionId;
            return resModel;

        }
        public async Task<int> ProcessCreditCardRefund(CreditCardRefundModel model)
        {
            // Get processor details
            var paymentProcessor = await _paymentProcessorService.GetPaymentProcessorByOrganizationId(model.OrganizationId);

            RefundTransaction refundTransaction = new RefundTransaction(paymentProcessor.TransactionMode == (int)PaymentTransactionMode.Live ? paymentProcessor.LiveUrl : paymentProcessor.TestUrl, paymentProcessor.LoginId, paymentProcessor.TransactionKey);

            refundTransaction.TransactionId = model.RefundTransactionId;
            refundTransaction.CreditCardNumber = model.CreditCardNumber;
            refundTransaction.Amount = model.RefundAmount.ToString();
            refundTransaction.ReceiptId = model.ReceiptId;
            refundTransaction.ReceiptDetailId = model.ReceiptDetailId;
            refundTransaction.RefundTransactionId = model.RefundTransactionId;

            var request = refundTransaction.CreateRefundRequest();

            // Create PaymentRecord
            PaymentTransactionModel paymentTransaction = new PaymentTransactionModel();
            paymentTransaction.TransactionDate = DateTime.Now;
            paymentTransaction.ReceiptId = model.ReceiptId;
            paymentTransaction.EntityId = model.EntityId;
            paymentTransaction.Status = (int)PaymentTransactionStatus.Created;
            paymentTransaction.Amount = model.RefundAmount;
            paymentTransaction.PaymentType = PaymentType.CREDITCARD;
            paymentTransaction.TransactionType = (int)PaymentTransactionType.Refund;

            var response = await refundTransaction.ProcessRefund();
            _logger.Information($"ProcessCreditCardRefund - Entity Id:{model.EntityId} - Response: {response}");

            dynamic responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject(response);

            if (responseObject.transactionResponse != null)
            {
                var txResponse = responseObject.transactionResponse;
                if (txResponse.responseCode == "1")
                {
                    paymentTransaction.Status = (int)PaymentTransactionStatus.Approved;
                    paymentTransaction.TransactionId = txResponse.transId;
                    paymentTransaction.ResponseDetails = response;
                    paymentTransaction.MessageDetails = JsonConvert.SerializeObject(responseObject.messages);
                    paymentTransaction.ResponseCode = txResponse.responseCode;
                    paymentTransaction.AuthCode = txResponse.authCode;
                    paymentTransaction.AccountNumber = txResponse.accountNumber;
                    paymentTransaction.CardType = txResponse.accountType;
                    paymentTransaction.Result = (int)ReceiptStatus.Active;
                    paymentTransaction.ReferenceTransactionId = model.RefundTransactionId;
                }
                else
                {
                    paymentTransaction.ResponseCode = txResponse.responseCode;
                    paymentTransaction.Status = (int)PaymentTransactionStatus.Declined;
                    paymentTransaction.Result = (int)ReceiptStatus.Created;
                    paymentTransaction.MessageDetails = JsonConvert.SerializeObject(responseObject.messages);
                    if (txResponse != null && txResponse.errors != null)
                    {
                        paymentTransaction.ResponseDetails = response;
                    }
                }
            }
            else
            {
                paymentTransaction.Status = (int)PaymentTransactionStatus.NoResponse;
                paymentTransaction.Result = (int)ReceiptStatus.Created;
                paymentTransaction.MessageDetails = JsonConvert.SerializeObject(responseObject.messages);
            }
            //Update PaymentTransaction with Response
            var paymentResult = await _paymentTransactionService.CreatePaymentTransaction(paymentTransaction);
            paymentTransaction.PaymentTransactionId = paymentResult.PaymentTransactionId;

            return paymentTransaction.PaymentTransactionId;

        }
        public async Task<AuthNetPaymentResponseModel> ChargePaymentProfile(AuthNetChargePaymentProfileRequestModel model)
        {
            //Get ProcessorDetails

            var paymentProcessor = await _paymentProcessorService.GetPaymentProcessorByOrganizationId(model.OrganizationId);

            //Chcek for Test mode
            var processorUrl = string.Empty;

            if (paymentProcessor.TransactionMode == (int)PaymentTransactionMode.Live)
            {
                processorUrl = paymentProcessor.LiveUrl;
            }
            else
            {
                processorUrl = paymentProcessor.TestUrl;
            }

            MaxPaymentProfileTransaction authNetPaymentTransaction = new MaxPaymentProfileTransaction(processorUrl,
                                                                                                    paymentProcessor.LoginId,
                                                                                                    paymentProcessor.TransactionKey);
            // Get shopping Cart Details
            var cart = await _unitOfWork.ShoppingCarts.GetShoppingCartByIdAsync(model.CartId);
            if (cart.PaymentStatus != (int)PaymentTransactionStatus.Approved)
            {
                decimal creditBalance = 0;
                decimal paymentAmount = cart.Shoppingcartdetails.Sum(x => x.Amount);

                if (cart.UseCreditBalance > 0)
                {
                    creditBalance = await _unitOfWork.CreditTransactions.GetCreditBalanceByEntityIdAsync(cart.EntityId ?? 0);
                    paymentAmount = paymentAmount - creditBalance;
                }

                //Process Payment

                authNetPaymentTransaction.ReceiptId = cart.ReceiptId ?? 0;
                authNetPaymentTransaction.ProfileId = model.ProfileId;
                authNetPaymentTransaction.PaymentProfileId = model.AuthNetPaymentProfileId;
                authNetPaymentTransaction.Amount = paymentAmount.ToString();

                //Add Line Items

                AuthorizeNetCore.PaymentProfileModels.LineItems lineItems = new AuthorizeNetCore.PaymentProfileModels.LineItems();
                lineItems.LineItem = new AuthorizeNetCore.PaymentProfileModels.LineItem[cart.Shoppingcartdetails.Count()];
                int i = 0;
                foreach (var item in cart.Shoppingcartdetails)
                {
                    AuthorizeNetCore.PaymentProfileModels.LineItem lineItem = new AuthorizeNetCore.PaymentProfileModels.LineItem();

                    lineItem.ItemId = item.ItemId.ToString();
                    lineItem.Name = item.Description.Truncate(25);
                    lineItem.Quantity = item.Quantity.ToString();
                    lineItem.UnitPrice = item.Price.ToString();
                    lineItem.Description = item.Description.Truncate(25);
                    lineItems.LineItem[i++] = lineItem;
                }

                authNetPaymentTransaction.LineItems = lineItems;
                authNetPaymentTransaction.InvoiceNumber = cart.ReceiptId.ToString();

                // Create PaymentRecord
                PaymentTransactionModel paymentTransaction = new PaymentTransactionModel();

                paymentTransaction.TransactionDate = DateTime.Now;
                paymentTransaction.ReceiptId = cart.ReceiptId;
                paymentTransaction.EntityId = cart.EntityId;
                paymentTransaction.ShoppingCartId = cart.ShoppingCartId;
                paymentTransaction.Status = (int)PaymentTransactionStatus.Created;
                paymentTransaction.Amount = paymentAmount;
                paymentTransaction.CreditBalanceUsed = creditBalance;
                paymentTransaction.PaymentType = model.PaymentMode;

                //process Payment
                var request = authNetPaymentTransaction.CreatePaymentRequest();

                var response = await authNetPaymentTransaction.ProcessPayment();
                _logger.Information($"ProcessCreditCardRefund - Entity Id:{cart.EntityId} - Response: {JsonConvert.SerializeObject(response)}");
                AuthNetPaymentResponseModel responseModel = new AuthNetPaymentResponseModel();
                //dynamic responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject(response);
                //var txnResponse = responseObject.transactionResponse;

                if (response != null && response.TransactionResponse != null)
                {
                    var txnResponse = response.TransactionResponse;
                    if (txnResponse.ResponseCode == "1")
                    {
                        paymentTransaction.Status = (int)PaymentTransactionStatus.Approved;
                        paymentTransaction.TransactionId = txnResponse.TransId;
                        paymentTransaction.ResponseDetails = JsonConvert.SerializeObject(txnResponse);
                        paymentTransaction.MessageDetails = JsonConvert.SerializeObject(txnResponse.Messages);
                        paymentTransaction.ResponseCode = txnResponse.ResponseCode;
                        paymentTransaction.AuthCode = txnResponse.AuthCode;
                        paymentTransaction.AccountNumber = txnResponse.AccountNumber;
                        paymentTransaction.CardType = txnResponse.AccountType;
                        paymentTransaction.Result = (int)ReceiptStatus.Active;

                        responseModel.TransactionStatus = (int)PaymentTransactionStatus.Approved;
                        responseModel.TransactionId = paymentTransaction.TransactionId;
                        responseModel.ResponseCode = paymentTransaction.ResponseCode;
                        responseModel.AuthCode = paymentTransaction.AuthCode;
                    }
                    else
                    {
                        paymentTransaction.Status = (int)PaymentTransactionStatus.Declined;
                        paymentTransaction.Result = (int)ReceiptStatus.Created;
                        paymentTransaction.MessageDetails = JsonConvert.SerializeObject(response.Messages);
                        if (txnResponse != null)
                        {
                            paymentTransaction.ResponseDetails = JsonConvert.SerializeObject(response);
                        }

                        responseModel.TransactionStatus = (int)PaymentTransactionStatus.Declined;
                        responseModel.TransactionId = txnResponse.TransId;
                        responseModel.ResponseCode = txnResponse.ResponseCode;
                        responseModel.AuthCode = txnResponse.AuthCode;
                        responseModel.ErrorMessage = response.Messages.ResultMessages[0].Text;
                    }

                    //Update PaymentTransaction with Response
                    var paymentResult = await _paymentTransactionService.CreatePaymentTransaction(paymentTransaction);
                    paymentTransaction.PaymentTransactionId = paymentResult.PaymentTransactionId;

                    var updateshoppingCartResult = await _shoppingCartService.UpdateShoppingCartPaymentStatus(cart.UserId ?? 0, cart.ShoppingCartId, paymentTransaction.Status ?? 0, creditBalance, paymentTransaction.PaymentType);
                    if (paymentTransaction.Status == (int)PaymentTransactionStatus.Approved)
                    {
                        //Create GL Entries
                        await _transactionService.UpdateTransactionStatus(paymentTransaction);
                    }
                }
                else
                {
                    var txnResponse = response.TransactionResponse;
                    paymentTransaction.Status = (int)PaymentTransactionStatus.NoResponse;
                    paymentTransaction.Result = (int)ReceiptStatus.Created;
                    paymentTransaction.MessageDetails = JsonConvert.SerializeObject(response.Messages);
                    responseModel.TransactionStatus = (int)PaymentTransactionStatus.Declined;
                    responseModel.TransactionId = "";
                    responseModel.ResponseCode = "";
                    responseModel.ErrorMessage = response.Messages.ResultMessages[0].Text;
                }

                return responseModel;
            }
            else
            {
                return null;
            }
        }
        public async Task<PaymentProfileModel> DeletePaymentProfile(AuthNetPaymentProfileRequestModel model)
        {
            PaymentProfileModel paymentProfile = new PaymentProfileModel();
            // Get processor details
            var paymentProcessor = await _paymentProcessorService.GetPaymentProcessorByOrganizationId(model.OrganizationId);

            // define the merchant information (authentication / transaction id)
            var merchantAuthentication = new MerchantAuthentication()
            {
                LoginId = paymentProcessor.LoginId,
                TransactionKey = paymentProcessor.TransactionKey
            };
            var profiles = await _unitOfWork.PaymentProfiles.GetPaymentProfileByEntityIdAsync(model.EntityId);
            var profile = profiles.FirstOrDefault();
            if (profile == null)
            {
                return paymentProfile;
            }
            var deletePaymentProfileRequest = new DeleteCustomerPaymentProfileRequest
            {
                DeletePaymentProfileTransactionRequest = new DeletePaymentProfileTransactionRequest
                {
                    MerchantAuthentication = new MerchantAuthentication
                    {
                        LoginId = paymentProcessor.LoginId,
                        TransactionKey = paymentProcessor.TransactionKey
                    },
                    CustomerProfileId = profile.ProfileId,
                    CustomerPaymentProfileId = model.AuthNetPaymentProfileId
                }
            };
            //Chcek for Test mode
            var processorUrl = string.Empty;

            if (paymentProcessor.TransactionMode == (int)PaymentTransactionMode.Live)
            {
                processorUrl = paymentProcessor.LiveUrl;
            }
            else
            {
                processorUrl = paymentProcessor.TestUrl;
            }

            // Serialize the object
            var stringContent = new StringContent(JsonConvert.SerializeObject(deletePaymentProfileRequest), Encoding.UTF8, "application/json");
            _logger.Information($"DeletePaymentProfile - Request: {JsonConvert.SerializeObject(deletePaymentProfileRequest)}");
            // Connect to Authorize.net
            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(processorUrl, stringContent);

            // Serialize the object
            //var stringContent = new StringContent(JsonConvert.SerializeObject(deleteCustomerProfileRequest), Encoding.UTF8, "application/json");

            var responseJson = await response.Content.ReadAsStringAsync();
            _logger.Information($"DeletePaymentProfile - Entity Id:{model.EntityId} - Response: {responseJson}");

            await _paymentProfileService.DeletePaymentProfile(model.PaymentProfileId);
            return paymentProfile;
        }
    }
}
