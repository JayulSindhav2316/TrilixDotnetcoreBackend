using Max.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Max.Data.Interfaces
{
    public interface IReportFieldRepository: IRepository<Reportfield>
    {       

        Task<IEnumerable<Reportfield>> GetAllFieldsAsync();
        Task<Reportfield> GetFieldByTitleAsync(string title);
        Task<Reportfield> GetFieldByCategoryAndTitleAsync(int id, string title);
    }
}
