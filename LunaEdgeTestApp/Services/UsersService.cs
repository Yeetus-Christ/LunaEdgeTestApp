using LunaEdgeTestApp.Data;
using LunaEdgeTestApp.Models;
using LunaEdgeTestApp.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace LunaEdgeTestApp.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UsersService(IConfiguration configuration, IUsersRepository usersRepository)
        {
            _configuration = configuration;
            _userRepository = usersRepository;
        }

        public User Authenticate(string? username, string? email, string password)
        {
            var user = _userRepository.GetUserByUsernameOrEmail(username, email);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return user;
            }

            return null!;
        }

        public void Register(string username, string email, string password)
        {
            if (!CheckComplexity(password)) 
            {
                throw new ArgumentException("Password should contain minimum 8 characters, at least 1 uppercase, 1 lowercase, 1 digit, and 1 special character");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            };

            _userRepository.AddUser(user);
        }

        public string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("UserId", user.Id.ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static bool CheckComplexity(string password)
        {
            if (string.IsNullOrEmpty(password)) return false;

            // Minimum 8 characters, at least 1 uppercase, 1 lowercase, 1 digit, and 1 special character
            var passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
            return Regex.IsMatch(password, passwordPattern);
        }
    }
}
