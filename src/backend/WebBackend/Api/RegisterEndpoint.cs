using WebBackend.Dto;
using WebBackend.Models;
namespace WebBackend.Api;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
public static class RegisterEndpoint
{
    public static void MapRegisterEndpoint(this IEndpointRouteBuilder app)
    {
        
        app.MapPost("api/register", async (RegisterDto dto, JudgeDbContext db) =>
        {
            Console.WriteLine($"Received form: {dto.Username}");
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return Results.BadRequest(new{message="Empty"});
            }
            string ValidationString=await ValidateRequest(dto, db);
            if ( ValidationString!= "success")
            {
                return Results.BadRequest(new{message=ValidationString});
            }
            Console.WriteLine($"Received register form: {dto.Username}");
            var user=new User
            {
                Username=dto.Username,
                Email=dto.Email,
                
            };

            var hasher=new PasswordHasher<User>();
            user.PasswordHash=hasher.HashPassword(user, dto.Password);
            db.Users.Add(user);
            await db.SaveChangesAsync();
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
        bool emailTaken= await db.Users.AnyAsync(u=>u.Email==dto.Email);
        if (usernameTaken == true)
        {
            return "this username is already taken. Try again";
        }
        return "success";

        //TODO - FURTHER VERIFICATION AND VALIDATION (like if email is right, if password is secure etc.) also moving length check from frontend to backend
    }
}