using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.Repositories;
using Max.Data.Interfaces;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;
using Max.Core;
using Max.Data;
using Max.Services;
using System.Linq;
using AutoMapper;
using System.Text;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;

namespace Max.Services
{
    public class EntityService : IEntityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IInvoiceService _invoiceService;
        private readonly ISociableService _sociableService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public EntityService(IUnitOfWork unitOfWork, IMapper mapper, IInvoiceService invoiceService, ISociableService sociableService, IHttpContextAccessor httpContextAccessor)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._invoiceService = invoiceService;
            this._sociableService = sociableService;
            this._httpContextAccessor = httpContextAccessor;
        }

        public async Task<EntityModel> GetEntityById(int id)
        {
            EntityModel model = null;
            if (id > 0)
            {
                var entity = await _unitOfWork.Entities.GetEntityByIdAsync(id);
                model = _mapper.Map<EntityModel>(entity);

                if (model.PersonId != null)
                {
                    var person = await _unitOfWork.Persons.GetPersonByIdAsync(model.PersonId ?? 0);
                    model.Person = _mapper.Map<PersonModel>(person);
                    //Map additional fields
                    var primaryAddress = model.Person.Addresses.GetPrimaryAddress();
                    model.Person.StreetAddress = primaryAddress.StreetAddress;
                    model.Person.City = primaryAddress.City;
                    model.Person.State = primaryAddress.State;
                    model.Person.Zip = primaryAddress.Zip.FormatZip();
                }
                if (model.CompanyId != null)
                {
                    var company = await _unitOfWork.Companies.GetCompanyByIdAsync(model.CompanyId ?? 0);
                    model.Company = _mapper.Map<CompanyModel>(company);
                    var address = model.Company.Addresses.GetPrimaryAddress();
                    if(address != null)
                    {
                        model.Company.StreetAddress = address.StreetAddress;
                        model.Company.City = address.City;
                        model.Company.State = address.State;
                        model.Company.Zip = address.Zip.FormatZip();
                        model.Company.Country = address.Country;
                    }
                    
                }
            }
            return model;
        }

        public async Task<Entity> GetEntityByWebLogin(string webLogin)
        {
            var entity = await _unitOfWork.Entities.GetEntityByUserNameAsync(webLogin);
            return entity;
        }

        public async Task<EntitySummaryModel> GetEntitySummaryById(int id)
        {
            var entity = await _unitOfWork.Entities.GetEntityByIdAsync(id);
            if (entity == null)
            {
                return new EntitySummaryModel();
            }
            var model = _mapper.Map<EntitySummaryModel>(entity);

            if (entity.PreferredBillingCommunication == null)
            {
                model.PreferredBillingCommunication = null;
            }


            model.IsBillableNonMember = entity.InvoiceEntities.Where(x => x.MembershipId.HasValue).Count() > 0;

            if (model.PersonId != null)
            {
                var person = await _unitOfWork.Persons.GetPersonByIdAsync(model.PersonId ?? 0);
                model.Person = _mapper.Map<PersonModel>(person);
                model.Person.PrimaryEmail = model.Person.Emails.GetPrimaryEmail();
                model.Person.PrimaryPhone = model.Person.Phones.GetPrimaryPhoneNumber();
                model.Company = new CompanyModel();
            }
            if (model.CompanyId != null)
            {
                var company = await _unitOfWork.Companies.GetCompanyByIdAsync(model.CompanyId ?? 0);
                model.Company = _mapper.Map<CompanyModel>(company);
                var primaryEmail = model.Company.Emails.GetPrimaryEmail();
                model.Company.Email = primaryEmail;
                model.Person = new PersonModel();
            }

            //Map Membership
            var membershipConnections = await _unitOfWork.MembershipConnections.GetMembershipConnectionsByEntityIdAsync(id);

            if (membershipConnections.Count() > 0)
            {
                //Get active membership
                var activeMembership = membershipConnections.Where(x => x.Membership.Status == (int)MembershipStatus.Active).FirstOrDefault();
                if (activeMembership != null)
                {
                    model.IsMember = true;
                    model.MembershipStatus = "Member";
                    model.JoinDate = activeMembership.Membership.CreateDate.ToString();
                    model.ExpirationDate = activeMembership.Membership.EndDate.ToString();
                    model.NextBillDate = activeMembership.Membership.NextBillDate.ToString();
                }
                else
                {
                    //Get History of previous membership
                    var expiredMembership = membershipConnections.OrderByDescending(x => x.Membership.CreateDate).FirstOrDefault();
                    if (expiredMembership != null)
                    {
                        model.IsMember = false;
                        model.MembershipStatus = "Non Member";
                        model.JoinDate = expiredMembership.Membership.CreateDate.ToString();
                        model.ExpirationDate = "";
                        model.NextBillDate = "";
                    }
                }
            }
            else
            {
                model.IsMember = false;
                model.MembershipStatus = "Non Member";
                model.JoinDate = "";
                model.ExpirationDate = "";
            }

            //check if he is a billable member
            var billableMembership = await _unitOfWork.Memberships.GetActiveMembershipByEntityIdAsync(id);
            model.IsBillable = 0;
            if (billableMembership.Count() > 0)
            {
                model.IsBillable = 1;
            }
            //map notes

            var notes = await _unitOfWork.Notes.GetNotesByEntityIdAsync(id);
            if (notes != null)
            {
                model.Notes = _mapper.Map<List<NotesModel>>(notes);
            }

            return model;
        }

        public async Task<List<Entity>> GetEntitiesByName(string name)
        {
            var entities = await _unitOfWork.Entities.GetEntitiesByNameAsync(name);
            return entities.ToList();
        }
        public async Task<List<EntityMembershipHistoryModel>> GetMembershipHistoryByEntityId(int id)
        {
            var entityHistory = await _unitOfWork.Entities.GetMembershipHistoryByEntityId(id);
            List<EntityMembershipHistoryModel> history = new List<EntityMembershipHistoryModel>();

            if (entityHistory.Memberships.Count() > 0)
            {
                var list = entityHistory.Memberships.OrderByDescending(x => x.StartDate);

                foreach (var item in list)
                {
                    EntityMembershipHistoryModel historyModel = new EntityMembershipHistoryModel();
                    historyModel.EntityId = entityHistory.EntityId;
                    historyModel.MembershipId = item.MembershipId;
                    historyModel.Code = item.MembershipType.Code;
                    historyModel.Category = item.MembershipType.CategoryNavigation.Name;
                    historyModel.Name = item.MembershipType.Name;
                    historyModel.Period = item.MembershipType.PeriodNavigation.Name;
                    historyModel.StartDate = item.StartDate;
                    historyModel.EndDate = item.CreateDate;
                    historyModel.CurrentStatus = MembershipStatus.GetName(typeof(MembershipStatus), item.Status);
                    history.Add(historyModel);
                }
            }
            return history;
        }
        public async Task<List<EntityBillingModel>> GetScheduledBillingByEntityId(int entityId)
        {
            var billings = new List<EntityBillingModel>();
            var entity = await _unitOfWork.Entities.GetMembershipDetailByEntityId(entityId);

            if (entity != null)
            {
                var model = new EntityBillingModel();
                var activeMembership = entity.Memberships.Where(x => x.Status == (int)MembershipStatus.Active).FirstOrDefault();
                if (activeMembership != null)
                {
                    model.EntityId = entity.EntityId;
                    model.MembershipId = activeMembership.MembershipId;
                    model.MembershipName = activeMembership.MembershipType.Name;
                    model.PaymentMethod = activeMembership.AutoPayEnabled == 1 ? "CreditCard" : "Paper Invoice";
                    model.BillingFrequency = activeMembership.AutoPayEnabled == 1 ? "Monthly" : "On Due";
                    model.Amount = activeMembership.Billingfees.Where(x => x.MembershipFee.BillingFrequency == (int)FeeBillingFrequency.Recurring).Sum(x => x.Fee);
                    model.BillingOnHold = activeMembership.BillingOnHold;
                    model.StartDate = activeMembership.StartDate;
                    model.NextBillDate = activeMembership.NextBillDate;
                    model.EndDate = activeMembership.EndDate;
                }
                billings.Add(model);
            }
            return billings;
        }
        public async Task<bool> AddBillableContact(int entityId, int billingContactId)
        {
            var entity = await _unitOfWork.Entities.GetByIdAsync(entityId);
            var person = await _unitOfWork.Persons.GetByIdAsync(billingContactId);

            var company = await _unitOfWork.Companies.GetByIdAsync(entity.CompanyId ?? 0);

            if (company != null)
            {
                company.BillableContactId = billingContactId;

                try
                {
                    _unitOfWork.Companies.Update(company);
                    await _unitOfWork.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return false;
        }
        public async Task<EntityModel> GetEntityProfileById(int id)
        {
            EntityModel model = new EntityModel();

            string entityType = string.Empty;

            if (id <= 0) return model;

            var entity = await _unitOfWork.Entities.GetEntityDetailsByIdAsync(id);
            model = _mapper.Map<EntityModel>(entity);

            //Check if person/ prima

            if (entity.PersonId != null)
            {
                entityType = "Person";
            }
            else
            {
                entityType = "Company";
            }

            if (entityType == "Person")
            {
                var person = await _unitOfWork.Persons.GetPersonByIdAsync(entity.PersonId ?? 0);
                model.Person = _mapper.Map<PersonModel>(person);
                if (person.Company != null)
                {
                    model.Person.Company = _mapper.Map<CompanyModel>(person.Company);
                }
                else
                {
                    model.Person.Company = new CompanyModel();
                }

                model.Person.PreferredContact = person.PreferredContact;

                //Map additional fields
                List<AddressModel> addressModel = _mapper.Map<List<AddressModel>>(person.Addresses);

                var primaryAddress = addressModel.GetPrimaryAddress();
                model.Person.StreetAddress = primaryAddress.StreetAddress;
                model.Person.City = primaryAddress.City;
                model.Person.State = primaryAddress.State;
                model.Person.Zip = primaryAddress.Zip.FormatZip();

                model.Person.PrimaryEmail = model.Person.Emails.GetPrimaryEmail();
                model.Person.PrimaryPhone = model.Person.Phones.GetPrimaryPhoneNumber();

                var entityRoles = await _unitOfWork.EntityRoles.GetActiveEntityRolesByEntityIdAsync(entity.EntityId);
                model.Person.EntityRoles = _mapper.Map<List<EntityRoleModel>>(entityRoles);

                model.Company = new CompanyModel();
            }
            else
            {
                model.Company = _mapper.Map<CompanyModel>(await _unitOfWork.Companies.GetCompanyByIdAsync(entity.CompanyId ?? 0));

                //Map additional fields
                List<AddressModel> addressModel = _mapper.Map<List<AddressModel>>(model.Company.Addresses);
                List<EmailModel> emailModel = _mapper.Map<List<EmailModel>>(model.Company.Emails);
                List<PhoneModel> phoneModel = _mapper.Map<List<PhoneModel>>(model.Company.Phones);
                var primaryAddress = addressModel.GetPrimaryAddress();
                model.Company.StreetAddress = primaryAddress.StreetAddress;
                model.Company.City = primaryAddress.City == null ? string.Empty : primaryAddress.City;
                model.Company.State = primaryAddress.State == null ? string.Empty : primaryAddress.State;
                model.Company.Zip = primaryAddress.Zip.FormatZip();
                var primaryEmail = emailModel.GetPrimaryEmail();
                var primaryPhone = phoneModel.GetPrimaryPhoneNumber().GetCleanPhoneNumber();
                model.Company.Email = primaryEmail;
                model.Company.Phone = primaryPhone.FormatPhoneNumber();
                model.Person = new PersonModel();
            }

            var entityDetails = await _unitOfWork.Entities.GetEntityDetailsByIdAsync(id);
            var billableEntity = await _unitOfWork.Invoices.GetAllInvoicesByEntityIdAsync(id);
            billableEntity.ToList().ForEach(item =>
            {
                if (!entity.InvoiceEntities.Any(x => x.InvoiceId == item.InvoiceId))
                    entity.InvoiceEntities.Add(item);
            });

            //Map Invoices

            foreach (var item in entity.InvoiceEntities.Where(x => x.Status != (int)InvoiceStatus.Draft))
            {
                InvoicePaymentModel invoiceItem = new InvoicePaymentModel();
                invoiceItem.InvoiceId = item.InvoiceId;
                invoiceItem.Date = item.Date;
                invoiceItem.DueDate = item.DueDate;
                invoiceItem.Description = item.MembershipId > 0 ? "Membership" : "General";
                invoiceItem.Total = item.Invoicedetails.Sum(x => x.Amount);
                invoiceItem.Paid = item.Invoicedetails.Select(x => x.Receiptdetails.Where(x => x.Status != (int)ReceiptStatus.Created && x.Status != (int)ReceiptStatus.Void).Sum(x => x.Amount)).Sum();
                invoiceItem.WriteOff = item.Invoicedetails.Select(x => x.Writeoffs).Select(x => x.Sum(x => x.Amount ?? 0)).Sum();
                invoiceItem.Balance = invoiceItem.Total - invoiceItem.Paid - invoiceItem.WriteOff;
                invoiceItem.ReceiptId = invoiceItem.ReceiptId;
                model.InvoicePayments.Add(invoiceItem);

            }

            //Map Membership
            var membershipConnections = _mapper.Map<List<MembershipConnectionModel>>(entity.Membershipconnections);

            if (membershipConnections.Count > 0)
            {
                model.MembershipConnections = membershipConnections.Where(x => x.Status == (int)MembershipStatus.Active).ToList();
                foreach (var membershipConnection in model.MembershipConnections)
                {
                    model.Memberships.Add(membershipConnection.Membership);
                }

                //Get active membership
                var activeMembership = membershipConnections.Where(x => x.Membership.Status == (int)MembershipStatus.Active).FirstOrDefault();
                if (activeMembership != null)
                {
                    model.MembershipStatus = "Member";
                    model.JoinDate = activeMembership.Membership.CreateDate.ToString();
                    model.ExpirationDate = activeMembership.Membership.EndDate.ToString();
                    model.NextBillDate = activeMembership.Membership.NextBillDate.ToString();
                }
                else
                {
                    //Get History of previous membership
                    var expiredMembership = model.MembershipConnections.OrderByDescending(x => x.Membership.CreateDate).FirstOrDefault();
                    if (expiredMembership != null)
                    {
                        model.MembershipStatus = "Non Member";
                        model.JoinDate = expiredMembership.Membership.CreateDate.ToString();
                        model.ExpirationDate = expiredMembership.Membership.EndDate.ToString();
                        model.NextBillDate = "";
                    }
                }
            }
            else
            {
                model.MembershipStatus = "Non Member";
                model.JoinDate = "";
                model.ExpirationDate = "";
            }
            //Map payment profile

            model.PaymentProfiles = _mapper.Map<List<PaymentProfileModel>>(entity.Paymentprofiles);
            model.RelationEntities.Clear();

            model.GroupMembers = model.GroupMembers.Where(x => x.IsGroupActive == true).ToList();

            //Get Credit Balance

            model.CreditBalance = 0;
            if (model.CreditTransactions.Count > 0)
            {
                var totalCredit = model.CreditTransactions.Where(x => x.EntryType == (int)CreditEntryType.CreditEntry).Sum(x => x.Amount);
                var totalDebit = model.CreditTransactions.Where(x => x.EntryType == (int)CreditEntryType.DebitEntry).Sum(x => x.Amount);
                model.CreditBalance = totalCredit - totalDebit;
            }

            return model;
        }

        public async Task<decimal> GetCreditBalanceById(int entityId)
        {
            decimal creditBalance = 0;

            var credits = await _unitOfWork.CreditTransactions.GetCreditsByEntityIdAsync(entityId);
            if (credits != null)
            {
                if (credits.Count() > 0)
                {
                    var totalCredit = credits.Where(x => x.EntryType == (int)CreditEntryType.CreditEntry).Sum(x => x.Amount);
                    var totalDebit = credits.Where(x => x.EntryType == (int)CreditEntryType.DebitEntry).Sum(x => x.Amount);
                    creditBalance = totalCredit ?? 0 - totalDebit ?? 0;
                }
            }

            return creditBalance;
        }
        public async Task<EntityMembershipProfileModel> GetMembershipProfileById(int entityId)
        {
            var model = new EntityMembershipProfileModel();
            var entity = await _unitOfWork.Entities.GetMembershipDetailByEntityId(entityId);

            if (entity != null)
            {
                model.EntityId = entity.EntityId;
                model.MembershipBalance = _unitOfWork.Invoices.GetInvoiceBalanceByEntityId(entityId);

                //If billable member then get billing information
                var activeMemberships = entity.Memberships.Where(x => x.Status == (int)MembershipStatus.Active);
                foreach (var activeMembership in activeMemberships)
                {
                    if (activeMembership != null)
                    {
                        EntityBillingModel billingModel = new EntityBillingModel();
                        model.IsMember = true;
                        model.ActiveMembershipId = activeMembership.MembershipId;
                        model.MembershipName = activeMembership.MembershipType.Name;
                        model.Code = activeMembership.MembershipType.Code;
                        model.Category = activeMembership.MembershipType.CategoryNavigation.Name;
                        model.Name = activeMembership.MembershipType.Name;
                        model.Period = activeMembership.MembershipType.PeriodNavigation.Name;
                        model.StartDate = activeMembership.StartDate;
                        model.EndDate = activeMembership.EndDate;
                        model.NextBillDate = activeMembership.NextBillDate;
                        model.MaxUnits = activeMembership.MembershipType.Units;

                        var billingFees = activeMembership.Billingfees.Select(x => x.MembershipFee.BillingFrequency);
                        if (billingFees.Contains((int)FeeBillingFrequency.Recurring))
                        {
                            billingModel.EntityId = entity.EntityId;
                            billingModel.MembershipId = activeMembership.MembershipId;
                            billingModel.MembershipName = activeMembership.MembershipType.Name;
                            billingModel.PaymentMethod = activeMembership.AutoPayEnabled == 1 ? "CreditCard" : "Paper Invoice";
                            billingModel.BillingFrequency = activeMembership.AutoPayEnabled == 1 ? "Monthly" : "On Due";
                            billingModel.Amount = activeMembership.Billingfees.Where(x => x.MembershipFee.BillingFrequency == (int)FeeBillingFrequency.Recurring).Sum(x => x.Fee);
                            billingModel.BillingOnHold = activeMembership.BillingOnHold;
                            billingModel.StartDate = activeMembership.StartDate;
                            billingModel.NextBillDate = activeMembership.NextBillDate;
                            billingModel.EndDate = activeMembership.EndDate;

                            model.BillingSchedule.Add(billingModel);
                        }
                    }
                    else
                    {
                        model.IsMember = false;
                        model.ActiveMembershipId = 0;
                        model.MembershipName = "Non Member";
                    }
                }
                //check membership connections
                if (entity.Membershipconnections.Count() > 0)
                {
                    var list = entity.Membershipconnections;

                    foreach (var item in list)
                    {
                        var membership = await _unitOfWork.Memberships.GetMembershipByIdAsync(item.MembershipId);
                        EntityMembershipHistoryModel historyModel = new EntityMembershipHistoryModel();
                        historyModel.EntityId = entity.EntityId;
                        historyModel.MembershipId = item.MembershipId;
                        historyModel.Code = membership.MembershipType.Code;
                        historyModel.Category = membership.MembershipType.CategoryNavigation.Name;
                        historyModel.Name = membership.MembershipType.Name;
                        historyModel.Period = membership.MembershipType.PeriodNavigation.Name;
                        historyModel.StartDate = membership.StartDate;
                        historyModel.EndDate = membership.EndDate;
                        historyModel.NextBillDate = membership.NextBillDate;
                        historyModel.CurrentStatus = MembershipStatus.GetName(typeof(MembershipStatus), membership.Status);
                        historyModel.BillableEntityName = item.Membership.BillableEntity.Name;
                        historyModel.MemberName = item.Entity.Name;
                        historyModel.MaxUnits = membership.MembershipType.Units;
                        //Get billable Member details
                        var billableEntity = await _unitOfWork.Entities.GetEntityByIdAsync(item.Membership.BillableEntity.EntityId);
                        historyModel.BillableEntity = _mapper.Map<EntityModel>(billableEntity);
                        historyModel.BillableEntity.Person = _mapper.Map<PersonModel>(billableEntity.People.FirstOrDefault());

                        var additionalMembers = await _unitOfWork.MembershipConnections.GetMembershipConnectionsByMembershipIdAsync(item.MembershipId);
                        foreach (var additionalMember in additionalMembers.Where(f => f.EntityId != entity.EntityId))
                        {
                            if (additionalMember.Entity.PersonId > 0)
                            {
                                var people = additionalMember.Entity.People.FirstOrDefault();
                                if (people != null)
                                {
                                    historyModel.AdditionalMembers.Add(new AdditionalMemberModel
                                    {
                                        FirstName = people.FirstName,
                                        LastName = people.LastName,
                                        Gender = people.Gender,
                                        DateOfBirth = people.DateOfBirth.ToString(),
                                        EntityId = (int)people.EntityId
                                    });
                                }
                            }
                            else if (additionalMember.Entity.CompanyId > 0)
                            {
                                var company = additionalMember.Entity.Companies.FirstOrDefault();
                                if (company != null)
                                {
                                    historyModel.AdditionalMembers.Add(new AdditionalMemberModel
                                    {
                                        FirstName = company.CompanyName,
                                        LastName = string.Empty,
                                        Gender = string.Empty,
                                        DateOfBirth = string.Empty,
                                        EntityId = (int)company.EntityId
                                    });
                                }
                            }


                        }
                        model.MembershipHistory.Add(historyModel);
                    }
                }

                //Get invoice Payments
                var billingHistory = await _invoiceService.GetInvoicePaymentsByEntityId(entityId);
                if (billingHistory != null)
                {
                    foreach (var item in billingHistory)
                    {
                        if (item.MembershipId > 0)
                        {
                            model.MembershipBillingHistory.Add(item);
                        }

                    }
                }

                //Get Credit Balance

                model.AvailableCredit = await _unitOfWork.CreditTransactions.GetCreditBalanceByEntityIdAsync(entityId);

            }
            return model;

        }
        public async Task<Entity> CreateEntity(EntityModel model)
        {
            Entity entity = new Entity();

            if (model.CompanyId > 0)
            {
                entity.CompanyId = model.CompanyId;
            }
            else
            {
                entity.PersonId = model.PersonId;
            }

            entity.Name = model.Name;
            entity.OrganizationId = model.OrganizationId;

            try
            {
                await _unitOfWork.Entities.AddAsync(entity);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return entity;
        }
        public async Task<IEnumerable<EntityDisplayModel>> GetEntitiesByEntityIds(string ids)
        {
            int[] entityIds = ids.Split(',').Select(int.Parse).ToArray();

            var entities = await _unitOfWork.Entities.GetEntitiesByIdsAsync(entityIds);

            List<EntityDisplayModel> model = new List<EntityDisplayModel>();

            foreach (var item in entities)
            {
                EntityDisplayModel entityItem = new EntityDisplayModel();

                entityItem.EntityId = item.EntityId;
                entityItem.PersonId = item.PersonId;
                entityItem.CompanyId = item.CompanyId;

                if (item.PersonId > 0)
                {
                    DateTime? dob = item.People.Select(x => x.DateOfBirth).FirstOrDefault() != null ? (DateTime)item.People.Select(x => x.DateOfBirth).FirstOrDefault() : null;
                    entityItem.DateOfBirth = dob != null && dob.HasValue ? dob.Value.ToString("MM/dd/yyyy") : null;
                    entityItem.Gender = item.People.Select(x => x.Gender).FirstOrDefault();
                    entityItem.Name = $"{item.People.Select(x => x.FirstName).FirstOrDefault()} {item.People.Select(x => x.LastName).FirstOrDefault()}";
                }
                else
                {
                    entityItem.Gender = string.Empty;
                    entityItem.Name = item.Name;
                }

                model.Add(entityItem);
            }

            return model.OrderBy(x => x.Name);
        }

        public async Task<BillingAddressModel> GetBillingAddressByEntityId(int entiidtyId)
        {
            BillingAddressModel billingAddress = new BillingAddressModel();

            var entity = await _unitOfWork.Entities.GetByIdAsync(entiidtyId);

            if (entity.PersonId != null)
            {
                var person = await _unitOfWork.Persons.GetPersonByIdAsync(entity.PersonId ?? 0);

                var personModel = _mapper.Map<PersonModel>(person);

                billingAddress.BillToEmail = personModel.Emails.GetPrimaryEmail();
                billingAddress.BillToName = $"{personModel.Prefix} {personModel.FirstName} {personModel.LastName}";

                var address = person.Addresses.Where(x => x.IsPrimary == (int)Status.Active).FirstOrDefault();
                if (address != null)
                {
                    billingAddress.StreetAddress = $"{address.Address1} {address.Address2} {address.Address3}";
                    billingAddress.City = address.City;
                    billingAddress.State = address.State;
                    billingAddress.Zip = address.Zip.FormatZip();
                }
                else
                {
                    address = person.Addresses.FirstOrDefault();
                    if (address != null)
                    {
                        billingAddress.StreetAddress = $"{address.Address1} {address.Address2} {address.Address3}";
                        billingAddress.City = address.City;
                        billingAddress.State = address.State;
                        billingAddress.Zip = address.Zip.FormatZip();
                    }
                }

            }
            else
            {
                var company = await _unitOfWork.Companies.GetCompanyByIdAsync(entity.CompanyId ?? 0);
                var emails = _mapper.Map<List<EmailModel>>(company.Emails);
                billingAddress.BillToEmail = emails.GetPrimaryEmail();
                billingAddress.BillToName = company.CompanyName;

                var primaryAddress = _mapper.Map<AddressModel>(company.Addresses.Where(x => x.IsPrimary == (int)Status.Active).FirstOrDefault());
                if (primaryAddress != null)
                {
                    billingAddress.StreetAddress = primaryAddress.StreetAddress;
                    billingAddress.City = primaryAddress.City;
                    billingAddress.State = primaryAddress.State;
                    billingAddress.Zip = primaryAddress.Zip.FormatZip();
                }
                else
                {
                    var address = company.Addresses.FirstOrDefault();
                    if (address != null)
                    {
                        billingAddress.StreetAddress = $"{address.Address1} {address.Address2} {address.Address3}";
                        billingAddress.City = address.City;
                        billingAddress.State = address.State;
                        billingAddress.Zip = address.Zip.FormatZip();
                    }
                }
            }
            return billingAddress;
        }

        public async Task<bool> UpdateWebLoginPasword(WebLoginModel model)
        {
            if (model.EntityId > 0)
            {
                var duplicateEntity = await _unitOfWork.Entities.GetEntityByUserNameAsync(model.LoginName.Trim());
                if (duplicateEntity != null)
                {
                    if (duplicateEntity.EntityId != model.EntityId)
                    {
                        throw new InvalidOperationException($"Username already exists.");
                    }
                }

                var entity = await _unitOfWork.Entities.GetByIdAsync(model.EntityId);

                var hasPasswordChanged = false;
                entity.WebLoginName = model.LoginName.Trim();
                entity.AccountLocked = model.AccountLocked;
                if (model.AccountLocked == 0)
                {
                    entity.PasswordFailedAttempts = 0;
                }
                if (model.Password != "*************")
                {
                    PasswordHash hash = new PasswordHash(model.Password);
                    entity.WebPasswordSalt = hash.Salt;
                    entity.WebPassword = hash.Password;
                    hasPasswordChanged = true;
                }
                try
                {
                    _unitOfWork.Entities.Update(entity);
                    await _unitOfWork.CommitAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to update WebLogin");
                }
                //Try Updating User Info
                if (entity.SociableUserId > 0)
                {
                    try
                    {
                        var person = await _unitOfWork.Persons.GetPersonByIdAsync(entity.PersonId ?? 0);
                        var password = String.Empty;
                        var personModel = _mapper.Map<PersonModel>(person);
                        if (hasPasswordChanged)
                        {
                            password = model.Password;
                        }
                        var staffContext = (Staffuser)_httpContextAccessor.HttpContext.Items["StafffUser"];
                        var userInfoExist = await _sociableService.GetUserById(Convert.ToInt32(entity.SociableUserId), staffContext.OrganizationId);
                        dynamic uInfo = JObject.Parse(userInfoExist);
                        if (uInfo.uid == null)
                        {
                            int sociableUserId = await _sociableService.CreatePerson(personModel, null, personModel.OrganizationId ?? 0);
                            entity.SociableUserId = sociableUserId;
                        }
                        if (entity.SociableUserId > 0)
                        {
                            var userInfo = await _sociableService.GetUserById(Convert.ToInt32(entity.SociableUserId), staffContext.OrganizationId);
                            dynamic profile = JObject.Parse(userInfo);
                            if (profile != null && profile.profile_profiles != null)
                            {
                                var profileId = profile.profile_profiles[0].target_id;
                                await _sociableService.UpdatePerson(entity.SociableUserId ?? 0, model.LoginName.Trim(), password, personModel.Emails.GetPrimaryEmail(), personModel.OrganizationId ?? 0, false, false, false);
                                entity.SociableProfileId = profileId;
                                _unitOfWork.Entities.Update(entity);
                                await _unitOfWork.CommitAsync();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Failed to update Sociable login info");
                    }
                }
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateBillingNotification(BillingNotificationModel model)
        {
            if (model.EntityId > 0)
            {
                var entity = await _unitOfWork.Entities.GetByIdAsync(model.EntityId);

                if (model.BillingNotification == "Paper")
                {
                    entity.PreferredBillingCommunication = (int)BillingCommunication.PaperInvoice;
                }
                else if (model.BillingNotification == "Email")
                {
                    entity.PreferredBillingCommunication = (int)BillingCommunication.EmailInvoice;
                }
                else
                {
                    entity.PreferredBillingCommunication = (int)BillingCommunication.PaperAndEmail;
                }
                _unitOfWork.Entities.Update(entity);
                await _unitOfWork.CommitAsync();
                return true;
            }
            return false;
        }

        public async Task<int> UpdateLoginStatus(int entityId, int status)
        {
            var entity = await _unitOfWork.Entities.GetByIdAsync(entityId);

            if (status == (int)LoginStatus.Success)
            {
                entity.PasswordFailedAttempts = 0;
            }
            else
            {
                entity.PasswordFailedAttempts += 1;
                if ((Constants.MAX_FAILED_ATTEMPTS - entity.PasswordFailedAttempts) <= 0)
                {
                    entity.AccountLocked = (int)UserAccountStatus.Locked;
                }
            }
            _unitOfWork.Entities.Update(entity);
            await _unitOfWork.CommitAsync();
            return entity.PasswordFailedAttempts ?? 0;
        }

        public async Task<int> UpdateLoginStatus(Multifactorcode code, int rememberDevice, int status)
        {
            var device = await _unitOfWork.UserDevices.GetByIdAsync(code.DeviceId ?? 0);
            var entity = await _unitOfWork.Entities.GetByIdAsync(code.EntityId ?? 0);

            if (status == (int)LoginStatus.Success)
            {
                entity.PasswordFailedAttempts = 0;
                entity.PortalLastAccessed = DateTime.Now;
                device.LastAccessed = DateTime.Now;
                device.Authenticated = (int)LoginStatus.Success;
                device.RemberDevice = rememberDevice;
                device.LastValidated = DateTime.Now;
                _unitOfWork.UserDevices.Update(device);
            }
            else
            {
                if ((Constants.MAX_FAILED_ATTEMPTS - code.Attempts) <= 0)
                {
                    entity.AccountLocked = (int)UserAccountStatus.Locked;
                }
            }
            _unitOfWork.MultiFactorCodes.Update(code);
            _unitOfWork.Entities.Update(entity);
            await _unitOfWork.CommitAsync();
            return entity.PasswordFailedAttempts ?? 0;
        }

        public async Task<EntitySociableModel> GetSociableEntityDetailsById(int entityId)
        {
            EntitySociableModel model = new EntitySociableModel();
            var entity = await _unitOfWork.Entities.GetByIdAsync(entityId);

            if (entity != null)
            {
                if (entity.PersonId > 0)
                {
                    var person = await _unitOfWork.Persons.GetPersonDetailByIdAsync(entity.PersonId ?? 0);
                    var personModel = _mapper.Map<PersonModel>(person);

                    model = _mapper.Map<EntitySociableModel>(personModel);

                    model.Name = entity.Name;
                    model.Email = personModel.Emails.GetPrimaryEmail();
                    model.Phone = personModel.Phones.GetPrimaryPhoneNumber();

                    var primaryAddress = personModel.Addresses.GetPrimaryAddress();
                    model.StreetAddress = primaryAddress.StreetAddress;
                    model.City = primaryAddress.City;
                    model.State = primaryAddress.State;
                    model.Zip = primaryAddress.Zip.FormatZip();

                }
            }

            return model;
        }
    }
}
