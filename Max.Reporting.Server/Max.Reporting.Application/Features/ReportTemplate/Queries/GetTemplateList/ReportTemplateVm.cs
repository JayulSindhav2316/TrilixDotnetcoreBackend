using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Reporting.Application.Features.ReportTemplate.Queries.GetTemplateList
{
    public class ReportTemplateVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Definition { get; set; }
    }
}
