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

        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _mediator.Send(new Boksi.Application.Employees.Queries.GetEmployeesQuery());
            return Ok(employees);
        }

        [HttpPut("{id}/deactivate")]
        public async Task<IActionResult> DeactivateEmployee(System.Guid id)
        {
            var result = await _mediator.Send(new Boksi.Application.Employees.Commands.DeactivateEmployeeCommand { EmployeeId = id });
            if (!result) return NotFound("Pracownik nie został znaleziony.");
            return Ok(new { Message = "Status pracownika został pomyślnie zmieniony." });
        }

        [HttpPut("{id}/settings")]
        public async Task<IActionResult> UpdateSettings(System.Guid id, [FromBody] UpdateEmployeeSettingsCommand command)
        {
            if (id != command.EmployeeId) return BadRequest("ID mismatch");
            
            var result = await _mediator.Send(command);
            if (!result) return NotFound("Pracownik nie został znaleziony.");
            
            return Ok(new { Message = "Ustawienia pracownika zaktualizowane." });
        }

        [HttpPut("{id}/profile")]
        public async Task<IActionResult> UpdateProfile(System.Guid id, [FromBody] UpdateEmployeeProfileCommand command)
        {
            if (id != command.EmployeeId) return BadRequest("ID mismatch");
            
            var result = await _mediator.Send(command);
            if (!result) return NotFound("Pracownik nie został znaleziony.");
            
            return Ok(new { Message = "Profil pracownika zaktualizowany." });
        }

        [HttpPut("{id}/services")]
        public async Task<IActionResult> UpdateServices(System.Guid id, [FromBody] UpdateEmployeeServicesCommand command)
        {
            if (id != command.EmployeeId) return BadRequest("ID mismatch");
            
            var result = await _mediator.Send(command);
            if (!result) return NotFound("Pracownik nie został znaleziony.");
            
            return Ok(new { Message = "Usługi pracownika zaktualizowane." });
        }

        [HttpPost("{id}/gallery")]
        public async Task<IActionResult> AddGalleryImage(System.Guid id, [FromBody] Boksi.Domain.Entities.GalleryImage image, [FromServices] Boksi.Application.Interfaces.IApplicationDbContext dbContext)
        {
            image.EmployeeId = id;
            dbContext.GalleryImages.Add(image);
            await dbContext.SaveChangesAsync(default);
            return Ok(image);
        }

        [HttpGet("{id}/gallery")]
        public async Task<IActionResult> GetGalleryImages(System.Guid id, [FromServices] Boksi.Application.Interfaces.IApplicationDbContext dbContext)
        {
            var images = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(
                System.Linq.Queryable.Where(dbContext.GalleryImages, g => g.EmployeeId == id));
            return Ok(images);
        }
    }
}
