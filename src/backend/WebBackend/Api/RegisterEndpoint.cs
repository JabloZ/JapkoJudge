using WebBackend.Dto;
namespace WebBackend.Api;
public static class RegisterEndpoint
{
    public static void MapRegisterEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("api/register", async (RegisterDto dto) =>
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return Results.BadRequest(new{message="Empty"});
            }
            Console.WriteLine($"Received form: {dto.Username}");
            return Results.Ok(new{message="received"});
        });
    }
}