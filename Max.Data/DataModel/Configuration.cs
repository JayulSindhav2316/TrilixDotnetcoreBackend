using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Configuration
    {
        public int ConfigurationId { get; set; }
        public int? OrganizationId { get; set; }
        public string DocumentAccessControl { get; set; }
        public string ContactDisplayTabs { get; set; }
        public int? ContactDisplayMembership { get; set; }
        public string DashboardType { get; set; }
        public int? SociableSyncEnabled { get; set; }
        public int? DisplayCodes { get; set; }
        public string SociableBaseUrl { get; set; }
        public int? DisplayRoles { get; set; }
        public string DataViewLayout { get; set; }
        public int? DisplayCustomFields { get; set; }
        public int? DisplayDashboard { get; set; }
        public int? DisplayContactCRMDeleteMemberProfile { get; set; }
        public int? DisplayContactCRMDeleteCompanyProfile { get; set; }
        public int? DisplayContactCRMRelationContactCompany { get; set; }
        public int? OnlyShowDealershipAutoGroupDropdownInContactCRMConnectionTab { get; set; }
        public int? DisplayCompanySearchForMembershipAdditionalMember { get; set; }
        public int? ShowDealerShipComanyAsBillableForMembership { get; set; }
        public int? DisplayBillableContactFieldOnSearch { get; set; }
        public int? IsGenerateRandomEmailForCompany { get; set; }
        public string CompanyRandomEmailDomain { get; set; }
        public int? DisplayMemberIdFieldOnPersonalInfo { get; set; }
        public string ContactMemberIdFormat { get; set; }
        public string CustomFieldsToSync { get; set; }
        public virtual Organization Organization { get; set; }
    }
}
