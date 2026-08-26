namespace Presentation.DTOs
{
    public record CreateCaseRequest(string CaseNumber, string Title, string Area, DateOnly StartDate, string Description, string? Notes, int ClientId, int LawyerId);
}