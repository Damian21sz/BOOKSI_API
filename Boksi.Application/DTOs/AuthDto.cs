namespace Boksi.Application.DTOs
{
    public class RegisterClientRequest
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class LoginRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class SocialLoginRequest
    {
        public string Token { get; set; } = null!;
        public string Provider { get; set; } = null!; // "Google" or "Facebook"
    }

    public class AuthResponse
    {
        public string Token { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
    }
}
