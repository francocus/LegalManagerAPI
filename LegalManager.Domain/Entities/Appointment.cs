namespace LegalManager.Domain.Entities
{
    public class Appointment
    {
        private static int nextId = 1;
        public static readonly TimeOnly[] ValidSlots =
        {
            new(9, 0), new(10, 30), new(12, 0), new(14, 0), new(15, 30), new(17, 0)
        };

        public int Id { get; }
        public string Title { get; }
        public DateOnly Date { get; private set; }
        public TimeOnly Time { get; private set; }
        public TimeOnly EndTime { get; private set; }
        public string Reason { get; }
        public AppointmentStatus Status { get; private set; }
        public AppointmentStatus EffectiveStatus =>
            Status == AppointmentStatus.Confirmado && Date.ToDateTime(Time) < DateTime.Now
                ? AppointmentStatus.Finalizado
                : Status;
        public string? Area { get; }
        public string? Location { get; private set; }
        public string? Notes { get; private set; }
        public int ClientId { get; }
        public int LawyerId { get; }
        public int? CaseId { get; }
        public bool Active { get; private set; }

        public Appointment(string title, DateOnly date, TimeOnly time, TimeOnly endTime, string reason, string? area, string? location, string? notes, int clientId, int lawyerId, int? caseId)
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
            Status = AppointmentStatus.Pendiente;
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
            if (Status == AppointmentStatus.Cancelado)
                throw new InvalidOperationException("No se puede confirmar un turno cancelado.");

            if (Status == AppointmentStatus.Confirmado)
                throw new InvalidOperationException("El turno ya está confirmado.");

            Status = AppointmentStatus.Confirmado;
        }

        public void Cancel()
        {
            if (Status == AppointmentStatus.Cancelado)
                throw new InvalidOperationException("El turno ya está cancelado.");

            Status = AppointmentStatus.Cancelado;
        }

        public void Reschedule(DateOnly newDate, TimeOnly newTime, TimeOnly newEndTime)
        {
            if (Status == AppointmentStatus.Cancelado)
                throw new InvalidOperationException("No se puede reprogramar un turno cancelado.");

            if (!ValidSlots.Contains(newTime))
                throw new ArgumentException($"Horario inválido. Valores permitidos: {string.Join(", ", ValidSlots)}.", nameof(newTime));

            Date = newDate;
            Time = newTime;
            EndTime = newEndTime;
            Status = AppointmentStatus.Pendiente;
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

        public bool OverlapsWith(DateOnly date, TimeOnly time, TimeOnly endTime)
            => Active && Status != AppointmentStatus.Cancelado && Date == date && Time == time;
    }
}