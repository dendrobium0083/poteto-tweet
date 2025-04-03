using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Poteto.Application.DTOs;
using Poteto.Application.Interfaces.Services;
using Poteto.Application.Requests;
using Poteto.WebAPI.Requests;

namespace Poteto.WebAPI.Controllers
{
    /// <summary>
    /// ブロック機能を提供するコントローラー
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BlockController : BaseController<BlockController>
    {
        private readonly IBlockService _blockService;

        /// <summary>
        /// BlockControllerの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="blockService">ブロックサービス</param>
        /// <param name="logger">ロガー</param>
        /// <exception cref="ArgumentNullException">blockServiceまたはloggerがnullの場合</exception>
        public BlockController(IBlockService blockService, ILogger<BlockController> logger)
            : base(logger)
        {
            _blockService = blockService ?? throw new ArgumentNullException(nameof(blockService));
        }

        /// <summary>
        /// 新しいブロックを作成します
        /// </summary>
        /// <param name="request">ブロック作成リクエスト</param>
        /// <returns>作成されたブロック</returns>
        /// <response code="200">ブロックの作成に成功</response>
        /// <response code="400">リクエストが無効</response>
        /// <response code="500">サーバーエラー</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BlockDTO>> CreateBlock([FromBody] CreateBlockRequest request)
        {
            try
            {
                _logger.LogInformation("ブロック作成リクエスト開始: BlockerId={BlockerId}, BlockedId={BlockedId}",
                    request.BlockerId, request.BlockedId);

                if (request.BlockerId <= 0 || request.BlockedId <= 0)
                {
                    _logger.LogWarning("無効なIDが指定されました: BlockerId={BlockerId}, BlockedId={BlockedId}",
                        request.BlockerId, request.BlockedId);
                    return BadRequest(new { Message = "IDは正の値である必要があります" });
                }

                var result = await _blockService.CreateBlockAsync(request.BlockerId, request.BlockedId);
                _logger.LogInformation("ブロック作成成功: BlockerId={BlockerId}, BlockedId={BlockedId}",
                    request.BlockerId, request.BlockedId);

                return SuccessResponse(result);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex, "ブロックの作成中にエラーが発生しました");
            }
        }

        /// <summary>
        /// 指定されたユーザーがブロックしているユーザー一覧を取得します
        /// </summary>
        /// <param name="blockerId">ブロックしているユーザーのID</param>
        /// <returns>ブロックしているユーザー一覧</returns>
        /// <response code="200">ブロック一覧の取得に成功</response>
        /// <response code="400">リクエストが無効</response>
        /// <response code="404">ユーザーが見つからない</response>
        /// <response code="500">サーバーエラー</response>
        [HttpGet("{blockerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<BlockDTO>>> GetBlocksByBlockerId(int blockerId)
        {
            try
            {
                _logger.LogInformation("ブロック一覧取得リクエスト開始: BlockerId={BlockerId}", blockerId);

                if (blockerId <= 0)
                {
                    _logger.LogWarning("無効なIDが指定されました: BlockerId={BlockerId}", blockerId);
                    return BadRequest(new { Message = "IDは正の値である必要があります" });
                }

                var blocks = await _blockService.GetBlocksByBlockerIdAsync(blockerId);
                if (blocks == null || !blocks.Any())
                {
                    return NotFoundResponse("ブロック", blockerId);
                }

                _logger.LogInformation("ブロック一覧取得成功: BlockerId={BlockerId}, Count={Count}",
                    blockerId, blocks.Count());

                return SuccessResponse(blocks);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex, "ブロック一覧の取得中にエラーが発生しました");
            }
        }

        /// <summary>
        /// 指定されたブロックを削除します
        /// </summary>
        /// <param name="blockerId">ブロックしているユーザーのID</param>
        /// <param name="blockedId">ブロックされているユーザーのID</param>
        /// <returns>削除結果</returns>
        /// <response code="200">ブロックの削除に成功</response>
        /// <response code="400">リクエストが無効</response>
        /// <response code="404">ブロックが見つからない</response>
        /// <response code="500">サーバーエラー</response>
        [HttpDelete("{blockerId}/{blockedId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteBlock(int blockerId, int blockedId)
        {
            try
            {
                _logger.LogInformation("ブロック削除リクエスト開始: BlockerId={BlockerId}, BlockedId={BlockedId}",
                    blockerId, blockedId);

                if (blockerId <= 0 || blockedId <= 0)
                {
                    _logger.LogWarning("無効なIDが指定されました: BlockerId={BlockerId}, BlockedId={BlockedId}",
                        blockerId, blockedId);
                    return BadRequest(new { Message = "IDは正の値である必要があります" });
                }

                await _blockService.DeleteBlockAsync(blockerId, blockedId);
                _logger.LogInformation("ブロック削除成功: BlockerId={BlockerId}, BlockedId={BlockedId}",
                    blockerId, blockedId);

                return SuccessResponse();
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex, "ブロックの削除中にエラーが発生しました");
            }
        }
    }
}
