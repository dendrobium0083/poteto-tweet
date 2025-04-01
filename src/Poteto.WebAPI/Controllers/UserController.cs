using Microsoft.AspNetCore.Mvc;
using Poteto.Application.Interfaces.Services;
using Poteto.Application.DTOs;
using Poteto.Infrastructure.Configurations;
using System.Data;

namespace Poteto.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly DbConnectionFactory _dbConnectionFactory;

        public UserController(IUserService userService, DbConnectionFactory dbConnectionFactory)
        {
            _userService = userService;
            _dbConnectionFactory = dbConnectionFactory;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var userDto = await _userService.RegisterUserAsync(
                request.UserName,
                request.Email,
                request.Password,
                connection
            );

            return Ok(userDto);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var userDto = await _userService.AuthenticateUserAsync(
                request.Email,
                request.Password,
                connection
            );

            if (userDto == null)
            {
                return Unauthorized("認証に失敗しました。");
            }

            return Ok(userDto);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(int userId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var userDto = await _userService.GetUserByIdAsync(userId, connection);
            return Ok(userDto);
        }
    }

    public class RegisterRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
