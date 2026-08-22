namespace Presentation.DTOs
{
    public record UpdateCaseRequest(string Title, string Area, string? Description, string? Notes);
}