using Presentation.DTOs;
using Presentation.Models;
using Presentation.Services;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaseController : ControllerBase
    {
        private readonly ICaseService caseService;

        public CaseController(ICaseService caseService)
        {
            this.caseService = caseService;
        }

        private static CaseResponse ToResponse(Case c) => new(
            c.Id, c.CaseNumber, c.Title, c.Area, c.Status,
            c.StartDate, c.LastUpdate, c.ClosingDate,
            c.Description, c.Notes, c.ClientId, c.CreatedByUserId, c.LawyerIds, c.Active);

        [HttpPost]
        public ActionResult<CaseResponse> Create([FromBody] CreateCaseRequest request)
        {
            try
            {
                var caseItem = caseService.Create(request);
                return CreatedAtAction(nameof(GetById), new { id = caseItem.Id }, ToResponse(caseItem));
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (InvalidOperationException ex) { return Conflict(ex.Message); }
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<CaseResponse>> GetAll()
        {
            var cases = caseService.GetAll();
            if (!cases.Any()) return NotFound("No hay elementos en la lista.");
            return Ok(cases.Select(ToResponse).ToList());
        }

        [HttpGet("{id}")]
        public ActionResult<CaseResponse> GetById([FromRoute] int id)
        {
            var caseItem = caseService.GetById(id);
            if (caseItem == null) return NotFound($"No existe un elemento con el id {id}.");
            return Ok(ToResponse(caseItem));
        }

        [HttpPut("{id}")]
        public ActionResult<CaseResponse> Update([FromRoute] int id, [FromBody] UpdateCaseRequest request)
        {
            try
            {
                var caseItem = caseService.Update(id, request);
                if (caseItem == null) return NotFound($"No existe un elemento con el id {id}.");
                return Ok(ToResponse(caseItem));
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (InvalidOperationException ex) { return Conflict(ex.Message); }
        }

        [HttpPatch("{id}/status")]
        public ActionResult<CaseResponse> ChangeStatus([FromRoute] int id, [FromBody] ChangeStatusRequest request)
        {
            try
            {
                var caseItem = caseService.ChangeStatus(id, request);
                if (caseItem == null) return NotFound($"No existe un elemento con el id {id}.");
                return Ok(ToResponse(caseItem));
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (InvalidOperationException ex) { return Conflict(ex.Message); }
        }

        [HttpPatch("{id}/lawyers/add")]
        public ActionResult<CaseResponse> AddLawyer([FromRoute] int id, [FromBody] AddLawyerRequest request)
        {
            try
            {
                var caseItem = caseService.AddLawyer(id, request);
                if (caseItem == null) return NotFound($"No existe un elemento con el id {id}.");
                return Ok(ToResponse(caseItem));
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (InvalidOperationException ex) { return Conflict(ex.Message); }
        }

        [HttpPatch("{id}/lawyers/remove")]
        public ActionResult<CaseResponse> RemoveLawyer([FromRoute] int id, [FromBody] RemoveLawyerRequest request)
        {
            try
            {
                var caseItem = caseService.RemoveLawyer(id, request);
                if (caseItem == null) return NotFound($"No existe un elemento con el id {id}.");
                return Ok(ToResponse(caseItem));
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (InvalidOperationException ex) { return Conflict(ex.Message); }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            try
            {
                if (!caseService.Delete(id))
                    return NotFound($"No existe un elemento con el id {id}.");
                return NoContent();
            }
            catch (InvalidOperationException ex) { return Conflict(ex.Message); }
        }
    }
}