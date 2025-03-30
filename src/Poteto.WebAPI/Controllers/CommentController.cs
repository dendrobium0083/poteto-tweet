using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Poteto.Application.DTOs;
using Poteto.Application.Interfaces;

namespace Poteto.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        /// <summary>
        /// 新規コメント投稿のエンドポイント
        /// </summary>
        /// <param name="request">コメント投稿用リクエストDTO</param>
        /// <returns>登録されたコメントのDTO</returns>
        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var commentDto = await _commentService.CreateCommentAsync(request.TweetId, request.UserId, request.Content);
            return CreatedAtAction(nameof(GetCommentById), new { id = commentDto.CommentId }, commentDto);
        }

        /// <summary>
        /// コメントID を指定してコメント情報を取得するエンドポイント
        /// </summary>
        /// <param name="id">コメントの一意な識別子</param>
        /// <returns>コメントのDTO</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCommentById(int id)
        {
            var commentDto = await _commentService.GetCommentByIdAsync(id);
            if (commentDto == null)
            {
                return NotFound();
            }
            return Ok(commentDto);
        }

        /// <summary>
        /// 指定したツイートに対する全コメント一覧を取得するエンドポイント
        /// </summary>
        /// <param name="tweetId">対象ツイートのID</param>
        /// <returns>コメントDTOの一覧</returns>
        [HttpGet("tweet/{tweetId}")]
        public async Task<IActionResult> GetCommentsByTweetId(int tweetId)
        {
            var commentDtos = await _commentService.GetCommentsByTweetIdAsync(tweetId);
            return Ok(commentDtos);
        }

        /// <summary>
        /// コメントの内容を更新するエンドポイント
        /// </summary>
        /// <param name="id">更新対象のコメントID</param>
        /// <param name="request">更新内容を含むリクエストDTO</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(int id, [FromBody] UpdateCommentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _commentService.UpdateCommentAsync(id, request.NewContent);
            return NoContent();
        }
    }

    /// <summary>
    /// コメント投稿用のリクエストDTO
    /// </summary>
    public class CreateCommentRequest
    {
        /// <summary>
        /// 対象ツイートのID
        /// </summary>
        public int TweetId { get; set; }

        /// <summary>
        /// コメント投稿者のユーザID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// コメント内容
        /// </summary>
        public string Content { get; set; }
    }

    /// <summary>
    /// コメント更新用のリクエストDTO
    /// </summary>
    public class UpdateCommentRequest
    {
        /// <summary>
        /// 新しいコメント内容
        /// </summary>
        public string NewContent { get; set; }
    }
}
