using System;

namespace Max.Core
{
    /// <summary>
    /// Global Constants
    /// </summary>
    public static class Constants
    {
        #region Global Constants
        public static readonly DateTime MySQL_MinDate = new DateTime(0001, 1, 1);
        public static readonly DateTime MySQL_MaxDate = new DateTime(9999, 12, 31);
        public static readonly DateTime EntityRole_MinDate = new DateTime(1900, 1, 1);

        /// <summary>
        /// .net Core Authroization scheme tries to match roles with a specfic cliam name
        /// so make sure that we supply the expected claim
        /// </summary>
        public static readonly string RoleClaim = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

        /// <summary>
        /// The default character length to create salt strings
        /// </summary>
        public static readonly Int32 CDefaultSaltLength = 64;

        /// <summary>
        /// The default iteration count for key stretching
        /// </summary>
        public static readonly Int32 CDefaultIterationCount = 5000;

        /// <summary>
        /// The minimum size in characters the password hashing function will allow for a salt string, salt must be always greater than 8 for PBKDF2 key derivitation to function
        /// </summary>
        public static readonly Int32 CMinSaltLength = 8;

        /// <summary>
        /// The maximum size a password can be to avoid massive passwords that force the initial hash to take so long it creates a DOS effect. Be careful increasing this, also make sure you catch exception for password too long!
        /// </summary>
        public static readonly Int32 CMaxPasswordLength = 1024;

        /// <summary>
        /// The key length used in the PBKDF2 derive bytes and matches the output of the underlying HMACSHA512 psuedo random function
        /// </summary>
        public static readonly Int32 CKeyLength = 64;

        //Number of hash iterations
        public static readonly int iterations = 10002;

        //Salt length
        private static readonly int saltSize = CDefaultSaltLength + 2;

        //Promo Code Length
        public static readonly int PromoCodeLength = 10;

        //Promo Code Lax Trial
        public static readonly int PromoCodeTrial = 10;

        public static readonly int TRUE = 1;
        public static readonly int FALSE = 0;
        public static class Roles
        {
            public const string AdminRole = "Admin";
            public const string InternalRole = "Internal";
            public const string ExternalRole = "External";
        }

        public static class BillingTypes
        {
            public const string MEMBERSHIP = "Membership";
            public const string GENERAL = "General";
        }

        public static class AutoBillingDraftType
        {
            public const string Finalized = "Finalized";
            public const string Preliminary = "Preliminary";
        }

        public static class InvoiceType
        {
            public const string CREDITCARD = "CreditCardDraft";
            public const string ACH = "BankDraft";
            public const string PAPER = "Paper";
            public const string INDIVIDUAL = "Individual";

        }
        public static class PaymentType
        {
            public const string CREDITCARD = "CreditCard";
            public const string ECHECK = "eCheck";
            public const string CHECK = "Check";
            public const string CASH = "Cash";
            public const string MAXCREDIT = "Member Credit";
            public const string OFFLINE = "Off Line";
        }

        public static class JournalEntryType
        {
            public const string ADJUSTMENT = "Adjustment";
            public const string DISCOUNT = "Discount";
            public const string ONLINE_CREDIT_CR = "OLC-CR";
            public const string ONLINE_CREDIT_DB = "OLC-DB";
            public const string ONLINE_CREDIT_USED = "OLC-USED";
            public const string REFUND = "Refund";
            public const string SALE = "Sale";
            public const string TAX = "Tax";
            public const string WRITEOFF = "Write Off";
            public const string VOID = "Void";
        }

        public static class JournalTransactionType
        {
            public const string MEMBERSHIP = "Membership";
            public const string VOID = "Void";
            public const string CANCELLATION = "Cancel";
            public const string WRITEOFF = "Write Off";
        }

        public static readonly string[] AllowedFileTypes = { ".doc", ".docx", ".pdf", ".txt", ".png", ".jpg", ".jpeg", ".gif", ".csv" };

        public static readonly int MAX_FILE_SIZE = 4000000;

        public static readonly string[] GraphColor = {"#bfd3e6","#9ebcda","#8c96c6","#8c6bb1","#88419d","#810f7c","#4d004b","#e0f3db","#a8ddb5","#7bccc4","#4eb3d3","#2b8cbe","#0868ac","#084081","#fed976","#fd8d3c","#fc4e2a","#e31a1c","#bd0026","#800026"};

        public static readonly int MAX_FAILED_ATTEMPTS = 5;
        public static readonly string[] StaffMenus = { "Main", "Administration", "CMS", "Events" };
        public static readonly int DEVICE_VALIDATION_LIMIT = 30;
        public static readonly int TOKEN_VALID_TIME = 120;


        public static class DocumentAccessControl
        {
            public const string MEMBERSHIP = "Membership";
            public const string GROUP = "Group";
        }
        public static class INVOICE_PAYMENT_STATUS
        {
            public const string BALANCE_DUE = "BalanceDue";
            public const string PAID = "Paid";
        }
        public static readonly int SOLR_PAGE_SIZE = 10;

        public static class AuditEntrtyType
        {
            public const string CREATED = "Created";
            public const string UPDATED = "Updated";
            public const string DELETED = "Deleted";
        }
        #endregion Global Constants
    }
}