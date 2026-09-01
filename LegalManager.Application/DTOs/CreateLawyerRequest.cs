namespace LegalManager.Application.DTOs
{
    public record CreateLawyerRequest(string FirstName, string LastName, string Dni, string Email, string Password, string BarNumber, string? Phone, List<string>? Specialties);
}