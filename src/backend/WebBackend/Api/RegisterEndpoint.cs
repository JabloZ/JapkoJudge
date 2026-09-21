using WebBackend.Dto;
using WebBackend.Models;
namespace WebBackend.Api;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebBackend.Globals;
using System.Text.RegularExpressions;
using WebBackend.Globals;

public static class RegisterEndpoint
{
    
    public static void MapRegisterEndpoint(this IEndpointRouteBuilder app)
    {
        
        app.MapPost("api/register", async (RegisterDto dto, JudgeDbContext db) =>
        {
            //todo - password and email validation, maybe in the future mail verification
           
            try{
                if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                {
                    return Results.BadRequest(new{message="Empty"});
                }
                if (dto.Password.Length < 8)
                {
                    return Results.BadRequest(new{message="password must be at least 8 characters long"});
                }
                if (dto.Password.Length > 128)
                {
                    return Results.BadRequest(new{message="password must be at most 128 characters long"});
                }
                var normalizedEmail = dto.Email.Trim().ToLowerInvariant();
                string? ValidationString=await ValidateRequest(dto, db);
                if ( ValidationString!= "success")
                {
                    return Results.BadRequest(new{message=ValidationString});
                }
                
                var user=new User
                {
                    Username=dto.Username,
                    Email=dto.Email
                    
                };
                
                var hasher=new PasswordHasher<User>();
                user.PasswordHash=hasher.HashPassword(user, dto.Password);
                db.Users.Add(user);
                
                await db.SaveChangesAsync();
            }
            catch(DbUpdateException ex)
            {
                return Results.Conflict(new{message="username or email already taken"});
            }
            return Results.Ok(new{message=$"Created user {dto.Username}"});
        });
        
    }
    public static async Task<string?> ValidateRequest(RegisterDto dto, JudgeDbContext db)
    {
        //does the password meet minimum safety requirements (before release)
        bool usernameTaken= await db.Users.AnyAsync(u=>u.Username==dto.Username);
        if (usernameTaken == true)
        {
            return "this username is already taken. Try again";
        }
        if (dto.Username.Length > Globals.Username)
        {
            return "this username is too long! max is 24 characters, try again";
        }
        if (!Regex.IsMatch(dto.Username, @"^[a-zA-Z0-9_-]+$"))
        {
            return "username can only contain alphanumeric characters (a-z, A-Z, 0-9) and '_', '-'";
        }
        bool emailTaken= await db.Users.AnyAsync(u=>u.Email==dto.Email);
        if (emailTaken == true)
        {
            return "this email is already taken. Try again";
        }
        if (dto.Email.Length > 254)
        {
            return "this email is too long!";
        }
        if (!Regex.IsMatch(dto.Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
        {
            return "this email is invalid. correct format is [mail]@[domain]";
        }
        return "success";

        //TODO - FURTHER VERIFICATION AND VALIDATION (like if email is right, if password is secure etc.) also moving length check from frontend to backend
    }
}