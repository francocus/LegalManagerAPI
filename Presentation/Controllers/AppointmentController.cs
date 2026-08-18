using Presentation.Models;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private static readonly List<Appointment> Appointments = new List<Appointment>();

        [HttpPost]
        public ActionResult Create([FromBody] Appointment appointment)
        {
            var objetoAppointment = new Appointment();

            objetoAppointment.Id = appointment.Id;
            objetoAppointment.Title = appointment.Title;
            objetoAppointment.Date = appointment.Date;
            objetoAppointment.Time = appointment.Time;
            objetoAppointment.EndTime = appointment.EndTime;
            objetoAppointment.Reason = appointment.Reason;
            objetoAppointment.Status = appointment.Status;
            objetoAppointment.Area = appointment.Area;
            objetoAppointment.Location = appointment.Location;
            objetoAppointment.Notes = appointment.Notes;
            objetoAppointment.ClientId = appointment.ClientId;
            objetoAppointment.LawyerId = appointment.LawyerId;
            objetoAppointment.CaseId = appointment.CaseId;

            Appointments.Add(objetoAppointment);

            return Created();
        }

        [HttpGet]
        public ActionResult<List<Appointment>> GetAll()
        {
            if (!Appointments.Any())
            {
                return NotFound("No elements within the list");
            }

            return Ok(Appointments);
        }

        [HttpGet("{id}")]
        public ActionResult<Appointment> GetById([FromRoute] int id)
        {
            var appointment = Appointments.FirstOrDefault(x => x.Id == id);

            if (appointment == null)
            {
                return NotFound($"There is no element that match with the id {id}");
            }

            return Ok(appointment);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            var appointment = Appointments.FirstOrDefault(x => x.Id == id);

            if (appointment == null)
            {
                return NotFound($"There is no element that match with the id {id}");
            }

            if (!Appointments.Remove(appointment))
            {
                return Conflict($"Problem to delete the item {id}");
            }

            return NoContent();
        }

        [HttpPatch("{id}")]
        public ActionResult<Appointment> PartialUpdate([FromRoute] int id, [FromBody] Appointment appointment)
        {
            var appointmentFound = Appointments.FirstOrDefault(x => x.Id == id);

            if (appointmentFound == null)
            {
                return NotFound($"There is no element that match with the id {id}");
            }

            appointmentFound.Status = appointment.Status ?? appointmentFound.Status;
            appointmentFound.Notes = appointment.Notes ?? appointmentFound.Notes;
            appointmentFound.Location = appointment.Location ?? appointmentFound.Location;
            appointmentFound.EndTime = appointment.EndTime ?? appointmentFound.EndTime;

            return Ok(appointmentFound);
        }

        [HttpPut("{id}")]
        public ActionResult<Appointment> Update([FromRoute] int id, [FromBody] Appointment appointment)
        {
            var appointmentFound = Appointments.FirstOrDefault(x => x.Id == id);

            if (appointmentFound == null)
            {
                return NotFound($"There is no element that match with the id {id}");
            }

            appointmentFound.Title = appointment.Title;
            appointmentFound.Date = appointment.Date;
            appointmentFound.Time = appointment.Time;
            appointmentFound.EndTime = appointment.EndTime;
            appointmentFound.Reason = appointment.Reason;
            appointmentFound.Status = appointment.Status;
            appointmentFound.Area = appointment.Area;
            appointmentFound.Location = appointment.Location;
            appointmentFound.Notes = appointment.Notes;
            appointmentFound.ClientId = appointment.ClientId;
            appointmentFound.LawyerId = appointment.LawyerId;
            appointmentFound.CaseId = appointment.CaseId;

            return Ok(appointmentFound);
        }
    }
}