namespace Presentation.DTOs
{
    public record UserResponse(int Id, string FirstName, string LastName, string Dni, string Email, string Type, string? Phone, string? Address);
}