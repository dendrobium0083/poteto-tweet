using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Poteto.Application.DTOs;
using Poteto.Application.Interfaces;

namespace Poteto.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LikeController : ControllerBase
    {
        private readonly ILikeService _likeService;

        public LikeController(ILikeService likeService)
        {
            _likeService = likeService;
        }

        /// <summary>
        /// 新規いいね登録のエンドポイント
        /// </summary>
        /// <param name="request">いいね登録用のリクエストDTO</param>
        /// <returns>登録されたいいね情報のDTO</returns>
        [HttpPost]
        public async Task<IActionResult> CreateLike([FromBody] CreateLikeRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var likeDto = await _likeService.CreateLikeAsync(request.TweetId, request.UserId);
            return CreatedAtAction(nameof(GetLikesByTweetId), new { tweetId = likeDto.TweetId }, likeDto);
        }

        /// <summary>
        /// 指定のツイートに対するいいね一覧を取得するエンドポイント
        /// </summary>
        /// <param name="tweetId">対象ツイートのID</param>
        /// <returns>いいね情報のDTO一覧</returns>
        [HttpGet("tweet/{tweetId}")]
        public async Task<IActionResult> GetLikesByTweetId(int tweetId)
        {
            var likes = await _likeService.GetLikesByTweetIdAsync(tweetId);
            return Ok(likes);
        }

        /// <summary>
        /// 指定されたいいね関係を解除するエンドポイント
        /// </summary>
        /// <param name="tweetId">対象ツイートのID</param>
        /// <param name="userId">いいね解除するユーザのID</param>
        [HttpDelete("{tweetId}/{userId}")]
        public async Task<IActionResult> DeleteLike(int tweetId, int userId)
        {
            await _likeService.DeleteLikeAsync(tweetId, userId);
            return NoContent();
        }
    }

    /// <summary>
    /// いいね登録用のリクエストDTO
    /// </summary>
    public class CreateLikeRequest
    {
        /// <summary>
        /// 対象のツイートID
        /// </summary>
        public int TweetId { get; set; }

        /// <summary>
        /// いいねを行ったユーザのID
        /// </summary>
        public int UserId { get; set; }
    }
}
