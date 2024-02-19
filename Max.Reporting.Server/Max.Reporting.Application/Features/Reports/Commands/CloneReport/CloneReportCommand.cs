using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Reporting.Application.Features.Reports.Commands.CloneReport
{
    public class CloneReportCommand:IRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public int TemplateId { get; set; }
    }
}
