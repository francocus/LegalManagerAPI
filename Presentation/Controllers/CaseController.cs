using Presentation.Data;
using Presentation.DTOs;
using Presentation.Models;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaseController : ControllerBase
    {
        private static CaseResponse ToResponse(Case c) => new(
            c.Id, c.CaseNumber, c.Title, c.Area, c.Status,
            c.StartDate, c.LastUpdate, c.ClosingDate,
            c.Description, c.Notes, c.ClientId, c.LawyerIds, c.Active);

        [HttpPost]
        public ActionResult<CaseResponse> Create([FromBody] CreateCaseRequest request)
        {
            if (CasesRepository.GetAll().Any(c => c.CaseNumber == request.CaseNumber))
                return Conflict("Ya existe un expediente con ese número.");

            var client = UsersRepository.GetById(request.ClientId);
            if (client is not Client)
                return BadRequest("El cliente indicado no es válido.");

            var lawyer = UsersRepository.GetById(request.LawyerId);
            if (lawyer is not Lawyer)
                return BadRequest("El abogado indicado no es válido.");

            try
            {
                var caseItem = new Case(request.CaseNumber, request.Title, request.Area, request.StartDate, request.Description, request.Notes, request.ClientId, request.LawyerId);
                CasesRepository.Add(caseItem);
                return CreatedAtAction(nameof(GetById), new { id = caseItem.Id }, ToResponse(caseItem));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<CaseResponse>> GetAll()
        {
            var cases = CasesRepository.GetAll();
            if (!cases.Any()) return NotFound("No hay elementos en la lista.");
            return Ok(cases.Select(ToResponse).ToList());
        }

        [HttpGet("{id}")]
        public ActionResult<CaseResponse> GetById([FromRoute] int id)
        {
            var caseItem = CasesRepository.GetById(id);
            if (caseItem == null) return NotFound($"No existe un elemento con el id {id}.");
            return Ok(ToResponse(caseItem));
        }

        [HttpPut("{id}")]
        public ActionResult<CaseResponse> Update([FromRoute] int id, [FromBody] UpdateCaseRequest request)
        {
            var caseItem = CasesRepository.GetById(id);
            if (caseItem == null) return NotFound($"No existe un elemento con el id {id}.");

            try
            {
                caseItem.UpdateDetails(request.Title, request.Area, request.Description, request.Notes);
                return Ok(ToResponse(caseItem));
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (InvalidOperationException ex) { return Conflict(ex.Message); }
        }

        [HttpPatch("{id}/status")]
        public ActionResult<CaseResponse> ChangeStatus([FromRoute] int id, [FromBody] ChangeStatusRequest request)
        {
            var caseItem = CasesRepository.GetById(id);
            if (caseItem == null) return NotFound($"No existe un elemento con el id {id}.");

            try
            {
                caseItem.ChangeStatus(request.Status);
                return Ok(ToResponse(caseItem));
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (InvalidOperationException ex) { return Conflict(ex.Message); }
        }

        [HttpPatch("{id}/lawyers/add")]
        public ActionResult<CaseResponse> AddLawyer([FromRoute] int id, [FromBody] AddLawyerRequest request)
        {
            var caseItem = CasesRepository.GetById(id);
            if (caseItem == null) return NotFound($"No existe un elemento con el id {id}.");

            var lawyer = UsersRepository.GetById(request.LawyerId);
            if (lawyer is not Lawyer)
                return BadRequest("El abogado indicado no es válido.");

            try
            {
                caseItem.AddLawyer(request.LawyerId);
                return Ok(ToResponse(caseItem));
            }
            catch (InvalidOperationException ex) { return Conflict(ex.Message); }
        }

        [HttpPatch("{id}/lawyers/remove")]
        public ActionResult<CaseResponse> RemoveLawyer([FromRoute] int id, [FromBody] RemoveLawyerRequest request)
        {
            var caseItem = CasesRepository.GetById(id);
            if (caseItem == null) return NotFound($"No existe un elemento con el id {id}.");

            try
            {
                caseItem.RemoveLawyer(request.LawyerId);
                return Ok(ToResponse(caseItem));
            }
            catch (InvalidOperationException ex) { return Conflict(ex.Message); }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            var caseItem = CasesRepository.GetById(id);
            if (caseItem == null) return NotFound($"No existe un elemento con el id {id}.");

            var hasActiveAppointments = AppointmentsRepository.GetAll()
                .Any(a => a.CaseId == id
                    && a.EffectiveStatus != "cancelado" && a.EffectiveStatus != "finalizado");

            if (hasActiveAppointments)
                return Conflict($"El expediente con id {id} tiene turnos activos asociados.");

            try
            {
                caseItem.Deactivate();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}