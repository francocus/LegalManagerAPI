namespace Presentation.Models
{
    public class Client : User
    {
        public string? Phone { get; private set; }

        public Client(string name, string dni, string email, string password, string? phone)
            : base(name, dni, email, password)
        {
            Phone = phone;
        }
    }
}