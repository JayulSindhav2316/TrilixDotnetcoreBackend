﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface ICreditServiceService
    {
        Task<Receiptdetail> CreateCreditTransaction(CreditTransactionModel model);

    }
}
