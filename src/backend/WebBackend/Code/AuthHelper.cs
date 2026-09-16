using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using WebBackend.Models;
using System.Data.Common;
namespace WebBackend.Code;
public static class AuthHelper
{
    public static async Task<bool?> IsUserAdmin(ClaimsPrincipal claims, JudgeDbContext db)
    {
        var userId=claims.FindFirstValue(JwtRegisteredClaimNames.SUb);
        if (!int.TryParse(userId, out var id))
        {
            return null;
        }
        var user=await db.Users.FirstOrDefaultAsync(u=>u.Id==id);
        return user?.Admin;
    }
}