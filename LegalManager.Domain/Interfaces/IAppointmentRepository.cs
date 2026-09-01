using LegalManager.Domain.Entities;

namespace LegalManager.Domain.Interfaces
{
    public interface IAppointmentRepository
    {
        void Add(Appointment appointment);

        IReadOnlyList<Appointment> GetAll();

        Appointment? GetById(int id);

        bool HasScheduleConflict(int lawyerId, DateOnly date, TimeOnly time, TimeOnly endTime);
    }
}