using MediatR;


namespace Max.Reporting.Application.Features.ReportCategory.Commands.UpdateCategory
{
    public class UpdateCategoryCommand:IRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }        
    }
}
