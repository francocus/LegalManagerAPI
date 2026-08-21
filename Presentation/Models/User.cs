namespace Presentation.Models
{
    public class User
    {
        private static int nextId = 1;
        private static readonly string[] ValidRoles = { "cliente", "abogado", "sysadmin" };

        public int Id { get; }
        public string Name { get; private set; }
        public string Dni { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public string Role { get; private set; }
        public bool Active { get; private set; }

        public User(string name, string dni, string email, string password, string role)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required.", nameof(name));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));

            if (!ValidRoles.Contains(role))
                throw new ArgumentException($"Invalid role. Allowed values: {string.Join(", ", ValidRoles)}.", nameof(role));

            Id = nextId++;
            Name = name;
            Dni = dni;
            Email = email;
            Password = password;
            Role = role;
            Active = true;
        }

        public void UpdateDetails(string name, string dni, string email)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required.", nameof(name));

            Name = name;
            Dni = dni;
            Email = email;
        }

        public void AssignRole(string role)
        {
            if (!ValidRoles.Contains(role))
                throw new ArgumentException($"Invalid role. Allowed values: {string.Join(", ", ValidRoles)}.", nameof(role));

            Role = role;
        }

        public void Deactivate()
        {
            if (!Active)
                throw new InvalidOperationException("The user is already inactive.");

            Active = false;
        }
    }
}