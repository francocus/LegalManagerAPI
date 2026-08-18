namespace Presentation.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Dni { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; } = string.Empty; // "cliente" | "abogado" | "sysadmin"
        public bool Active { get; set; } = true;
    }
}