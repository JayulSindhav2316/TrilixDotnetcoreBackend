using AutoMapper;
using Max.Reporting.Application.Contracts.Persistence;
using Max.Reporting.Application.Exceptions;
using Max.Reporting.Application.Features.ReportCategory.Commands.DeleteCategory;
using Max.Reporting.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Reporting.Application.Features.Reports.Commands.DeleteReport
{
    public class DeleteReportCommandhandler : IRequestHandler<DeleteReportCommand>
    {
        private readonly IReportRepository  _reportRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DeleteCategoryCommand> _logger;
        public DeleteReportCommandhandler(IReportRepository reportRepository, IMapper mapper, ILogger<DeleteCategoryCommand> logger)
        {
            _reportRepository = reportRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteReportCommand request, CancellationToken cancellationToken)
        {
            var reportToDelete = await _reportRepository.GetByIdAsync(request.Id);
            if (reportToDelete == null)
            {
                throw new ApiException("Report Not Found");
            }

            await _reportRepository.DeleteAsync(reportToDelete);

            _logger.LogInformation($"Report {reportToDelete.Id} is successfully deleted.");

            return Unit.Value;
        }
    }
}
