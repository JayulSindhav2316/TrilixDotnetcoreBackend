using AutoMapper;
using Max.Core.Models;
using Max.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Import
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
            CreateMap<OrganizationModel, Organization>().ReverseMap()
                 .ForMember(dest => dest.Logo, act => act.Ignore())
                 .ForMember(dest => dest.HeaderImage, act => act.Ignore());
            CreateMap<CreditTransactionModel, Credittransaction>().ReverseMap();
            CreateMap<RefundDetailModel, Refunddetail>().ReverseMap();
            CreateMap<NotesModel, Note>().ReverseMap();
            CreateMap<MembershipConnectionModel, Membershipconnection>().ReverseMap();
            CreateMap<ItemModel, Item>().ReverseMap();
            CreateMap<CompanyModel, Company>().ReverseMap();
            CreateMap<EntityModel, Entity>().ReverseMap();
            CreateMap<RelationshipModel, Relationship>().ReverseMap();
            CreateMap<DocumentContainerModel, Documentcontainer>().ReverseMap();
            CreateMap<DocumentObjectModel, Documentobject>().ReverseMap();
            CreateMap<ContainerAccessModel, Containeraccess>().ReverseMap();
            CreateMap<DocumentObjectAccessHistoryModel, Documentobjectaccesshistory>().ReverseMap();
            CreateMap<TagModel, Tag>().ReverseMap();
            CreateMap<DocumentTagModel, Documenttag>().ReverseMap();
        }
    }
}
