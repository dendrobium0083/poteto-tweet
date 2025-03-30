using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Poteto.Application.DTOs;
using Poteto.Application.Interfaces;

namespace Poteto.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlockController : ControllerBase
    {
        private readonly IBlockService _blockService;

        public BlockController(IBlockService blockService)
        {
            _blockService = blockService;
        }

        /// <summary>
        /// 新規ブロック登録のエンドポイント
        /// </summary>
        /// <param name="request">ブロック登録用のリクエストDTO</param>
        /// <returns>登録されたブロック情報のDTO</returns>
        [HttpPost]
        public async Task<IActionResult> CreateBlock([FromBody] CreateBlockRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var blockDto = await _blockService.CreateBlockAsync(request.BlockerId, request.BlockedId);
            return CreatedAtAction(nameof(GetBlocksByBlockerId), new { blockerId = blockDto.BlockerId }, blockDto);
        }

        /// <summary>
        /// 指定したユーザが行った全ブロック一覧を取得するエンドポイント
        /// </summary>
        /// <param name="blockerId">ブロックを行ったユーザのID</param>
        /// <returns>ブロック情報のDTO一覧</returns>
        [HttpGet("blocker/{blockerId}")]
        public async Task<IActionResult> GetBlocksByBlockerId(int blockerId)
        {
            var blocks = await _blockService.GetBlocksByBlockerIdAsync(blockerId);
            return Ok(blocks);
        }

        /// <summary>
        /// 指定されたブロック関係を解除するエンドポイント
        /// </summary>
        /// <param name="blockerId">ブロックを行うユーザのID</param>
        /// <param name="blockedId">ブロック解除する対象ユーザのID</param>
        [HttpDelete("{blockerId}/{blockedId}")]
        public async Task<IActionResult> DeleteBlock(int blockerId, int blockedId)
        {
            await _blockService.DeleteBlockAsync(blockerId, blockedId);
            return NoContent();
        }
    }

    /// <summary>
    /// ブロック登録用のリクエストDTO
    /// </summary>
    public class CreateBlockRequest
    {
        /// <summary>
        /// ブロックを行うユーザのID
        /// </summary>
        public int BlockerId { get; set; }

        /// <summary>
        /// ブロック対象のユーザのID
        /// </summary>
        public int BlockedId { get; set; }
    }
}
