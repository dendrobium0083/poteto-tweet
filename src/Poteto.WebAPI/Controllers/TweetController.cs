using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Poteto.Application.DTOs;
using Poteto.Application.Interfaces.Services;

namespace Poteto.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TweetController : ControllerBase
    {
        private readonly ITweetService _tweetService;

        public TweetController(ITweetService tweetService)
        {
            _tweetService = tweetService;
        }

        /// <summary>
        /// 新規ツイート投稿のエンドポイント
        /// </summary>
        /// <param name="request">ツイート投稿用リクエストDTO</param>
        /// <returns>登録されたツイートのDTO</returns>
        [HttpPost]
        public async Task<IActionResult> CreateTweet([FromBody] CreateTweetRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tweetDto = await _tweetService.CreateTweetAsync(request.UserId, request.Content);
            return CreatedAtAction(nameof(GetTweetById), new { id = tweetDto.TweetId }, tweetDto);
        }

        /// <summary>
        /// ツイートID を指定してツイート情報を取得するエンドポイント
        /// </summary>
        /// <param name="id">ツイートの一意な識別子</param>
        /// <returns>ツイートのDTO</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTweetById(int id)
        {
            var tweetDto = await _tweetService.GetTweetByIdAsync(id);
            if (tweetDto == null)
                return NotFound();
            return Ok(tweetDto);
        }

        /// <summary>
        /// 指定したユーザの全ツイート一覧を取得するエンドポイント
        /// </summary>
        /// <param name="userId">ユーザの一意な識別子</param>
        /// <returns>ツイートのDTO一覧</returns>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetTweetsByUserId(int userId)
        {
            var tweets = await _tweetService.GetTweetsByUserIdAsync(userId);
            return Ok(tweets);
        }

        /// <summary>
        /// ツイートの内容を更新するエンドポイント
        /// </summary>
        /// <param name="id">更新対象のツイートID</param>
        /// <param name="request">更新内容を含むリクエストDTO</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTweet(int id, [FromBody] UpdateTweetRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _tweetService.UpdateTweetAsync(id, request.NewContent);
            return NoContent();
        }
    }

    /// <summary>
    /// ツイート投稿用のリクエストDTO
    /// </summary>
    public class CreateTweetRequest
    {
        /// <summary>
        /// ツイート投稿者のユーザID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// ツイートの内容
        /// </summary>
        public required string Content { get; set; }
    }

    /// <summary>
    /// ツイート更新用のリクエストDTO
    /// </summary>
    public class UpdateTweetRequest
    {
        /// <summary>
        /// 新しいツイート内容
        /// </summary>
        public required string NewContent { get; set; }
    }
}
