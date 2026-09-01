using LegalManager.Domain.Entities;

namespace LegalManager.Application.DTOs
{
    public record ChangeStatusRequest(CaseStatus Status);
}