using WebBackend.Dto;
using WebBackend.Models;
namespace WebBackend.Api;
public static class RegisterEndpoint
{
    public static void MapRegisterEndpoint(this IEndpointRouteBuilder app)
    {
        
        app.MapPost("api/register", async (RegisterDto dto) =>
        {
            Console.WriteLine($"Received form: {dto.Username}");
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            {
                Console.WriteLine(dto.Password);
                return Results.BadRequest(new{message="Empty"});
            }
            Console.WriteLine($"Received form: {dto.Username}");
            /*var user=new User
            {
                Username=dto.Username,
                Email=dto.Email,
                
            }*/
            return Results.Ok(new{message="received"});
        });
    }
}