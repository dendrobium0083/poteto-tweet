using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Poteto.Application.Interfaces.Repositories;
using Poteto.Domain.Entities;
using Poteto.Infrastructure.Configurations;

namespace Poteto.Infrastructure.Data
{
    /// <summary>
    /// ユーザーリポジトリ
    /// </summary>
    public class UserRepository : BaseRepository, IUserRepository
    {
        /// <summary>
        /// UserRepositoryの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="connectionFactory">データベース接続ファクトリ</param>
        /// <param name="logger">ロガー</param>
        public UserRepository(IDbConnectionFactory connectionFactory, ILogger<UserRepository> logger)
            : base(connectionFactory, logger)
        {
        }

        /// <summary>
        /// ユーザーを取得します
        /// </summary>
        /// <param name="id">ユーザーID</param>
        /// <returns>ユーザー</returns>
        public async Task<User?> GetByIdAsync(int id)
        {
            _logger.LogInformation("ユーザー取得開始: ID={Id}", id);

            return await ExecuteInTransactionAsync<User?>(async (connection, transaction) =>
            {
                const string sql = @"
                    SELECT id, username, email, password_hash, created_at, updated_at
                    FROM users
                    WHERE id = :id";

                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = sql;
                command.AddParameter("id", id);

                using var reader = await ((DbCommand)command).ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    return null;
                }

                return new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Email = reader.GetString(2),
                    PasswordHash = reader.GetString(3),
                    CreatedAt = reader.GetDateTime(4),
                    UpdatedAt = reader.GetDateTime(5)
                };
            });
        }

        /// <summary>
        /// メールアドレスでユーザーを取得します
        /// </summary>
        /// <param name="email">メールアドレス</param>
        /// <returns>ユーザー</returns>
        public async Task<User?> GetByEmailAsync(string email)
        {
            _logger.LogInformation("ユーザー取得開始: Email={Email}", email);

            return await ExecuteInTransactionAsync<User?>(async (connection, transaction) =>
            {
                const string sql = @"
                    SELECT id, username, email, password_hash, created_at, updated_at
                    FROM users
                    WHERE email = :email";

                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = sql;
                command.AddParameter("email", email);

                using var reader = await ((DbCommand)command).ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    return null;
                }

                return new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Email = reader.GetString(2),
                    PasswordHash = reader.GetString(3),
                    CreatedAt = reader.GetDateTime(4),
                    UpdatedAt = reader.GetDateTime(5)
                };
            });
        }

        /// <summary>
        /// ユーザーを作成します
        /// </summary>
        /// <param name="user">ユーザー</param>
        /// <returns>作成されたユーザー</returns>
        public async Task<User> CreateAsync(User user)
        {
            _logger.LogInformation("ユーザー作成開始: Username={Username}, Email={Email}",
                user.Username, user.Email);

            return await ExecuteInTransactionAsync<User>(async (connection, transaction) =>
            {
                const string sql = @"
                    INSERT INTO users (username, email, password_hash, created_at, updated_at)
                    VALUES (:username, :email, :password_hash, :created_at, :updated_at)
                    RETURNING id INTO :id";

                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = sql;

                var idParameter = command.CreateParameter();
                idParameter.ParameterName = "id";
                idParameter.DbType = DbType.Int32;
                idParameter.Direction = ParameterDirection.Output;
                command.Parameters.Add(idParameter);

                command.AddParameter("username", user.Username);
                command.AddParameter("email", user.Email);
                command.AddParameter("password_hash", user.PasswordHash);
                command.AddParameter("created_at", user.CreatedAt);
                command.AddParameter("updated_at", user.UpdatedAt);

                await ((DbCommand)command).ExecuteNonQueryAsync();

                user.Id = (int)idParameter.Value;
                return user;
            });
        }

        /// <summary>
        /// ユーザーを更新します
        /// </summary>
        /// <param name="user">ユーザー</param>
        /// <returns>更新されたユーザー</returns>
        public async Task<User> UpdateAsync(User user)
        {
            _logger.LogInformation("ユーザー更新開始: ID={Id}, Username={Username}, Email={Email}",
                user.Id, user.Username, user.Email);

            return await ExecuteInTransactionAsync<User>(async (connection, transaction) =>
            {
                const string sql = @"
                    UPDATE users
                    SET username = :username,
                        email = :email,
                        password_hash = :password_hash,
                        updated_at = :updated_at
                    WHERE id = :id";

                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = sql;

                command.AddParameter("id", user.Id);
                command.AddParameter("username", user.Username);
                command.AddParameter("email", user.Email);
                command.AddParameter("password_hash", user.PasswordHash);
                command.AddParameter("updated_at", user.UpdatedAt);

                await ((DbCommand)command).ExecuteNonQueryAsync();
                return user;
            });
        }

        /// <summary>
        /// ユーザーを削除します
        /// </summary>
        /// <param name="id">ユーザーID</param>
        /// <returns>削除が成功したかどうか</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("ユーザー削除開始: ID={Id}", id);

            return await ExecuteInTransactionAsync<bool>(async (connection, transaction) =>
            {
                const string sql = "DELETE FROM users WHERE id = :id";

                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = sql;
                command.AddParameter("id", id);

                var result = await ((DbCommand)command).ExecuteNonQueryAsync();
                return result > 0;
            });
        }
    }
}
