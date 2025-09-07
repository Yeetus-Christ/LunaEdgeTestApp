using LunaEdgeTestApp.Dtos;
using LunaEdgeTestApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LunaEdgeTestApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService userService)
        {
            _usersService = userService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            var user = _usersService.Authenticate(loginRequest.Username, loginRequest.Username, loginRequest.Password);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            var token = _usersService.GenerateJwtToken(user);
            return Ok(new { token });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest registerRequest)
        {
            if (_usersService.Authenticate(registerRequest.Username, registerRequest.Email, registerRequest.Password) != null)
            {
                return BadRequest(new { message = "Username or email already taken" });
            }

            try
            {
                _usersService.Register(registerRequest.Username, registerRequest.Email, registerRequest.Password);
            }
            catch (Exception ex) 
            { 
                return BadRequest(new { message = ex.Message });
            }

            return Ok(new { message = "User registered successfully" });
        }
    }
}
