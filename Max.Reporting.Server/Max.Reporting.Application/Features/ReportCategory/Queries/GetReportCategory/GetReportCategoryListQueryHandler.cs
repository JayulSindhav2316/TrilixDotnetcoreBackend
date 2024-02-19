using AutoMapper;
using Max.Reporting.Application.Contracts.Persistence;
using Max.Reporting.Domain.Entities;
using MediatR;

namespace Max.Reporting.Application.Features.ReportCategory.Queries.GetReportCategory
{
    public class GetReportCategoryListQueryHandler : IRequestHandler<GetReportCategoryListQuery, List<TrCategory>>
    {
        private readonly IReportCategoryRepository  _reportCategoryRepository;
        private readonly IMapper _mapper;
        public GetReportCategoryListQueryHandler(IReportCategoryRepository  reportCategoryRepository, IMapper mapper)
        {
            _reportCategoryRepository = reportCategoryRepository ?? throw new ArgumentNullException(nameof(reportCategoryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<List<TrCategory>> Handle(GetReportCategoryListQuery request, CancellationToken cancellationToken)
        {
            var reportList = await _reportCategoryRepository.GetAllAsync();
            return _mapper.Map<List<TrCategory>>(reportList);
        }
    }
}
