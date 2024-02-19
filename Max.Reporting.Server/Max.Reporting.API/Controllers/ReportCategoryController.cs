using Max.Reporting.Application.Features.ReportCategory.Commands.AddCategory;
using Max.Reporting.Application.Features.ReportCategory.Commands.DeleteCategory;
using Max.Reporting.Application.Features.ReportCategory.Commands.UpdateCategory;
using Max.Reporting.Application.Features.ReportCategory.Queries.GetReportCategory;
using Max.Reporting.Application.Features.ReportTemplate.Queries.GetTemplateList;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Max.Reporting.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportCategoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ReportCategoryController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet(Name ="GetAllCategoryList")]
        [ProducesResponseType(typeof(IEnumerable<ReportCategoryVm>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ReportCategoryVm>>> GetAllCategoryList()
        {
            var query = new GetReportCategoryListQuery();
            var results = await _mediator.Send(query);
            return Ok(results);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<int>> AddCategory([FromBody] AddCategoryCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> UpdateCategory([FromBody] UpdateCategoryCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteCategory")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var command = new DeleteCategoryCommand() { Id = id };
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
