using System;
using System.Threading.Tasks;

namespace Max.Data.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        //IAuditHistoryRepository AuditHistory { get; }
        IGrouphistoryRepository GroupHistories { get; }
        IGroupMemberRoleRepository GroupMemberRoles { get; }
        IGroupMemberRepository GroupMembers { get; }
        ILinkGroupRoleRepository LinkGroupRoles { get; }
        IGroupRoleRepository GroupRoles { get; }
        IGroupRepository Groups { get; }
        IStaffUserRepository Staffusers { get; }
        IRoleRepository Roles { get; }
        IStaffRoleRepository Staffroles { get; }
        IMembershipTypeRepository Membershiptypes { get; }
        IMembershipFeeRepository Membershipfees { get; }
        IMembershipPeriodRepository MembershipPeriods { get; }
        IMembershipCategoryRepsoitory MembershipCategories { get; }
        IDepartmentRepository Departments { get; }
        IGlAccountTypeRepository GlAccountTypes { get; }
        IGlAccountRepository GlAccounts { get; }
        IRoleMenuRepository Rolemenus { get; }
        IMenuRepository Menus { get; }
        IMenuGroupRepository MenuGroups { get; }
        IPersonRepository Persons { get; }
        ILookupRepository Lookups { get; }
        ICommunicationRepository Communications { get; }
        IInvoiceRepository Invoices { get; }
        IInvoiceDetailRepository InvoiceDetails { get; }
        IMembershipRepository Memberships { get; }
        IMembershipHistoryRepository MembershipHistories { get; }
        IRelationRepository Relations { get; }
        IAddressRepository Addresses { get; }
        IEmailRepository Emails { get; }
        IPhoneRepository Phones { get; }
        ICompanyRepository Companies { get; }
        IDocumentRepository Documents { get; }
        IOrganizationRepository Organizations { get; }
        IReceiptHeaderRepository ReceiptHeaders { get; }
        IReceiptDetailRepository ReceiptDetails { get; }
        IShoppingCartRepository ShoppingCarts { get; }
        IShoppingCartDetailRepository ShoppingCartDetails { get; }
        IPaymentTransactionRepository PaymentTransactions { get; }
        IAutoBillingDraftRepository AutoBillingDrafts { get; }
        IAutoBillingNotificationRepository AutoBillingNotifications { get; }
        IAutoBillingProcessingDateRepository AutoBillingProcessingDates { get; }
        IAutoBillingSettingRepository AutoBillingSettings { get; }
        IBillingDocumentRepository BillingDocuments { get; }
        IPaymentProcessorRepository PaymentProcessors { get; }
        IPaymentProfileRepository PaymentProfiles { get; }
        IAutoBillingJobRepository AutoBillingJobs { get; }
        IBillingJobRepository BillingJobs { get; }
        IBillingCycleRepository BillingCycles { get; }
        IPaperInvoiceRepository PaperInvoices { get; }
        IJournalEntryHeaderRepository JournalEntryHeaders { get; }
        IJournalEntryDetailRepository JournalEntryDetails { get; }
        IBillingFeeRepository BillingFees { get; }
        IMultiFactorCodeRepository MultiFactorCodes { get; }
        IAccessLogRepository AccessLogs { get; }
        IUserDeviceRepository UserDevices { get; }
        IAccessTokenRepository AccessTokens { get; }
        IMembershipConnectionRepository MembershipConnections { get; }
        ICreditTransactionRepository CreditTransactions { get; }
        IRefundDetailRepository RefundDetails { get; }
        IVoidDetailRepository VoidDetails { get; }
        IAccountingSetupRepository AccountingSetups { get; }
        IAutoBillingOnHoldRepository AutoBillingOnHolds { get; }
        INoteRepository Notes { get; }
        IAutoBillingPaymentRepository AutoBillingPayments { get; }
        IPromoCodeRepository PromoCodes { get; }
        IRelationshipRepository Relationships { get; }
        IReportRepository Reports { get; }
        IBillingBatchRepository BillingBatches { get; }
        IBillingEmailRepository BillingEmails { get; }
        IItemRepository Items { get; }
        IItemTypeRepository ItemTypes { get; }
        IEntityRepository Entities { get; }
        IMembershipReportRepository MembershipReports { get; }
        IOpportunityReportRepository OpportunityReports { get; }
        IReportFilterRepository ReportFilters { get; }
        IReportShareRepository ReportShares { get; }
        IReportParameterRepository ReportParemeters{ get; }
        IReportSortorderRepository ReportSortorders { get; }
        IReportFieldRepository ReportFields { get; }
        IResetPasswordRepository ResetPasswords { get; }
        IWriteOffRepository WriteOffs { get; }
        IStaffSearchHistoryRepository StaffSearchHistories { get; }
        IClientLogRepository ClientLogs { get; }
        IContainerAccessRepository ContainerAccesses { get; }
        IDocumentObjectRepository DocumentObjects { get; }
        IDocumentContainerRepository DocumentContainers { get; }
        IConfigurationRepository Configurations { get; }
        ITagRepository Tags { get; }
        IDocumentObjectAccessRepository DocumentObjectAccesses { get; }
        IDocumentObjectTagRepository DocumentObjectTags { get; }
        IDocumentObjectAccessHistoryRepository DocumentObjectAccessHistories { get; }
        IQuestionBankRepository QuestionBanks { get; }
        IQuestionLinkRepository QuestionLinks { get; }
        IAnswerOptionRepository AnswerOptions { get; }
        IAnswerTypeLookUpRepository AnswerTypeLookUps { get; }
        IGroupRegistrationRepository GroupRegistrations { get; }
        IEventTypeRepository EventTypes { get; }
        ITimeZoneRepository TimeZones { get; }
        IRegistrationFeeTypeRepository RegistrationFeeTypes { get; }
        IEventRepository Events { get; }
        ILinkEventFeeTypeRepository LinkEventFeeTypes { get; }
        ILinkRegistrationGroupFeeRepository LinkRegistrationGroupFees { get; }
        ILinkEventGroupRepository LinkEventGroups { get; }
        ISessionRepository Sessions { get; }
        ISessionLeaderLinkRepository SessionLeaderLinks { get; }
        ISessionRegistrationGroupPricingRepository SessionRegistrationGroupPricings { get; }
        ICountryRepository CountryRepository { get; }
        IStateRepository StateRepository { get; }
        IEventContactRepository EventContacts { get; }
        ISolrExportRepository SolrExports { get; }
        IContactRoleRepository ContactRoles { get; }
        IEntityRoleRepository EntityRoles { get; }
        IEntityRoleHistoryRepository EntityRoleHistories { get; }
        IContactActivityRepository ContactActivities { get; }
        IEventRegisterRepository EventRegister { get; }
        IEventRegisterSessionRepository EventRegisterSession { get; }
        IEventRegisterQuestionRepository EventRegisterQuestion { get; }
        ICustomFieldRepository CustomFields { get; }
        IContactTokenRepository ContactTokens { get; }
        IContactActivityInteractionRepository ContactActivityInteractions { get; }
        IOpportunityPipelineRepository OpportunityPipelines { get; }
        IPipelineStageRepository PipelineStages { get; }
        IPipelineProductRepository PipelineProducts { get; }
        IBillingCycleNotificationRepository BillingCycleNotifications { get; }
        IOpportunityRepository Opportunities { get; }
        Task<int> CommitAsync();
  }
}