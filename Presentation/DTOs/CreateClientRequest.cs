namespace Presentation.DTOs
{
    public record CreateClientRequest(string Name, string Dni, string Email, string Password, string? Phone);
}