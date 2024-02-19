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
    public class ReportparameterService : IReportParameterService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ReportparameterService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        } 
        public async Task<Reportparameter> CreateReportParameter(ReportParameterModel model) 
        {
            Reportparameter Reportparameter = new Reportparameter(); 

                Reportparameter.ReportParameterId = model.ReportParameterId;
                Reportparameter.CategoryId = model.CategoryId;
                Reportparameter.Parameter= model.Parameter;
                Reportparameter.Type = model.Type;               

                await _unitOfWork.ReportParemeters.AddAsync(Reportparameter);
                await _unitOfWork.CommitAsync();
            //}
            return Reportparameter;
        }

        public async Task<IEnumerable<Reportparameter>> GetAllRelations()
        {
            return await _unitOfWork.ReportParemeters
                .GetAllParametersAsync();
        }


        //public async Task<bool> DeleteReportparameter(int ReportparameterId)
        //{
        //    Reportparameter Reportparameter = await _unitOfWork.Reportparameters.GetReportparameterByIdAsync(ReportparameterId);

        //    if (Reportparameter != null)
        //    {
        //        _unitOfWork.Reportparameters.Remove(Reportparameter);
        //        await _unitOfWork.CommitAsync();
        //        return true;

        //    }
        //    throw new InvalidOperationException($"Reportparameter: {ReportparameterId} not found.");

        //}

    }
}
