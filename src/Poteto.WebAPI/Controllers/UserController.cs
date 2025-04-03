using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<UserController> _logger;

        public UserController(
            IUserService userService,
            DbConnectionFactory dbConnectionFactory,
            ILogger<UserController> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 新規ユーザー登録のエンドポイント
        /// </summary>
        /// <param name="request">ユーザー登録用のリクエストDTO</param>
        /// <returns>登録されたユーザー情報のDTO</returns>
        /// <response code="200">ユーザーが正常に登録された場合</response>
        /// <response code="400">リクエストが無効な場合</response>
        /// <response code="500">サーバーエラーが発生した場合</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                _logger.LogInformation("ユーザー登録リクエストを受信: UserName={UserName}, Email={Email}",
                    request.UserName, request.Email);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("無効なリクエスト: {ModelState}", ModelState);
                    return BadRequest(ModelState);
                }

                using var connection = _dbConnectionFactory.CreateConnection();

                var userDto = await _userService.RegisterUserAsync(
                    request.UserName,
                    request.Email,
                    request.Password,
                    connection
                );

                _logger.LogInformation("ユーザーが正常に登録されました: UserId={UserId}", userDto.UserId);
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ユーザー登録中にエラーが発生しました: UserName={UserName}, Email={Email}",
                    request.UserName, request.Email);
                return StatusCode(StatusCodes.Status500InternalServerError, "ユーザー登録中にエラーが発生しました。");
            }
        }

        /// <summary>
        /// ユーザーログインのエンドポイント
        /// </summary>
        /// <param name="request">ログイン用のリクエストDTO</param>
        /// <returns>認証されたユーザー情報のDTO</returns>
        /// <response code="200">ログインが成功した場合</response>
        /// <response code="400">リクエストが無効な場合</response>
        /// <response code="401">認証に失敗した場合</response>
        /// <response code="500">サーバーエラーが発生した場合</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                _logger.LogInformation("ログインリクエストを受信: Email={Email}", request.Email);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("無効なリクエスト: {ModelState}", ModelState);
                    return BadRequest(ModelState);
                }

                using var connection = _dbConnectionFactory.CreateConnection();

                var userDto = await _userService.AuthenticateUserAsync(
                    request.Email,
                    request.Password,
                    connection
                );

                if (userDto == null)
                {
                    _logger.LogWarning("認証に失敗しました: Email={Email}", request.Email);
                    return Unauthorized("認証に失敗しました。");
                }

                _logger.LogInformation("ログインが成功しました: UserId={UserId}", userDto.UserId);
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ログイン中にエラーが発生しました: Email={Email}", request.Email);
                return StatusCode(StatusCodes.Status500InternalServerError, "ログイン中にエラーが発生しました。");
            }
        }

        /// <summary>
        /// ユーザー情報取得のエンドポイント
        /// </summary>
        /// <param name="userId">ユーザーの一意な識別子</param>
        /// <returns>ユーザー情報のDTO</returns>
        /// <response code="200">ユーザー情報が正常に取得された場合</response>
        /// <response code="400">リクエストが無効な場合</response>
        /// <response code="404">ユーザーが見つからない場合</response>
        /// <response code="500">サーバーエラーが発生した場合</response>
        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUser(int userId)
        {
            try
            {
                _logger.LogInformation("ユーザー情報取得リクエストを受信: UserId={UserId}", userId);

                if (userId <= 0)
                {
                    _logger.LogWarning("無効なUserId: {UserId}", userId);
                    return BadRequest("UserIdは正の値でなければなりません。");
                }

                using var connection = _dbConnectionFactory.CreateConnection();

                var userDto = await _userService.GetUserByIdAsync(userId, connection);
                if (userDto == null)
                {
                    _logger.LogWarning("ユーザーが見つかりません: UserId={UserId}", userId);
                    return NotFound();
                }

                _logger.LogInformation("ユーザー情報を正常に取得しました: UserId={UserId}", userId);
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ユーザー情報取得中にエラーが発生しました: UserId={UserId}", userId);
                return StatusCode(StatusCodes.Status500InternalServerError, "ユーザー情報の取得中にエラーが発生しました。");
            }
        }
    }

    namespace Poteto.WebAPI.Requests
    {
        /// <summary>
        /// ユーザー登録用のリクエストDTO
        /// </summary>
        public class RegisterRequest
        {
            /// <summary>
            /// ユーザー名
            /// </summary>
            public string UserName { get; set; } = string.Empty;

            /// <summary>
            /// メールアドレス
            /// </summary>
            public string Email { get; set; } = string.Empty;

            /// <summary>
            /// パスワード
            /// </summary>
            public string Password { get; set; } = string.Empty;
        }

        /// <summary>
        /// ログイン用のリクエストDTO
        /// </summary>
        public class LoginRequest
        {
            /// <summary>
            /// メールアドレス
            /// </summary>
            public string Email { get; set; } = string.Empty;

            /// <summary>
            /// パスワード
            /// </summary>
            public string Password { get; set; } = string.Empty;
        }
    }
}
