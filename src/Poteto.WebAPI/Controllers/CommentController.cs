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
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly ILogger<CommentController> _logger;

        public CommentController(ICommentService commentService, ILogger<CommentController> logger)
        {
            _commentService = commentService ?? throw new ArgumentNullException(nameof(commentService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 新規コメント投稿のエンドポイント
        /// </summary>
        /// <param name="request">コメント投稿用リクエストDTO</param>
        /// <returns>登録されたコメントのDTO</returns>
        /// <response code="201">コメントが正常に作成された場合</response>
        /// <response code="400">リクエストが無効な場合</response>
        /// <response code="500">サーバーエラーが発生した場合</response>
        [HttpPost]
        [ProducesResponseType(typeof(CommentDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequest request)
        {
            try
            {
                _logger.LogInformation("コメント作成リクエストを受信: TweetId={TweetId}, UserId={UserId}",
                    request.TweetId, request.UserId);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("無効なリクエスト: {ModelState}", ModelState);
                    return BadRequest(ModelState);
                }

                var commentDto = await _commentService.CreateCommentAsync(request.TweetId, request.UserId, request.Content);
                _logger.LogInformation("コメントが正常に作成されました: CommentId={CommentId}", commentDto.CommentId);

                return CreatedAtAction(nameof(GetCommentById), new { id = commentDto.CommentId }, commentDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "コメント作成中にエラーが発生しました: TweetId={TweetId}, UserId={UserId}",
                    request.TweetId, request.UserId);
                return StatusCode(StatusCodes.Status500InternalServerError, "コメントの作成中にエラーが発生しました。");
            }
        }

        /// <summary>
        /// コメントID を指定してコメント情報を取得するエンドポイント
        /// </summary>
        /// <param name="id">コメントの一意な識別子</param>
        /// <returns>コメントのDTO</returns>
        /// <response code="200">コメントが正常に取得された場合</response>
        /// <response code="400">リクエストが無効な場合</response>
        /// <response code="404">コメントが見つからない場合</response>
        /// <response code="500">サーバーエラーが発生した場合</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CommentDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCommentById(int id)
        {
            try
            {
                _logger.LogInformation("コメント取得リクエストを受信: CommentId={CommentId}", id);

                if (id <= 0)
                {
                    _logger.LogWarning("無効なCommentId: {CommentId}", id);
                    return BadRequest("CommentIdは正の値でなければなりません。");
                }

                var commentDto = await _commentService.GetCommentByIdAsync(id);
                if (commentDto == null)
                {
                    _logger.LogWarning("コメントが見つかりません: CommentId={CommentId}", id);
                    return NotFound();
                }

                _logger.LogInformation("コメントを正常に取得しました: CommentId={CommentId}", id);
                return Ok(commentDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "コメント取得中にエラーが発生しました: CommentId={CommentId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "コメントの取得中にエラーが発生しました。");
            }
        }

        /// <summary>
        /// 指定したツイートに対する全コメント一覧を取得するエンドポイント
        /// </summary>
        /// <param name="tweetId">対象ツイートのID</param>
        /// <returns>コメントDTOの一覧</returns>
        /// <response code="200">コメント一覧が正常に取得された場合</response>
        /// <response code="400">リクエストが無効な場合</response>
        /// <response code="500">サーバーエラーが発生した場合</response>
        [HttpGet("tweet/{tweetId}")]
        [ProducesResponseType(typeof(IEnumerable<CommentDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCommentsByTweetId(int tweetId)
        {
            try
            {
                _logger.LogInformation("ツイートのコメント一覧取得リクエストを受信: TweetId={TweetId}", tweetId);

                if (tweetId <= 0)
                {
                    _logger.LogWarning("無効なTweetId: {TweetId}", tweetId);
                    return BadRequest("TweetIdは正の値でなければなりません。");
                }

                var commentDtos = await _commentService.GetCommentsByTweetIdAsync(tweetId);
                _logger.LogInformation("コメント一覧を正常に取得しました: TweetId={TweetId}, 件数={Count}",
                    tweetId, commentDtos.Count());

                return Ok(commentDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "コメント一覧取得中にエラーが発生しました: TweetId={TweetId}", tweetId);
                return StatusCode(StatusCodes.Status500InternalServerError, "コメント一覧の取得中にエラーが発生しました。");
            }
        }

        /// <summary>
        /// コメントの内容を更新するエンドポイント
        /// </summary>
        /// <param name="id">更新対象のコメントID</param>
        /// <param name="request">更新内容を含むリクエストDTO</param>
        /// <response code="204">コメントが正常に更新された場合</response>
        /// <response code="400">リクエストが無効な場合</response>
        /// <response code="500">サーバーエラーが発生した場合</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateComment(int id, [FromBody] UpdateCommentRequest request)
        {
            try
            {
                _logger.LogInformation("コメント更新リクエストを受信: CommentId={CommentId}", id);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("無効なリクエスト: {ModelState}", ModelState);
                    return BadRequest(ModelState);
                }

                if (id <= 0)
                {
                    _logger.LogWarning("無効なCommentId: {CommentId}", id);
                    return BadRequest("CommentIdは正の値でなければなりません。");
                }

                await _commentService.UpdateCommentAsync(id, request.NewContent);
                _logger.LogInformation("コメントが正常に更新されました: CommentId={CommentId}", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "コメント更新中にエラーが発生しました: CommentId={CommentId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "コメントの更新中にエラーが発生しました。");
            }
        }
    }
}
