using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;
using Max.Core;
using System;

namespace Max.Data.Repositories
{
    public class RefundDetailRepository : Repository<Refunddetail>, IRefundDetailRepository
    {
        public RefundDetailRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Refunddetail>> GetRefundsByEntityIdAsync(int id)
        {
            var tokens = await membermaxContext.Refunddetails
                        .Where(x => x.EntityId == id)
                        .ToListAsync();

            return tokens;
        }

        public async Task<IEnumerable<Refunddetail>> GetRefundsByReceiptIdAsync(int id)
        {
            var tokens = await membermaxContext.Refunddetails
                        .Where(x => x.ReceiptDetail.ReceiptHeaderId == id)
                        .ToListAsync();

            return tokens;
        }
        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
