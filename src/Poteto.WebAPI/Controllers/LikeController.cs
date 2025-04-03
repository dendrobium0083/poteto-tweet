using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Poteto.Application.DTOs;
using Poteto.Application.Interfaces.Services;
using Poteto.Application.Requests;

namespace Poteto.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LikeController : ControllerBase
    {
        private readonly ILikeService _likeService;
        private readonly ILogger<LikeController> _logger;

        public LikeController(ILikeService likeService, ILogger<LikeController> logger)
        {
            _likeService = likeService ?? throw new ArgumentNullException(nameof(likeService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 新規いいね登録のエンドポイント
        /// </summary>
        /// <param name="request">いいね登録用のリクエストDTO</param>
        /// <returns>登録されたいいね情報のDTO</returns>
        /// <response code="201">いいねが正常に作成された場合</response>
        /// <response code="400">リクエストが無効な場合</response>
        /// <response code="500">サーバーエラーが発生した場合</response>
        [HttpPost]
        [ProducesResponseType(typeof(LikeDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateLike([FromBody] CreateLikeRequest request)
        {
            try
            {
                _logger.LogInformation("いいね作成リクエストを受信: TweetId={TweetId}, UserId={UserId}",
                    request.TweetId, request.UserId);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("無効なリクエスト: {ModelState}", ModelState);
                    return BadRequest(ModelState);
                }

                var likeDto = await _likeService.CreateLikeAsync(request.TweetId, request.UserId);
                _logger.LogInformation("いいねが正常に作成されました: LikeId={Id}", likeDto.Id);

                return CreatedAtAction(nameof(GetLikesByTweetId), new { tweetId = likeDto.TweetId }, likeDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "いいね作成中にエラーが発生しました: TweetId={TweetId}, UserId={UserId}",
                    request.TweetId, request.UserId);
                return StatusCode(StatusCodes.Status500InternalServerError, "いいねの作成中にエラーが発生しました。");
            }
        }

        /// <summary>
        /// 指定のツイートに対するいいね一覧を取得するエンドポイント
        /// </summary>
        /// <param name="tweetId">対象ツイートのID</param>
        /// <returns>いいね情報のDTO一覧</returns>
        /// <response code="200">いいね一覧が正常に取得された場合</response>
        /// <response code="400">リクエストが無効な場合</response>
        /// <response code="500">サーバーエラーが発生した場合</response>
        [HttpGet("tweet/{tweetId}")]
        [ProducesResponseType(typeof(IEnumerable<LikeDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetLikesByTweetId(int tweetId)
        {
            try
            {
                _logger.LogInformation("いいね一覧取得リクエストを受信: TweetId={TweetId}", tweetId);

                if (tweetId <= 0)
                {
                    _logger.LogWarning("無効なTweetId: {TweetId}", tweetId);
                    return BadRequest("TweetIdは正の値でなければなりません。");
                }

                var likes = await _likeService.GetLikesByTweetIdAsync(tweetId);
                _logger.LogInformation("いいね一覧を正常に取得しました: TweetId={TweetId}, 件数={Count}",
                    tweetId, likes.Count());

                return Ok(likes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "いいね一覧取得中にエラーが発生しました: TweetId={TweetId}", tweetId);
                return StatusCode(StatusCodes.Status500InternalServerError, "いいね一覧の取得中にエラーが発生しました。");
            }
        }

        /// <summary>
        /// 指定されたいいね関係を解除するエンドポイント
        /// </summary>
        /// <param name="tweetId">対象ツイートのID</param>
        /// <param name="userId">いいね解除するユーザのID</param>
        /// <response code="204">いいねが正常に解除された場合</response>
        /// <response code="400">リクエストが無効な場合</response>
        /// <response code="500">サーバーエラーが発生した場合</response>
        [HttpDelete("{tweetId}/{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteLike(int tweetId, int userId)
        {
            try
            {
                _logger.LogInformation("いいね解除リクエストを受信: TweetId={TweetId}, UserId={UserId}",
                    tweetId, userId);

                if (tweetId <= 0 || userId <= 0)
                {
                    _logger.LogWarning("無効なID: TweetId={TweetId}, UserId={UserId}",
                        tweetId, userId);
                    return BadRequest("IDは正の値でなければなりません。");
                }

                await _likeService.DeleteLikeAsync(tweetId, userId);
                _logger.LogInformation("いいねが正常に解除されました: TweetId={TweetId}, UserId={UserId}",
                    tweetId, userId);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "いいね解除中にエラーが発生しました: TweetId={TweetId}, UserId={UserId}",
                    tweetId, userId);
                return StatusCode(StatusCodes.Status500InternalServerError, "いいねの解除中にエラーが発生しました。");
            }
        }
    }
}
