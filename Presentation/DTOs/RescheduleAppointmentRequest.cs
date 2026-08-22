namespace Presentation.DTOs
{
    public record RescheduleAppointmentRequest(DateOnly Date, string Time, string EndTime);
}