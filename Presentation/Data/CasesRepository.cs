using Presentation.Models;

namespace Presentation.Data
{
    public static class CasesRepository
    {
        private static readonly List<Case> cases = new List<Case>();

        public static void Add(Case caseItem) => cases.Add(caseItem);

        public static IReadOnlyList<Case> GetAll()
            => cases.Where(c => c.Active).ToList().AsReadOnly();

        public static Case? GetById(int id)
            => cases.FirstOrDefault(c => c.Id == id && c.Active);
    }
}