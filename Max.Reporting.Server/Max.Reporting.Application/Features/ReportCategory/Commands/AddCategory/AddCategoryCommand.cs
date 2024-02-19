using Max.Reporting.Application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Reporting.Application.Features.ReportCategory.Commands.AddCategory
{
    public class AddCategoryCommand:IRequest<Response<int>>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
