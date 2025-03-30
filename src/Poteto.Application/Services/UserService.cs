using System;
using System.Threading.Tasks;

using Poteto.Application.DTOs;
using Poteto.Application.Interfaces;
using Poteto.Domain.Entities;
using Poteto.Domain.ValueObjects;
using Poteto.Infrastructure.Data;

namespace Poteto.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        /// <summary>
        /// 新規ユーザの登録を行い、登録されたユーザ情報の DTO を返します。
        /// </summary>
        public async Task<UserDTO> RegisterUserAsync(string userName, string email, string password)
        {
            // 入力値の検証と値オブジェクトの生成
            var emailObj = new Email(email);
            var passwordObj = new Password(password);

            // ドメインエンティティの生成（Password はハッシュ化済みの値を利用）
            var user = new User(userName, emailObj.Value, passwordObj.HashedValue);

            // リポジトリを通してユーザ登録（自動採番された UserId を取得）
            int newUserId = await _userRepository.CreateUserAsync(user);

            // 登録されたユーザ情報の取得
            var createdUser = await _userRepository.GetUserByIdAsync(newUserId);

            // DTO への変換
            return new UserDTO
            {
                UserId = createdUser.UserId,
                UserName = createdUser.UserName,
                Email = createdUser.Email,
                CreatedAt = createdUser.CreatedAt
            };
        }

        /// <summary>
        /// ユーザ認証を行い、認証されたユーザ情報の DTO を返します。
        /// 認証に失敗した場合は null を返します。
        /// </summary>
        public async Task<UserDTO> AuthenticateUserAsync(string email, string password)
        {
            // メールアドレスでユーザ取得
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                return null;
            }

            // 入力されたパスワードをハッシュ化し、保存されたハッシュ値と比較
            var passwordObj = new Password(password);
            if (!string.Equals(user.PasswordHash, passwordObj.HashedValue, StringComparison.Ordinal))
            {
                return null;
            }

            // 認証成功時は DTO に変換して返却
            return new UserDTO
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            };
        }

        /// <summary>
        /// 指定したユーザID のユーザ情報を取得し、DTO に変換して返します。
        /// </summary>
        public async Task<UserDTO> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            return new UserDTO
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
