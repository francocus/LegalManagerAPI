using Presentation.Models;

namespace Presentation.Data
{
    public static class UserRepository
    {
        private static readonly List<User> users = new List<User>();

        public static void Add(User user) => users.Add(user);

        public static IReadOnlyList<User> GetAll()
            => users.Where(u => u.Active).ToList().AsReadOnly();

        public static User? GetById(int id)
            => users.FirstOrDefault(u => u.Id == id && u.Active);
    }
}