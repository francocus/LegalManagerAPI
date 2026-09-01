namespace LegalManager.Application.DTOs
{
    public record CreateAppointmentRequest(string Title, DateOnly Date, TimeOnly Time, TimeOnly EndTime, string Reason, string? Area, string? Location, string? Notes, int ClientId, int LawyerId, int? CaseId);
}