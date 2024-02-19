using Max.Reporting.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Reporting.Application.Features.Reports.Queries.GetReportList
{
    public class GetReportListQuery:IRequest<List<TrReport>>
    {
    }
}
