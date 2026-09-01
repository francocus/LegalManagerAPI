using LegalManager.Domain.Entities;
using LegalManager.Domain.Interfaces;

namespace LegalManager.Infrastructure.Repositories
{
    public class AppointmentsRepository : IAppointmentRepository
    {
        private readonly List<Appointment> appointments = new List<Appointment>();

        public void Add(Appointment appointment) => appointments.Add(appointment);

        public IReadOnlyList<Appointment> GetAll()
            => appointments.Where(a => a.Active).ToList().AsReadOnly();

        public Appointment? GetById(int id)
            => appointments.FirstOrDefault(a => a.Id == id && a.Active);

        public bool HasScheduleConflict(int lawyerId, DateOnly date, TimeOnly time, TimeOnly endTime)
            => appointments.Any(a => a.LawyerId == lawyerId && a.OverlapsWith(date, time, endTime));
    }
}