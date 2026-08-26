namespace Presentation.DTOs
{
    public record AvailabilityResponse(IReadOnlyList<TimeOnly> Slots);
}