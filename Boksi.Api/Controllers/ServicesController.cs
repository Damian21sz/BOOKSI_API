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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateService(Guid id, [FromBody] UpdateServiceCommand command)
        {
            if (id != command.Id) return BadRequest("ID mismatch");
            var result = await _mediator.Send(command);
            if (!result) return NotFound();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(Guid id)
        {
            var result = await _mediator.Send(new Boksi.Application.Services.Commands.DeleteServiceCommand { Id = id });
            if (!result) return NotFound();
            return Ok();
        }
    }
}
