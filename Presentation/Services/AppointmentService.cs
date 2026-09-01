using Presentation.Data;
using Presentation.DTOs;
using Presentation.Models;

namespace Presentation.Services
{
    public class AppointmentService : IAppointmentService
    {
        public Appointment Create(CreateAppointmentRequest request)
        {
            if (UsersRepository.GetById(request.ClientId) is not Client)
                throw new ArgumentException("El cliente indicado no es válido.");

            if (UsersRepository.GetById(request.LawyerId) is not Lawyer)
                throw new ArgumentException("El abogado indicado no es válido.");

            Case? relatedCase = null;
            if (request.CaseId.HasValue)
            {
                relatedCase = CasesRepository.GetById(request.CaseId.Value);
                if (relatedCase == null)
                    throw new ArgumentException("El expediente indicado no existe.");
            }

            if (AppointmentsRepository.HasScheduleConflict(request.LawyerId, request.Date, request.Time, request.EndTime))
                throw new InvalidOperationException("El abogado ya tiene un turno en ese horario.");

            var area = relatedCase != null ? relatedCase.Area : request.Area;
            var appointment = new Appointment(request.Title, request.Date, request.Time, request.EndTime, request.Reason, area, request.Location, request.Notes, request.ClientId, request.LawyerId, request.CaseId);
            AppointmentsRepository.Add(appointment);
            return appointment;
        }

        public IReadOnlyList<Appointment> GetAll() => AppointmentsRepository.GetAll();

        public IReadOnlyList<TimeOnly> GetAvailability(int lawyerId, DateOnly date)
        {
            if (UsersRepository.GetById(lawyerId) is not Lawyer)
                throw new ArgumentException("El abogado indicado no es válido.");

            return Appointment.ValidSlots
                .Where(slot => !AppointmentsRepository.HasScheduleConflict(lawyerId, date, slot, slot))
                .ToList();
        }

        public Appointment? GetById(int id) => AppointmentsRepository.GetById(id);

        public Appointment? Confirm(int id)
        {
            var appointment = GetById(id);
            if (appointment == null) return null;

            appointment.Confirm();
            return appointment;
        }

        public Appointment? Cancel(int id)
        {
            var appointment = GetById(id);
            if (appointment == null) return null;

            appointment.Cancel();
            return appointment;
        }

        public Appointment? Reschedule(int id, RescheduleAppointmentRequest request)
        {
            var appointment = GetById(id);
            if (appointment == null) return null;

            if (AppointmentsRepository.HasScheduleConflict(appointment.LawyerId, request.Date, request.Time, request.EndTime))
                throw new InvalidOperationException("El abogado ya tiene un turno en ese horario.");

            appointment.Reschedule(request.Date, request.Time, request.EndTime);
            return appointment;
        }

        public bool Delete(int id)
        {
            var appointment = GetById(id);
            if (appointment == null) return false;

            appointment.Deactivate();
            return true;
        }
    }
}