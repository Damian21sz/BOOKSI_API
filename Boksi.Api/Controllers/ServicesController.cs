using Boksi.Application.DTOs;
using Boksi.Application.Services.Commands;
using Boksi.Application.Services.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Boksi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ServicesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ServicesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<ServiceDto>>> GetServices()
        {
            var services = await _mediator.Send(new GetServicesQuery());
            return Ok(services);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateService([FromBody] CreateServiceCommand command)
        {
            var serviceId = await _mediator.Send(command);
            return Ok(serviceId);
        }
    }
}
