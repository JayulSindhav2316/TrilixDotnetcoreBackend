using AutoMapper;
using Max.Reporting.Application.Contracts.Persistence;
using Max.Reporting.Application.Wrappers;
using Max.Reporting.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Max.Reporting.Application.Features.ReportCategory.Commands.AddCategory
{
    public class AddCategoryCommandHandler : IRequestHandler<AddCategoryCommand, Response<int>>
    {
        private readonly IReportCategoryRepository _reportCategoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AddCategoryCommandHandler> _logger;

        public AddCategoryCommandHandler(IReportCategoryRepository reportCategoryRepository, IMapper mapper, ILogger<AddCategoryCommandHandler> logger)
        {
            _reportCategoryRepository = reportCategoryRepository ?? throw new ArgumentNullException(nameof(reportCategoryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Response<int>> Handle(AddCategoryCommand request, CancellationToken cancellationToken)
        {
            var categoryEntity = _mapper.Map<TrCategory>(request);
            var newCategory = await _reportCategoryRepository.AddAsync(categoryEntity);
            _logger.LogInformation($"Report Category {newCategory.Id} is successfully created.");
            return new Response<int>(newCategory.Id);
        }
    }
}
