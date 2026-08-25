namespace Presentation.Models
{
    public class Lawyer : User
    {
        public string BarNumber { get; private set; }
        public IReadOnlyList<string> Specialties { get; private set; }

        public Lawyer(string name, string dni, string email, string password, string barNumber, IEnumerable<string>? specialties = null)
            : base(name, dni, email, password)
        {
            if (string.IsNullOrWhiteSpace(barNumber))
                throw new ArgumentException("Bar number is required.", nameof(barNumber));

            BarNumber = barNumber;
            Specialties = (specialties ?? Enumerable.Empty<string>()).ToList().AsReadOnly();
        }

        public void UpdateBarNumber(string barNumber)
        {
            if (string.IsNullOrWhiteSpace(barNumber))
                throw new ArgumentException("Bar number is required.", nameof(barNumber));

            BarNumber = barNumber;
        }

        public void UpdateSpecialties(IEnumerable<string> specialties)
        {
            var cleaned = specialties?.Where(s => !string.IsNullOrWhiteSpace(s)).ToList() ?? new List<string>();

            if (cleaned.Count == 0)
                throw new ArgumentException("At least one specialty is required.", nameof(specialties));

            Specialties = cleaned.AsReadOnly();
        }
    }
}