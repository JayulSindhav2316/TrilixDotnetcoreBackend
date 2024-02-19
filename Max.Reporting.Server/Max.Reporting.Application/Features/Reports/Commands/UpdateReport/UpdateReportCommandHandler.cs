using AutoMapper;
using Max.Reporting.Application.Contracts.Persistence;
using Max.Reporting.Application.Exceptions;
using Max.Reporting.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Max.Reporting.Application.Features.Reports.Commands.UpdateReport
{
    public class UpdateReportCommandHandler : IRequestHandler<UpdateReportCommand>
    {
        private readonly IReportRepository _reportRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateReportCommand> _logger;

        public UpdateReportCommandHandler(IReportRepository reportRepository, IMapper mapper, ILogger<UpdateReportCommand> logger)
        {
            _reportRepository = reportRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateReportCommand request, CancellationToken cancellationToken)
        {
            var reportToUpdate = await _reportRepository.GetByIdAsync(request.Id);
            if (reportToUpdate == null)
            {
                throw new ApiException("Report Not Found");
            }

            _mapper.Map(request, reportToUpdate, typeof(UpdateReportCommand), typeof(TrReport));
            reportToUpdate.ModifiedDate = DateTime.UtcNow;
            await _reportRepository.UpdateAsync(reportToUpdate);

            _logger.LogInformation($"Report {reportToUpdate.Id} is successfully updated.");

            return Unit.Value;
        }
    }
}
