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
using AutoMapper;
using Max.Core.Helpers;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.AspNetCore.Http;

namespace Max.Services
{
    public class MembershipService : IMembershipService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<MembershipService> _logger;
        private readonly ISociableService _sociableService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public MembershipService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MembershipService> logger, ISociableService sociableService, IHttpContextAccessor httpContextAccessor)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._logger = logger;
            this._sociableService = sociableService;
            this._httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<Membership>> GetAllMemberships()
        {
            return await _unitOfWork.Memberships
                .GetAllMembershipsAsync();
        }

        public async Task<MembershipModel> GetMembershipById(int id)
        {
            var membership = await _unitOfWork.Memberships
                .GetMembershipByIdAsync(id);
            var membershipMode = _mapper.Map<MembershipModel>(membership);
            return membershipMode;
        }

        public async Task<IEnumerable<Membership>> GetAllMembershipDueByThroughDate(int billingType, DateTime throughDate)
        {
            return await _unitOfWork.Memberships.GetAllMembershipDueByThroughDateAsync(billingType, throughDate);
        }

        public async Task<IEnumerable<Membership>> GetMembershipDuesByMembershipTypeAndThroughDate(int membershipType, DateTime throughDate)
        {
            return await _unitOfWork.Memberships.GetMembershipDuesByMembershipTypeAsync(membershipType, throughDate);
        }
        public async Task<IEnumerable<Membership>> GetMembershipRenewalsDuesByMembershipTypeAsync(int membershipType, DateTime throughDate)
        {
            return await _unitOfWork.Memberships.GetMembershipRenewalDuesByMembershipTypeAsync(membershipType, throughDate);
        }
        public async Task<DateTime> GetMembershipEndDate(int periodId, DateTime startDate)
        {
            return await _unitOfWork.MembershipPeriods.GetMembershipEndDateByIdAsync(periodId, startDate);
        }

        public async Task<Membership> CreateMembership(MembershipModel model)
        {
            Membership membership = new Membership();
            var isValid = ValidMembership(model);
            if (isValid)
            {
                //Map Model Data
                membership = _mapper.Map<Membership>(model);

                await _unitOfWork.Memberships.AddAsync(membership);
                await _unitOfWork.CommitAsync();
            }
            return membership;
        }

        public async Task<MembershipModel> CreateNewMembership(MembershipSessionModel model)
        {
            MembershipModel membershipModel = new MembershipModel();

            if (model.EntityId > 0 && model.MembershipTypeId > 0)
            {

                var entity = await _unitOfWork.Entities.GetByIdAsync(model.EntityId);
                //Get MembershipType details
                var membershipType = await _unitOfWork.Membershiptypes.GetMembershipTypeByIdAsync(model.MembershipTypeId);

                //Create Membership
                var membership = new MembershipModel();
                membership.MembershipTypeId = model.MembershipTypeId;
                membership.StartDate = model.StartDate;
                var period = await _unitOfWork.MembershipPeriods.GetMembershipPeriodByIdAsync(membershipType.PaymentFrequency ?? 0);
                if (period.PeriodUnit == "Year")
                {
                    membership.NextBillDate = membership.StartDate.AddYears(period.Duration);
                }
                else if (period.PeriodUnit == "Month")
                {
                    membership.NextBillDate = membership.StartDate.AddMonths(period.Duration);
                }
                else
                {
                    membership.NextBillDate = membership.StartDate.AddDays(period.Duration);
                }
                membership.EndDate = model.EndDate;
                membership.ReviewDate = membership.EndDate;
                membership.BillableEntityId = model.BillableEntityId;
                membership.CreateDate = DateTime.Now;
                membership.RenewalDate = Constants.MySQL_MinDate;
                membership.TerminationDate = Constants.MySQL_MinDate;
                membership.Status = 1;

                //Add selected Fee
                var membershipFee = model.MembershipFees;
                int counter = 0;
                foreach (var feeItem in model.MembershipFeeIds)
                {
                    var editedFee = membershipFee[counter];
                    var billinFee = new BillingFeeModel();
                    billinFee.MembershipFeeId = feeItem;
                    var fee = await _unitOfWork.Membershipfees.GetMembershipFeeByIdAsync(feeItem);
                    if (fee.FeeAmount != editedFee)
                    {
                        billinFee.Fee = editedFee;
                    }
                    else
                    {
                        billinFee.Fee = fee.FeeAmount;
                    }

                    billinFee.Status = 1;
                    membership.BillingFees.Add(billinFee);
                    counter++;
                }


                var newMembership = await CreateMembership(membership);

                if (newMembership.MembershipId > 0)
                {
                    membershipModel = await GetMembershipById(newMembership.MembershipId);

                    //Add connection
                    var connection = new Membershipconnection();

                    connection.MembershipId = newMembership.MembershipId;
                    connection.EntityId = model.PrimaryMemberEntityId;
                    connection.Status = (int)Status.Active;

                    await _unitOfWork.MembershipConnections.AddAsync(connection);

                    await UpdateSociableMembership(model.PrimaryMemberEntityId, entity.OrganizationId ?? 0, (int)Status.Active, membershipModel.MembershipType.Name);

                     var remainingMembers = model.AdditionalPersons.Where(id => id  != model.PrimaryMemberEntityId);

                    //Add Additional Member connections if any
                    if (remainingMembers.Any())
                    {
                        foreach (var entityId in remainingMembers)
                        {
                            connection = new Membershipconnection();

                            connection.MembershipId = newMembership.MembershipId;
                            connection.EntityId = entityId;
                            connection.Status = (int)Status.Active;

                            await _unitOfWork.MembershipConnections.AddAsync(connection);
                            await UpdateSociableMembership(entityId, entity.OrganizationId ?? 0, (int)Status.Active, membershipModel.MembershipType.Name);

                            //Add relations

                            var relations = await _unitOfWork.Relations.GetAllRelationsByEntityIdAsync(entityId);

                            var reverseRelations = await _unitOfWork.Relations.GetAllReverseRelationsByEntityIdAsync(entityId);

                            if (!(reverseRelations.Where(x => x.RelatedEntityId == entityId).Any() && reverseRelations.Where(x => x.EntityId == model.EntityId).Any()))
                            {
                                if (entityId == model.EntityId)
                                {
                                    continue;
                                }
                                if ((relations.Where(x => x.EntityId == entityId).Any() && relations.Where(x => x.RelatedEntityId == model.EntityId).Any()))
                                {
                                    continue;
                                }


                                //During membership creation we do not know relationship so set it to UnKnown
                                var relationshsips = await _unitOfWork.Relationships.GetAllRelationshipsAsync();

                                Relation relation = new Relation();
                                relation.EntityId = model.EntityId;
                                relation.RelatedEntityId = entityId;
                                var configuration = await _unitOfWork.Configurations.GetConfigurationByOrganizationIdAsync(entity.OrganizationId ?? 0);
                                var checkEntity = await _unitOfWork.Entities.GetByIdAsync(entityId);
                                if (checkEntity.CompanyId > 0)
                                {
                                    if (configuration.ShowDealerShipComanyAsBillableForMembership == (int)Status.Active)
                                    {
                                        relation.RelationshipId = relationshsips.Where(x => x.Relation == "Dealership").Select(x => x.RelationshipId).FirstOrDefault();
                                    }
                                    else
                                    {
                                        relation.RelationshipId = relationshsips.Where(x => x.Relation == "Company").Select(x => x.RelationshipId).FirstOrDefault();
                                    }
                                }
                                else if (checkEntity.PersonId > 0)
                                {
                                    relation.RelationshipId = relationshsips.Where(x => x.Relation == "Employee").Select(x => x.RelationshipId).FirstOrDefault();
                                }
                                else
                                {
                                    relation.RelationshipId = relationshsips.Where(x => x.Relation == "Unknown").Select(x => x.RelationshipId).FirstOrDefault();
                                }
                                relation.StartDate = DateTime.Now;
                                relation.Status = (int)Status.Active;
                                await _unitOfWork.Relations.AddAsync(relation);
                            }
                        }

                        
                        }
                    try
                    {
                        await _unitOfWork.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                                  ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                        throw new Exception("Failed to add Membership Connections");
                    }
                }
            }

            return membershipModel;
        }

        public async Task<bool> CancelNewMembership(MembershipCancelModel model)
        {
            // We are trying to revert the newly created Membership so removing the billing fee, invoice & membership
            // shall work

            //Get Membership

            var membership = await _unitOfWork.Memberships.GetMembershipByIdAsync(model.MembershipId);

            if (membership != null)
            {
                //Get membership connections

                var connections = await _unitOfWork.MembershipConnections.GetMembershipConnectionsByMembershipIdAsync(model.MembershipId);

                if (connections != null)
                {
                    try
                    {
                        _unitOfWork.MembershipConnections.RemoveRange(connections);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                                  ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                        throw new Exception("Failed to delete Membership");
                    }
                }
                //Remove Membership
                try
                {
                    _unitOfWork.Memberships.Remove(membership);
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                              ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                    throw new Exception("Failed to delete Membership");
                }

                //Remove Invoice  
                var invoice = await _unitOfWork.Invoices.GetByIdAsync(model.InvoiceId);
                try
                {
                    _unitOfWork.Invoices.Remove(invoice);
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                              ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                    throw new Exception("Failed to delete Invoice");
                }

                try
                {
                    await _unitOfWork.CommitAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                              ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                    throw new Exception("Failed to delete Invoice");
                }
                return true;
            }
            return false;
        }

        public async Task<Membership> UpdateMembership(MembershipModel model)
        {
            Membership membership = await _unitOfWork.Memberships.GetMembershipByIdAsync(model.MembershipId);

            var isValid = ValidMembership(model);
            if (isValid)
            {
                //Map Model Data
                membership.MembershipTypeId = model.MembershipTypeId;
                membership.StartDate = model.StartDate;
                membership.EndDate = model.EndDate;
                membership.ReviewDate = model.ReviewDate;
                membership.BillableEntityId = model.BillableEntityId;
                membership.NextBillDate = model.NextBillDate;
                membership.BillingOnHold = model.BillingOnHold;
                membership.CreateDate = model.CreateDate;
                membership.RenewalDate = model.RenewalDate;
                membership.TerminationDate = model.TerminationDate;

                _unitOfWork.Memberships.Update(membership);
                await _unitOfWork.CommitAsync();
            }
            return membership;
        }
        public async Task<Membership> UpdateMembershipDetails(MembershipEditModel model)
        {
            Membership membership = await _unitOfWork.Memberships.GetMembershipByIdAsync(model.MembershipId,isNoTracking:false);            
            var entity = await _unitOfWork.Entities.GetByIdAsync(model.BillableEntityId);

            var membershipConnection = await _unitOfWork.MembershipConnections.GetMembershipConnectionsByMembershipIdAsync(model.MembershipId);
            if (membershipConnection != null)
            {
                _unitOfWork.MembershipConnections.RemoveRange(membershipConnection);
            }

            var relations = await _unitOfWork.Relations.GetRevereAndNonReverseRelationsByEntityIdAsync(model.BillableEntityId);

            if (model.Members.Any())
            {
                foreach (var entityId in model.Members)
                {
                    var connection = new Membershipconnection();

                    connection.MembershipId = model.MembershipId;
                    connection.EntityId = entityId;
                    connection.Status = (int)Status.Active;

                    await _unitOfWork.MembershipConnections.AddAsync(connection);

                    //Add relations

                    if (!(relations.Where(x => x.RelatedEntityId == entityId).Any() || relations.Where(x => x.EntityId == entityId).Any()))
                    {
                        if (entityId == model.BillableEntityId)
                        {
                            continue;
                        }

                        //During membership creation we do not know relationship so set it to UnKnown
                        var relationshsips = await _unitOfWork.Relationships.GetAllRelationshipsAsync();

                        Relation relation = new Relation();
                        relation.EntityId = model.BillableEntityId;
                        relation.RelatedEntityId = entityId;
                        if (entity.CompanyId > 0)
                        {
                            relation.RelationshipId = relationshsips.Where(x => x.Relation == "Employee").Select(x => x.RelationshipId).FirstOrDefault();
                        }
                        else
                        {
                            relation.RelationshipId = relationshsips.Where(x => x.Relation == "Unknown").Select(x => x.RelationshipId).FirstOrDefault();
                        }
                        relation.StartDate = DateTime.Now;
                        relation.Status = (int)Status.Active;
                        await _unitOfWork.Relations.AddAsync(relation);
                    }
                }

            }

            //Map Model Data
            membership.StartDate = model.StartDate;
            membership.EndDate = model.EndDate;
            membership.BillableEntityId = model.BillableEntityId;
            membership.NextBillDate = model.NextBillDate;
            _unitOfWork.Memberships.Update(membership);

            //Update Entity            

            if (model.BillingNotificationPreference == "Paper")
            {
                entity.PreferredBillingCommunication = (int)BillingCommunication.PaperInvoice;
            }
            else if (model.BillingNotificationPreference == "Email")
            {
                entity.PreferredBillingCommunication = (int)BillingCommunication.EmailInvoice;
            }
            else
            {
                entity.PreferredBillingCommunication = (int)BillingCommunication.PaperAndEmail;
            }
            _unitOfWork.Entities.Update(entity);
            await _unitOfWork.CommitAsync();

            return membership;
        }
        public async Task<bool> UpdateNextBillDate(int membershipId)
        {
            Membership membership = await _unitOfWork.Memberships.GetByIdAsync(membershipId);
            Membershiptype membershiptype = await _unitOfWork.Membershiptypes.GetByIdAsync(membership.MembershipTypeId ?? 0);
            if (membership != null)
            {
                var period = await _unitOfWork.MembershipPeriods.GetMembershipPeriodByIdAsync(membershiptype.PaymentFrequency ?? 0);
                if (period.PeriodUnit == "Year")
                {
                    membership.NextBillDate = membership.NextBillDate.AddYears(period.Duration);
                }
                else if (period.PeriodUnit == "Month")
                {
                    membership.NextBillDate = membership.NextBillDate.AddMonths(period.Duration);
                }
                else
                {
                    membership.NextBillDate = membership.NextBillDate.AddDays(period.Duration);
                }
                _unitOfWork.Memberships.Update(membership);
                await _unitOfWork.CommitAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> RenewMembership(int membershipId)
        {
            Membership membership = await _unitOfWork.Memberships.GetByIdAsync(membershipId);

            membership.RenewalDate = membership.StartDate.AddDays(1);
            membership.StartDate = membership.EndDate.AddDays(1);

            Membershiptype membershiptype = await _unitOfWork.Membershiptypes.GetByIdAsync(membership.MembershipTypeId ?? 0);
            if (membership != null)
            {
                var frequency = await _unitOfWork.MembershipPeriods.GetMembershipPeriodByIdAsync(membershiptype.PaymentFrequency ?? 0);
                var period = await _unitOfWork.MembershipPeriods.GetMembershipPeriodByIdAsync(membershiptype.Period ?? 0);

                if (frequency.PeriodUnit == "Year")
                {
                    membership.NextBillDate = membership.NextBillDate.AddYears(period.Duration);
                }
                else if (frequency.PeriodUnit == "Month")
                {
                    membership.NextBillDate = membership.NextBillDate.AddMonths(period.Duration);
                }
                else
                {
                    membership.NextBillDate = membership.NextBillDate.AddDays(period.Duration);
                }

                if (period.PeriodUnit == "Year")
                {
                    membership.EndDate = membership.EndDate.AddYears(period.Duration);
                }
                else if (frequency.PeriodUnit == "Month")
                {
                    membership.EndDate = membership.EndDate.AddMonths(period.Duration);
                }
                else
                {
                    membership.EndDate = membership.EndDate.AddDays(period.Duration);
                }

                _unitOfWork.Memberships.Update(membership);
                await _unitOfWork.CommitAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> UpdateNextBillDate(int membershipId, string date)
        {
            Membership membership = await _unitOfWork.Memberships.GetMembershipByIdAsync(membershipId);
            if (membership != null)
            {
                try
                {
                    membership.NextBillDate = DateTime.Parse(date);
                    _unitOfWork.Memberships.Update(membership);
                    await _unitOfWork.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to update next bill date.");
                }
            }
            throw new InvalidOperationException($"Membership: {membershipId} not found.");
        }

        public async Task<bool> DeleteMembership(int MembershipId)
        {
            Membership Membership = await _unitOfWork.Memberships.GetMembershipByIdAsync(MembershipId);

            if (Membership != null)
            {
                _unitOfWork.Memberships.Remove(Membership);
                await _unitOfWork.CommitAsync();
                return true;

            }
            throw new InvalidOperationException($"Membership: {MembershipId} not found.");

        }

        public async Task<bool> TerminateMembership(MembershipChangeModel model)
        {
            var membership = await _unitOfWork.Memberships.GetByIdAsync(model.MembershipId);
            if (membership != null)
            {
                //Inactiavte old MembershipHistory if any
                var previousHistory = await _unitOfWork.MembershipHistories.GetActiveMembershipHistoryByIdAsync(model.MembershipId);

                if (previousHistory != null)
                {
                    previousHistory.Status = (int)MembershipStatus.InActive;
                    _unitOfWork.MembershipHistories.Update(previousHistory);
                }

                //Add membershipHistory record

                var membershipHistory = new Membershiphistory();
                membershipHistory.MembershipId = membership.MembershipId;
                membershipHistory.Reason = model.Reason;
                membershipHistory.StatusDate = DateTime.Now;
                membershipHistory.Status = (int)MembershipStatus.Active;

                await _unitOfWork.MembershipHistories.AddAsync(membershipHistory);


                if (model.ChangeStatus == "OnHold")
                {
                    membership.Status = (int)MembershipStatus.OnHold;
                }
                else if (model.ChangeStatus == "Terminate")
                {
                    membership.Status = (int)MembershipStatus.Terminated;
                }
                else
                {
                    membership.Status = (int)MembershipStatus.Active;
                }
                membership.TerminationDate = DateTime.Now;

                _unitOfWork.Memberships.Update(membership);

                try
                {
                    await _unitOfWork.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to terminate membership.");
                }
            }
            return false;
        }

        public async Task<PieChartModel> GetMembershipsByType()
        {
            var graphData = await _unitOfWork.Memberships.GetMembershipsByType();
            PieChartModel pieChart = new PieChartModel();
            List<int> data = new List<int>();
            List<string> color = new List<string>();
            List<string> labels = new List<string>();
            PieChartData pieChartData = new PieChartData();
            foreach (var item in graphData)
            {
                labels.Add(item.GroupName);
                data.Add(item.Value);
                color.Add(GraphHelper.GetRandomColor());
            }
            pieChart.Labels = labels;
            pieChartData = new PieChartData { Data = data, BackgroundColor = color };
            pieChart.Datasets.Add(pieChartData);
            return pieChart;
        }

        public async Task<BarChartModel> GetMembershipExpirationData()
        {
            var barChartModel = new BarChartModel();

            var data_30 = await _unitOfWork.Memberships.GetMembershipExpirationsInDays(0, 30);
            var data_60 = await _unitOfWork.Memberships.GetMembershipExpirationsInDays(30, 60);
            var data_90 = await _unitOfWork.Memberships.GetMembershipExpirationsInDays(60, 90);
            var data_120 = await _unitOfWork.Memberships.GetMembershipExpirationsInDays(120, 1000);

            BarChartDataset dataSet = new BarChartDataset();

            barChartModel.Labels.Add("30 Days");
            dataSet.Label = "Count";
            dataSet.Data.Add(data_30);
            dataSet.BackgroundColor.Add(GraphHelper.GetRandomColor());

            barChartModel.Labels.Add("60 Days");
            dataSet.Data.Add(data_60);
            dataSet.BackgroundColor.Add(GraphHelper.GetRandomColor());

            barChartModel.Labels.Add("90 Days");
            dataSet.Data.Add(data_90);
            dataSet.BackgroundColor.Add(GraphHelper.GetRandomColor());

            barChartModel.Labels.Add("120 Days");
            dataSet.Data.Add(data_120);
            dataSet.BackgroundColor.Add(GraphHelper.GetRandomColor());

            barChartModel.Datasets.Add(dataSet);

            return barChartModel;

        }
        public async Task<DoughnutChartModel> GetMembershipTerminationsByType()
        {
            var graphData = await _unitOfWork.Memberships.GetMembershipTerminationsByType();
            DoughnutChartModel doughnutChart = new DoughnutChartModel();
            List<int> data = new List<int>();
            List<string> backgroundColor = new List<string>();
            List<string> hoverBackgroundColor = new List<string>();
            List<string> labels = new List<string>();
            DoughnutChartDataSet chartData = new DoughnutChartDataSet();
            foreach (var item in graphData)
            {
                labels.Add(item.GroupName);
                data.Add(item.Value);
                backgroundColor.Add(GraphHelper.GetRandomColor());
                hoverBackgroundColor.Add(GraphHelper.GetRandomColor());
            }
            doughnutChart.Labels = labels;
            chartData = new DoughnutChartDataSet { Data = data, BackgroundColor = backgroundColor, HoverBackgroundColor = hoverBackgroundColor };
            doughnutChart.Datasets.Add(chartData);
            return doughnutChart;
        }
        private async Task<bool> UpdateSociableMembership(int entityId, int organizationId,int membershipStatus, string membershipType)
        {
            //Update Sociable if enabled
            var staff = (Staffuser)_httpContextAccessor.HttpContext.Items["StafffUser"];
            var configuration = await _unitOfWork.Configurations.GetConfigurationByOrganizationIdAsync(organizationId);
            if (configuration.SociableSyncEnabled == (int)Status.Active && staff != null)
            {
                try
                {
                    var memberEntity = await _unitOfWork.Entities.GetByIdAsync(entityId);

                    if (memberEntity.SociableUserId > 0)
                    {

                        //Update user primary email
                        var result = await _sociableService.UpdatePersonMembership(membershipType,membershipStatus,memberEntity.SociableProfileId??0, organizationId);

                        return result;
                    }
                }

                catch (Exception ex)
                {
                    _logger.LogError($"Update UpdatePersonMembership: Update sociable profile failed for EntityId: {entityId} failed with error {ex.Message} {ex.StackTrace}");
                    throw new Exception($"Failed to update Sociable Profile:");
                }
            }
            return false;
        }
        private bool ValidMembership(MembershipModel model)
        {
            //Validate  Name
            if (model.BillableEntityId == 0)
            {
                throw new InvalidOperationException($"Billabble Entity Id not defined.");
            }

            if (model.MembershipTypeId == 0)
            {
                throw new NullReferenceException($"Membbership Type not defined.");
            }

            return true;
        }
    }
}
