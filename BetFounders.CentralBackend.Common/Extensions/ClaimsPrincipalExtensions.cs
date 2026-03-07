using System.Security.Claims;

namespace BetFounders.CentralBackend.Common.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static long GetId(this ClaimsPrincipal principal)
    {
        if (principal == null) {
            return 0;
        }
        
        var idString = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return long.TryParse(idString, out var id) ? id : 0;
    }

    public static string GetName(this ClaimsPrincipal principal)
    {
        if (principal == null)
        {
            return string.Empty;
        }

        var identityName = principal.Identity?.Name;
        if (!string.IsNullOrEmpty(identityName))
        {
            return identityName;
        }

        return principal.FindFirst(ClaimTypes.Name)?.Value
               ?? principal.FindFirst("name")?.Value
               ?? string.Empty;
    }
}