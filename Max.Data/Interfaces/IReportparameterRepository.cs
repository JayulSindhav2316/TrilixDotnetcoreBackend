using Max.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Max.Data.Interfaces
{
    public interface IReportParameterRepository: IRepository<Reportparameter>
    {

        //Task<Reportparameter> GetParameterByIdAsync(int id); //GetParameterByIdAsync

        Task<IEnumerable<Reportparameter>> GetAllParametersAsync();


    }
}
