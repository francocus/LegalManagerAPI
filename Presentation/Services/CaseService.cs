using Presentation.Data;
using Presentation.DTOs;
using Presentation.Models;

namespace Presentation.Services
{
    public class CaseService : ICaseService
    {
        public Case Create(CreateCaseRequest request)
        {
            if (CasesRepository.GetAll().Any(c => c.CaseNumber == request.CaseNumber))
                throw new InvalidOperationException("Ya existe un expediente con ese número.");

            if (UsersRepository.GetById(request.ClientId) is not Client)
                throw new ArgumentException("El cliente indicado no es válido.");

            if (UsersRepository.GetById(request.LawyerId) is not Lawyer)
                throw new ArgumentException("El abogado indicado no es válido.");

            var createdBy = UsersRepository.GetById(request.CreatedByUserId);
            if (createdBy is not Lawyer && createdBy is not Admin)
                throw new ArgumentException("El expediente solo puede ser creado por un abogado o un administrador.");

            var caseItem = new Case(request.CaseNumber, request.Title, request.Area, request.StartDate, request.Description, request.Notes, request.ClientId, request.LawyerId, request.CreatedByUserId);
            CasesRepository.Add(caseItem);
            return caseItem;
        }

        public IReadOnlyList<Case> GetAll() => CasesRepository.GetAll();

        public Case? GetById(int id) => CasesRepository.GetById(id);

        public Case? Update(int id, UpdateCaseRequest request)
        {
            var caseItem = GetById(id);
            if (caseItem == null) return null;

            caseItem.UpdateDetails(request.Title, request.Area, request.Description, request.Notes);
            return caseItem;
        }

        public Case? ChangeStatus(int id, ChangeStatusRequest request)
        {
            var caseItem = GetById(id);
            if (caseItem == null) return null;

            caseItem.ChangeStatus(request.Status);
            return caseItem;
        }

        public Case? AddLawyer(int id, AddLawyerRequest request)
        {
            var caseItem = GetById(id);
            if (caseItem == null) return null;

            if (UsersRepository.GetById(request.LawyerId) is not Lawyer)
                throw new ArgumentException("El abogado indicado no es válido.");

            caseItem.AddLawyer(request.LawyerId);
            return caseItem;
        }

        public Case? RemoveLawyer(int id, RemoveLawyerRequest request)
        {
            var caseItem = GetById(id);
            if (caseItem == null) return null;

            if (UsersRepository.GetById(request.LawyerId) is not Lawyer)
                throw new ArgumentException("El abogado indicado no es válido.");

            caseItem.RemoveLawyer(request.LawyerId);
            return caseItem;
        }

        public bool Delete(int id)
        {
            var caseItem = GetById(id);
            if (caseItem == null) return false;

            var hasActiveAppointments = AppointmentsRepository.GetAll()
                .Any(a => a.CaseId == id
                    && a.EffectiveStatus != AppointmentStatus.Cancelado && a.EffectiveStatus != AppointmentStatus.Finalizado);

            if (hasActiveAppointments)
                throw new InvalidOperationException($"El expediente con id {id} tiene turnos activos asociados.");

            caseItem.Deactivate();
            return true;
        }
    }
}