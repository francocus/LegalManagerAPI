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

        private static UserResponse ToResponse(User user) => new(user.Id, user.Name, user.Dni, user.Email, user switch { Client => "client", Lawyer => "lawyer", Admin => "admin", _ => "unknown" });

        [HttpPost("client")]
        public ActionResult<Client> CreateClient([FromBody] CreateClientRequest request)
        {
            if (UsersRepository.GetAll().Any(u => u.Email == request.Email))
                return Conflict("Ya existe un usuario con ese email.");

            if (UsersRepository.GetAll().Any(u => u.Dni == request.Dni))
                return Conflict("Ya existe un usuario con ese DNI.");

            try
            {
                var client = new Client(request.Name, request.Dni, request.Email, request.Password, request.Phone);
                UsersRepository.Add(client);
                return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("lawyer")]
        public ActionResult<Lawyer> CreateLawyer([FromBody] CreateLawyerRequest request)
        {
            if (UsersRepository.GetAll().Any(u => u.Email == request.Email))
                return Conflict("Ya existe un usuario con ese email.");

            if (UsersRepository.GetAll().Any(u => u.Dni == request.Dni))
                return Conflict("Ya existe un usuario con ese DNI.");

            try
            {
                var lawyer = new Lawyer(request.Name, request.Dni, request.Email, request.Password, request.BarNumber);
                UsersRepository.Add(lawyer);
                return CreatedAtAction(nameof(GetById), new { id = lawyer.Id }, lawyer);
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("admin")]
        public ActionResult<Admin> CreateAdmin([FromBody] CreateAdminRequest request)
        {
            if (UsersRepository.GetAll().Any(u => u.Email == request.Email))
                return Conflict("Ya existe un usuario con ese email.");

            if (UsersRepository.GetAll().Any(u => u.Dni == request.Dni))
                return Conflict("Ya existe un usuario con ese DNI.");

            try
            {
                var admin = new Admin(request.Name, request.Dni, request.Email, request.Password);
                UsersRepository.Add(admin);
                return CreatedAtAction(nameof(GetById), new { id = admin.Id }, admin);
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<User>> GetAll()
        {
            var users = UsersRepository.GetAll();
            if (!users.Any()) return NotFound("No elements within the list");
            return Ok(users);
        }

        [HttpGet("clients")]
        public ActionResult<IReadOnlyList<UserResponse>> GetClients()
    => Ok(UsersRepository.GetAll().OfType<Client>().Select(ToResponse));

        [HttpGet("lawyers")]
        public ActionResult<IReadOnlyList<UserResponse>> GetLawyers()
            => Ok(UsersRepository.GetAll().OfType<Lawyer>().Select(ToResponse));

        [HttpGet("admins")]
        public ActionResult<IReadOnlyList<UserResponse>> GetAdmins()
            => Ok(UsersRepository.GetAll().OfType<Admin>().Select(ToResponse));

        [HttpGet("{id}")]
        public ActionResult<User> GetById([FromRoute] int id)
        {
            var user = UsersRepository.GetById(id);
            if (user == null) return NotFound($"There is no element that match with the id {id}");
            return Ok(user);
        }

        [HttpPut("{id}")]
        public ActionResult<UserResponse> Update([FromRoute] int id, [FromBody] UpdateUserRequest request)
        {
            var user = UsersRepository.GetById(id);
            if (user == null) return NotFound($"There is no element that match with the id {id}");

            if (UsersRepository.GetAll().Any(u => u.Id != id && u.Email == request.Email))
                return Conflict("Ya existe un usuario con ese email.");

            if (UsersRepository.GetAll().Any(u => u.Id != id && u.Dni == request.Dni))
                return Conflict("Ya existe un usuario con ese DNI.");

            try
            {
                user.UpdateDetails(request.Name, request.Dni, request.Email);
                return Ok(ToResponse(user));
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        [HttpPatch("client/{id}/phone")]
        public ActionResult<Client> UpdatePhone([FromRoute] int id, [FromBody] UpdatePhoneRequest request)
        {
            var user = UsersRepository.GetById(id);
            if (user == null) return NotFound($"There is no element that match with the id {id}");

            if (user is not Client client)
                return BadRequest("The specified user is not a client.");

            client.UpdatePhone(request.Phone);
            return Ok(client);
        }

        [HttpPatch("lawyer/{id}/bar-number")]
        public ActionResult<Lawyer> UpdateBarNumber([FromRoute] int id, [FromBody] UpdateBarNumberRequest request)
        {
            var user = UsersRepository.GetById(id);
            if (user == null) return NotFound($"There is no element that match with the id {id}");

            if (user is not Lawyer lawyer)
                return BadRequest("The specified user is not a lawyer.");

            try
            {
                lawyer.UpdateBarNumber(request.BarNumber);
                return Ok(lawyer);
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            var user = UsersRepository.GetById(id);
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
            catch (InvalidOperationException ex) { return Conflict(ex.Message); }
        }
    }
}