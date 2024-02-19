using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.Interfaces;
using Max.Data.DataModel;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Max.Core;
using Max.Core.Helpers;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Max.Data.Repositories
{
    public class ContactActivityRepository : Repository<Contactactivity>, IContactActivityRepository
    {
        public ContactActivityRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Contactactivity>> GetAllContactActivitiesAsync()
        {
            return await membermaxContext.Contactactivities
                .Include(x => x.Entity)
                .Include(x => x.StaffUser)
                .Include(x => x.Account)
                .Where(x => x.Status == (int)Status.Active)
                .OrderByDescending(x => x.ActivityDate).ToListAsync();
        }

        public async Task<IEnumerable<Contactactivity>> GetContactActivityByEntityIdAsync(int id)
        {
            var contactActivities = await membermaxContext.Contactactivities
                .Include(x => x.Entity)
                    .ThenInclude(x => x.People)
                .Include(x => x.Entity)
                    .ThenInclude(x => x.Companies)
                .Include(x => x.StaffUser)
                .Include(x => x.Account)
                .Include(x => x.Contactactivityinteractions)
                    .ThenInclude(a => a.InteractionEntity)
                        .ThenInclude(b => b.People)
                .Include(x => x.Contactactivityinteractions)
                    .ThenInclude(a => a.InteractionAccount)
                .Where(x =>
                    (x.EntityId == id && x.Status == (int)Status.Active
                    && x.IsDeleted != (int)Status.Active) ||
                    (x.Contactactivityinteractions.Any(a => a.ContactActivityId == x.ContactActivityId
                    && a.InteractionEntityId == id))
                    && (x.IsDeleteforPerson == (int)ActivityDeleteStatus.NotDeleted ||
                        x.IsDeleteforPerson == (int)ActivityDeleteStatus.PartialDeleted ||
                        x.IsDeleteforPerson == null)
                )
                .OrderByDescending(x => x.ActivityDate)
                .ToListAsync();

            //var data = aa.Where(x => !x.Contactactivityinteractions
            //
            //.Any(a => a.IsDeleted == (int)ActivityDeleteStatus.Deleted
            //&& a.InteractionEntityId == id));

            var interactedContactActivities = contactActivities.Where(x => x.Contactactivityinteractions
                                                .Any(a => a.InteractionEntityId == id));

            var deletedInteractedContactActivities = interactedContactActivities
                          .Where(x => (x.Contactactivityinteractions
                          .All(xw => xw.IsDeleted == (int)ActivityDeleteStatus.Deleted)
                          || x.Contactactivityinteractions.Any(a => a.IsDeleted == (int)ActivityDeleteStatus.Deleted))
                          && x.ActivityConnection != (int)ContactActivityConnectionType.RoleOnly);

            deletedInteractedContactActivities = deletedInteractedContactActivities
                .Where(x => x.Contactactivityinteractions
                .Where(a => a.InteractionEntityId == id).All(xa => xa.IsDeleted == 1));
            var result = contactActivities.Except(deletedInteractedContactActivities);
            return result;

            //var ay = aa.Where(x => x.Contactactivityinteractions
            //.Any(a => a.IsDeleted != (int)ActivityDeleteStatus.Deleted
            //&& a.InteractionEntityId == id)
            //&& x.IsDeleteforPerson == (int)ActivityDeleteStatus.PartialDeleted);
            //var data = aa.Except(ab);

            //        var result = await membermaxContext.Contactactivities
            //.Include(x => x.Entity)
            //    .ThenInclude(x => x.People)
            //.Include(x => x.Entity)
            //    .ThenInclude(x => x.Companies)
            //.Include(x => x.StaffUser)
            //.Include(x => x.Account)
            //.Include(x => x.Contactactivityinteractions)
            //    .ThenInclude(a => a.InteractionEntity)
            //        .ThenInclude(b => b.People)
            //.Include(x => x.Contactactivityinteractions)
            //    .ThenInclude(a => a.InteractionAccount)
            //.Where(x =>
            //    (x.EntityId == id && x.Status == (int)Status.Active) &&
            //    (!x.Contactactivityinteractions.Any(a => a.ContactActivityId == x.ContactActivityId &&
            //        a.InteractionEntityId == id && a.IsDeleted == (int)ActivityDeleteStatus.Deleted)) &&
            //    (x.IsDeleteforPerson == (int)ActivityDeleteStatus.NotDeleted ||
            //        x.IsDeleteforPerson == (int)ActivityDeleteStatus.PartialDeleted ||
            //        x.IsDeleteforPerson == null)
            //)
            //.OrderByDescending(x => x.ActivityDate)
            //.ToListAsync();


            //        var bb = await membermaxContext.Contactactivities
            //                             .Include(x => x.Entity)
            //                               .ThenInclude(x => x.People)
            //                                  .Include(x => x.Entity)
            //               .ThenInclude(x => x.Companies)
            //                  .Include(x => x.StaffUser)
            //              .Include(x => x.Account)
            //              .Include(x => x.Contactactivityinteractions)
            //                 .ThenInclude(a => a.InteractionEntity)
            //                 .ThenInclude(b => b.People)
            //               .Include(x => x.Contactactivityinteractions)
            //                   .ThenInclude(a => a.InteractionAccount)
            //                   .Where(x => (x.EntityId == id && x.Status == (int)Status.Active)
            //                   && (!x.Contactactivityinteractions.Any(a =>
            //                 a.InteractionEntityId == id && a.IsDeleted != (int)Status.InActive)) &&
            //                     (x.IsDeleteforPerson == (int)ActivityDeleteStatus.NotDeleted ||
            //                    x.IsDeleteforPerson == (int)ActivityDeleteStatus.PartialDeleted ||
            //                       x.IsDeleteforPerson == null))
            //                   .OrderByDescending(x => x.ActivityDate)
            //                  .ToListAsync();
        }

        public async Task<IEnumerable<Contactactivity>> GetEntityRoleActivityByEntityIdAsync(int id,
            List<int> selectedRoleContactsEntity, List<int> selectedRoleHistoryEntity, string roleName, int roleId)
        {

            var roleContactsActivities = await membermaxContext.Contactactivities
                .Include(x => x.Entity)
                     .ThenInclude(x => x.People)
                 .Include(x => x.Entity)
                     .ThenInclude(x => x.Companies)
                .Include(x => x.StaffUser)
                .Include(x => x.Account)
                .Include(x => x.Contactactivityinteractions)
                   .ThenInclude(a => a.InteractionEntity)
                     .ThenInclude(b => b.People)
                .Include(x => x.Contactactivityinteractions)
                   .ThenInclude(a => a.InteractionAccount)
                   .Where(x => x.AccountId == id && x.Status == (int)Status.Active
                   && x.IsDeleted != (int)Status.Active
                  && x.ActivityConnection != (int)ContactActivityConnectionType.ContactOnly
                  && (x.IsDeleteForRole == 0 || x.IsDeleteForRole == null)
                  && (x.Contactactivityinteractions.Any(a => selectedRoleContactsEntity
                                .Contains(Convert.ToInt32(a.InteractionEntityId)))
                  && x.Contactactivityinteractions.Any(a => a.InteractionRoleId == roleId)
                   && x.ActivityConnection == (int)ContactActivityConnectionType.RoleContact))
               .OrderByDescending(x => x.ActivityDate).ToListAsync();

            var RoleHistoryEntityActivities = await membermaxContext.Contactactivities
               .Include(x => x.Entity)
                    .ThenInclude(x => x.People)
                .Include(x => x.Entity)
                    .ThenInclude(x => x.Companies)
               .Include(x => x.StaffUser)
               .Include(x => x.Account)
               .Include(x => x.Contactactivityinteractions)
                  .ThenInclude(a => a.InteractionEntity)
                    .ThenInclude(b => b.People)
               .Include(x => x.Contactactivityinteractions)
                  .ThenInclude(a => a.InteractionAccount)
                  .Where(x => x.AccountId == id && x.Status == (int)Status.Active
                   && x.IsDeleted != (int)Status.Active
                  && x.ActivityConnection != (int)ContactActivityConnectionType.ContactOnly
                  && x.ContactRoleId == roleId
                 && (x.IsDeleteForRole == 0 || x.IsDeleteForRole == null)
                 && (selectedRoleHistoryEntity.Contains(Convert.ToInt32(x.EntityId))
                  && (x.ActivityConnection == (int)ContactActivityConnectionType.RoleOnly
                      && x.Description.Contains(roleName))))
              .OrderByDescending(x => x.ActivityDate).ToListAsync();

            var finalActivities = roleContactsActivities.Union(RoleHistoryEntityActivities);
            return finalActivities.OrderByDescending(x => x.ActivityDate);
        }

        public async Task<IEnumerable<Contactactivity>> GetContactActivitiesByAccountIdAsync(int accountId)
        {
            return await membermaxContext.Contactactivities
                .Include(x => x.Entity)
                     .ThenInclude(x => x.People)
                 .Include(x => x.Entity)
                     .ThenInclude(x => x.Companies)
                .Include(x => x.StaffUser)
                .Include(x => x.Account)
                .Include(x => x.Contactactivityinteractions)
                   .ThenInclude(a => a.InteractionEntity)
                     .ThenInclude(b => b.People)
                .Include(x => x.Contactactivityinteractions)
                   .ThenInclude(a => a.InteractionAccount)
                 .Where(x => x.AccountId == accountId
                          && x.ActivityConnection != (int)ContactActivityConnectionType.ContactOnly
                         && x.IsDeleted != (int)Status.Active
                          && ((x.Status == (int)Status.Active)
                          || (x.Contactactivityinteractions.Any(a => a.ContactActivityId == x.ContactActivityId
                          && a.InteractionAccountId == accountId)))
                          && ((x.IsDeleteForAccount == 0 || x.IsDeleteForAccount == null)
                            || ((x.IsDeleteForAccount == 0 || x.IsDeleteForAccount == null)
                          && (x.IsDeleteforPerson == 1 || x.IsDeleteforPerson == 2) && x.IsHistoricalDelete == 1)))
                 .OrderByDescending(x => x.ActivityDate).ToListAsync();
        }

        public async Task<IEnumerable<Contactactivity>> GetContactActivityBySearchConditionAsync(int entityId,
            DateTime? fromDate, DateTime? toDate, int? interactionType, int? interactionEntityId)
        {

            var predicate = PredicateBuilder.True<Contactactivity>();
            predicate = predicate.And(x => ((x.EntityId == entityId
               && x.Status == (int)Status.Active
                && x.IsDeleted != (int)Status.Active)
                && x.Contactactivityinteractions.Any(x => x.IsDeleted == null
                || x.IsDeleted == (int)Status.InActive)
                 || x.Contactactivityinteractions.Any(a => a.InteractionEntityId == entityId)
                 || ((x.InteractionType == (int)ContactActivityInteractionType.AccountChange
                     || x.InteractionType == (int)ContactActivityInteractionType.RoleChange)
                     && x.Contactactivityinteractions.Count >= 0
                     && x.EntityId == entityId))
                 && (x.IsDeleteforPerson == 0 || x.IsDeleteforPerson == null) && (x.IsDeleted != (int)Status.Active));

            if (fromDate.HasValue && toDate.HasValue)
            {
                if (fromDate.Value.Date > toDate.Value.Date)
                {
                    throw new InvalidOperationException("Please select a valid Date Range.");
                }
                predicate = predicate.And(x => x.ActivityDate.Value.Date >= fromDate.Value.Date
                               && x.ActivityDate.Value.Date <= toDate.Value.Date);
            }
            else if (fromDate.HasValue)
            {
                predicate = predicate.And(x => x.ActivityDate.Value.Date == fromDate.Value.Date);
            }

            if (interactionType.HasValue)
            {
                if (interactionType.Value == (int)ContactActivityInteractionType.RoleChange)
                {
                    predicate = PredicateBuilder.True<Contactactivity>();
                    predicate = predicate.And(x => ((x.EntityId == entityId
                         && x.Status == (int)Status.Active
                        && x.IsDeleted != (int)Status.Active)
                        && (x.IsDeleteforPerson == 0 || x.IsDeleteforPerson == null))
                        && x.InteractionType == interactionType
                        && x.Contactactivityinteractions.Count >= 0);
                }
                else
                {
                    predicate = predicate.And(x => x.InteractionType == interactionType);
                }
            }

            if (interactionEntityId.HasValue)
            {
                predicate = predicate.And(x => x.Contactactivityinteractions
                            .Any(a => a.InteractionEntityId == interactionEntityId)
                            || x.EntityId == interactionEntityId);
            }
            var data = await membermaxContext.Contactactivities
                .Where(predicate)
                .Include(x => x.Entity)
                .Include(x => x.StaffUser)
                .Include(x => x.Account)
                .Include(x => x.Contactactivityinteractions)
                   .ThenInclude(a => a.InteractionEntity)
                     .ThenInclude(b => b.People)
                .Include(x => x.Contactactivityinteractions)
                   .ThenInclude(a => a.InteractionAccount)
                 .OrderByDescending(x => x.ActivityDate).ToListAsync();
            return data;
        }
        public async Task<IEnumerable<Contactactivity>> GetAccountActivityBySearchConditionAsync(int accountId,
                 DateTime? fromDate, DateTime? toDate, int? interactionType, int? interactionEntityId)
        {

            var predicate = PredicateBuilder.True<Contactactivity>();

            predicate = predicate.And(x =>
                 x.ActivityConnection != (int)ContactActivityConnectionType.ContactOnly
                        && ((x.AccountId == accountId
                        && x.Status == (int)Status.Active
                         && x.IsDeleted != (int)Status.Active)
                       && x.Contactactivityinteractions.Any(x => x.IsDeleted == null
                            || x.IsDeleted == (int)Status.InActive)
                   || (x.Contactactivityinteractions
                     .Any(a => a.InteractionAccountId == accountId))
                     || ((x.InteractionType == (int)ContactActivityInteractionType.AccountChange
                     || x.InteractionType == (int)ContactActivityInteractionType.RoleChange)
                     && x.Contactactivityinteractions.Count >= 0
                     && x.AccountId == accountId))
                      && ((x.IsDeleteForAccount == 0 || x.IsDeleteForAccount == null)
                      || ((x.IsDeleteForAccount == 0 || x.IsDeleteForAccount == null)
                       && x.IsDeleteforPerson == 1 && x.IsHistoricalDelete == 1)) &&(x.IsDeleted != (int)Status.Active));

            if (fromDate.HasValue && toDate.HasValue)
            {
                if (fromDate.Value.Date > toDate.Value.Date)
                {
                    throw new InvalidOperationException("Please select a valid Date Range.");
                }
                predicate = predicate.And(x => x.ActivityDate.Value.Date >= fromDate.Value.Date
                               && x.ActivityDate.Value.Date <= toDate.Value.Date);
            }
            else if (fromDate.HasValue)
            {
                predicate = predicate.And(x => x.ActivityDate.Value.Date == fromDate.Value.Date);
            }

            if (interactionType.HasValue)
            {
                if (interactionType.Value == (int)ContactActivityInteractionType.RoleChange)
                {
                    predicate = PredicateBuilder.True<Contactactivity>();

                    predicate = predicate.And(x =>
                         x.ActivityConnection != (int)ContactActivityConnectionType.ContactOnly
                                && ((x.AccountId == accountId
                                && x.Status == (int)Status.Active
                                 && x.IsDeleted != (int)Status.Active))
                              && ((x.IsDeleteForAccount == 0 || x.IsDeleteForAccount == null)
                              || ((x.IsDeleteForAccount == 0 || x.IsDeleteForAccount == null)
                              && x.IsDeleteforPerson == 1 && x.IsHistoricalDelete == 1))
                              && x.InteractionType == interactionType
                              && x.Contactactivityinteractions.Count >= 0);
                }
                else
                {
                    predicate = predicate.And(x => x.InteractionType == interactionType);
                }
            }

            if (interactionEntityId.HasValue)
            {
                predicate = predicate.And(x => x.Contactactivityinteractions
                .Any(a => a.InteractionEntityId == interactionEntityId)
                || x.EntityId == interactionEntityId);
            }
            var data = await membermaxContext.Contactactivities
                 .Where(predicate)
                 .Include(x => x.Entity)
                 .Include(x => x.StaffUser)
                 .Include(x => x.Account)
                  .Include(x => x.Contactactivityinteractions)
                    .ThenInclude(a => a.InteractionEntity)
                      .ThenInclude(b => b.People)
                 .Include(x => x.Contactactivityinteractions)
                    .ThenInclude(a => a.InteractionAccount)
                  .OrderByDescending(x => x.ActivityDate).ToListAsync();
            return data;
        }

        public async Task<IEnumerable<Contactactivity>> GetEntityRoleActivityBySearchConditionAsync(int accountId,
            DateTime? fromDate, DateTime? toDate, int? interactionType, int? interactionEntityId,
            List<int> selectedRoleContactsEntity, List<int> selectedRoleHistoryEntity, string roleName, int roleId)
        {
            var predicateRoleContactsEntityActivities = PredicateBuilder.True<Contactactivity>();
            var predicateRoleHistoryEntityActivities = PredicateBuilder.True<Contactactivity>();
            predicateRoleContactsEntityActivities = predicateRoleContactsEntityActivities
                .And(x => x.AccountId == accountId && x.Status == (int)Status.Active
                 && x.IsDeleted != (int)Status.Active
                 && x.ActivityConnection != (int)ContactActivityConnectionType.ContactOnly
                 && (x.IsDeleteForRole == 0 || x.IsDeleteForRole == null)
                 && (x.Contactactivityinteractions.Any(a => selectedRoleContactsEntity
                 .Contains(Convert.ToInt32(a.InteractionEntityId)))
                  && x.Contactactivityinteractions.Any(a => a.InteractionRoleId == roleId)
                  && x.ActivityConnection == (int)ContactActivityConnectionType.RoleContact
                  ));

            predicateRoleHistoryEntityActivities = predicateRoleHistoryEntityActivities
                .And(x => x.AccountId == accountId && x.Status == (int)Status.Active
                 && x.IsDeleted != (int)Status.Active
                 && (x.IsDeleteForRole == 0 || x.IsDeleteForRole == null)
                      && x.ActivityConnection == (int)ContactActivityConnectionType.RoleOnly
                   && x.ContactRoleId == roleId
                 && x.Contactactivityinteractions
                 .Select(x => x.InteractionRoleId).Any(x => x == roleId)
                 || ((x.InteractionType == (int)ContactActivityInteractionType.AccountChange
                     || x.InteractionType == (int)ContactActivityInteractionType.RoleChange)
                     && x.Contactactivityinteractions.Count >= 0
                     && x.AccountId == accountId && x.ContactRoleId == roleId)
                 && (selectedRoleHistoryEntity.Contains(Convert.ToInt32(x.EntityId))
                       && x.ActivityConnection != (int)ContactActivityConnectionType.ContactOnly
                      && x.Description.Contains(roleName)));

            if (fromDate.HasValue && toDate.HasValue)
            {
                if (fromDate.Value.Date > toDate.Value.Date)
                {
                    throw new InvalidOperationException("Please select a valid Date Range.");
                }
                predicateRoleContactsEntityActivities = predicateRoleContactsEntityActivities
                    .And(x => x.ActivityDate.Value.Date >= fromDate.Value.Date
                               && x.ActivityDate.Value.Date <= toDate.Value.Date);
                predicateRoleHistoryEntityActivities = predicateRoleHistoryEntityActivities
                    .And(x => x.ActivityDate.Value.Date >= fromDate.Value.Date
                               && x.ActivityDate.Value.Date <= toDate.Value.Date);
            }
            else if (fromDate.HasValue)
            {
                predicateRoleContactsEntityActivities = predicateRoleContactsEntityActivities
                    .And(x => x.ActivityDate.Value.Date == fromDate.Value.Date);
                predicateRoleHistoryEntityActivities = predicateRoleHistoryEntityActivities
                    .And(x => x.ActivityDate.Value.Date == fromDate.Value.Date);
            }

            if (interactionType.HasValue)
            {
                predicateRoleContactsEntityActivities = predicateRoleContactsEntityActivities
                    .And(x => x.InteractionType == interactionType);

                if (interactionType.Value == (int)ContactActivityInteractionType.RoleChange)
                {
                    predicateRoleHistoryEntityActivities = PredicateBuilder.True<Contactactivity>();
                    predicateRoleHistoryEntityActivities = predicateRoleHistoryEntityActivities
                       .And(x => x.AccountId == accountId && x.Status == (int)Status.Active
                          && x.IsDeleted != (int)Status.Active
                          && (x.IsDeleteForRole == 0 || x.IsDeleteForRole == null)
                          && x.ActivityConnection == (int)ContactActivityConnectionType.RoleOnly
                          && x.ContactRoleId == roleId
                          && (selectedRoleHistoryEntity.Contains(Convert.ToInt32(x.EntityId))
                          && x.ActivityConnection != (int)ContactActivityConnectionType.ContactOnly
                          && x.Description.Contains(roleName))
                          && x.InteractionType == interactionType
                          && x.Contactactivityinteractions.Count >= 0);
                }
                else
                {
                    predicateRoleHistoryEntityActivities = predicateRoleHistoryEntityActivities
                    .And(x => x.InteractionType == interactionType);
                }
            }

            if (interactionEntityId.HasValue)
            {
                predicateRoleContactsEntityActivities = predicateRoleContactsEntityActivities
                    .And(x => x.Contactactivityinteractions
                    .Any(a => a.InteractionEntityId == interactionEntityId)
                || x.EntityId == interactionEntityId);
                predicateRoleHistoryEntityActivities = predicateRoleHistoryEntityActivities
                    .And(x => x.Contactactivityinteractions
                    .Any(a => a.InteractionEntityId == interactionEntityId)
               || x.EntityId == interactionEntityId);
            }

            var roleContactsActivities = await membermaxContext.Contactactivities
                .Where(predicateRoleContactsEntityActivities)
                .Include(x => x.Entity)
                .Include(x => x.StaffUser)
                .Include(x => x.Account)
                 .Include(x => x.Contactactivityinteractions)
                   .ThenInclude(a => a.InteractionEntity)
                     .ThenInclude(b => b.People)
                .Include(x => x.Contactactivityinteractions)
                   .ThenInclude(a => a.InteractionAccount)
                 .OrderByDescending(x => x.ActivityDate).ToListAsync();

            var RoleHistoryEntityActivities = await membermaxContext.Contactactivities
                .Where(predicateRoleHistoryEntityActivities)
                .Include(x => x.Entity)
                .Include(x => x.StaffUser)
                .Include(x => x.Account)
                 .Include(x => x.Contactactivityinteractions)
                   .ThenInclude(a => a.InteractionEntity)
                     .ThenInclude(b => b.People)
                .Include(x => x.Contactactivityinteractions)
                   .ThenInclude(a => a.InteractionAccount)
                 .OrderByDescending(x => x.ActivityDate).ToListAsync();

            var finalActivities = roleContactsActivities.Union(RoleHistoryEntityActivities);
            return finalActivities.OrderByDescending(x => x.ActivityDate);
        }


        public async Task<IEnumerable<Contactactivity>> GetContactActivityByAccountAndEntityIdAsync(int accountId, int entityId)
        {
            return await membermaxContext.Contactactivities
                .Include(x => x.Entity)
                .Include(x => x.StaffUser)
                .Include(x => x.Account)
                .Include(x => x.Contactactivityinteractions)
                   .ThenInclude(a => a.InteractionEntity)
                     .ThenInclude(b => b.People)
                .Include(x => x.Contactactivityinteractions)
                   .ThenInclude(a => a.InteractionAccount)
             .Where(x => x.AccountId == accountId
                            && x.Status == (int)Status.Active
                             && x.IsDeleted != (int)Status.Active
                            && (x.EntityId == entityId
                            || x.Contactactivityinteractions.Any(a => a.ContactActivityId == x.ContactActivityId
                            && a.InteractionEntityId == entityId)))
             .OrderByDescending(x => x.ActivityDate).ToListAsync();
        }

        public async Task<IEnumerable<Contactactivity>> GetContactActivityByActivityDateAsync(int entityId, DateTime activityDate)
        {
            return await membermaxContext.Contactactivities
                .Include(x => x.Entity)
                .Include(x => x.StaffUser)
                .Include(x => x.Account)
                .Include(x => x.Contactactivityinteractions)
                   .ThenInclude(a => a.InteractionEntity)
                     .ThenInclude(b => b.People)
                .Include(x => x.Contactactivityinteractions)
                   .ThenInclude(a => a.InteractionAccount)
                .Where(x => (x.EntityId == entityId
                            && x.Status == (int)Status.Active
                             && x.IsDeleted != (int)Status.Active
                           || x.Contactactivityinteractions
                                .Any(a => a.ContactActivityId == x.ContactActivityId
                           && a.InteractionEntityId == entityId))
                        && x.ActivityConnection != (int)ContactActivityConnectionType.ContactOnly
                        && x.ActivityDate > activityDate
                        && x.InteractionType != (int)ContactActivityInteractionType.RoleChange
                        && x.InteractionType != (int)ContactActivityInteractionType.AccountChange
                        && (x.IsDeleteforPerson == 0
                        || x.IsDeleteforPerson == 2
                        || x.IsDeleteforPerson == null))
                       .OrderByDescending(x => x.ActivityDate).ToListAsync();
        }


        public async Task<IEnumerable<Contactactivity>> GetContactActivityByRoleIdAndActivityDateAsync(int entityId, int roleId, DateTime activityDate)
        {
            var activities = new List<Contactactivity>();
            var entity = await membermaxContext.Entities.Include(x => x.Companies)
                .Include(x => x.People).Where(x => x.EntityId == entityId).FirstOrDefaultAsync();
            if (entity.Companies.Count > 0)
            {
                activities = await membermaxContext.Contactactivities
                   .Include(x => x.Entity)
                   .Include(x => x.StaffUser)
                   .Include(x => x.Account)
                   .Include(x => x.Contactactivityinteractions)
                      .ThenInclude(a => a.InteractionEntity)
                        .ThenInclude(b => b.People)
                   .Include(x => x.Contactactivityinteractions)
                      .ThenInclude(a => a.InteractionAccount)
                   .Where(x => (x.AccountId == entity.CompanyId
                              || x.Contactactivityinteractions
                                   .Any(a => a.ContactActivityId == x.ContactActivityId
                              && a.InteractionAccountId == entity.CompanyId))
                           && x.ActivityConnection != (int)ContactActivityConnectionType.ContactOnly
                           && x.ActivityDate > activityDate
                           && x.InteractionType != (int)ContactActivityInteractionType.RoleChange
                           && x.InteractionType != (int)ContactActivityInteractionType.AccountChange
                           && (x.IsDeleteforPerson == 0
                           || x.IsDeleteforPerson == 2
                           || x.IsDeleteforPerson == null
                           || x.IsDeleteForRole == 0
                           || x.IsDeleteForRole == null)
                               && x.Status == (int)Status.Active
                                && x.IsDeleted != (int)Status.Active)
                          .OrderByDescending(x => x.ActivityDate).ToListAsync();
            }
            else
            {
                activities = await membermaxContext.Contactactivities
                    .Include(x => x.Entity)
                    .Include(x => x.StaffUser)
                    .Include(x => x.Account)
                    .Include(x => x.Contactactivityinteractions)
                       .ThenInclude(a => a.InteractionEntity)
                         .ThenInclude(b => b.People)
                    .Include(x => x.Contactactivityinteractions)
                       .ThenInclude(a => a.InteractionAccount)
                    .Where(x => (x.EntityId == entityId
                               || x.Contactactivityinteractions
                                    .Any(a => a.ContactActivityId == x.ContactActivityId
                               && a.InteractionEntityId == entityId))
                            && x.ActivityConnection != (int)ContactActivityConnectionType.ContactOnly
                            && x.ActivityDate > activityDate
                            && x.InteractionType != (int)ContactActivityInteractionType.RoleChange
                            && x.InteractionType != (int)ContactActivityInteractionType.AccountChange
                            && (x.IsDeleteforPerson == 0
                            || x.IsDeleteforPerson == 2
                            || x.IsDeleteforPerson == null
                            || x.IsDeleteForRole == 0
                            || x.IsDeleteForRole == null)
                            && x.Status == (int)Status.Active
                                && x.IsDeleted != (int)Status.Active)
                           .OrderByDescending(x => x.ActivityDate).ToListAsync();
            }
            return activities.Where(x => x.Contactactivityinteractions.Any(x => x.InteractionRoleId == roleId)).ToList();

        }

        public async Task<Contactactivity> GetContactActivityByIdAsync(int id)
        {
            return await membermaxContext.Contactactivities
                .Include(x => x.Contactactivityinteractions)
                .SingleOrDefaultAsync(x => x.ContactActivityId == id
                && x.Status == (int)Status.Active
                 && x.IsDeleted != (int)Status.Active);
        }

        public async Task<IEnumerable<Contactactivity>> GetAccountActivityByEntityAccountDateAndRole(int accountId,
            int entityId, string roleName, DateTime date, bool isAssign)
        {
            return await membermaxContext.Contactactivities
                .Include(x => x.Contactactivityinteractions)
                .Where(x => x.AccountId == accountId && x.IsDeleted != (int)Status.Active
                && x.EntityId == entityId && x.ActivityDate == date && x.Subject.ToLower()
                .Contains("role change")
                && x.Status == (int)Status.Active
                && (x.Description.Contains(roleName) && (isAssign ? x.Description.ToLower()
                .Contains("assigned") : x.Description.ToLower().Contains("unassigned")))).ToListAsync();
        }

        public async Task<IEnumerable<Contactactivity>> GetContactActivityByEffectiveDateAndEndDateAsync(int entityId,
            int accountId, int contactRoleId, DateTime? effectiveDate, DateTime? endDate)
        {
            var assignedData = membermaxContext.Contactactivities.FirstOrDefault(s => s.EntityId == entityId
            && s.AccountId == accountId && s.ContactRoleId == contactRoleId
            && s.ActivityDate.Value.Date == effectiveDate
            && s.Description.ToLower().Contains("assigned"));
            var unassignedData = membermaxContext.Contactactivities.FirstOrDefault(s => s.EntityId == entityId
            && s.AccountId == accountId && s.ContactRoleId == contactRoleId && s.ActivityDate.Value.Date == endDate
            && s.Description.ToLower().Contains("unassigned"));

            if (assignedData != null && unassignedData != null)
            {
                //await membermaxContext.Contactactivities
                //  .Include(x => x.Entity)
                //  .Include(x => x.StaffUser)
                //  .Include(x => x.Account)
                //  .Include(x => x.Contactactivityinteractions)
                //     .ThenInclude(a => a.InteractionEntity)
                //       .ThenInclude(b => b.People)
                //  .Include(x => x.Contactactivityinteractions)
                //     .ThenInclude(a => a.InteractionAccount)
                //  .Where(x => x.EntityId == entityId
                //              && x.Status == (int)Status.Active
                //          && (x.ContactActivityId > assignedData.ContactActivityId
                //          && x.ContactActivityId < unassignedData.ContactActivityId)
                //          && x.Contactactivityinteractions.Any(a=>a.InteractionAccountId==accountId
                //          && a.InteractionEntityId==entityId)
                //          && x.InteractionType != (int)ContactActivityInteractionType.RoleChange
                //          && x.InteractionType != (int)ContactActivityInteractionType.AccountChange)
                //         .OrderByDescending(x => x.ActivityDate).ToListAsync();

                return await membermaxContext.Contactactivities
                   .Include(x => x.Entity)
                   .Include(x => x.StaffUser)
                   .Include(x => x.Account)
                   .Include(x => x.Contactactivityinteractions)
                      .ThenInclude(a => a.InteractionEntity)
                        .ThenInclude(b => b.People)
                   .Include(x => x.Contactactivityinteractions)
                      .ThenInclude(a => a.InteractionAccount)
                   .Where(x => (x.EntityId == entityId ||
                                x.Contactactivityinteractions.Any(a => a.ContactActivityId == x.ContactActivityId
                                && a.InteractionAccountId == accountId && a.InteractionEntityId == entityId))
                                && x.Status == (int)Status.Active
                                 && x.IsDeleted != (int)Status.Active
                           && (x.ContactActivityId != assignedData.ContactActivityId
                           && x.ContactActivityId != unassignedData.ContactActivityId)
                            && x.InteractionType != (int)ContactActivityInteractionType.RoleChange
                           && x.InteractionType != (int)ContactActivityInteractionType.AccountChange)
                          .OrderByDescending(x => x.ActivityDate).ToListAsync();

            }
            else
            {
                return new List<Contactactivity>();
            }
        }

        public async Task<IEnumerable<Contactactivity>> GetRoleAssignmentActivities(int entityId, int accountId, int roleId)
        {
            var activities = await membermaxContext.Contactactivities
                .Where(x => (x.EntityId == entityId
                && x.AccountId == accountId && x.ContactRoleId == roleId)
                || (x.EntityId == entityId && x.AccountId == accountId
                && x.Contactactivityinteractions.Any(x => x.InteractionRoleId == roleId
                && x.InteractionEntityId == entityId && x.InteractionAccountId == accountId)
                && x.InteractionType != (int)ContactActivityInteractionType.AccountChange)
                && x.IsDeleted != (int)Status.Active)
                .ToListAsync();
            return activities.Where(x => x.Contactactivityinteractions.Count <= 1).ToList();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
