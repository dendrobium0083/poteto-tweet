using System.Collections.Generic;
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
    public class TweetController : ControllerBase
    {
        private readonly ITweetService _tweetService;
        private readonly ILogger<TweetController> _logger;

        public TweetController(ITweetService tweetService, ILogger<TweetController> logger)
        {
            _tweetService = tweetService ?? throw new ArgumentNullException(nameof(tweetService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 新規ツイート投稿のエンドポイント
        /// </summary>
        /// <param name="request">ツイート投稿用リクエストDTO</param>
        /// <returns>登録されたツイートのDTO</returns>
        /// <response code="201">ツイートが正常に作成された場合</response>
        /// <response code="400">リクエストが無効な場合</response>
        /// <response code="500">サーバーエラーが発生した場合</response>
        [HttpPost]
        [ProducesResponseType(typeof(TweetDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateTweet([FromBody] CreateTweetRequest request)
        {
            try
            {
                _logger.LogInformation("ツイート作成リクエストを受信: UserId={UserId}", request.UserId);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("無効なリクエスト: {ModelState}", ModelState);
                    return BadRequest(ModelState);
                }

                var tweetDto = await _tweetService.CreateTweetAsync(request.UserId, request.Content);
                _logger.LogInformation("ツイートが正常に作成されました: TweetId={TweetId}", tweetDto.TweetId);

                return CreatedAtAction(nameof(GetTweetById), new { id = tweetDto.TweetId }, tweetDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ツイート作成中にエラーが発生しました: UserId={UserId}", request.UserId);
                return StatusCode(StatusCodes.Status500InternalServerError, "ツイートの作成中にエラーが発生しました。");
            }
        }

        /// <summary>
        /// ツイートID を指定してツイート情報を取得するエンドポイント
        /// </summary>
        /// <param name="id">ツイートの一意な識別子</param>
        /// <returns>ツイートのDTO</returns>
        /// <response code="200">ツイートが正常に取得された場合</response>
        /// <response code="400">リクエストが無効な場合</response>
        /// <response code="404">ツイートが見つからない場合</response>
        /// <response code="500">サーバーエラーが発生した場合</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TweetDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTweetById(int id)
        {
            try
            {
                _logger.LogInformation("ツイート取得リクエストを受信: TweetId={TweetId}", id);

                if (id <= 0)
                {
                    _logger.LogWarning("無効なTweetId: {TweetId}", id);
                    return BadRequest("TweetIdは正の値でなければなりません。");
                }

                var tweetDto = await _tweetService.GetTweetByIdAsync(id);
                if (tweetDto == null)
                {
                    _logger.LogWarning("ツイートが見つかりません: TweetId={TweetId}", id);
                    return NotFound();
                }

                _logger.LogInformation("ツイートを正常に取得しました: TweetId={TweetId}", id);
                return Ok(tweetDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ツイート取得中にエラーが発生しました: TweetId={TweetId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "ツイートの取得中にエラーが発生しました。");
            }
        }

        /// <summary>
        /// 指定したユーザの全ツイート一覧を取得するエンドポイント
        /// </summary>
        /// <param name="userId">ユーザの一意な識別子</param>
        /// <returns>ツイートのDTO一覧</returns>
        /// <response code="200">ツイート一覧が正常に取得された場合</response>
        /// <response code="400">リクエストが無効な場合</response>
        /// <response code="500">サーバーエラーが発生した場合</response>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<TweetDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTweetsByUserId(int userId)
        {
            try
            {
                _logger.LogInformation("ユーザのツイート一覧取得リクエストを受信: UserId={UserId}", userId);

                if (userId <= 0)
                {
                    _logger.LogWarning("無効なUserId: {UserId}", userId);
                    return BadRequest("UserIdは正の値でなければなりません。");
                }

                var tweets = await _tweetService.GetTweetsByUserIdAsync(userId);
                _logger.LogInformation("ツイート一覧を正常に取得しました: UserId={UserId}, 件数={Count}",
                    userId, tweets.Count());

                return Ok(tweets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ツイート一覧取得中にエラーが発生しました: UserId={UserId}", userId);
                return StatusCode(StatusCodes.Status500InternalServerError, "ツイート一覧の取得中にエラーが発生しました。");
            }
        }

        /// <summary>
        /// ツイートの内容を更新するエンドポイント
        /// </summary>
        /// <param name="id">更新対象のツイートID</param>
        /// <param name="request">更新内容を含むリクエストDTO</param>
        /// <response code="204">ツイートが正常に更新された場合</response>
        /// <response code="400">リクエストが無効な場合</response>
        /// <response code="500">サーバーエラーが発生した場合</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateTweet(int id, [FromBody] UpdateTweetRequest request)
        {
            try
            {
                _logger.LogInformation("ツイート更新リクエストを受信: TweetId={TweetId}", id);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("無効なリクエスト: {ModelState}", ModelState);
                    return BadRequest(ModelState);
                }

                if (id <= 0)
                {
                    _logger.LogWarning("無効なTweetId: {TweetId}", id);
                    return BadRequest("TweetIdは正の値でなければなりません。");
                }

                await _tweetService.UpdateTweetAsync(id, request.NewContent);
                _logger.LogInformation("ツイートが正常に更新されました: TweetId={TweetId}", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ツイート更新中にエラーが発生しました: TweetId={TweetId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "ツイートの更新中にエラーが発生しました。");
            }
        }
    }
}
