using WebBackend.Dto;
using WebBackend.Models;
namespace WebBackend.Api;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using WebBackend.Migrations;
using System.IO;
using WebBackend.Code;
using System.Reflection.Metadata;

public static class UserEndpoint
{
    public static void MapUserEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/IsUserAdmin",async(JudgeDbContext db, ClaimsPrincipal claims) =>
        {
            User user=null;
            try{
                var userId=claims.FindFirstValue(JwtRegisteredClaimNames.Sub);
                if (!int.TryParse(userId, out var id)){
                    return Results.BadRequest(new{message="User not found!"});
                }
                user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user is null)
                {
                    return Results.BadRequest(new{message="No user"});
                }
                return Results.Ok(new{admin=user.Admin});
            }
            catch(Exception ex)
            {
                return Results.Problem(statusCode: 500, detail: "Internal server error");
            }
            
            
        }).RequireAuthorization();
        app.MapGet("api/users/{username}/profile", async ( string username, JudgeDbContext db) =>
        {
            var user=await db.Users.FirstOrDefaultAsync(u=>u.Username==username);
            if (user is null)
            {
                return Results.NotFound($"User '{username}' not found.");
            }
            var submissions = await db.Submissions
            .Where(s => s.OwnerId == user.Id && s.Status=="1")
            .ToListAsync();

            var manifestIds = submissions
                .Select(s => s.ManifestId)
                .Distinct();

            var manifests = await db.ChallengesLanguages
                .Where(cl => manifestIds.Contains(cl.Id))
                .ToListAsync();

            var challengeIds = manifests
                .Select(m => m.ChallengeId)
                .Distinct();

            var challenges = await db.Challenges
                .Where(c => challengeIds.Contains(c.Id))
                .ToListAsync();
            int points=0;
            int challengeCount=0;
            foreach(Challenge ch in challenges)
            {
                points=points+(int)Math.Pow(2.0,(double)ch.Difficulty);
                challengeCount++;
            }
            ProfileDto dto=new ProfileDto();
            dto.Points=points;
            dto.Challenges=challengeCount;
            return Results.Ok(new{message="Success", Profile=dto});
        }).RequireAuthorization();
    }
}
