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
        public ActionResult<Case> ChangeStatus([FromRoute] int id, [FromBody] ChangeStatusRequest request)
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

        [HttpPatch("{id}/lawyers/add")]
        public ActionResult<Case> AddLawyer([FromRoute] int id, [FromBody] AddLawyerRequest request)
        {
            var caseItem = CasesRepository.GetById(id);
            if (caseItem == null) return NotFound($"There is no element that match with the id {id}");

            var lawyer = UsersRepository.GetById(request.LawyerId);
            if (lawyer is not Lawyer)
                return BadRequest("El abogado indicado no es válido.");

            try
            {
                caseItem.AddLawyer(request.LawyerId);
                return Ok(caseItem);
            }
            catch (InvalidOperationException ex) { return Conflict(ex.Message); }
        }

        [HttpPatch("{id}/lawyers/remove")]
        public ActionResult<Case> RemoveLawyer([FromRoute] int id, [FromBody] RemoveLawyerRequest request)
        {
            var caseItem = CasesRepository.GetById(id);
            if (caseItem == null) return NotFound($"There is no element that match with the id {id}");

            try
            {
                caseItem.RemoveLawyer(request.LawyerId);
                return Ok(caseItem);
            }
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