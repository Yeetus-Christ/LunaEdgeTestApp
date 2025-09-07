using LunaEdgeTestApp.Models;

namespace LunaEdgeTestApp.Repositories
{
    public interface IUsersRepository
    {
        User? GetUserByUsernameOrEmail(string? username, string? email);
        User GetUserById(Guid id);
        void AddUser(User user);
    }
}
