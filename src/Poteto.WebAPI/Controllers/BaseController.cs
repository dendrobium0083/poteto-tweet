using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Poteto.WebAPI.Controllers
{
    /// <summary>
    /// すべてのコントローラーの基底クラス
    /// </summary>
    /// <typeparam name="T">コントローラーの型</typeparam>
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController<T> : ControllerBase where T : class
    {
        protected readonly ILogger<T> _logger;

        /// <summary>
        /// BaseControllerの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="logger">ロガーインスタンス</param>
        /// <exception cref="ArgumentNullException">loggerがnullの場合</exception>
        protected BaseController(ILogger<T> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// モデルの状態を検証し、無効な場合はBadRequestを返します
        /// </summary>
        /// <returns>モデルの状態が無効な場合はBadRequest、それ以外の場合はnull</returns>
        protected ActionResult? ValidateModelState()
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("モデルの状態が無効です: {Errors}",
                    string.Join(", ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)));
                return BadRequest(ModelState);
            }
            return null;
        }

        /// <summary>
        /// リソースが見つからない場合のレスポンスを返します
        /// </summary>
        /// <param name="resourceName">リソース名</param>
        /// <param name="id">リソースID</param>
        /// <returns>NotFoundレスポンス</returns>
        protected ActionResult NotFoundResponse(string resourceName, long id)
        {
            var message = $"{resourceName} (ID: {id}) が見つかりません";
            _logger.LogWarning(message);
            return NotFound(new { Message = message });
        }

        /// <summary>
        /// 成功レスポンスを返します
        /// </summary>
        /// <typeparam name="TResult">レスポンスの型</typeparam>
        /// <param name="data">レスポンスデータ</param>
        /// <returns>成功レスポンス</returns>
        protected ActionResult<TResult> SuccessResponse<TResult>(TResult data)
        {
            _logger.LogInformation("操作が成功しました");
            return Ok(new
            {
                Success = true,
                Data = data
            });
        }

        /// <summary>
        /// 成功レスポンスを返します（データなし）
        /// </summary>
        /// <returns>成功レスポンス</returns>
        protected ActionResult SuccessResponse()
        {
            _logger.LogInformation("操作が成功しました");
            return Ok(new
            {
                Success = true
            });
        }

        /// <summary>
        /// エラーレスポンスを返します
        /// </summary>
        /// <param name="ex">例外</param>
        /// <param name="message">エラーメッセージ</param>
        /// <returns>エラーレスポンス</returns>
        protected ActionResult ErrorResponse(Exception ex, string message)
        {
            _logger.LogError(ex, message);
            return StatusCode(500, new
            {
                Success = false,
                Message = message,
                Error = ex.Message
            });
        }
    }
}
