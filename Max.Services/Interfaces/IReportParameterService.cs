using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface IReportParameterService
    {        
        Task<Reportparameter> CreateReportParameter(ReportParameterModel ReportParameterModel);
        Task<IEnumerable<Reportparameter>> GetAllRelations();
        //Task<bool> DeleteReportParameter(int id);
    }
}
