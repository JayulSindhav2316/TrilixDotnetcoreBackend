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

namespace Max.Services
{
    public class ReportSortOrderService : IReportSortOrderService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ReportSortOrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        } 
        public async Task<Reportsortorder> CreateReportSortorder(ReportSortOrderModel model) 
        {
            Reportsortorder ReportSortOrder = new Reportsortorder();

            ReportSortOrder.ReportSortOrderId = model.ReportSortOrderId;
            ReportSortOrder.ReportId = model.ReportId;
            ReportSortOrder.FieldName = model.FieldName;
            ReportSortOrder.Order = model.Order;
             
               await _unitOfWork.ReportSortorders.AddAsync(ReportSortOrder);
               await _unitOfWork.CommitAsync();
            
            return ReportSortOrder;
        }     

    }
}
