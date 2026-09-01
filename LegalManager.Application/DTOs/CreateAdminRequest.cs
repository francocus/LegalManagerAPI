namespace LegalManager.Application.DTOs
{
    public record CreateAdminRequest(string FirstName, string LastName, string Dni, string Email, string Password);
}