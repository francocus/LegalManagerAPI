using Presentation.DTOs;
using Presentation.Models;

namespace Presentation.Services
{
    public interface IUserService
    {
        Client CreateClient(CreateClientRequest request);

        Lawyer CreateLawyer(CreateLawyerRequest request);

        Admin CreateAdmin(CreateAdminRequest request);

        IReadOnlyList<User> GetAll();

        IReadOnlyList<User> GetClients();

        IReadOnlyList<User> GetLawyers();

        IReadOnlyList<User> GetAdmins();

        User? GetById(int id);

        User? Update(int id, UpdateUserRequest request);

        User? UpdateClientPhone(int id, string? phone);

        User? UpdateLawyerPhone(int id, string? phone);

        User? UpdateClientAddress(int id, string? address);

        User? UpdateBarNumber(int id, string barNumber);

        User? UpdateSpecialties(int id, IEnumerable<string> specialties);

        bool Delete(int id);
    }
}