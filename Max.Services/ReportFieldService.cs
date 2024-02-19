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
    public class ReportFieldService : IReportFieldService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ReportFieldService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        } 
        public async Task<Reportfield> CreateReportField(ReportFieldModel model) 
        {
            Reportfield ReportField = new Reportfield();

            ReportField.ReportFieldId = model.ReportFieldId;
            ReportField.ReportCategoryId = model.ReportCategoryId;
            ReportField.FieldName = model.FieldName;
            ReportField.TableName = model.TableName;
            ReportField.FieldTitle = model.FieldTitle;
             
               await _unitOfWork.ReportFields.AddAsync(ReportField);
               await _unitOfWork.CommitAsync();
            
            return ReportField;
        }     

    }
}
