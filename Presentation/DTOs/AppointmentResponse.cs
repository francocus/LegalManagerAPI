using Presentation.Models;

namespace Presentation.DTOs
{
    public record AppointmentResponse(
        int Id, string Title, DateOnly Date, TimeOnly Time, TimeOnly EndTime,
        string Reason, AppointmentStatus Status, AppointmentStatus EffectiveStatus, string? Area,
        string? Location, string? Notes, int ClientId, int LawyerId,
        int? CaseId, bool Active);
}