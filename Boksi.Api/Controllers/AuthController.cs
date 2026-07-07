
using Boksi.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Boksi.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly Boksi.Application.Interfaces.IEmailService _emailService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            Boksi.Application.Interfaces.IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailService = emailService;
        }

        public class LoginRequest
        {
            public string Email { get; set; } = null!;
            public string Password { get; set; } = null!;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) return Unauthorized(new { message = "Nieprawidłowy e-mail lub hasło" });

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            
            if (result.IsNotAllowed) 
                return Unauthorized(new { message = "Konto nie zostało jeszcze aktywowane. Sprawdź swoją skrzynkę e-mail." });
            
            if (!result.Succeeded) 
                return Unauthorized(new { message = "Nieprawidłowy e-mail lub hasło." });

            if (user.MustChangePassword)
            {
                return Ok(new { 
                    requiresPasswordChange = true, 
                    message = "Wymagana zmiana hasła",
                    email = user.Email
                });
            }

            var token = await GenerateJwtToken(user);
            return Ok(new { token });
        }

        public class RegisterRequest
        {
            public string Email { get; set; } = null!;
            public string Password { get; set; } = null!;
            public string FirstName { get; set; } = null!;
            public string LastName { get; set; } = null!;
            public string AccountType { get; set; } = "User"; // "User" or "Salon"
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (request.AccountType != "User" && request.AccountType != "SalonOwner")
            {
                return BadRequest(new { message = "Nieprawidłowy typ konta." });
            }

            var existing = await _userManager.FindByEmailAsync(request.Email);
            if (existing != null) return BadRequest(new { message = "Użytkownik o podanym adresie email już istnieje." });

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                EmailConfirmed = request.AccountType != "User" 
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new { message = errors });
            }

            await _userManager.AddToRoleAsync(user, request.AccountType);

            if (request.AccountType == "User")
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedToken = Uri.EscapeDataString(token);
                var verifyLink = $"http://localhost:5173/auth/verify-email?token={encodedToken}&email={Uri.EscapeDataString(user.Email)}";
                
                await _emailService.SendEmailAsync(user.Email, "Potwierdź swój e-mail w RIVIE", 
                    $"Dziękujemy za rejestrację! Aby aktywować konto, kliknij w poniższy link:\n\n{verifyLink}");

                return Ok(new { message = "Konto zostało utworzone. Na Twój adres e-mail wysłaliśmy link aktywacyjny." });
            }

            return Ok(new { message = "Konto zostało poprawnie utworzone." });
        }

        public class VerifyEmailRequest
        {
            public string Email { get; set; } = null!;
            public string Token { get; set; } = null!;
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) return BadRequest(new { message = "Nieprawidłowe żądanie." });

            var result = await _userManager.ConfirmEmailAsync(user, request.Token);
            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Nie udało się potwierdzić adresu e-mail. Link mógł wygasnąć." });
            }

            return Ok(new { message = "Adres e-mail został pomyślnie zweryfikowany. Możesz się teraz zalogować." });
        }

        [HttpGet("external-login/{provider}")]
        public IActionResult ExternalLogin(string provider)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Auth", new { ReturnUrl = "/" });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet("external-login-callback")]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                return Redirect($"http://localhost:5173/login?error=Error from external provider: {remoteError}");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return Redirect($"http://localhost:5173/login?error=Nie udało się pobrać danych logowania.");
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (signInResult.Succeeded)
            {
                var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                var token = await GenerateJwtToken(user);
                return Redirect($"http://localhost:5173/login?token={token}");
            }

            if (signInResult.IsNotAllowed)
            {
                return Redirect($"http://localhost:5173/login?error=Konto nie zostało aktywowane.");
            }

            // User doesn't exist, create them
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? "";
            var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? "";

            if (email != null)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        FirstName = firstName,
                        LastName = lastName,
                        EmailConfirmed = true // Trusted provider
                    };
                    var createResult = await _userManager.CreateAsync(user);
                    if (createResult.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "User");
                    }
                }
                
                await _userManager.AddLoginAsync(user, info);
                var token = await GenerateJwtToken(user);
                return Redirect($"http://localhost:5173/login?token={token}");
            }

            return Redirect($"http://localhost:5173/login?error=Nie udało się zalogować.");
        }

        public class ForgotPasswordRequest
        {
            public string Email { get; set; } = null!;
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) 
            {
                // We return Ok even if user doesn't exist for security reasons
                return Ok(new { message = "Jeśli podano poprawny adres e-mail, wysłano na niego link do resetu hasła." });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            
            // Encode token for URL
            var encodedToken = Uri.EscapeDataString(token);
            var resetLink = $"http://localhost:5173/auth/reset-password?token={encodedToken}&email={Uri.EscapeDataString(user.Email ?? "")}";
            
            await _emailService.SendEmailAsync(user.Email, "Resetowanie hasła w RIVIE", 
                $"Aby zresetować hasło, kliknij w poniższy link:\n\n{resetLink}");

            return Ok(new { message = "Jeśli podano poprawny adres e-mail, wysłano na niego link do resetu hasła." });
        }

        public class ResetPasswordRequest
        {
            public string Email { get; set; } = null!;
            public string Token { get; set; } = null!;
            public string NewPassword { get; set; } = null!;
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) return BadRequest(new { message = "Nieprawidłowe żądanie." });

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new { message = errors });
            }

            return Ok(new { message = "Hasło zostało zmienione poprawnie." });
        }

        public class ChangePasswordRequest
        {
            public string Email { get; set; } = null!;
            public string OldPassword { get; set; } = null!;
            public string NewPassword { get; set; } = null!;
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) return BadRequest("Użytkownik nie istnieje");

            var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            user.MustChangePassword = false;
            user.FirstLoginDate = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            var token = await GenerateJwtToken(user);
            return Ok(new { message = "Hasło zmienione poprawnie", token });
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (!string.IsNullOrEmpty(user.SalonId))
            {
                claims.Add(new Claim("SalonId", user.SalonId));
            }

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"] ?? "bardzodlugysekretnykluczdlawystarczajacejdlugosci123!"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
