using FluentValidation;
using Max.Reporting.Application.Contracts.Persistence;

namespace Max.Reporting.Application.Features.ReportCategory.Commands.DeleteCategory
{
    public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
    {
        private readonly IReportRepository _reportRepository;
        public DeleteCategoryCommandValidator(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
            RuleFor(v => v.Id).MustAsync(async (model, oldName, cancellation) =>
            {
                return await IsCategoryExistInReport(model.Id, cancellation);
            }).WithMessage(x => $"This Category is accocitated with Report. You can't delete it.");
        }

        private async Task<bool> IsCategoryExistInReport(int id, CancellationToken cancellationToken)
        {
            var isCategoryExists = await _reportRepository.GetAsync(x => x.CategoryId == id, null, "");
            return !isCategoryExists.Any();
        }
    }
}
