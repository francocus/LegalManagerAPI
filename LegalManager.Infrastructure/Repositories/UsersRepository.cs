using LegalManager.Domain.Entities;
using LegalManager.Domain.Interfaces;

namespace LegalManager.Infrastructure.Repositories
{
    public class UsersRepository : IUserRepository
    {
        private readonly List<User> users = new List<User>();

        public void Add(User user) => users.Add(user);

        public IReadOnlyList<User> GetAll()
            => users.Where(u => u.Active).ToList().AsReadOnly();

        public User? GetById(int id)
            => users.FirstOrDefault(u => u.Id == id && u.Active);
    }
}