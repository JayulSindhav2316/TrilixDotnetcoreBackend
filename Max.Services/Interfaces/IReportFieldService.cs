using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface IReportFieldService
    {        
        Task<Reportfield> CreateReportField(ReportFieldModel ReportFieldModel);     
        
    }
}
