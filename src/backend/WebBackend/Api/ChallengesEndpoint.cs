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

public static class ChallengesEndpoint
{
    public static void MapChallengesEndpoint(this IEndpointRouteBuilder app) 
    {
        //app.MapPost("api/create")
        //app.MapGet("api/read")
        //app.MapPost("api/update")
        //app.MapPost("api/delete")
        app.MapPost("api/create", async(ChallengeDto dto, ClaimsPrincipal claims, JudgeDbContext db, IConfiguration config) =>
        {
            string title=dto.Title;
            string description=dto.Description;
            var userId=claims.FindFirstValue(JwtRegisteredClaimNames.Sub);
            
            if (userId is null || !int.TryParse(userId, out var userIdClaim)){
                return Results.Unauthorized();
            }
            var username=claims.FindFirstValue(JwtRegisteredClaimNames.UniqueName);
            var challenge=new Challenge
            {
                OwnerId=userIdClaim,
                Title=title,
                Difficulty=0,
                Description=description
            };
            db.Challenges.Add(challenge);
            await db.SaveChangesAsync();
            return Results.Ok(new{message="Challenge created!"});
            
        }).RequireAuthorization();
    }
}