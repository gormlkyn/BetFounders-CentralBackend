namespace BetFounders.CentralBackend.Common.Models.Users;

public class UserDashboardModel
{
    public long Id { get; set; }

    public string FullName { get; set; }

    public string Username { get; set; }

    public string Email { get; set; }

    public string RoleName { get; set; }

    public IEnumerable<LoginDashboardModel> Logins { get; set; }
}

public class LoginDashboardModel
{
    public DateTime LoginAt { get; set; }

    public string IpAddress { get; set; }

    public string UserAgent { get; set; }

    public bool IsSuccess { get; set; }
}