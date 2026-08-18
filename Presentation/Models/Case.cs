namespace Presentation.Models
{
    public class Case
    {
        public int Id { get; set; }
        public string? CaseNumber { get; set; }
        public string? Title { get; set; }
        public string? Area { get; set; }
        public string? Status { get; set; } = "activo"; // "activo" | "pendiente" | "cerrado"
        public DateOnly? StartDate { get; set; }
        public DateOnly? LastUpdate { get; set; }
        public string? Description { get; set; }
        public string? Notes { get; set; }
        public int ClientId { get; set; }
        public int LawyerId { get; set; }
        public bool Active { get; set; } = true;
    }
}