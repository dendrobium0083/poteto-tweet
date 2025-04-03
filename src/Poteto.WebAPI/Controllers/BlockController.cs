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
    public class BlockController : ControllerBase
    {
        private readonly IBlockService _blockService;
        private readonly ILogger<BlockController> _logger;

        public BlockController(IBlockService blockService, ILogger<BlockController> logger)
        {
            _blockService = blockService ?? throw new ArgumentNullException(nameof(blockService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 新規ブロック登録のエンドポイント
        /// </summary>
        /// <param name="request">ブロック登録用のリクエストDTO</param>
        /// <returns>登録されたブロック情報のDTO</returns>
        /// <response code="201">ブロックが正常に作成された場合</response>
        /// <response code="400">リクエストが無効な場合</response>
        /// <response code="500">サーバーエラーが発生した場合</response>
        [HttpPost]
        [ProducesResponseType(typeof(BlockDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateBlock([FromBody] CreateBlockRequest request)
        {
            try
            {
                _logger.LogInformation("ブロック作成リクエストを受信: BlockerId={BlockerId}, BlockedId={BlockedId}",
                    request.BlockerId, request.BlockedId);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("無効なリクエスト: {ModelState}", ModelState);
                    return BadRequest(ModelState);
                }

                var blockDto = await _blockService.CreateBlockAsync(request.BlockerId, request.BlockedId);
                _logger.LogInformation("ブロックが正常に作成されました: BlockId={BlockId}", blockDto.BlockId);

                return CreatedAtAction(nameof(GetBlocksByBlockerId),
                    new { blockerId = blockDto.BlockerId },
                    blockDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ブロック作成中にエラーが発生しました: BlockerId={BlockerId}, BlockedId={BlockedId}",
                    request.BlockerId, request.BlockedId);
                return StatusCode(StatusCodes.Status500InternalServerError, "ブロックの作成中にエラーが発生しました。");
            }
        }

        /// <summary>
        /// 指定したユーザが行った全ブロック一覧を取得するエンドポイント
        /// </summary>
        /// <param name="blockerId">ブロックを行ったユーザのID</param>
        /// <returns>ブロック情報のDTO一覧</returns>
        /// <response code="200">ブロック一覧が正常に取得された場合</response>
        /// <response code="400">リクエストが無効な場合</response>
        /// <response code="500">サーバーエラーが発生した場合</response>
        [HttpGet("blocker/{blockerId}")]
        [ProducesResponseType(typeof(IEnumerable<BlockDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBlocksByBlockerId(int blockerId)
        {
            try
            {
                _logger.LogInformation("ブロック一覧取得リクエストを受信: BlockerId={BlockerId}", blockerId);

                if (blockerId <= 0)
                {
                    _logger.LogWarning("無効なBlockerId: {BlockerId}", blockerId);
                    return BadRequest("BlockerIdは正の値でなければなりません。");
                }

                var blocks = await _blockService.GetBlocksByBlockerIdAsync(blockerId);
                _logger.LogInformation("ブロック一覧を正常に取得しました: 件数={Count}", blocks.Count());

                return Ok(blocks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ブロック一覧取得中にエラーが発生しました: BlockerId={BlockerId}", blockerId);
                return StatusCode(StatusCodes.Status500InternalServerError, "ブロック一覧の取得中にエラーが発生しました。");
            }
        }

        /// <summary>
        /// 指定されたブロック関係を解除するエンドポイント
        /// </summary>
        /// <param name="blockerId">ブロックを行うユーザのID</param>
        /// <param name="blockedId">ブロック解除する対象ユーザのID</param>
        /// <response code="204">ブロックが正常に解除された場合</response>
        /// <response code="400">リクエストが無効な場合</response>
        /// <response code="500">サーバーエラーが発生した場合</response>
        [HttpDelete("{blockerId}/{blockedId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteBlock(int blockerId, int blockedId)
        {
            try
            {
                _logger.LogInformation("ブロック解除リクエストを受信: BlockerId={BlockerId}, BlockedId={BlockedId}",
                    blockerId, blockedId);

                if (blockerId <= 0 || blockedId <= 0)
                {
                    _logger.LogWarning("無効なID: BlockerId={BlockerId}, BlockedId={BlockedId}",
                        blockerId, blockedId);
                    return BadRequest("IDは正の値でなければなりません。");
                }

                await _blockService.DeleteBlockAsync(blockerId, blockedId);
                _logger.LogInformation("ブロックが正常に解除されました: BlockerId={BlockerId}, BlockedId={BlockedId}",
                    blockerId, blockedId);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ブロック解除中にエラーが発生しました: BlockerId={BlockerId}, BlockedId={BlockedId}",
                    blockerId, blockedId);
                return StatusCode(StatusCodes.Status500InternalServerError, "ブロックの解除中にエラーが発生しました。");
            }
        }
    }
}
