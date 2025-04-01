using System.Data;
using Dapper;
using Poteto.Application.Interfaces.Repositories;
using Poteto.Domain.Entities;

namespace Poteto.Infrastructure.Data
{
    public class UserRepository : IUserRepository
    {
        public async Task<int> CreateUserAsync(IDbConnection connection, IDbTransaction transaction, User user)
        {
            var sql = @"
                INSERT INTO Users (UserName, Email, PasswordHash, CreatedAt)
                VALUES (:UserName, :Email, :PasswordHash, :CreatedAt)
                RETURNING UserId INTO :UserId";

            var parameters = new DynamicParameters();
            parameters.Add(":UserName", user.UserName);
            parameters.Add(":Email", user.Email);
            parameters.Add(":PasswordHash", user.PasswordHash);
            parameters.Add(":CreatedAt", user.CreatedAt);
            parameters.Add(":UserId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(sql, parameters, transaction);
            return parameters.Get<int>(":UserId");
        }

        public async Task<User?> GetUserByIdAsync(IDbConnection connection, int userId)
        {
            var sql = @"
                SELECT UserId, UserName, Email, PasswordHash, CreatedAt
                FROM Users
                WHERE UserId = :UserId";

            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { UserId = userId });
        }

        public async Task<User?> GetUserByEmailAsync(IDbConnection connection, string email)
        {
            var sql = @"
                SELECT UserId, UserName, Email, PasswordHash, CreatedAt
                FROM Users
                WHERE Email = :Email";

            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
        }
    }
}
