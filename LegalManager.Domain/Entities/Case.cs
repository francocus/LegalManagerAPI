namespace LegalManager.Domain.Entities
{
    public class Case
    {
        private static int nextId = 1;

        public int Id { get; }
        public string CaseNumber { get; }
        public string Title { get; private set; }
        public string Area { get; private set; }
        public CaseStatus Status { get; private set; }
        public DateOnly StartDate { get; }
        public DateOnly LastUpdate { get; private set; }
        public DateOnly? ClosingDate { get; private set; }
        public string Description { get; private set; }
        public string? Notes { get; private set; }
        public int ClientId { get; }
        public int CreatedByUserId { get; }
        private readonly List<int> lawyerIds;
        public IReadOnlyList<int> LawyerIds => lawyerIds.AsReadOnly();
        public bool Active { get; private set; }

        public Case(string caseNumber, string title, string area, DateOnly startDate, string description, string? notes, int clientId, int lawyerId, int createdByUserId)
        {
            if (string.IsNullOrWhiteSpace(caseNumber))
                throw new ArgumentException("El número de expediente es obligatorio.", nameof(caseNumber));

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("El título es obligatorio.", nameof(title));

            if (string.IsNullOrWhiteSpace(area))
                throw new ArgumentException("El área es obligatoria.", nameof(area));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("La descripción es obligatoria.", nameof(description));

            if (clientId <= 0)
                throw new ArgumentException("El expediente necesita un cliente asignado.", nameof(clientId));

            if (lawyerId <= 0)
                throw new ArgumentException("El expediente necesita un abogado asignado.", nameof(lawyerId));

            if (createdByUserId <= 0)
                throw new ArgumentException("El expediente necesita un usuario que lo cree.", nameof(createdByUserId));

            Id = nextId++;
            CaseNumber = caseNumber;
            Title = title;
            Area = area;
            Status = CaseStatus.Activo;
            StartDate = startDate;
            LastUpdate = startDate;
            Description = description;
            Notes = notes;
            ClientId = clientId;
            CreatedByUserId = createdByUserId;
            lawyerIds = new List<int> { lawyerId };
            Active = true;
        }

        public void UpdateDetails(string title, string area, string description, string? notes)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("El título es obligatorio.", nameof(title));

            if (string.IsNullOrWhiteSpace(area))
                throw new ArgumentException("El área es obligatoria.", nameof(area));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("La descripción es obligatoria.", nameof(description));

            if (Status == CaseStatus.Cerrado)
                throw new InvalidOperationException("No se puede modificar un expediente cerrado.");

            Title = title;
            Area = area;
            Description = description;
            Notes = notes;
            LastUpdate = DateOnly.FromDateTime(DateTime.Now);
        }

        public void ChangeStatus(CaseStatus newStatus)
        {
            if (Status == CaseStatus.Cerrado)
                throw new InvalidOperationException("No se puede reabrir un expediente cerrado.");

            Status = newStatus;
            LastUpdate = DateOnly.FromDateTime(DateTime.Now);

            if (newStatus == CaseStatus.Cerrado)
                ClosingDate = LastUpdate;
        }

        public void AddLawyer(int lawyerId)
        {
            if (Status == CaseStatus.Cerrado)
                throw new InvalidOperationException("No se pueden modificar los abogados de un expediente cerrado.");

            if (lawyerIds.Contains(lawyerId))
                throw new InvalidOperationException("Este abogado ya está asignado al expediente.");

            lawyerIds.Add(lawyerId);
        }

        public void RemoveLawyer(int lawyerId)
        {
            if (Status == CaseStatus.Cerrado)
                throw new InvalidOperationException("No se pueden modificar los abogados de un expediente cerrado.");

            if (lawyerIds.Count == 1)
                throw new InvalidOperationException("El expediente debe tener al menos un abogado asignado.");

            if (!lawyerIds.Remove(lawyerId))
                throw new InvalidOperationException("Este abogado no está asignado al expediente.");
        }

        public void Deactivate()
        {
            if (!Active)
                throw new InvalidOperationException("El expediente ya está inactivo.");

            Active = false;
        }
    }
}