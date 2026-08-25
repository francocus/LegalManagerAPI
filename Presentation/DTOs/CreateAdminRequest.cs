namespace Presentation.DTOs
{
    public record CreateAdminRequest(string Name, string Dni, string Email, string Password);
}