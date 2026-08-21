using Presentation.Data;
using Presentation.DTOs;
using Presentation.Models;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpPost]
        public ActionResult<User> Create([FromBody] CreateUserRequest request)
        {
            if (UserRepository.GetAll().Any(u => u.Email == request.Email))
                return Conflict("Ya existe un usuario con ese email.");

            if (UserRepository.GetAll().Any(u => u.Dni == request.Dni))
                return Conflict("Ya existe un usuario con ese DNI.");

            try
            {
                var user = new User(request.Name, request.Dni, request.Email, request.Password, request.Role);
                UserRepository.Add(user);
                return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<User>> GetAll()
        {
            var users = UserRepository.GetAll();
            if (!users.Any()) return NotFound("No elements within the list");
            return Ok(users);
        }

        [HttpGet("{id}")]
        public ActionResult<User> GetById([FromRoute] int id)
        {
            var user = UserRepository.GetById(id);
            if (user == null) return NotFound($"There is no element that match with the id {id}");
            return Ok(user);
        }

        [HttpPut("{id}")]
        public ActionResult<User> Update([FromRoute] int id, [FromBody] UpdateUserRequest request)
        {
            var user = UserRepository.GetById(id);
            if (user == null) return NotFound($"There is no element that match with the id {id}");

            try
            {
                user.UpdateDetails(request.Name, request.Dni, request.Email);
                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}/role")]
        public ActionResult<User> AssignRole([FromRoute] int id, [FromBody] AssignRoleRequest request)
        {
            var user = UserRepository.GetById(id);
            if (user == null) return NotFound($"There is no element that match with the id {id}");

            try
            {
                user.AssignRole(request.Role);
                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            var user = UserRepository.GetById(id);
            if (user == null) return NotFound($"There is no element that match with the id {id}");

            var hasActiveCases = CasesRepository.GetAll()
                .Any(c => (c.ClientId == id || c.LawyerId == id) && c.Status != "cerrado");

            var hasActiveAppointments = AppointmentsRepository.GetAll()
                .Any(a => (a.ClientId == id || a.LawyerId == id)
                    && a.EffectiveStatus != "cancelado" && a.EffectiveStatus != "finalizado");

            if (hasActiveCases || hasActiveAppointments)
                return Conflict($"The user with id {id} has active cases or appointments and cannot be deactivated.");

            try
            {
                user.Deactivate();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}