using Presentation.Models;

namespace Presentation.Data
{
    public static class AppointmentsRepository
    {
        private static readonly List<Appointment> appointments = new List<Appointment>();

        public static void Add(Appointment appointment) => appointments.Add(appointment);
        public static IReadOnlyList<Appointment> GetAll() => appointments.AsReadOnly();
        public static Appointment? GetById(int id) => appointments.FirstOrDefault(a => a.Id == id);

        public static bool HasScheduleConflict(int lawyerId, DateOnly date, string time, string endTime)
            => appointments.Any(a => a.LawyerId == lawyerId && a.OverlapsWith(date, time, endTime));
    }
}