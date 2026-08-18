namespace Presentation.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public DateOnly? Date { get; set; }
        public string? Time { get; set; }
        public string? EndTime { get; set; }
        public string? Reason { get; set; }
        public string? Status { get; set; } = "pendiente"; // "pendiente" | "confirmado" | "cancelado"
        public string? Area { get; set; }
        public string? Location { get; set; }
        public string? Notes { get; set; }
        public int ClientId { get; set; }
        public int LawyerId { get; set; }
        public int? CaseId { get; set; }
    }
}