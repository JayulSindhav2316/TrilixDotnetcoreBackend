using FluentValidation;
using Max.Reporting.Application.Contracts.Persistence;
using Max.Reporting.Application.Features.ReportCategory.Commands.UpdateCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Reporting.Application.Features.Reports.Commands.UpdateReport
{
    public class UpdateReportCommandValidator: AbstractValidator<UpdateReportCommand>
    {
        private readonly IReportRepository _reportRepository;
        public UpdateReportCommandValidator(IReportRepository reportRepository)
        {
            _reportRepository= reportRepository;
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("Report Name is required")
                .NotNull()
                .MaximumLength(100).WithMessage("Report Name must no exceed 100 characters.");

            RuleFor(p => p.CategoryId)
                .NotEmpty().WithMessage("Category is required.");

            RuleFor(p => p.Description)
                .NotEmpty().WithMessage("Report Description is required")
                .NotNull()
                .MaximumLength(500).WithMessage("Report Description must no exceed 500 characters.");

            RuleFor(v => v.Name).MustAsync(async (model, oldName, cancellation) =>
            {
                return await IsReportName(model.Name, model.Id, cancellation);
            }).WithMessage(x => $"Report {x.Name} already exists.");

        }

        private async Task<bool> IsReportName(string name, int id, CancellationToken cancellationToken)
        {
            var existName = await _reportRepository.GetByIdAsync(id);
            if (existName != null)
            {
                if (existName.Name.ToLower().Equals(name.ToLower())) { return true; }
            }

            var isNameExists = await _reportRepository.GetAsync(x => x.Name.ToLower() == name.ToLower(), null, "");
            return !isNameExists.Any();
        }
    }
}
