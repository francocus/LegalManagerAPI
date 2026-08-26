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

        private static UserResponse ToResponse(User user)
        {
            var phone = user switch { Client c => c.Phone, Lawyer l => l.Phone, _ => null };
            var address = (user as Client)?.Address;
            return new(user.Id, user.FirstName, user.LastName, user.Dni, user.Email, user.RegistrationDate, user switch { Client => "client", Lawyer => "lawyer", Admin => "admin", _ => "unknown" }, phone, address);
        }

        private ActionResult? ValidateUnique(string email, string dni, int? excludeId = null)
        {
            var users = UsersRepository.GetAll();
            var normalizedEmail = email?.Trim();

            if (users.Any(u => u.Id != excludeId && string.Equals(u.Email.Trim(), normalizedEmail, StringComparison.OrdinalIgnoreCase)))
                return Conflict("Ya existe un usuario con ese email.");

            if (users.Any(u => u.Id != excludeId && u.Dni == dni))
                return Conflict("Ya existe un usuario con ese DNI.");

            return null;
        }

        [HttpPost("client")]
        public ActionResult<UserResponse> CreateClient([FromBody] CreateClientRequest request)
        {
            var duplicate = ValidateUnique(request.Email, request.Dni);
            if (duplicate != null) return duplicate;

            try
            {
                var client = new Client(request.FirstName, request.LastName, request.Dni, request.Email, request.Password, request.Phone, request.Address);
                UsersRepository.Add(client);
                return CreatedAtAction(nameof(GetById), new { id = client.Id }, ToResponse(client));
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("lawyer")]
        public ActionResult<UserResponse> CreateLawyer([FromBody] CreateLawyerRequest request)
        {
            var duplicate = ValidateUnique(request.Email, request.Dni);
            if (duplicate != null) return duplicate;

            try
            {
                var lawyer = new Lawyer(request.FirstName, request.LastName, request.Dni, request.Email, request.Password, request.BarNumber, request.Phone, request.Specialties);
                UsersRepository.Add(lawyer);
                return CreatedAtAction(nameof(GetById), new { id = lawyer.Id }, ToResponse(lawyer));
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("admin")]
        public ActionResult<UserResponse> CreateAdmin([FromBody] CreateAdminRequest request)
        {
            var duplicate = ValidateUnique(request.Email, request.Dni);
            if (duplicate != null) return duplicate;

            try
            {
                var admin = new Admin(request.FirstName, request.LastName, request.Dni, request.Email, request.Password);
                UsersRepository.Add(admin);
                return CreatedAtAction(nameof(GetById), new { id = admin.Id }, ToResponse(admin));
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<UserResponse>> GetAll()
        {
            var users = UsersRepository.GetAll();
            if (!users.Any()) return NotFound("No hay elementos en la lista.");
            return Ok(users.Select(ToResponse).ToList());
        }

        [HttpGet("clients")]
        public ActionResult<IReadOnlyList<UserResponse>> GetClients()
            => Ok(UsersRepository.GetAll().OfType<Client>().Select(ToResponse).ToList());

        [HttpGet("lawyers")]
        public ActionResult<IReadOnlyList<UserResponse>> GetLawyers()
            => Ok(UsersRepository.GetAll().OfType<Lawyer>().Select(ToResponse).ToList());

        [HttpGet("admins")]
        public ActionResult<IReadOnlyList<UserResponse>> GetAdmins()
            => Ok(UsersRepository.GetAll().OfType<Admin>().Select(ToResponse).ToList());

        [HttpGet("{id}")]
        public ActionResult<UserResponse> GetById([FromRoute] int id)
        {
            var user = UsersRepository.GetById(id);
            if (user == null) return NotFound($"No existe un elemento con el id {id}.");
            return Ok(ToResponse(user));
        }

        [HttpPut("{id}")]
        public ActionResult<UserResponse> Update([FromRoute] int id, [FromBody] UpdateUserRequest request)
        {
            var user = UsersRepository.GetById(id);
            if (user == null) return NotFound($"No existe un elemento con el id {id}.");

            var duplicate = ValidateUnique(request.Email, request.Dni, id);
            if (duplicate != null) return duplicate;

            try
            {
                user.UpdateDetails(request.FirstName, request.LastName, request.Dni, request.Email);
                return Ok(ToResponse(user));
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        [HttpPatch("client/{id}/phone")]
        public ActionResult<UserResponse> UpdatePhone([FromRoute] int id, [FromBody] UpdatePhoneRequest request)
        {
            var user = UsersRepository.GetById(id);
            if (user == null) return NotFound($"No existe un elemento con el id {id}.");

            if (user is not Client client)
                return BadRequest("El usuario indicado no es un cliente.");

            client.UpdatePhone(request.Phone);
            return Ok(ToResponse(client));
        }

        [HttpPatch("lawyer/{id}/phone")]
        public ActionResult<UserResponse> UpdateLawyerPhone([FromRoute] int id, [FromBody] UpdatePhoneRequest request)
        {
            var user = UsersRepository.GetById(id);
            if (user == null) return NotFound($"No existe un elemento con el id {id}.");

            if (user is not Lawyer lawyer)
                return BadRequest("El usuario indicado no es un abogado.");

            lawyer.UpdatePhone(request.Phone);
            return Ok(ToResponse(lawyer));
        }

        [HttpPatch("client/{id}/address")]
        public ActionResult<UserResponse> UpdateAddress([FromRoute] int id, [FromBody] UpdateAddressRequest request)
        {
            var user = UsersRepository.GetById(id);
            if (user == null) return NotFound($"No existe un elemento con el id {id}.");

            if (user is not Client client)
                return BadRequest("El usuario indicado no es un cliente.");

            client.UpdateAddress(request.Address);
            return Ok(ToResponse(client));
        }

        [HttpPatch("lawyer/{id}/bar-number")]
        public ActionResult<UserResponse> UpdateBarNumber([FromRoute] int id, [FromBody] UpdateBarNumberRequest request)
        {
            var user = UsersRepository.GetById(id);
            if (user == null) return NotFound($"No existe un elemento con el id {id}.");

            if (user is not Lawyer lawyer)
                return BadRequest("El usuario indicado no es un abogado.");

            try
            {
                lawyer.UpdateBarNumber(request.BarNumber);
                return Ok(ToResponse(lawyer));
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        [HttpPatch("lawyer/{id}/specialties")]
        public ActionResult<UserResponse> UpdateSpecialties([FromRoute] int id, [FromBody] UpdateSpecialtiesRequest request)
        {
            var user = UsersRepository.GetById(id);
            if (user == null) return NotFound($"No existe un elemento con el id {id}.");

            if (user is not Lawyer lawyer)
                return BadRequest("El usuario indicado no es un abogado.");

            try
            {
                lawyer.UpdateSpecialties(request.Specialties);
                return Ok(ToResponse(lawyer));
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            var user = UsersRepository.GetById(id);
            if (user == null) return NotFound($"No existe un elemento con el id {id}.");

            var hasActiveCases = CasesRepository.GetAll()
                .Any(c => (c.ClientId == id || c.LawyerIds.Contains(id)) && c.Status != CaseStatus.Cerrado);

            var hasActiveAppointments = AppointmentsRepository.GetAll()
                .Any(a => (a.ClientId == id || a.LawyerId == id)
                    && a.EffectiveStatus != AppointmentStatus.Cancelado && a.EffectiveStatus != AppointmentStatus.Finalizado);

            if (hasActiveCases || hasActiveAppointments)
                return Conflict($"El usuario con id {id} tiene expedientes o turnos activos y no puede ser desactivado.");

            try
            {
                user.Deactivate();
                return NoContent();
            }
            catch (InvalidOperationException ex) { return Conflict(ex.Message); }
        }
    }
}