using AutoMapper;
using Max.Reporting.Application.Contracts.Persistence;
using Max.Reporting.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Reporting.Application.Features.ReportTemplate.Queries.GetTemplateList
{
    public class GetReportTemplateListQueryHandler : IRequestHandler<GetReportTemplateListQuery, List<TrTemplate>>
    {
        private readonly IReportTemplateRepository  _reportTemplateRepository;
        private readonly IMapper _mapper;

        public GetReportTemplateListQueryHandler(IReportTemplateRepository  reportTemplateRepository, IMapper mapper)
        {
            _reportTemplateRepository = reportTemplateRepository ?? throw new ArgumentNullException(nameof(reportTemplateRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<List<TrTemplate>> Handle(GetReportTemplateListQuery request, CancellationToken cancellationToken)
        {
            var reportTemplateList = await _reportTemplateRepository.GetAllAsync();
            return _mapper.Map<List<TrTemplate>>(reportTemplateList);
        }
    }
}
