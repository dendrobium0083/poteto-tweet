using System.Data;
using Poteto.Domain.Entities;

namespace Poteto.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        /// <summary>
        /// ユーザを新規登録し、採番された UserId を返します。
        /// </summary>
        Task<int> CreateUserAsync(IDbConnection connection, IDbTransaction transaction, User user);

        /// <summary>
        /// 指定した UserId のユーザを取得します（トランザクション付き）。
        /// </summary>
        Task<User?> GetUserByIdAsync(IDbConnection connection, int userId);

        /// <summary>
        /// メールアドレスでユーザを検索します（読み取り専用）。
        /// </summary>
        Task<User?> GetUserByEmailAsync(IDbConnection connection, string email);
    }
}
