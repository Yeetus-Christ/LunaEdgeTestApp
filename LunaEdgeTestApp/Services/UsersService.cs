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
        private readonly ILogger<UsersService> _logger;

        public UsersService(IConfiguration configuration, IUsersRepository usersRepository, ILogger<UsersService> logger)
        {
            _configuration = configuration;
            _userRepository = usersRepository;
            _logger = logger;
        }

        public User Authenticate(string? username, string? email, string password)
        {
            _logger.LogInformation($"Authenticating user with username {username} or email {email}");

            var user = _userRepository.GetUserByUsernameOrEmail(username, email);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                _logger.LogInformation($"User with username {username} authenticated successfully");
                return user;
            }

            _logger.LogWarning($"Authentication failed for user with username {username} or email {email}");
            return null!;
        }

        public void Register(string username, string email, string password)
        {
            _logger.LogInformation($"Registering new user with username {username} and email {email}");

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

            try
            {
                _userRepository.AddUser(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error registering user with username {username}");
                throw;
            }

            _logger.LogInformation($"User with username {username} registered successfully");
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
