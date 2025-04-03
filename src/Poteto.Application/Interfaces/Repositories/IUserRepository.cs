using System.Threading.Tasks;

using Poteto.Domain.Entities;

namespace Poteto.Application.Interfaces.Repositories
{
    /// <summary>
    /// ユーザーリポジトリのインターフェース
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// ユーザーを取得します
        /// </summary>
        /// <param name="id">ユーザーID</param>
        /// <returns>ユーザー</returns>
        Task<User?> GetByIdAsync(int id);

        /// <summary>
        /// ユーザーを作成します
        /// </summary>
        /// <param name="user">ユーザー</param>
        /// <returns>作成されたユーザー</returns>
        Task<User> CreateAsync(User user);

        /// <summary>
        /// ユーザーを更新します
        /// </summary>
        /// <param name="user">ユーザー</param>
        /// <returns>更新されたユーザー</returns>
        Task<User> UpdateAsync(User user);

        /// <summary>
        /// ユーザーを削除します
        /// </summary>
        /// <param name="id">ユーザーID</param>
        /// <returns>削除が成功したかどうか</returns>
        Task<bool> DeleteAsync(int id);
    }
}
