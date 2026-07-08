namespace WebBackend.Dto;
public record RegisterDto(
    string Username,
    string Email,
    string Password
);