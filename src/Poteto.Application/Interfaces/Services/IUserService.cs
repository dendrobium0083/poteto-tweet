using System.Data;
using System.Threading.Tasks;
using Poteto.Application.DTOs;

namespace Poteto.Application.Interfaces.Services
{
    public interface IUserService
    {
        /// <summary>
        /// 新規ユーザを登録し、登録されたユーザ情報を返します。
        /// </summary>
        Task<UserDTO> RegisterUserAsync(string userName, string email, string password, IDbConnection connection);

        /// <summary>
        /// ユーザ認証を行い、成功時にユーザ情報を返します。失敗時は null。
        /// </summary>
        Task<UserDTO?> AuthenticateUserAsync(string email, string password, IDbConnection connection);

        /// <summary>
        /// ユーザIDに基づきユーザ情報を取得します。
        /// </summary>
        Task<UserDTO> GetUserByIdAsync(int userId, IDbConnection connection);
    }
}
