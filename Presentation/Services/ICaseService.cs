using Presentation.DTOs;
using Presentation.Models;

namespace Presentation.Services
{
    public interface ICaseService
    {
        Case Create(CreateCaseRequest request);

        IReadOnlyList<Case> GetAll();

        Case? GetById(int id);

        Case? Update(int id, UpdateCaseRequest request);

        Case? ChangeStatus(int id, ChangeStatusRequest request);

        Case? AddLawyer(int id, AddLawyerRequest request);

        Case? RemoveLawyer(int id, RemoveLawyerRequest request);

        bool Delete(int id);
    }
}