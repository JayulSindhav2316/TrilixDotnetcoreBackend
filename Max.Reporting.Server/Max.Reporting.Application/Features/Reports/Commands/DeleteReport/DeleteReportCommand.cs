using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Reporting.Application.Features.Reports.Commands.DeleteReport
{
    public class DeleteReportCommand:IRequest
    {
        public int Id { get; set; }
    }
}
