namespace Boksi.Application.Interfaces
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? SalonId { get; }
    }
}
