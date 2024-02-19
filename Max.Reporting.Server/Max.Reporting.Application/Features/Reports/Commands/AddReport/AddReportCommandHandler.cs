using AutoMapper;
using Max.Reporting.Application.Contracts.Persistence;
using Max.Reporting.Application.Exceptions;
using Max.Reporting.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Max.Reporting.Application.Features.Reports.Commands.AddReport
{
    public class AddReportCommandHandler : IRequestHandler<AddReportCommand,int>
    {
        private readonly IReportRepository _reportRepository;
        private readonly IReportTemplateRepository _reportTemplateRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AddReportCommandHandler> _logger;
        public AddReportCommandHandler(IReportRepository reportRepository, IMapper mapper, ILogger<AddReportCommandHandler> logger,IReportTemplateRepository reportTemplateRepository)
        {
            _reportRepository = reportRepository ?? throw new ArgumentNullException(nameof(reportRepository));
            _reportTemplateRepository = reportTemplateRepository ?? throw new ArgumentNullException(nameof(reportTemplateRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<int> Handle(AddReportCommand request, CancellationToken cancellationToken)
        {
            var reportBlankTemplate = await _reportTemplateRepository.GetByIdAsync(request.TemplateId);
            if (reportBlankTemplate == null)
            {
                throw new NotFoundException(nameof(TrTemplate), request.TemplateId);
            }            
            var reportEntity = _mapper.Map<TrReport>(request);
            reportEntity.CreatedDate = DateTime.UtcNow;
            reportEntity.ModifiedDate = DateTime.UtcNow;
            reportEntity.TrDefinitions.Add(new TrDefinition { Definition = reportBlankTemplate.Definition });
            var newReport = await _reportRepository.AddAsync(reportEntity);
            var id = newReport.Id;

            var reportData = await _reportRepository.GetAsync(x=>x.Id == id,null, "TrDefinitions");
            


            _logger.LogInformation($"Report {newReport.Id} is successfully created.");
            return id;
        }
    }
}
