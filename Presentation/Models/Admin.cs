namespace Presentation.Models
{
    public class Admin : User
    {
        public Admin(string name, string dni, string email, string password)
            : base(name, dni, email, password)
        {
        }
    }
}