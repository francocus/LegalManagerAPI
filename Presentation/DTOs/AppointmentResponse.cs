public record AppointmentResponse(
    int Id, string Title, DateOnly Date, string Time, string EndTime,
    string Reason, string Status, string EffectiveStatus, string? Area,
    string? Location, string? Notes, int ClientId, int LawyerId,
    int? CaseId, bool Active);