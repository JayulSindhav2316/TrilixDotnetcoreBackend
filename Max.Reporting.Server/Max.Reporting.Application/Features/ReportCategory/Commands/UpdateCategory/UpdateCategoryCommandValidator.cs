using FluentValidation;
using Max.Reporting.Application.Contracts.Persistence;

namespace Max.Reporting.Application.Features.ReportCategory.Commands.UpdateCategory
{
    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        private readonly IReportCategoryRepository _reportCategoryRepository;
        public UpdateCategoryCommandValidator(IReportCategoryRepository reportCategoryRepository)
        {
            _reportCategoryRepository = reportCategoryRepository;

            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{Name} is required")
                .NotNull()
                .MaximumLength(100).WithMessage("{Name} must no exceed 100 characters.");

            RuleFor(p => p.Description)
                .NotEmpty().WithMessage("Category Description is required")
                .NotNull()
                .MaximumLength(500).WithMessage("Category Description must no exceed 500 characters.");

            RuleFor(v => v.Name).MustAsync(async (model, oldName, cancellation) =>
            {
                return await IsCategoryName(model.Name, model.Id, cancellation);
            }
            ).WithMessage(x => $"Cateatory {x.Name} already exists.");
        }

        private async Task<bool> IsCategoryName(string name, int id, CancellationToken cancellationToken)
        {
            var existName = await _reportCategoryRepository.GetByIdAsync(id);
            if (existName != null)
            {
                if (existName.Name.ToLower().Equals(name.ToLower())) { return true; }
            }
            
            var isNameExists = await _reportCategoryRepository.GetAsync(x => x.Name.ToLower() == name.ToLower(), null, "");
            return !isNameExists.Any();
        }
    }
}
