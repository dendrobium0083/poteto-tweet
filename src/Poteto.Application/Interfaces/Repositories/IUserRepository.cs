using System.Collections.Generic;
using System.Threading.Tasks;
using Poteto.Domain.Entities;

namespace Poteto.Application.Interfaces.Repositories
{
    // ユーザに対する DB 操作を抽象化するインターフェース
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(int userId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<int> CreateUserAsync(User user);
        // 必要に応じて、Update や Delete なども定義
    }
}
