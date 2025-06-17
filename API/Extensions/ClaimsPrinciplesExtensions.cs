using System;
using System.Security.Claims;

namespace API.Extensions;

public static class ClaimsPrinciplesExtensions
{
    public static string GetUserName(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.Name) ?? throw new Exception("Cannot get username");

        
    }

}
