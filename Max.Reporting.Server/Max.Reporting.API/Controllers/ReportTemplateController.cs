using Max.Reporting.Application.Features.Reports.Queries.GetReportList;
using Max.Reporting.Application.Features.ReportTemplate.Queries.GetTemplateList;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Max.Reporting.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportTemplateController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ReportTemplateController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ReportTemplateVm>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ReportTemplateVm>>> GetAllReportTempletes()
        {
            var query = new GetReportTemplateListQuery();
            var results = await _mediator.Send(query);
            return Ok(results);
        }
    }
}
