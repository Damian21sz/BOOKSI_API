using Boksi.Application.Appointments.Commands;
using Boksi.Application.Appointments.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Boksi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AppointmentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAppointments()
        {
            var result = await _mediator.Send(new GetAppointmentsQuery());
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new { Id = result });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointment(Guid id, [FromBody] UpdateAppointmentCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("ID mismatch");
            }

            var result = await _mediator.Send(command);
            if (!result) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(Guid id)
        {
            var result = await _mediator.Send(new DeleteAppointmentCommand { Id = id });
            if (!result) return NotFound();

            return NoContent();
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveAppointment(Guid id)
        {
            var result = await _mediator.Send(new ApproveAppointmentCommand { AppointmentId = id });
            if (!result) return NotFound();

            return NoContent();
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectAppointment(Guid id)
        {
            var result = await _mediator.Send(new RejectAppointmentCommand { AppointmentId = id });
            if (!result) return NotFound();

            return NoContent();
        }
    }
}
