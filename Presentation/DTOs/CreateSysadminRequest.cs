namespace Presentation.DTOs
{
    public record CreateSysadminRequest(string Name, string Dni, string Email, string Password);
}