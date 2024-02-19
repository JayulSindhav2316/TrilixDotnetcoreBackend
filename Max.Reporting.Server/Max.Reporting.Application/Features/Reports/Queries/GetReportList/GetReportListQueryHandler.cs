using AutoMapper;
using Max.Reporting.Application.Contracts.Persistence;
using Max.Reporting.Domain.Entities;
using MediatR;


namespace Max.Reporting.Application.Features.Reports.Queries.GetReportList
{
    public class GetReportListQueryHandler : IRequestHandler<GetReportListQuery,List<TrReport>>
    {
        private readonly IReportRepository _reportRepository;
        private readonly IMapper _mapper;
        public GetReportListQueryHandler(IReportRepository reportRepository, IMapper mapper)
        {
            _reportRepository = reportRepository ?? throw new ArgumentNullException(nameof(reportRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<TrReport>> Handle(GetReportListQuery request, CancellationToken cancellationToken)
        {            
            var reportList = await _reportRepository.GetAsync(null, null, "Category,Template");

            return _mapper.Map<List<TrReport>>(reportList);
        }
    }
}
