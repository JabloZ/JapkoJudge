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

public static class LoginEndpoint
{
   
    public static void MapLoginEndpoint(this IEndpointRouteBuilder app)
    {
        //TODO
        app.MapPost("api/login", async (LoginDto dto, JudgeDbContext db, IConfiguration config)=> //lambda, db and config from dependency injection
        {
            string username=dto.Username;
            string password=dto.Password;
            
            var user= await db.Users.FirstOrDefaultAsync(u=>u.Username==dto.Username);
            if (user==null)
            {
                return Results.BadRequest(new{message=$"User with username {dto.Username} doesn't exist!"});
            }

            var hasher=new PasswordHasher<User>();

            var res = hasher.VerifyHashedPassword(user,  user.PasswordHash, dto.Password);//order is important...
            if (res==PasswordVerificationResult.Failed)
            {
                return Results.BadRequest(new{message="Wrong password! Try again"});
            }
            var token=GenerateJwtToken(user,config);
            
            return Results.Ok(new{message="Succesfully logged in!", token});
        });
        app.MapGet("api/me", (ClaimsPrincipal user) => //is user logged in? works with getSession
        {
            var username=user.FindFirstValue(JwtRegisteredClaimNames.UniqueName);
            return Results.Ok(new{username});
        }).RequireAuthorization();  
        
         
        app.MapPost("api/logout", async(ClaimsPrincipal claims, JudgeDbContext db)=>{
            Console.WriteLine("aaa");
           var userId=int.Parse(claims.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);
           var user= await db.Users.FindAsync(userId);
           if (user is null)
            {
                return Results.NotFound(new{message="User not found"});
            } 

            user.TokenVersion++;
            Console.WriteLine(user.TokenVersion);
            await db.SaveChangesAsync();
            return Results.Ok(new{message="Logged out"});
        });//.RequireAuthorization();
    }
    private static string GenerateJwtToken(User user, IConfiguration config)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim("tokenVersion", user.TokenVersion.ToString())
        };//claims for jwt token, cant put private stuff here
        var key=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
        var creds= new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token=new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims:claims,
            expires:DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );
 
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
   
    
}