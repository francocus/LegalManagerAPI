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
        [HttpPost]
        public ActionResult<Case> Create([FromBody] CreateCaseRequest request)
        {
            if (CasesRepository.GetAll().Any(c => c.CaseNumber == request.CaseNumber))
                return Conflict("Ya existe un expediente con ese número.");

            var client = UserRepository.GetById(request.ClientId);
            if (client == null || client.Role != "cliente")
                return BadRequest("El cliente indicado no es válido.");

            var lawyer = UserRepository.GetById(request.LawyerId);
            if (lawyer == null || lawyer.Role != "abogado")
                return BadRequest("El abogado indicado no es válido.");

            try
            {
                var caseItem = new Case(request.CaseNumber, request.Title, request.Area, request.StartDate, request.Description, request.Notes, request.ClientId, request.LawyerId);
                CasesRepository.Add(caseItem);
                return CreatedAtAction(nameof(GetById), new { id = caseItem.Id }, caseItem);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<Case>> GetAll()
        {
            var cases = CasesRepository.GetAll();
            if (!cases.Any()) return NotFound("No elements within the list");
            return Ok(cases);
        }

        [HttpGet("{id}")]
        public ActionResult<Case> GetById([FromRoute] int id)
        {
            var caseItem = CasesRepository.GetById(id);
            if (caseItem == null) return NotFound($"There is no element that match with the id {id}");
            return Ok(caseItem);
        }

        [HttpPut("{id}")]
        public ActionResult<Case> Update([FromRoute] int id, [FromBody] UpdateCaseRequest request)
        {
            var caseItem = CasesRepository.GetById(id);
            if (caseItem == null) return NotFound($"There is no element that match with the id {id}");

            try
            {
                caseItem.UpdateDetails(request.Title, request.Area, request.Description, request.Notes);
                return Ok(caseItem);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPatch("{id}/status")]
        public ActionResult<Case> ChangeStatus([FromRoute] int id, [FromBody] ChangeCaseStatusRequest request)
        {
            var caseItem = CasesRepository.GetById(id);
            if (caseItem == null) return NotFound($"There is no element that match with the id {id}");

            try
            {
                caseItem.ChangeStatus(request.Status);
                return Ok(caseItem);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPatch("{id}/lawyer")]
        public ActionResult<Case> ReassignLawyer([FromRoute] int id, [FromBody] ReassignLawyerRequest request)
        {
            var caseItem = CasesRepository.GetById(id);
            if (caseItem == null) return NotFound($"There is no element that match with the id {id}");

            var lawyer = UserRepository.GetById(request.LawyerId);
            if (lawyer == null || lawyer.Role != "abogado")
                return BadRequest("El abogado indicado no es válido.");

            try
            {
                caseItem.ReassignLawyer(request.LawyerId);
                return Ok(caseItem);
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (InvalidOperationException ex) { return Conflict(ex.Message); }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            var caseItem = CasesRepository.GetById(id);
            if (caseItem == null) return NotFound($"There is no element that match with the id {id}");

            var hasActiveAppointments = AppointmentsRepository.GetAll()
                .Any(a => a.CaseId == id
                    && a.EffectiveStatus != "cancelado" && a.EffectiveStatus != "finalizado");

            if (hasActiveAppointments)
                return Conflict($"The case with id {id} has active appointments linked to it.");

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