using System.ComponentModel;

namespace Max.Core
{
    /// <summary>
    /// Global Enums
    /// </summary>
    /// 

    // Address Type
    public enum AddressType
    {
        UNKNOWN = -1,
        Mailing = 0,
        Physical = 1
    }

    // Gender
    public enum GenderType
    {
        UNKNOWN = -1,
        Male = 0,
        Female = 1
    }

    //Ethnicity
    public enum EthnicityType
    {
        UNKNOWN = -1,
        American = 0,
        Asian = 1,
        Hispanic = 2,
        Hawaiian = 3
    }

    public enum Status
    {
        [Description("Inactive")]
        InActive = 0,
        [Description("Active")]
        Active = 1
    }
    public enum MembershipStatus
    {
        [Description("Inactive")]
        InActive = 0,
        [Description("Active")]
        Active = 1,
        [Description("Expired")]
        Expired = 2,
        [Description("Terminated")]
        Terminated = 3,
        [Description("On Hold")]
        OnHold = 4,
        [Description("Renewed")]
        Renewed = 5
    }
    public enum PageLayoutTypes
    {
        Portrait,
        Landscape
    }
    public enum DataSourceTypes
    {
        DataSet = 1,
        Session,
        File, Function
    }
    public enum DataTypes
    {
        StringType,
        DateTimeType,
        IntType,
        DecimalType,
        BoolType
    }

    public enum ShoppingCartItemType
    {
        Membership = 1,
        Donation = 2
    }

    public enum ReceiptStatus
    {
        Created = 0,
        Active = 1,
        Cancelled = 2,
        Void = 3
    }
    public enum PaymentTransactionStatus
    {
        Created = 0,
        Submitted = 1,
        Approved = 2,
        Declined = 3,
        NoResponse = 4
    }

    public enum PaymentTransactionType
    {
        Sale = 0,
        Refund = 1,
        Void = 2
    }
    public enum BillingJobStatus
    {
        Created = 0,
        Running = 1,
        Failed = 2,
        Completed = 3
    }
    public enum BillingStatus
    {
        Created = 0,
        Generated = 1,
        Finalizing = 2,
        Finalized = 3
    }
    public enum BillingType
    {
        Manual = 0,
        Auto = 1
    }
    public enum FeeBillingFrequency
    {
        [Description("On Joining")]
        Once = 1,
        [Description("On Renewal")]
        OnRenewal = 2,
        [Description("Every Cycle")]
        Recurring = 3
    }
    public enum InvoiceStatus
    {
        Draft = 0,
        Finalized = 1,
        PartialyPaid = 2,
        FullyPaid = 3
    }
    public enum Portal
    {
        StaffPortal = 0,
        MemberPortal = 1
    }

    public enum UserAccountStatus
    {
        Open = 0,
        Locked = 1
    }

    public enum LoginStatus
    {
        Failed = 0,
        Success = 1
    }

    public enum PreferredContact
    {
        [Description("Primary Email")]
        PrimaryEmail = 1,
        [Description("Primary Phone")]
        PrimaryPhone = 2,
        [Description("Primary Address")]
        PrimaryAddress = 3,
        [Description("Primary Text")]
        PrimaryText = 4
    }
    public enum RefundMode
    {
        [Description("Credit Card")]
        CreditCard = 1,

        [Description("On Line Credit")]
        OnLineCredit = 2,

        [Description("Off Line Payment")]
        OffLinePayment = 3,

        [Description("ACH / eCheck")]
        ACH = 4,
    }
    public enum CreditEntryType
    {
        CreditEntry = 1,
        DebitEntry = 2
    }
    public enum CreditStatus
    {
        Created = 0,
        Active = 1,
        Expired = 2
    }

    public enum AutoPayOnHoldStatus
    {
        OnHoldInActive = 0,
        OnHoldActive = 1
    }

    public enum TransactionType
    {
        [Description("All")]
        All = 0,

        [Description("Memberships")]
        Membership = 1,

        [Description("Marketplace Items")]
        MarketplaceItems = 3
    }

    public enum DiscountType
    {
        [Description("Percentage")]
        Percentage = 0,

        [Description("Fixed Amount")]
        Amount = 1,

        [Description("Manual Amount")]
        ManualDiscount = 2
    }
    public enum EmailStatus
    {
        Pending = 0,
        Sent = 1,
        Failed = 2
    }
    public enum InvoiceItemType
    {
        Donation = 1,
        Event = 2,
        Marketplace = 3,
        Membership = 4,
        Services = 5,
        Miscellaneous = 6
    }
    public enum EntityType
    {
        Person = 0,
        Company = 1
    }
    public enum BillingCommunication
    {
        PaperInvoice = 0,
        EmailInvoice = 1,
        PaperAndEmail = 2
    }
    public enum PaymentTransactionMode
    {
        Test = 0,
        Live = 1
    }

    public enum PaperInvoiceStatus
    {
        Inactive = 0,
        Active = 1,
        Edited = 2,
        Deleted = 3
    }
    public enum DocumentObjectType
    {
        Folder = 0,
        File = 1
    }

    public enum DocumentAccessType
    {
        Downloaded = 0,
        Uploaded = 1,
        Updated = 2,
        Deleted = 3
    }
    public enum ContactActivityInteractionType
    {
        [Description("Phone")]
        Phone = 0,
        [Description("In Person")]
        InPerson = 1,
        [Description("Email")]
        Email = 2,
        [Description("Other")]
        Other = 3,
        [Description("Role Change")]
        RoleChange = 4,
        [Description("Account Change")]
        AccountChange = 5
    }
    public enum ContactActivityConnectionType
    {
        [Description("Role and Contact")]
        RoleContact = 0,
        [Description("Contact Only")]
        ContactOnly = 1,
        [Description("Role Only")]
        RoleOnly = 2
    }
    public enum CustomFieldFor
    {
        [Description("Contact Only")]
        Contact = 1,
        [Description("Account Only")]
        Account = 2,
        [Description("Contact and Account")]
        All = 3
    }

    public enum ActivityDeleteStatus
    {
        [Description("Not Deleted")]
        NotDeleted = 0,
        [Description("Deleted")]
        Deleted = 1,
        [Description("Partially Deleted")]
        PartialDeleted = 2
    }


    public enum BillingCycleType
    {
        Manual = 0,
        Renewal = 1
    }

    public enum OfflinePaymentType
    {
        [Description("Credit Card")]
        CreditCard = 0,
        [Description("Other")]
        Other = 1
    }
    public enum InvoiceDetailType 
    {
        [Description("Email")]
        Email=1, 
        [Description("Paper Invoice & Email")]
        PaperInvoice=2
    }

    public enum FileDownloadStatus
    {
        FileSizeMismatch=-1,
        HashMismatch=0,
        Successful=1,
    }
}
