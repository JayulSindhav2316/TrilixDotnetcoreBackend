using FluentValidation;
using Max.Reporting.Application.Contracts.Persistence;

namespace Max.Reporting.Application.Features.ReportCategory.Commands.AddCategory
{
    public class AddCategoryCommandValidator: AbstractValidator<AddCategoryCommand>
    {
        private readonly IReportCategoryRepository _reportCategoryRepository;
        public AddCategoryCommandValidator(IReportCategoryRepository reportCategoryRepository) 
        {
            _reportCategoryRepository= reportCategoryRepository;
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage($"Catagory Name is required")
                .NotNull()
                .MaximumLength(100).WithMessage("Catagory Name must no exceed 100 characters.")
                .MustAsync(IsCategoryName).WithMessage(x=> $"Cateatory {x.Name} already exists.");
            RuleFor(p => p.Description)
                .NotEmpty().WithMessage("Report Description is required")
                .NotNull()
                .MaximumLength(500).WithMessage("Report Description must no exceed 500 characters.");
        }
        private async Task<bool> IsCategoryName(string name, CancellationToken cancellationToken)
        {
            var isNameExists = await _reportCategoryRepository.GetAsync(x => x.Name.ToLower() == name.ToLower(), null, "");
            return !isNameExists.Any();
        }

    }
}
