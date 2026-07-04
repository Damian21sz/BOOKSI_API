using Boksi.Application.Employees.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Boksi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmployeesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] AddEmployeeCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var employeeId = await _mediator.Send(command);
                return Ok(new { Message = "Pracownik został dodany pomyślnie.", EmployeeId = employeeId });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPut("{id}/settings")]
        public async Task<IActionResult> UpdateSettings(System.Guid id, [FromBody] UpdateEmployeeSettingsCommand command)
        {
            if (id != command.EmployeeId) return BadRequest("ID mismatch");
            
            var result = await _mediator.Send(command);
            if (!result) return NotFound("Pracownik nie został znaleziony.");
            
            return Ok(new { Message = "Ustawienia pracownika zaktualizowane." });
        }
    }
}
