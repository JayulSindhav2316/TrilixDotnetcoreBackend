using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Max.Core.Models;
using Max.Core;

namespace Max.Data.Repositories
{
    public class AutoBillingPaymentRepository : Repository<Autobillingpayment>, IAutoBillingPaymentRepository
    {
        public AutoBillingPaymentRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<Autobillingpayment> GetAutobillingPaymentByIdAsync(int autoBillingPaymentId)
        {
            return await membermaxContext.Autobillingpayments.SingleOrDefaultAsync(a => a.AutoBillingPaymentId == autoBillingPaymentId);
        }
        public async Task<IEnumerable<Autobillingpayment>> GetAutobillingPaymentsAsync()
        {
            return await membermaxContext.Autobillingpayments.ToListAsync();
        }
       
        public async Task<Autobillingpayment> GetAutobillingPaymentByPaymentTransactionIdAsync(int paymentTransactionId)
        {
            return await membermaxContext.Autobillingpayments.SingleOrDefaultAsync(a => a.PaymentTransactionId == paymentTransactionId);
        }
       
        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
