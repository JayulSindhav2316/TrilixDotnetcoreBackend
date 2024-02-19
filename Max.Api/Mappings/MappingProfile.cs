﻿using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Core;

namespace Max.Services.Mappings
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
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
            CreateMap<MembershipTypeModel, Membershiptype>().ReverseMap()
                 .ForMember(dest => dest.PeriodName, opt => opt.MapFrom(src => src.PeriodNavigation.Name))
                 .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryNavigation.Name));
            CreateMap<MembershipFeeModel, Membershipfee>().ReverseMap()
                .ForMember(dest => dest.GlAccountCode, opt => opt.MapFrom(src => src.GlAccount.Code));
            CreateMap<MembershipCategoryModel, Membershipcategory>().ReverseMap();
            CreateMap<ShoppingCartModel, Shoppingcart>().ReverseMap();
            CreateMap<ShoppingCartDetailModel, Shoppingcartdetail>().ReverseMap();
            CreateMap<StaffUserModel, Staffuser>().ReverseMap();
            CreateMap<ReceiptHeaderModel, Receiptheader>().ReverseMap();
            CreateMap<Receiptheader, ReceiptModel>();
            CreateMap<ReceiptDetailModel, Receiptdetail>().ReverseMap();
            CreateMap<PaymentTransactionModel, Paymenttransaction>().ReverseMap();
            CreateMap<AutoBillingJobModel, Autobillingjob>().ReverseMap();
            CreateMap<BillingFeeModel, Billingfee>().ReverseMap();
            CreateMap<PaymentProfileModel, Paymentprofile>().ReverseMap();
            CreateMap<BillingJobModel, Billingjob>().ReverseMap();
            CreateMap<BillingCycleModel, Billingcycle>().ReverseMap();
            CreateMap<BillingDocumentModel, Billingdocument>().ReverseMap();
            CreateMap<PaperInvoiceModel, Paperinvoice>().ReverseMap();
            CreateMap<RoleModel, Role>().ReverseMap();
            CreateMap<OrganizationModel, Organization>().ReverseMap();
            //.ForMember(dest => dest.Logo, act => act.Ignore())
            //.ForMember(dest => dest.HeaderImage, act => act.Ignore());
            CreateMap<CreditTransactionModel, Credittransaction>().ReverseMap();
            CreateMap<RefundDetailModel, Refunddetail>().ReverseMap();
            CreateMap<NotesModel, Note>().ReverseMap();
            CreateMap<ReportModel, Report>().ReverseMap();
            CreateMap<ReportArgumentModel, Reportargument>().ReverseMap();
            CreateMap<PromoCodeModel, Promocode>().ReverseMap();
            CreateMap<GlAccountModel, Glaccount>().ReverseMap();
            CreateMap<DepartmentModel, Department>().ReverseMap();
            CreateMap<MembershipConnectionModel, Membershipconnection>().ReverseMap();
            CreateMap<ItemModel, Item>().ReverseMap().ForMember(x => x.Amount, op => op.Ignore());
            CreateMap<EntityModel, Entity>().ReverseMap();
            CreateMap<ReportFieldModel, Reportfield>().ReverseMap();
            CreateMap<ReportParameterModel, Reportparameter>().ReverseMap();
            CreateMap<ReportFilterModel, Reportfilter>().ReverseMap();
            CreateMap<ReportShareModel, Reportshare>().ReverseMap();
            CreateMap<MembershipReportConfigurationModel, Membershipreport>().ReverseMap();
            CreateMap<OpportunityReportConfigurationModel, Opportunityreport>().ReverseMap();
            CreateMap<ReportConfigurationModel, Report>().ReverseMap();
            CreateMap<ReportSortOrderModel, Reportsortorder>().ReverseMap();
            CreateMap<EntityModel, Entity>().ReverseMap();
            CreateMap<Group, GroupModel>().ReverseMap();
            CreateMap<Groupmember, GroupMemberModel>().ReverseMap();
            CreateMap<Grouprole, GroupRoleModel>().ReverseMap();
            CreateMap<Writeoff, WriteOffModel>().ReverseMap();
            CreateMap<Groupmemberrole, GroupMemberRoleModel>().ReverseMap();
            CreateMap<EntitySummaryModel, Entity>().ReverseMap();
            CreateMap<ClientLogModel, Clientlog>().ReverseMap();
            CreateMap<DocumentContainerModel, Documentcontainer>().ReverseMap();
            CreateMap<DocumentObjectModel, Documentobject>().ReverseMap();
            CreateMap<ContainerAccessModel, Containeraccess>().ReverseMap();
            CreateMap<DocumentObjectAccessHistoryModel, Documentobjectaccesshistory>().ReverseMap();
            CreateMap<TagModel, Tag>().ReverseMap();
            CreateMap<DocumentTagModel, Documenttag>().ReverseMap();
            CreateMap<EntitySociableModel, PersonModel>().ReverseMap();
            CreateMap<QuestionBankModel, Questionbank>().ReverseMap();
            CreateMap<AnswerOptionModel, Answeroption>().ReverseMap();
            CreateMap<DocumentAccessModel, Documentaccess>().ReverseMap();
            CreateMap<ConfigurationModel, Configuration>().ReverseMap();
            CreateMap<RegistrationGroupModel, Registrationgroup>().ReverseMap();
            CreateMap<AnswerTypeLookUpModel, Answertypelookup>().ReverseMap();
            CreateMap<EventModel, Event>().ReverseMap();
            CreateMap<QuestionLinkModel, Questionlink>().ReverseMap();
            CreateMap<LinkEventFeeTypeModel, Linkeventfeetype>().ReverseMap();
            CreateMap<SessionModel, Session>().ReverseMap();
            CreateMap<Country, CountryModel>().ReverseMap();
            CreateMap<State, StateModel>().ReverseMap();
            CreateMap<EventContactModel, Eventcontact>().ReverseMap();
            CreateMap<EntityRoleModel, Entityrole>().ReverseMap()
                .ForMember(x => x.StaffUserId, op => op.Ignore())
                .ForMember(x => x.HaveHistoricRecords, op => op.Ignore());
            CreateMap<ContactRoleModel, Contactrole>().ReverseMap();
            CreateMap<CustomFieldModel, Customfield>().ReverseMap();
            CreateMap<ContactActivityModel, Contactactivity>().ReverseMap();
            CreateMap<ContactActivityOutputModel, Contactactivity>().ReverseMap();
            CreateMap<ContactActivityInputModel, Contactactivity>();
            CreateMap<Contactactivity, ContactActivityInputModel>()
                .ForMember(x => x.ContactActivityInteractions, op => op.Ignore())
                .ForMember(x => x.InteractionContactDetails, op => op.Ignore());
            CreateMap<ContactActivityInteractionModel, Contactactivityinteraction>().ReverseMap();
            CreateMap<ContactActivityInteractionOutputModel, Contactactivityinteraction>();
            CreateMap<Contactactivityinteraction, ContactActivityInteractionOutputModel>()
                .ForMember(x => x.InteractionRoles, op => op.Ignore());
            CreateMap<CustomfieldlookupModel, Customfieldlookup>().ReverseMap();
            CreateMap<CustomfielddataModel, Customfielddatum>().ReverseMap();
            CreateMap<CustomfieldoptionModel, Customfieldoption>().ReverseMap();
            CreateMap<FieldTypeModel, Fieldtype>().ReverseMap();
            CreateMap<EventListModel, Event>().ReverseMap();
            CreateMap<OpportunityPipelineModel, Opportunitypipeline>().ReverseMap();
            CreateMap<PipelineStageModel, Pipelinestage>().ReverseMap();
            CreateMap<PipelineProductModel, Pipelineproduct>().ReverseMap();
            CreateMap<OpportunityModel, Opportunity>().ReverseMap();
            CreateMap<CustomfieldblockModel, Customfieldblock>().ReverseMap();
        }
    }
}