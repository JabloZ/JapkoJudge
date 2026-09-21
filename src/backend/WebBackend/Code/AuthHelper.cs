namespace WebBackend.Code;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using WebBackend.Models;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;

public static class AuthHelper
{
    public static async Task<bool?> IsUserAdmin(ClaimsPrincipal claims, JudgeDbContext db)
    {
        var userId=claims.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (!int.TryParse(userId, out var id))
        {
            return null;
        }
        var user=await db.Users.FirstOrDefaultAsync(u=>u.Id.ToString()==userId);
        return user?.Admin;
    }
}