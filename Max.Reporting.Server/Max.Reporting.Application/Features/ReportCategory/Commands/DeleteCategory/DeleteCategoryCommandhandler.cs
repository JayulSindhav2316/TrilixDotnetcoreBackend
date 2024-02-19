using AutoMapper;
using Max.Reporting.Application.Contracts.Persistence;
using Max.Reporting.Application.Exceptions;
using Max.Reporting.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Max.Reporting.Application.Features.ReportCategory.Commands.DeleteCategory
{
    public class DeleteCategoryCommandhandler : IRequestHandler<DeleteCategoryCommand>
    {
        private readonly IReportCategoryRepository _reportCategoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DeleteCategoryCommand> _logger;

        public DeleteCategoryCommandhandler(IReportCategoryRepository reportCategoryRepository, IMapper mapper, ILogger<DeleteCategoryCommand> logger)
        {
            _reportCategoryRepository = reportCategoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var categoryToDelete = await _reportCategoryRepository.GetByIdAsync(request.Id);
            if (categoryToDelete == null)
            {
                throw new NotFoundException(nameof(TrCategory), request.Id);
            }

            await _reportCategoryRepository.DeleteAsync(categoryToDelete);

            _logger.LogInformation($"Report Category {categoryToDelete.Id} is successfully deleted.");

            return Unit.Value;
        }
    }
}
