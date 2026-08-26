namespace Presentation.Models
{
    public class Appointment
    {
        private static int nextId = 1;
        public static readonly string[] ValidSlots = { "09:00", "10:30", "12:00", "14:00", "15:30", "17:00" };

        public int Id { get; }
        public string Title { get; }
        public DateOnly Date { get; private set; }
        public string Time { get; private set; }
        public string EndTime { get; private set; }
        public string Reason { get; }
        public string Status { get; private set; }
        public string EffectiveStatus =>
            Status == "confirmado" && Date.ToDateTime(TimeOnly.Parse(Time)) < DateTime.Now
                ? "finalizado"
                : Status;
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
                throw new ArgumentException("El título es obligatorio.", nameof(title));

            if (clientId <= 0)
                throw new ArgumentException("El turno necesita un cliente asignado.", nameof(clientId));

            if (lawyerId <= 0)
                throw new ArgumentException("El turno necesita un abogado asignado.", nameof(lawyerId));

            if (!ValidSlots.Contains(time))
                throw new ArgumentException($"Horario inválido. Valores permitidos: {string.Join(", ", ValidSlots)}.", nameof(time));

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
                throw new InvalidOperationException("No se puede confirmar un turno cancelado.");

            if (Status == "confirmado")
                throw new InvalidOperationException("El turno ya está confirmado.");

            Status = "confirmado";
        }

        public void Cancel()
        {
            if (Status == "cancelado")
                throw new InvalidOperationException("El turno ya está cancelado.");

            Status = "cancelado";
        }

        public void Reschedule(DateOnly newDate, string newTime, string newEndTime)
        {
            if (Status == "cancelado")
                throw new InvalidOperationException("No se puede reprogramar un turno cancelado.");

            if (!ValidSlots.Contains(newTime))
                throw new ArgumentException($"Horario inválido. Valores permitidos: {string.Join(", ", ValidSlots)}.", nameof(newTime));

            Date = newDate;
            Time = newTime;
            EndTime = newEndTime;
            Status = "pendiente";
        }

        public void UpdateNotes(string? location, string? notes)
        {
            Location = location;
            Notes = notes;
        }

        public void Deactivate()
        {
            if (!Active)
                throw new InvalidOperationException("El turno ya está inactivo.");

            Active = false;
        }

        public bool OverlapsWith(DateOnly date, string time, string endTime)
            => Active && EffectiveStatus == "confirmado" && Date == date && Time == time;

    }
}