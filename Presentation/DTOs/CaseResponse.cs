using Presentation.Models;

namespace Presentation.DTOs
{
    public record CaseResponse(
        int Id, string CaseNumber, string Title, string Area, CaseStatus Status,
        DateOnly StartDate, DateOnly LastUpdate, DateOnly? ClosingDate,
        string? Description, string? Notes, int ClientId, int CreatedByUserId,
        IReadOnlyList<int> LawyerIds, bool Active);
}