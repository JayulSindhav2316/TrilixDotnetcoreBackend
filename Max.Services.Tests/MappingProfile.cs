using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Max.Core.Models;
using Max.Data.DataModel;

namespace Max.Services.Tests
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<StaffUserModel, Staffuser>().ReverseMap();
            CreateMap<DepartmentModel, Department>().ReverseMap();
            CreateMap<PersonModel, Person>().ReverseMap();
            CreateMap<Person, PersonProfileModel>();
            CreateMap<EmailModel, Email>().ReverseMap();
            CreateMap<CompanyModel, Company>().ReverseMap();
            CreateMap<PhoneModel, Phone>().ReverseMap();
            CreateMap<AddressModel, Address>().ReverseMap();
            CreateMap<RelationModel, Relation>().ReverseMap();
            CreateMap<CommunicationModel, Communication>().ReverseMap();
            CreateMap<InvoiceModel, Invoice>().ReverseMap();
            CreateMap<InvoiceDetailModel, Invoicedetail>().ReverseMap();
            CreateMap<MembershipModel, Membership>().ReverseMap();
            CreateMap<MembershipHistoryModel, Membershiphistory>().ReverseMap();
            CreateMap<ShoppingCartModel, Shoppingcart>().ReverseMap();
            CreateMap<ShoppingCartDetailModel, Shoppingcartdetail>().ReverseMap();
            CreateMap<ReceiptHeaderModel, Receiptheader>().ReverseMap();
            CreateMap<ReceiptDetailModel, Receiptdetail>().ReverseMap();
            CreateMap<ReceiptItemDetailModel, Receiptitemdetail>().ReverseMap();
            CreateMap<PaymentTransactionModel, Paymenttransaction>().ReverseMap();
            CreateMap<AutoBillingJobModel, Autobillingjob>().ReverseMap();
            CreateMap<BillingDocumentModel, Billingdocument>().ReverseMap();
            CreateMap<MembershipTypeModel, Membershiptype>().ReverseMap();
            CreateMap<PaymentTransactionModel, Paymenttransaction>().ReverseMap();
            CreateMap<PaymentProfileModel, Paymentprofile>().ReverseMap();
            CreateMap<OrganizationModel, Organization>().ReverseMap();
            CreateMap<MembershipFeeModel, Membershipfee>().ReverseMap();
            CreateMap<MembershipPeriodModel, Membershipperiod>().ReverseMap();
            CreateMap<MembershipCategoryModel, Membershipcategory>().ReverseMap();
            CreateMap<LookupModel, Lookup>().ReverseMap();
            CreateMap<BillingJobModel, Billingjob>().ReverseMap();
            CreateMap<BillingCycleModel, Billingcycle>().ReverseMap();
            CreateMap<PromoCodeModel, Promocode>().ReverseMap();
            CreateMap<NotesModel, Note>().ReverseMap();
            CreateMap<AutoBillingSettingModel, Autobillingsetting>().ReverseMap();
            CreateMap<EntityModel, Entity>().ReverseMap();
            CreateMap<RoleModel, Role>().ReverseMap();           
            CreateMap<MembershipReportConfigurationModel, Membershipreport>().ReverseMap();
            CreateMap<MembershipTypeModel, Membershiptype>().ReverseMap();
            CreateMap<StaffUserModel, Staffuser>().ReverseMap();
            CreateMap<ReportShareModel, Reportshare>().ReverseMap();
            CreateMap<ReportModel, Report>().ReverseMap();
            CreateMap<ReportArgumentModel, Reportargument>().ReverseMap();
            CreateMap<ReportParameterModel, Reportparameter>().ReverseMap();
            CreateMap<ReportFilterModel, Reportfilter>().ReverseMap();
            CreateMap<ReportFieldModel, Reportfield>().ReverseMap();
            CreateMap<RegistrationGroupModel, Registrationgroup>().ReverseMap();
        }
    }
}
