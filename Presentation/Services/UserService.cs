using Presentation.Data;
using Presentation.DTOs;
using Presentation.Models;

namespace Presentation.Services
{
    public class UserService : IUserService
    {
        private static void EnsureUnique(string email, string dni, int? excludeId = null)
        {
            var users = UsersRepository.GetAll();
            var normalizedEmail = email?.Trim();

            if (users.Any(u => u.Id != excludeId && string.Equals(u.Email.Trim(), normalizedEmail, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Ya existe un usuario con ese email.");

            if (users.Any(u => u.Id != excludeId && u.Dni == dni))
                throw new InvalidOperationException("Ya existe un usuario con ese DNI.");
        }

        public Client CreateClient(CreateClientRequest request)
        {
            EnsureUnique(request.Email, request.Dni);

            var client = new Client(request.FirstName, request.LastName, request.Dni, request.Email, request.Password, request.Phone, request.Address);
            UsersRepository.Add(client);
            return client;
        }

        public Lawyer CreateLawyer(CreateLawyerRequest request)
        {
            EnsureUnique(request.Email, request.Dni);

            var lawyer = new Lawyer(request.FirstName, request.LastName, request.Dni, request.Email, request.Password, request.BarNumber, request.Phone, request.Specialties);
            UsersRepository.Add(lawyer);
            return lawyer;
        }

        public Admin CreateAdmin(CreateAdminRequest request)
        {
            EnsureUnique(request.Email, request.Dni);

            var admin = new Admin(request.FirstName, request.LastName, request.Dni, request.Email, request.Password);
            UsersRepository.Add(admin);
            return admin;
        }

        public IReadOnlyList<User> GetAll() => UsersRepository.GetAll();

        public IReadOnlyList<User> GetClients() => UsersRepository.GetAll().OfType<Client>().ToList().AsReadOnly();

        public IReadOnlyList<User> GetLawyers() => UsersRepository.GetAll().OfType<Lawyer>().ToList().AsReadOnly();

        public IReadOnlyList<User> GetAdmins() => UsersRepository.GetAll().OfType<Admin>().ToList().AsReadOnly();

        public User? GetById(int id) => UsersRepository.GetById(id);

        public User? Update(int id, UpdateUserRequest request)
        {
            var user = GetById(id);
            if (user == null) return null;

            EnsureUnique(request.Email, request.Dni, id);

            user.UpdateDetails(request.FirstName, request.LastName, request.Dni, request.Email);
            return user;
        }

        public User? UpdateClientPhone(int id, string? phone)
        {
            var user = GetById(id);
            if (user == null) return null;

            if (user is not Client client)
                throw new ArgumentException("El usuario indicado no es un cliente.");

            client.UpdatePhone(phone);
            return client;
        }

        public User? UpdateLawyerPhone(int id, string? phone)
        {
            var user = GetById(id);
            if (user == null) return null;

            if (user is not Lawyer lawyer)
                throw new ArgumentException("El usuario indicado no es un abogado.");

            lawyer.UpdatePhone(phone);
            return lawyer;
        }

        public User? UpdateClientAddress(int id, string? address)
        {
            var user = GetById(id);
            if (user == null) return null;

            if (user is not Client client)
                throw new ArgumentException("El usuario indicado no es un cliente.");

            client.UpdateAddress(address);
            return client;
        }

        public User? UpdateBarNumber(int id, string barNumber)
        {
            var user = GetById(id);
            if (user == null) return null;

            if (user is not Lawyer lawyer)
                throw new ArgumentException("El usuario indicado no es un abogado.");

            lawyer.UpdateBarNumber(barNumber);
            return lawyer;
        }

        public User? UpdateSpecialties(int id, IEnumerable<string> specialties)
        {
            var user = GetById(id);
            if (user == null) return null;

            if (user is not Lawyer lawyer)
                throw new ArgumentException("El usuario indicado no es un abogado.");

            lawyer.UpdateSpecialties(specialties);
            return lawyer;
        }

        public bool Delete(int id)
        {
            var user = GetById(id);
            if (user == null) return false;

            var hasActiveCases = CasesRepository.GetAll()
                .Any(c => (c.ClientId == id || c.LawyerIds.Contains(id)) && c.Status != CaseStatus.Cerrado);

            var hasActiveAppointments = AppointmentsRepository.GetAll()
                .Any(a => (a.ClientId == id || a.LawyerId == id)
                    && a.EffectiveStatus != AppointmentStatus.Cancelado && a.EffectiveStatus != AppointmentStatus.Finalizado);

            if (hasActiveCases || hasActiveAppointments)
                throw new InvalidOperationException($"El usuario con id {id} tiene expedientes o turnos activos y no puede ser desactivado.");

            user.Deactivate();
            return true;
        }
    }
}