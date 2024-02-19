using FluentValidation;
using Max.Reporting.Application.Contracts.Persistence;


namespace Max.Reporting.Application.Features.Reports.Commands.AddReport
{
    public class AddReportCommandValidator:AbstractValidator<AddReportCommand>
    {
        private readonly IReportRepository _reportRepository;
        public AddReportCommandValidator(IReportRepository reportRepository) 
        {
            _reportRepository  = reportRepository;
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{Name} is required.")
                .NotNull()
                .MaximumLength(100).WithMessage("{Name} must not exceed 100 characters.")
                .MustAsync(IsReportName).WithMessage(x => $"Report {x.Name} already exists.");

            RuleFor(p => p.CategoryId)
               .NotEmpty().WithMessage("Category is required.");

            RuleFor(p => p.TemplateId)
                .NotEmpty().WithMessage("Template is required.");

            RuleFor(p => p.Description)
                .NotEmpty().WithMessage("Category Description is required")
                .NotNull()
                .MaximumLength(500).WithMessage("Category Description must no exceed 500 characters.");

        }
        private async Task<bool> IsReportName(string name, CancellationToken cancellationToken)
        {
            var isNameExists = await _reportRepository.GetAsync(x => x.Name.ToLower() == name.ToLower(), null, "");
            return !isNameExists.Any();
        }
    }
}
