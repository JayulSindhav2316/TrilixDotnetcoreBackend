using Max.Reporting.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Reporting.Application.Features.ReportTemplate.Queries.GetTemplateList
{
    public class GetReportTemplateListQuery:IRequest<List<TrTemplate>>
    {
    }
}
