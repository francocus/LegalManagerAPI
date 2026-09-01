using LegalManager.Application.DTOs;
using LegalManager.Domain.Entities;

namespace LegalManager.Application.Interfaces
{
    public interface IAppointmentService
    {
        Appointment Create(CreateAppointmentRequest request);

        IReadOnlyList<Appointment> GetAll();

        IReadOnlyList<TimeOnly> GetAvailability(int lawyerId, DateOnly date);

        Appointment? GetById(int id);

        Appointment? Confirm(int id);

        Appointment? Cancel(int id);

        Appointment? Reschedule(int id, RescheduleAppointmentRequest request);

        bool Delete(int id);
    }
}