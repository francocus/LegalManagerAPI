using System.Text.Json.Serialization;

namespace Presentation.Models
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
    [JsonDerivedType(typeof(Client), typeDiscriminator: "client")]
    [JsonDerivedType(typeof(Lawyer), typeDiscriminator: "lawyer")]
    [JsonDerivedType(typeof(Sysadmin), typeDiscriminator: "sysadmin")]
    public abstract class User
    {
        private static int nextId = 1;

        public int Id { get; }
        public string Name { get; private set; }
        public string Dni { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public bool Active { get; private set; }

        protected User(string name, string dni, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required.", nameof(name));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));

            Id = nextId++;
            Name = name;
            Dni = dni;
            Email = email;
            Password = password;
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

        public void Deactivate()
        {
            if (!Active)
                throw new InvalidOperationException("The user is already inactive.");

            Active = false;
        }
    }
}