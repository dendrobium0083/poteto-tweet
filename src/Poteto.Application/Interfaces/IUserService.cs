using System.Threading.Tasks;

using Poteto.Application.DTOs;

namespace Poteto.Application.Interfaces
{
    /// <summary>
    /// ユーザに関するユースケースを定義するインターフェース
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// 新規ユーザの登録を行い、登録されたユーザ情報を返します。
        /// </summary>
        /// <param name="userName">ユーザ名</param>
        /// <param name="email">メールアドレス</param>
        /// <param name="password">平文パスワード</param>
        /// <returns>登録されたユーザ情報の DTO</returns>
        Task<UserDTO> RegisterUserAsync(string userName, string email, string password);

        /// <summary>
        /// ユーザ認証を行い、認証されたユーザ情報を返します。
        /// </summary>
        /// <param name="email">メールアドレス</param>
        /// <param name="password">平文パスワード</param>
        /// <returns>認証されたユーザ情報の DTO。認証に失敗した場合は null など</returns>
        Task<UserDTO> AuthenticateUserAsync(string email, string password);

        /// <summary>
        /// 指定したユーザ ID のユーザ情報を取得します。
        /// </summary>
        /// <param name="userId">ユーザの一意な識別子</param>
        /// <returns>ユーザ情報の DTO</returns>
        Task<UserDTO> GetUserByIdAsync(int userId);
    }
}
