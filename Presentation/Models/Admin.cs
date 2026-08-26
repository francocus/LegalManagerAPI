namespace Presentation.Models
{
    public class Admin : User
    {
        public Admin(string firstName, string lastName, string dni, string email, string password)
            : base(firstName, lastName, dni, email, password)
        {
        }
    }
}