namespace Presentation.DTOs
{
    public record CreateUserRequest(string Name, string Dni, string Email, string Password, string Role);
}