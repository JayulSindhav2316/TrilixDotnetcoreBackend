using AutoMapper;
using Max.Reporting.Application.Contracts.Persistence;
using Max.Reporting.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Reporting.Application.Features.Reports.Queries.GetReportByCategory
{
    public class GetReportByCategoryHandler : IRequestHandler<GetReportByCategoryQuery, List<TrReport>>
    {
        private readonly IReportRepository _reportRepository;
        private readonly IMapper _mapper;

        public GetReportByCategoryHandler(IReportRepository reportRepository, IMapper mapper)
        {
            _reportRepository = reportRepository ?? throw new ArgumentNullException(nameof(reportRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<List<TrReport>> Handle(GetReportByCategoryQuery request, CancellationToken cancellationToken)
        {
            var reportList = await _reportRepository.GetAsync(x=>x.CategoryId == request.CategoryId, null, "Category,Template");

            return _mapper.Map<List<TrReport>>(reportList);
        }
    }
}
