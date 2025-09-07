using LunaEdgeTestApp.Data;
using LunaEdgeTestApp.Models;

namespace LunaEdgeTestApp.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly AppDbContext _context;

        public UsersRepository(AppDbContext context)
        {
            _context = context;
        }

        public User? GetUserByUsernameOrEmail(string? username, string? email)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username || u.Email == email);
        }

        public User GetUserById(Guid id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id)!;
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }
    }
}
