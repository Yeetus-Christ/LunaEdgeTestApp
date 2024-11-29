using LunaEdgeTestApp.Models;

namespace LunaEdgeTestApp.Services
{
    public interface IUsersService
    {
        public User Authenticate(string? username, string? email, string password);
        public void Register(string username, string email, string password);
        public string GenerateJwtToken(User user);
    }
}
