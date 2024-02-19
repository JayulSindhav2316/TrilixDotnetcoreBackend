
namespace Max.Reporting.Domain.Entities
{
    public partial class Tenant
    {
        public string TenantId { get; set; }
        public string OrganizationName { get; set; }
        public int OrganizationId { get; set; }
        public string DatabaseName { get; set; }
        public string HostName { get; set; }
        public string ConnectionString { get; set; }
        public string ReportConnectionString { get; set; }
        public string Apikey { get; set; }
        public string SecretKey { get; set; }
        public string BaseUrl { get; set; }
        public string SenderEmailAddress { get; set; }
        public string EmailServer { get; set; }
        public string EmailSenderUserName { get; set; }
        public string EmailSenderPassword { get; set; }
        public string EmailServerPort { get; set; }
        public int? EmailServerNeedsAuthentication { get; set; }
        public string StorageConnectionString { get; set; }
        public string SolrUrl { get; set; }
        public string SociableBaseUrl { get; set; }
        public string SociableAdminUserName { get; set; }
        public string SociableAdminPassword { get; set; }
        public double? VerificationMinutes { get; set; }
    }
}
