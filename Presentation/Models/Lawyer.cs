namespace Presentation.Models
{
    public class Lawyer : User
    {
        public string BarNumber { get; }

        public Lawyer(string name, string dni, string email, string password, string barNumber)
            : base(name, dni, email, password)
        {
            if (string.IsNullOrWhiteSpace(barNumber))
                throw new ArgumentException("Bar number is required.", nameof(barNumber));

            BarNumber = barNumber;
        }
    }
}