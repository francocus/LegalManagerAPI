namespace LegalManager.Application.DTOs
{
    public record AvailabilityResponse(IReadOnlyList<TimeOnly> Slots);
}