namespace Presentation.DTOs
{
    public record CreateCaseRequest(string CaseNumber, string Title, string Area, DateOnly StartDate, string? Description, string? Notes, int ClientId, int LawyerId);
    public record UpdateCaseRequest(string Title, string Area, string? Description, string? Notes);
    public record ChangeCaseStatusRequest(string Status);

    public record ReassignLawyerRequest(int LawyerId);
}