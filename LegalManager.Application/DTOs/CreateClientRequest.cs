namespace LegalManager.Application.DTOs
{
    public record CreateClientRequest(string FirstName, string LastName, string Dni, string Email, string Password, string? Phone, string? Address);
}