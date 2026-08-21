namespace Presentation.Models
{
    public class Appointment
    {
        private static int nextId = 1;

        public int Id { get; }
        public string Title { get; }
        public DateOnly Date { get; private set; }
        public string Time { get; private set; }
        public string EndTime { get; private set; }
        public string Reason { get; }
        public string Status { get; private set; }
        public string? Area { get; }
        public string? Location { get; private set; }
        public string? Notes { get; private set; }
        public int ClientId { get; }
        public int LawyerId { get; }
        public int? CaseId { get; }
        public bool Active { get; private set; }

        public Appointment(string title, DateOnly date, string time, string endTime, string reason, string? area, string? location, string? notes, int clientId, int lawyerId, int? caseId)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required.", nameof(title));

            if (clientId <= 0)
                throw new ArgumentException("The appointment needs a client assigned.", nameof(clientId));

            if (lawyerId <= 0)
                throw new ArgumentException("The appointment needs a lawyer assigned.", nameof(lawyerId));

            Id = nextId++;
            Title = title;
            Date = date;
            Time = time;
            EndTime = endTime;
            Reason = reason;
            Status = "pendiente";
            Area = area;
            Location = location;
            Notes = notes;
            ClientId = clientId;
            LawyerId = lawyerId;
            CaseId = caseId;
            Active = true;
        }

        public void Confirm()
        {
            if (Status == "cancelado")
                throw new InvalidOperationException("Cannot confirm a cancelled appointment.");

            if (Status == "confirmado")
                throw new InvalidOperationException("The appointment is already confirmed.");

            Status = "confirmado";
        }

        public void Cancel()
        {
            if (Status == "cancelado")
                throw new InvalidOperationException("The appointment is already cancelled.");

            Status = "cancelado";
        }

        public void Reschedule(DateOnly newDate, string newTime, string newEndTime)
        {
            if (Status == "cancelado")
                throw new InvalidOperationException("Cannot reschedule a cancelled appointment.");

            Date = newDate;
            Time = newTime;
            EndTime = newEndTime;
            Status = "pendiente"; // back to pending, requires reconfirmation
        }

        public void UpdateNotes(string? location, string? notes)
        {
            Location = location;
            Notes = notes;
        }

        public void Deactivate()
        {
            if (!Active)
                throw new InvalidOperationException("The appointment is already inactive.");

            Active = false;
        }

        public bool OverlapsWith(DateOnly date, string time, string endTime)
            => Active && Status != "cancelado" && Date == date && Time == time;
    }
}