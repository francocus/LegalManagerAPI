namespace Presentation.DTOs
{
    public record CreateAppointmentRequest(string Title, DateOnly Date, string Time, string EndTime, string Reason, string? Area, string? Location, string? Notes, int ClientId, int LawyerId, int? CaseId);
}