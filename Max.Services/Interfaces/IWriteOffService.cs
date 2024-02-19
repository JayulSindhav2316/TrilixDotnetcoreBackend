using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface IWriteOffService
    {
        Task<Writeoff> CreateWriteOff(WriteOffModel model);
        Task<List<WriteOffModel>> GetWriteOffByInvoiceDetailId(int id);
    }
}
