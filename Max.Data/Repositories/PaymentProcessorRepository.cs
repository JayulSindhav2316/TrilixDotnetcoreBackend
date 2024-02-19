using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;
using System;
using Max.Core.Helpers;

namespace Max.Data.Repositories
{
    public class PaymentProcessorRepository : Repository<Paymentprocessor>, IPaymentProcessorRepository
    {
        public PaymentProcessorRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Paymentprocessor>> GetAllPaymentProcessorsAsync()
        {
            return await membermaxContext.Paymentprocessors
                .ToListAsync();
        }

        public async Task<Paymentprocessor> GetPaymentProcessorByIdAsync(int id)
        {
            return await membermaxContext.Paymentprocessors
                .SingleOrDefaultAsync(m => m.PaymentProcessorId == id);
        }

        public async Task<Paymentprocessor> GetPaymentProcessorByOrganizationIdAsync(int id)
        {
            return await membermaxContext.Paymentprocessors
                .SingleOrDefaultAsync(m => m.OrganizationId == id);
        }

      

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
