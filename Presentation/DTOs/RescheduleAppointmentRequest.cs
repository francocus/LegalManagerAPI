namespace Presentation.DTOs
{
    public record RescheduleAppointmentRequest(DateOnly Date, TimeOnly Time, TimeOnly EndTime);
}