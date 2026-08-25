namespace Presentation.Models
{
    public class Sysadmin : User
    {
        public Sysadmin(string name, string dni, string email, string password)
            : base(name, dni, email, password)
        {
        }
    }
}