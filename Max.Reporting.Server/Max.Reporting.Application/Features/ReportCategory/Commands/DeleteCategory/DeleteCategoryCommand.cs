using MediatR;

namespace Max.Reporting.Application.Features.ReportCategory.Commands.DeleteCategory
{
    public class DeleteCategoryCommand:IRequest
    {
        public int Id { get; set; }
    }
}
