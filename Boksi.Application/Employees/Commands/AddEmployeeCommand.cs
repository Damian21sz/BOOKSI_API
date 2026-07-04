using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Boksi.Application.Employees.Commands
{
    public class AddEmployeeCommand : IRequest<string>
    {
        [Required]
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public string? JobTitle { get; set; }
    }
}
