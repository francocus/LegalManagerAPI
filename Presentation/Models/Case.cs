namespace Presentation.Models
{
    public class Case
    {
        private static int nextId = 1;
        private static readonly string[] ValidStatuses = { "activo", "pendiente", "cerrado" };

        public int Id { get; }
        public string CaseNumber { get; }
        public string Title { get; private set; }
        public string Area { get; private set; }
        public string Status { get; private set; }
        public DateOnly StartDate { get; }
        public DateOnly LastUpdate { get; private set; }
        public DateOnly? ClosingDate { get; private set; }
        public string? Description { get; private set; }
        public string? Notes { get; private set; }
        public int ClientId { get; }
        private readonly List<int> lawyerIds;
        public IReadOnlyList<int> LawyerIds => lawyerIds.AsReadOnly();
        public bool Active { get; private set; }

        public Case(string caseNumber, string title, string area, DateOnly startDate, string? description, string? notes, int clientId, int lawyerId)
        {
            if (string.IsNullOrWhiteSpace(caseNumber))
                throw new ArgumentException("Case number is required.", nameof(caseNumber));

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required.", nameof(title));

            if (clientId <= 0)
                throw new ArgumentException("The case needs a client assigned.", nameof(clientId));

            if (lawyerId <= 0)
                throw new ArgumentException("The case needs a lawyer assigned.", nameof(lawyerId));

            Id = nextId++;
            CaseNumber = caseNumber;
            Title = title;
            Area = area;
            Status = "activo";
            StartDate = startDate;
            LastUpdate = startDate;
            Description = description;
            Notes = notes;
            ClientId = clientId;
            lawyerIds = new List<int> { lawyerId };
            Active = true;
        }

        public void UpdateDetails(string title, string area, string? description, string? notes)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required.", nameof(title));

            if (Status == "cerrado")
                throw new InvalidOperationException("Cannot modify a closed case.");

            Title = title;
            Area = area;
            Description = description;
            Notes = notes;
            LastUpdate = DateOnly.FromDateTime(DateTime.Now);
        }

        public void ChangeStatus(string newStatus)
        {
            if (!ValidStatuses.Contains(newStatus))
                throw new ArgumentException($"Invalid status. Allowed values: {string.Join(", ", ValidStatuses)}.", nameof(newStatus));

            if (Status == "cerrado")
                throw new InvalidOperationException("Cannot reopen a closed case.");

            Status = newStatus;
            LastUpdate = DateOnly.FromDateTime(DateTime.Now);

            if (newStatus == "cerrado")
                ClosingDate = LastUpdate;
        }

        public void AddLawyer(int lawyerId)
        {
            if (Status == "cerrado")
                throw new InvalidOperationException("Cannot modify lawyers on a closed case.");

            if (lawyerIds.Contains(lawyerId))
                throw new InvalidOperationException("This lawyer is already assigned to the case.");

            lawyerIds.Add(lawyerId);
        }

        public void RemoveLawyer(int lawyerId)
        {
            if (Status == "cerrado")
                throw new InvalidOperationException("Cannot modify lawyers on a closed case.");

            if (lawyerIds.Count == 1)
                throw new InvalidOperationException("A case must have at least one lawyer assigned.");

            if (!lawyerIds.Remove(lawyerId))
                throw new InvalidOperationException("This lawyer is not assigned to the case.");
        }

        public void Deactivate()
        {
            if (!Active)
                throw new InvalidOperationException("The case is already inactive.");

            Active = false;
        }
    }
}