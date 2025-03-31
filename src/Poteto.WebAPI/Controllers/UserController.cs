using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Poteto.Application.DTOs;
using Poteto.Application.Interfaces.Services;

namespace Poteto.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// 新規ユーザ登録のエンドポイント
        /// </summary>
        /// <param name="request">登録に必要なユーザ情報</param>
        /// <returns>登録されたユーザ情報の DTO</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userDto = await _userService.RegisterUserAsync(request.UserName, request.Email, request.Password);
            // 作成されたリソースを示す CreatedAtAction を利用
            return CreatedAtAction(nameof(GetById), new { id = userDto.UserId }, userDto);
        }

        /// <summary>
        /// ユーザ認証のエンドポイント
        /// </summary>
        /// <param name="request">認証に必要なメールアドレスとパスワード</param>
        /// <returns>認証に成功したユーザ情報の DTO。失敗時は Unauthorized を返す。</returns>
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userDto = await _userService.AuthenticateUserAsync(request.Email, request.Password);
            if (userDto == null)
            {
                return Unauthorized();
            }
            return Ok(userDto);
        }

        /// <summary>
        /// 指定したユーザID のユーザ情報を取得するエンドポイント
        /// </summary>
        /// <param name="id">ユーザの一意な識別子</param>
        /// <returns>ユーザ情報の DTO</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userDto = await _userService.GetUserByIdAsync(id);
            if (userDto == null)
            {
                return NotFound();
            }
            return Ok(userDto);
        }
    }

    // ユーザ登録用のリクエスト DTO
    public class RegisterUserRequest
    {
        /// <summary>
        /// ユーザ名（必須）
        /// </summary>
        public required string UserName { get; set; }

        /// <summary>
        /// メールアドレス（必須）
        /// </summary>
        public required string Email { get; set; }

        /// <summary>
        /// 平文パスワード（必須）
        /// </summary>
        public required string Password { get; set; }
    }

    // ユーザ認証用のリクエスト DTO
    public class AuthenticateUserRequest
    {
        /// <summary>
        /// メールアドレス（必須）
        /// </summary>
        public required string Email { get; set; }

        /// <summary>
        /// 平文パスワード（必須）
        /// </summary>
        public required string Password { get; set; }
    }
}
