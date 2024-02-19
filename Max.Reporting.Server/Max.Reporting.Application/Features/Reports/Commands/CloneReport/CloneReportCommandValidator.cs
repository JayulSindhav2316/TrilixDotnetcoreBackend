using FluentValidation;
using Max.Reporting.Application.Contracts.Persistence;

namespace Max.Reporting.Application.Features.Reports.Commands.CloneReport
{
    
    public class CloneReportCommandValidator: AbstractValidator<CloneReportCommand>
    {
        private readonly IReportRepository _reportRepository;
        public CloneReportCommandValidator(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("Report Name is required")
                .NotNull()
                .MaximumLength(100).WithMessage("Report Name must no exceed 100 characters.")
                .MustAsync(IsReportName).WithMessage(x => $"Report {x.Name} already exists.");

            RuleFor(p => p.CategoryId)
                .NotEmpty().WithMessage("Category is required.");

            RuleFor(p => p.Description)
                .NotEmpty().WithMessage("Report Description is required")
                .NotNull()
                .MaximumLength(500).WithMessage("Report Description must no exceed 500 characters.");

        }

        private async Task<bool> IsReportName(string name, CancellationToken cancellationToken)
        {
            var isNameExists = await _reportRepository.GetAsync(x => x.Name.ToLower() == name.ToLower(), null, "");
            return !isNameExists.Any();
        }
    }
}
