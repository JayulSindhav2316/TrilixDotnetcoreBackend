using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;
using Max.Core;
using Max.Core.Helpers;

namespace Max.Data.Repositories
{
    public class OpportunityRepository : Repository<Opportunity>, IOpportunityRepository
    {
        public OpportunityRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Opportunity>> GetAllOpportunitiesAsync()
        {
            return await membermaxContext.Opportunities
                .Include(x => x.Pipeline)
                    .ThenInclude(x => x.Pipelinestages)
                .Include(x => x.Pipeline)
                    .ThenInclude(x => x.Pipelineproducts)
                .Include(x => x.AccountContact)
                .Include(x => x.StaffUser)
                .Include(x => x.Company)
                .ToListAsync();
        }
        public async Task<Opportunity> GetOpportunityByIdAsync(int id)
        {
            return await membermaxContext.Opportunities
                .Where(x => x.OpportunityId == id)
                 .Include(x => x.Pipeline)
                    .ThenInclude(x => x.Pipelinestages)
                .Include(x => x.Pipeline)
                    .ThenInclude(x => x.Pipelineproducts)
                .Include(x => x.AccountContact)
                .Include(x => x.StaffUser)
                .Include(x => x.Company)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Opportunity>> GetOpportunitiesByAccountContactAsync(int id)
        {
            return await membermaxContext.Opportunities
               .Where(x => x.AccountContactId == id)
                .Include(x => x.Pipeline)
                    .ThenInclude(x => x.Pipelinestages)
                .Include(x => x.Pipeline)
                    .ThenInclude(x => x.Pipelineproducts)
               .Include(x => x.AccountContact)
               .Include(x => x.StaffUser)
               .ToListAsync();
        }

        public async Task<IEnumerable<Opportunity>> GetOpportunitiesByPipelineIdAsync(int id)
        {
            return await membermaxContext.Opportunities
               .Where(x => x.PipelineId == id)
                .Include(x => x.Pipeline)
                    .ThenInclude(x => x.Pipelinestages)
                .Include(x => x.Pipeline)
                    .ThenInclude(x => x.Pipelineproducts)
               .Include(x => x.AccountContact)
               .Include(x => x.StaffUser)
               .Include(x => x.Company)
               .ToListAsync();
        }

        public async Task<IEnumerable<Opportunity>> GetOpportunitiesByStaffUserIdAsync(int id)
        {
            return await membermaxContext.Opportunities
               .Where(x => x.StaffUserId == id)
                .Include(x => x.Pipeline)
                    .ThenInclude(x => x.Pipelinestages)
                .Include(x => x.Pipeline)
                    .ThenInclude(x => x.Pipelineproducts)
               .Include(x => x.AccountContact)
               .Include(x => x.StaffUser)
               .Include(x => x.Company)
               .ToListAsync();
        }
        public async Task<IEnumerable<Opportunity>> GetOpportunitiesByCompanyIdAsync(int id)
        {
            return await membermaxContext.Opportunities
               .Where(x => x.CompanyId == id)
                .Include(x => x.Pipeline)
                    .ThenInclude(x => x.Pipelinestages)
                .Include(x => x.Pipeline)
                    .ThenInclude(x => x.Pipelineproducts)
               .Include(x => x.AccountContact)
               .Include(x => x.StaffUser)
               .Include(x => x.Company)
               .OrderByDescending(x => x.EstimatedCloseDate)
               .ToListAsync();
        }

        public async Task<IEnumerable<Opportunity>> GetOpportunitiesByStageIdAsync(int id)
        {
            return await membermaxContext.Opportunities
               .Where(x => x.StageId == id)
               .ToListAsync();
        }

        public async Task<IEnumerable<Opportunity>> GetOpportunitiesByProductIdAsync(int id)
        {
            return await membermaxContext.Opportunities
               .Where(x => x.ProductId == id)
               .ToListAsync();
        }


        public async Task<IEnumerable<Opportunity>> GetOpportunitiesBySearchTextAsync(int pipelineId, string searchText)
        {
            var predicate = PredicateBuilder.True<Opportunity>();
            predicate = predicate.And(x => x.PipelineId == pipelineId);
            if ((!searchText.IsNullOrEmpty()))
            {
                searchText = searchText.ToLower();
                var searchList = searchText.Trim().Split(' ').ToList();
                if (searchList.Count > 1)
                {
                    var firstName = searchList.FirstOrDefault();
                    var lastName = searchList.LastOrDefault();
                    predicate = predicate.And(x => (x.StaffUser != null && x.StaffUser.FirstName != null
                        && (x.StaffUser.FirstName.ToLower().StartsWith(firstName)
                                  || x.StaffUser.FirstName.ToLower().Contains(firstName))));
                    predicate = predicate.And(x => (x.StaffUser != null && x.StaffUser.LastName != null
                                && (x.StaffUser.LastName.ToLower().StartsWith(lastName)
                                    || x.StaffUser.LastName.ToLower().Contains(lastName))));
                    predicate = predicate.Or(x => (x.Company != null && x.Company.CompanyName != null
                      && (x.Company.CompanyName.ToLower().StartsWith(searchText)
                          || x.Company.CompanyName.ToLower().Contains(searchText))));

                    predicate = predicate.Or(x => (x.Pipeline.Pipelineproducts.Where(c => c.ProductId == x.ProductId &&
                   (c.ProductName != null && (c.ProductName.ToLower().Contains(firstName)
                    || c.ProductName.ToLower().StartsWith(firstName) || c.ProductName.ToLower().Contains(lastName)
                    || c.ProductName.ToLower().StartsWith(lastName)))).Any()));
                }
                else if (searchList.Count() == 1)
                {
                    predicate = predicate.And(x => (x.StaffUser != null && x.StaffUser.FirstName != null
                   ? x.StaffUser.FirstName.ToLower().StartsWith(searchText)
                                 || x.StaffUser.FirstName.ToLower().Contains(searchText) : false));
                    predicate = predicate.Or(x => (x.StaffUser != null && x.StaffUser.FirstName != null
                    ? x.StaffUser.LastName.ToLower().StartsWith(searchText)
                                    || x.StaffUser.LastName.ToLower().Contains(searchText) : false));
                    predicate = predicate.Or(x => (x.Company != null && x.Company.CompanyName != null
                     && (x.Company.CompanyName.ToLower().StartsWith(searchText)
                              || x.Company.CompanyName.ToLower().Contains(searchText))));
                    predicate = predicate.Or(x => (x.Pipeline.Pipelineproducts.Where(c => c.ProductId == x.ProductId &&
                    (c.ProductName != null && (c.ProductName.ToLower().Contains(searchText)
                     || c.ProductName.ToLower().StartsWith(searchText)))).Any()));
                }

            }

            var result = await membermaxContext.Opportunities
                .Where(x => x.PipelineId == pipelineId)
               .Where(predicate)
                .Include(x => x.Pipeline)
                    .ThenInclude(x => x.Pipelinestages)
                .Include(x => x.Pipeline)
                    .ThenInclude(x => x.Pipelineproducts)
               .Include(x => x.AccountContact)
               .Include(x => x.StaffUser)
               .Include(x => x.Company)
               .OrderByDescending(x => x.EstimatedCloseDate)
               .ToListAsync();
            return result;
        }


        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}