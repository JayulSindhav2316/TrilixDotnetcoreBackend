using Max.Reporting.Application.Features.Reports.Commands.AddReport;
using Max.Reporting.Application.Features.Reports.Commands.CloneReport;
using Max.Reporting.Application.Features.Reports.Commands.DeleteReport;
using Max.Reporting.Application.Features.Reports.Commands.UpdateReport;
using Max.Reporting.Application.Features.Reports.Queries.GetReportByCategory;
using Max.Reporting.Application.Features.Reports.Queries.GetReportList;
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
    public class ReportDefinitionController: ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportDefinitionController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        
        [HttpGet,Route("GetAllReports")]
        [ProducesResponseType(typeof(IEnumerable<ReportVm>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ReportVm>>> GetAllReports()
        {
            var query = new GetReportListQuery();
            var results = await _mediator.Send(query);
            return Ok(results);
        }

        [HttpGet, Route("GetAllReportsByCategory")]
        [ProducesResponseType(typeof(IEnumerable<ReportVm>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ReportVm>>> GetAllReportsByCategory(int categoryId)
        {
            var query = new GetReportByCategoryQuery { CategoryId = categoryId};
            var results = await _mediator.Send(query);
            return Ok(results);
        }

        [HttpPost,Route("AddReport")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<int>> AddReport([FromBody] AddReportCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        
        [HttpPut, Route("UpdateReport")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> UpdateReport([FromBody] UpdateReportCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }
        
        [HttpPost, Route("CloneReport")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> CloneReport([FromBody] CloneReportCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }

        
        [HttpDelete("{id}", Name = "DeleteReport")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> DeleteReport(int id)
        {
            var command = new DeleteReportCommand() { Id = id };
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
