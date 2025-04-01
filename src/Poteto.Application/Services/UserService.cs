using System;
using System.Data;
using System.Threading.Tasks;

using Poteto.Application.DTOs;
using Poteto.Application.Interfaces.Repositories;
using Poteto.Application.Interfaces.Services;
using Poteto.Domain.Entities;
using Poteto.Domain.ValueObjects;

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
        public async Task<UserDTO> RegisterUserAsync(string userName, string email, string password, IDbConnection connection)
        {
            using var transaction = connection.BeginTransaction();
            try
            {
                var emailObj = new Email(email);
                var passwordObj = new Password(password);
                var user = new User(userName, emailObj.Value, passwordObj.HashedValue);

                int newUserId = await _userRepository.CreateUserAsync(connection, transaction, user);
                var createdUser = await _userRepository.GetUserByIdAsync(connection, transaction, newUserId);

                if (createdUser == null)
                {
                    transaction.Rollback();
                    throw new InvalidOperationException("ユーザの登録に失敗しました。");
                }

                transaction.Commit();

                return new UserDTO
                {
                    UserId = createdUser.UserId,
                    UserName = createdUser.UserName,
                    Email = createdUser.Email,
                    CreatedAt = createdUser.CreatedAt
                };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        /// <summary>
        /// ユーザ認証を行い、認証されたユーザ情報の DTO を返します。
        /// 認証に失敗した場合は null を返します。
        /// </summary>
        public async Task<UserDTO?> AuthenticateUserAsync(string email, string password, IDbConnection connection)
        {
            var user = await _userRepository.GetUserByEmailAsync(connection, email);
            if (user == null)
            {
                return null;
            }

            var passwordObj = new Password(password);
            if (!string.Equals(user.PasswordHash, passwordObj.HashedValue, StringComparison.Ordinal))
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

        /// <summary>
        /// 指定したユーザID のユーザ情報を取得し、DTO に変換して返します。
        /// </summary>
        public async Task<UserDTO> GetUserByIdAsync(int userId, IDbConnection connection)
        {
            var user = await _userRepository.GetUserByIdAsync(connection, transaction: null, userId);
            if (user == null)
            {
                throw new InvalidOperationException("ユーザ情報が見つかりません。");
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
