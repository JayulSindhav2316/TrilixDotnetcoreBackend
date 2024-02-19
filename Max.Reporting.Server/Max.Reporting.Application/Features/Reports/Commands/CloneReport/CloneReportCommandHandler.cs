using AutoMapper;
using Max.Reporting.Application.Contracts.Persistence;
using Max.Reporting.Application.Exceptions;
using Max.Reporting.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Max.Reporting.Application.Features.Reports.Commands.CloneReport
{
    public class CloneReportCommandHandler : IRequestHandler<CloneReportCommand>
    {
        private readonly IReportRepository _reportRepository;
        private readonly IReportDefinitionRepository _reportDefinition;
        private readonly IMapper _mapper;
        private readonly ILogger<CloneReportCommand> _logger;

        public CloneReportCommandHandler(IReportRepository reportRepository, IMapper mapper, ILogger<CloneReportCommand> logger, IReportDefinitionRepository reportDefinition)
        {
            _reportRepository = reportRepository;
            _mapper = mapper;
            _logger = logger;
            _reportDefinition = reportDefinition;
        }

        public async Task<Unit> Handle(CloneReportCommand request, CancellationToken cancellationToken)
        {
            var reportToUpdate = await _reportRepository.GetByIdAsync(request.Id);
            if (reportToUpdate == null)
            {
                throw new ApiException("Report Not Found");
            }
            var reportDefinition = await _reportDefinition.GetAsync(x=>x.ReportId == request.Id);
            if (reportDefinition == null)
            {
                throw new ApiException("Report Definition Not Found");
            }

            var reportEntity = new TrReport();
            reportEntity.Id = 0;
            reportEntity.Name= request.Name;
            reportEntity.CategoryId = request.CategoryId;
            reportEntity.Description = request.Description;
            reportEntity.TemplateId = request.TemplateId;
            reportEntity.CreatedDate = DateTime.UtcNow;
            reportEntity.ModifiedDate = DateTime.UtcNow;
            reportEntity.TrDefinitions.Add(new TrDefinition { Definition = reportDefinition.FirstOrDefault().Definition });
            var newReport = await _reportRepository.AddAsync(reportEntity);
            var id = newReport.Id;

            _logger.LogInformation($"Report Cloning {reportToUpdate.Id} is successfully updated.");

            return Unit.Value;
        }
    }
}
