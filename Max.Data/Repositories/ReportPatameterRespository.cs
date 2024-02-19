using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Max.Data.Repositories
{
    public class ReportParameterRepository : Repository<Reportparameter>, IReportParameterRepository
    {
        public ReportParameterRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Reportparameter>> GetAllParametersAsync()
        {
            return await membermaxContext.Reportparameters
                .ToListAsync();
        }

        //public async Task<Reportparameter> GetParameterByIdAsync(int id)
        //{
        //    return await membermaxContext.Reportparameters
        //        .SingleOrDefaultAsync(m => m.ReportParameterId == id);
        //}

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
