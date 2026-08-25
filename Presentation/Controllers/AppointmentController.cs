using Presentation.Data;
using Presentation.DTOs;
using Presentation.Models;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {

        [HttpPost]
        public ActionResult<Appointment> Create([FromBody] CreateAppointmentRequest request)
        {
            var client = UsersRepository.GetById(request.ClientId);
            if (client is not Client)
                return BadRequest("El cliente indicado no es válido.");

            var lawyer = UsersRepository.GetById(request.LawyerId);
            if (lawyer is not Lawyer)
                return BadRequest("El abogado indicado no es válido.");

            if (request.CaseId.HasValue && CasesRepository.GetById(request.CaseId.Value) == null)
                return BadRequest("El expediente indicado no existe.");

            if (AppointmentsRepository.HasScheduleConflict(request.LawyerId, request.Date, request.Time, request.EndTime))
                return Conflict("The lawyer already has an appointment at that time.");

            try
            {
                var appointment = new Appointment(request.Title, request.Date, request.Time, request.EndTime, request.Reason, request.Area, request.Location, request.Notes, request.ClientId, request.LawyerId, request.CaseId);
                AppointmentsRepository.Add(appointment);
                return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<Appointment>> GetAll()
        {
            var appointments = AppointmentsRepository.GetAll();
            if (!appointments.Any()) return NotFound("No elements within the list");
            return Ok(appointments);
        }

        [HttpGet("availability")]
        public ActionResult<AvailabilityResponse> GetAvailability([FromQuery] int lawyerId, [FromQuery] DateOnly date)
        {
            if (UsersRepository.GetById(lawyerId) is not Lawyer)
                return BadRequest("El abogado indicado no es válido.");

            var freeSlots = Appointment.ValidSlots
                .Where(slot => !AppointmentsRepository.HasScheduleConflict(lawyerId, date, slot, slot))
                .ToList();

            return Ok(new AvailabilityResponse(freeSlots));
        }

        [HttpGet("{id}")]
        public ActionResult<Appointment> GetById([FromRoute] int id)
        {
            var appointment = AppointmentsRepository.GetById(id);
            if (appointment == null) return NotFound($"There is no element that match with the id {id}");
            return Ok(appointment);
        }

        [HttpPatch("{id}/confirm")]
        public ActionResult<Appointment> Confirm([FromRoute] int id)
        {
            var appointment = AppointmentsRepository.GetById(id);
            if (appointment == null) return NotFound($"There is no element that match with the id {id}");

            try
            {
                appointment.Confirm();
                return Ok(appointment);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPatch("{id}/cancel")]
        public ActionResult<Appointment> Cancel([FromRoute] int id)
        {
            var appointment = AppointmentsRepository.GetById(id);
            if (appointment == null) return NotFound($"There is no element that match with the id {id}");

            try
            {
                appointment.Cancel();
                return Ok(appointment);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPatch("{id}/reschedule")]
        public ActionResult<Appointment> Reschedule([FromRoute] int id, [FromBody] RescheduleAppointmentRequest request)
        {
            var appointment = AppointmentsRepository.GetById(id);
            if (appointment == null) return NotFound($"There is no element that match with the id {id}");

            if (AppointmentsRepository.HasScheduleConflict(appointment.LawyerId, request.Date, request.Time, request.EndTime))
            {
                return Conflict("The lawyer already has an appointment at that time.");
            }

            try
            {
                appointment.Reschedule(request.Date, request.Time, request.EndTime);
                return Ok(appointment);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            var appointment = AppointmentsRepository.GetById(id);
            if (appointment == null) return NotFound($"There is no element that match with the id {id}");

            try
            {
                appointment.Deactivate();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}