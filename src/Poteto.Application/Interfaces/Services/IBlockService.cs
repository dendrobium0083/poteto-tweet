using System.Collections.Generic;
using System.Threading.Tasks;

using Poteto.Application.DTOs;

namespace Poteto.Application.Interfaces.Services
{
    /// <summary>
    /// ユーザ間のブロック操作に関するユースケースを定義するインターフェース
    /// </summary>
    public interface IBlockService
    {
        /// <summary>
        /// 新規ブロックを登録し、登録されたブロック情報の DTO を返します。
        /// </summary>
        /// <param name="blockerId">ブロックを行うユーザのID</param>
        /// <param name="blockedId">ブロック対象のユーザのID</param>
        /// <returns>登録されたブロック情報の DTO</returns>
        Task<BlockDTO> CreateBlockAsync(int blockerId, int blockedId);

        /// <summary>
        /// 指定したユーザ（ブロックを行ったユーザ）が行った全てのブロック一覧を取得します。
        /// </summary>
        /// <param name="blockerId">ブロックを行ったユーザのID</param>
        /// <returns>ブロック情報の DTO の一覧</returns>
        Task<IEnumerable<BlockDTO>> GetBlocksByBlockerIdAsync(int blockerId);

        /// <summary>
        /// 指定されたブロック関係を解除します。
        /// </summary>
        /// <param name="blockerId">ブロックを行うユーザのID</param>
        /// <param name="blockedId">ブロック解除する対象ユーザのID</param>
        Task DeleteBlockAsync(int blockerId, int blockedId);
    }
}
