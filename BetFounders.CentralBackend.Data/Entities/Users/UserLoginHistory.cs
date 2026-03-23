namespace BetFounders.CentralBackend.Data.Entities.Users;

public class UserLoginHistory
{
    public long UserId { get; set; }

    public DateTime LoginAt { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public bool IsSuccess { get; set; }
}