using Presentation.Models;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaseController : ControllerBase
    {
        private static readonly List<Case> Cases = new List<Case>();

        [HttpPost]
        public ActionResult Create([FromBody] Case caseItem)
        {
            var objetoCase = new Case();

            objetoCase.Id = caseItem.Id;
            objetoCase.CaseNumber = caseItem.CaseNumber;
            objetoCase.Title = caseItem.Title;
            objetoCase.Area = caseItem.Area;
            objetoCase.Status = caseItem.Status;
            objetoCase.StartDate = caseItem.StartDate;
            objetoCase.LastUpdate = caseItem.LastUpdate;
            objetoCase.Description = caseItem.Description;
            objetoCase.Notes = caseItem.Notes;
            objetoCase.ClientId = caseItem.ClientId;
            objetoCase.LawyerId = caseItem.LawyerId;
            objetoCase.Active = caseItem.Active;

            Cases.Add(objetoCase);

            return Created();
        }

        [HttpGet]
        public ActionResult<List<Case>> GetAll()
        {
            if (!Cases.Any())
            {
                return NotFound("No elements within the list");
            }

            return Ok(Cases);
        }

        [HttpGet("{id}")]
        public ActionResult<Case> GetById([FromRoute] int id)
        {
            var caseItem = Cases.FirstOrDefault(x => x.Id == id);

            if (caseItem == null)
            {
                return NotFound($"There is no element that match with the id {id}");
            }

            return Ok(caseItem);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            var caseItem = Cases.FirstOrDefault(x => x.Id == id);

            if (caseItem == null)
            {
                return NotFound($"There is no element that match with the id {id}");
            }

            if (!caseItem.Active)
            {
                return Conflict($"The case with id {id} is already inactive");
            }

            caseItem.Active = false;

            return NoContent();
        }

        [HttpPatch("{id}")]
        public ActionResult<Case> PartialUpdate([FromRoute] int id, [FromBody] Case caseItem)
        {
            var caseFound = Cases.FirstOrDefault(x => x.Id == id);

            if (caseFound == null)
            {
                return NotFound($"There is no element that match with the id {id}");
            }

            caseFound.Title = caseItem.Title ?? caseFound.Title;
            caseFound.Area = caseItem.Area ?? caseFound.Area;
            caseFound.Status = caseItem.Status ?? caseFound.Status;
            caseFound.Notes = caseItem.Notes ?? caseFound.Notes;
            caseFound.LastUpdate = caseItem.LastUpdate ?? caseFound.LastUpdate;

            return Ok(caseFound);
        }

        [HttpPut("{id}")]
        public ActionResult<Case> Update([FromRoute] int id, [FromBody] Case caseItem)
        {
            var caseFound = Cases.FirstOrDefault(x => x.Id == id);

            if (caseFound == null)
            {
                return NotFound($"There is no element that match with the id {id}");
            }

            caseFound.CaseNumber = caseItem.CaseNumber;
            caseFound.Title = caseItem.Title;
            caseFound.Area = caseItem.Area;
            caseFound.Status = caseItem.Status;
            caseFound.StartDate = caseItem.StartDate;
            caseFound.LastUpdate = caseItem.LastUpdate;
            caseFound.Description = caseItem.Description;
            caseFound.Notes = caseItem.Notes;
            caseFound.ClientId = caseItem.ClientId;
            caseFound.LawyerId = caseItem.LawyerId;

            return Ok(caseFound);
        }
    }
}