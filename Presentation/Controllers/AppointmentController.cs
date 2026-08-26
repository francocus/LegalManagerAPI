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
        private static AppointmentResponse ToResponse(Appointment a) => new(
            a.Id, a.Title, a.Date, a.Time, a.EndTime, a.Reason, a.Status, a.EffectiveStatus,
            a.Area, a.Location, a.Notes, a.ClientId, a.LawyerId, a.CaseId, a.Active);

        [HttpPost]
        public ActionResult<AppointmentResponse> Create([FromBody] CreateAppointmentRequest request)
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
                return Conflict("El abogado ya tiene un turno en ese horario.");

            try
            {
                var appointment = new Appointment(request.Title, request.Date, request.Time, request.EndTime, request.Reason, request.Area, request.Location, request.Notes, request.ClientId, request.LawyerId, request.CaseId);
                AppointmentsRepository.Add(appointment);
                return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, ToResponse(appointment));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<AppointmentResponse>> GetAll()
        {
            var appointments = AppointmentsRepository.GetAll();
            if (!appointments.Any()) return NotFound("No hay elementos en la lista.");
            return Ok(appointments.Select(ToResponse).ToList());
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
        public ActionResult<AppointmentResponse> GetById([FromRoute] int id)
        {
            var appointment = AppointmentsRepository.GetById(id);
            if (appointment == null) return NotFound($"No existe un elemento con el id {id}.");
            return Ok(ToResponse(appointment));
        }

        [HttpPatch("{id}/confirm")]
        public ActionResult<AppointmentResponse> Confirm([FromRoute] int id)
        {
            var appointment = AppointmentsRepository.GetById(id);
            if (appointment == null) return NotFound($"No existe un elemento con el id {id}.");

            try
            {
                appointment.Confirm();
                return Ok(ToResponse(appointment));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPatch("{id}/cancel")]
        public ActionResult<AppointmentResponse> Cancel([FromRoute] int id)
        {
            var appointment = AppointmentsRepository.GetById(id);
            if (appointment == null) return NotFound($"No existe un elemento con el id {id}.");

            try
            {
                appointment.Cancel();
                return Ok(ToResponse(appointment));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPatch("{id}/reschedule")]
        public ActionResult<AppointmentResponse> Reschedule([FromRoute] int id, [FromBody] RescheduleAppointmentRequest request)
        {
            var appointment = AppointmentsRepository.GetById(id);
            if (appointment == null) return NotFound($"No existe un elemento con el id {id}.");

            if (AppointmentsRepository.HasScheduleConflict(appointment.LawyerId, request.Date, request.Time, request.EndTime))
            {
                return Conflict("El abogado ya tiene un turno en ese horario.");
            }

            try
            {
                appointment.Reschedule(request.Date, request.Time, request.EndTime);
                return Ok(ToResponse(appointment));
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
            if (appointment == null) return NotFound($"No existe un elemento con el id {id}.");

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