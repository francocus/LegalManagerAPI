using System.Text.Json.Serialization;

namespace Presentation.Models
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
    [JsonDerivedType(typeof(Client), typeDiscriminator: UserType.Client)]
    [JsonDerivedType(typeof(Lawyer), typeDiscriminator: UserType.Lawyer)]
    [JsonDerivedType(typeof(Admin), typeDiscriminator: UserType.Admin)]
    public abstract class User
    {
        private static int nextId = 1;

        public int Id { get; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Dni { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public DateOnly RegistrationDate { get; }
        public bool Active { get; private set; }

        protected User(string firstName, string lastName, string dni, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("El nombre es obligatorio.", nameof(firstName));

            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("El apellido es obligatorio.", nameof(lastName));

            if (string.IsNullOrWhiteSpace(dni))
                throw new ArgumentException("El DNI es obligatorio.", nameof(dni));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("El email es obligatorio.", nameof(email));

            Id = nextId++;
            FirstName = firstName;
            LastName = lastName;
            Dni = dni;
            Email = email;
            Password = password;
            RegistrationDate = DateOnly.FromDateTime(DateTime.Now);
            Active = true;
        }

        public void UpdateDetails(string firstName, string lastName, string dni, string email)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("El nombre es obligatorio.", nameof(firstName));

            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("El apellido es obligatorio.", nameof(lastName));

            if (string.IsNullOrWhiteSpace(dni))
                throw new ArgumentException("El DNI es obligatorio.", nameof(dni));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("El email es obligatorio.", nameof(email));

            FirstName = firstName;
            LastName = lastName;
            Dni = dni;
            Email = email;
        }

        public void Deactivate()
        {
            if (!Active)
                throw new InvalidOperationException("El usuario ya está inactivo.");

            Active = false;
        }
    }
}