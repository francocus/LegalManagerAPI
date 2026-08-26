public record CaseResponse(
    int Id, string CaseNumber, string Title, string Area, string Status,
    DateOnly StartDate, DateOnly LastUpdate, DateOnly? ClosingDate,
    string? Description, string? Notes, int ClientId,
    IReadOnlyList<int> LawyerIds, bool Active);