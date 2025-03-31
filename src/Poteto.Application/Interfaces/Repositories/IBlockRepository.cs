using System.Collections.Generic;
using System.Threading.Tasks;

using Poteto.Domain.Entities;

namespace Poteto.Application.Interfaces.Repositories
{
    /// <summary>
    /// Block に対する DB 操作を抽象化するリポジトリインターフェース
    /// </summary>
    public interface IBlockRepository
    {
        /// <summary>
        /// 指定されたブロック関係（BlockerId × BlockedId）を取得します。
        /// </summary>
        Task<Block?> GetBlockAsync(int blockerId, int blockedId);

        /// <summary>
        /// 指定ユーザ（Blocker）が行ったブロックの一覧を取得します。
        /// </summary>
        Task<IEnumerable<Block>> GetBlocksByBlockerIdAsync(int blockerId);

        /// <summary>
        /// 新規ブロックを登録し、自動採番された BlockId を返します。
        /// </summary>
        Task<int> CreateBlockAsync(Block block);

        /// <summary>
        /// 指定されたブロック関係を削除します。
        /// </summary>
        Task DeleteBlockAsync(int blockerId, int blockedId);
    }
}
