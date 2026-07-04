using Boksi.Application.Interfaces;
using Boksi.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Finbuckle.MultiTenant.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Boksi.Application.Employees.Commands
{
    public class AddEmployeeCommandHandler : IRequestHandler<AddEmployeeCommand, string>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IApplicationDbContext _dbContext;
        private readonly ITenantInfo _tenantInfo;

        public AddEmployeeCommandHandler(
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            IApplicationDbContext dbContext,
            ITenantInfo tenantInfo)
        {
            _userManager = userManager;
            _emailService = emailService;
            _dbContext = dbContext;
            _tenantInfo = tenantInfo;
        }

        public async Task<string> Handle(AddEmployeeCommand request, CancellationToken cancellationToken)
        {
            if (_tenantInfo == null || string.IsNullOrEmpty(_tenantInfo.Id))
            {
                throw new InvalidOperationException("Tenant not found in context.");
            }

            var user = await _userManager.FindByEmailAsync(request.Email);
            var isNewUser = false;

            if (user == null)
            {
                // Generate new user
                user = new ApplicationUser
                {
                    UserName = request.Email,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    IsActive = true
                };

                // Generate random password or send a link to create one.
                // For now, generating a dummy password.
                var dummyPassword = "Password123!";
                var result = await _userManager.CreateAsync(user, dummyPassword);

                if (!result.Succeeded)
                {
                    throw new Exception("Failed to create user account: " + string.Join(", ", result.Errors));
                }

                isNewUser = true;
            }

            // Check if employee already exists in this tenant
            var existingEmployee = await _dbContext.Employees
                .FirstOrDefaultAsync(e => e.Email == request.Email, cancellationToken);

            if (existingEmployee != null)
            {
                throw new Exception("Employee with this email already exists in this salon.");
            }

            var employee = new Employee
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                JobTitle = request.JobTitle,
                ApplicationUserId = user.Id,
                IsActive = true
            };

            _dbContext.Employees.Add(employee);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Send Email
            var salonName = _tenantInfo.Name ?? "naszego salonu";
            var subject = $"Witaj w systemie Booksi - salon {salonName}";
            
            var emailBody = isNewUser 
                ? $"Zostałeś dodany do salonu {salonName}. Twoje konto zostało utworzone. Login: {request.Email}, Hasło tymczasowe: Password123!" 
                : $"Zostałeś dodany do salonu {salonName}. Zaloguj się swoimi obecnymi danymi konta.";

            await _emailService.SendEmailAsync(request.Email, subject, emailBody);

            return employee.Id.ToString();
        }
    }
}
