using Boksi.Application.Schedules.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Boksi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SchedulesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SchedulesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("schedule")]
        public async Task<IActionResult> SetSchedule([FromBody] SetEmployeeScheduleCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result) return BadRequest("Nie udało się zaktualizować harmonogramu.");
            return Ok(new { Message = "Harmonogram zaktualizowany." });
        }

        [HttpPost("time-off")]
        public async Task<IActionResult> SetTimeOff([FromBody] SetEmployeeTimeOffCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new { Message = "Dodano nieobecność.", Id = result });
        }
    }
}
