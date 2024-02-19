using AutoMapper;
using Max.Reporting.Application.Contracts.Persistence;
using Max.Reporting.Application.Exceptions;
using Max.Reporting.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Max.Reporting.Application.Features.ReportCategory.Commands.UpdateCategory
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand>
    {
        private readonly IReportCategoryRepository _reportCategoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateCategoryCommand> _logger;

        public UpdateCategoryCommandHandler(IReportCategoryRepository reportCategoryRepository, IMapper mapper, ILogger<UpdateCategoryCommand> logger)
        {
            _reportCategoryRepository = reportCategoryRepository ?? throw new ArgumentNullException(nameof(reportCategoryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper)); 
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); 
        }

        public async Task<Unit> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var catagoryToUpdate = await _reportCategoryRepository.GetByIdAsync(request.Id);
            if (catagoryToUpdate == null)
            {
                throw new ApiException("Category Not Found");
            }

            _mapper.Map(request, catagoryToUpdate, typeof(UpdateCategoryCommand), typeof(TrCategory));

            await _reportCategoryRepository.UpdateAsync(catagoryToUpdate);

            _logger.LogInformation($"Report Catagory {catagoryToUpdate.Id} is successfully updated.");

            return Unit.Value;
        }
    }
}
