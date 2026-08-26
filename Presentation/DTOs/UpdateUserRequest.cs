namespace Presentation.DTOs
{
    public record UpdateUserRequest(string FirstName, string LastName, string Dni, string Email);
}