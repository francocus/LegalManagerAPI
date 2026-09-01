using LegalManager.Domain.Entities;
using LegalManager.Domain.Interfaces;

namespace LegalManager.Infrastructure.Repositories
{
    public class CasesRepository : ICaseRepository
    {
        private readonly List<Case> cases = new List<Case>();

        public void Add(Case caseItem) => cases.Add(caseItem);

        public IReadOnlyList<Case> GetAll()
            => cases.Where(c => c.Active).ToList().AsReadOnly();

        public Case? GetById(int id)
            => cases.FirstOrDefault(c => c.Id == id && c.Active);
    }
}