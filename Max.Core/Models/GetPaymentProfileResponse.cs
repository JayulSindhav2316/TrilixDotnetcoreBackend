using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class GetPaymentProfileResponse
    {
        public Profile profile { get; set; }
        public Messages messages { get; set; }
    }
    public class CreditCard
    {
        public string cardNumber { get; set; }
        public string expirationDate { get; set; }
        public string cardType { get; set; }
        public string issuerNumber { get; set; }
    }

    public class BankAccount
    {
        public string accountType { get; set; }
        public string routingNumber { get; set; }
        public string accountNumber { get; set; }
        public string nameOnAccount { get; set; }
        public string nickName { get; set; }
    }

    public class Payment
    {
        public CreditCard creditCard { get; set; }
        public BankAccount bankAccount { get; set; }
    }

    public class BillTo
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
    }

    public class PaymentProfile
    {
        public bool defaultPaymentProfile { get; set; }
        public string customerPaymentProfileId { get; set; }
        public Payment payment { get; set; }
        public BillTo billTo { get; set; }
    }

    public class Profile
    {
        public List<PaymentProfile> paymentProfiles { get; set; }
        public string profileType { get; set; }
        public string customerProfileId { get; set; }
        public string merchantCustomerId { get; set; }
        public string description { get; set; }
        public string email { get; set; }
    }

    public class Message
    {
        public string code { get; set; }
        public string text { get; set; }
    }

    public class Messages
    {
        public string resultCode { get; set; }
        public List<Message> message { get; set; }
    }

}