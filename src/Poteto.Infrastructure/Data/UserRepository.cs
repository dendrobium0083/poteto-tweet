using System;
using System.Data;
using System.Threading.Tasks;

using Dapper;

using Poteto.Domain.Entities;
using Poteto.Application.Interfaces.Repositories;

namespace Poteto.Infrastructure.Data
{

    // Dapper を用いたユーザリポジトリの実装
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        // IUnitOfWork から接続およびトランザクションを取得
        public UserRepository(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            // トランザクションから接続を取得
            _connection = unitOfWork.Transaction.Connection ?? throw new InvalidOperationException("Connection is null");
            _transaction = unitOfWork.Transaction ?? throw new InvalidOperationException("Transaction is null");
        }

        // ユーザIDでユーザ情報を取得する
        public async Task<User?> GetUserByIdAsync(int userId)
        {
            string sql = "SELECT * FROM Users WHERE UserId = :UserId";
            return await _connection.QueryFirstOrDefaultAsync<User>(
                sql,
                new { UserId = userId },
                _transaction
            );
        }

        // メールアドレスでユーザ情報を取得する
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            string sql = "SELECT * FROM Users WHERE Email = :Email";
            return await _connection.QueryFirstOrDefaultAsync<User>(
                sql,
                new { Email = email },
                _transaction
            );
        }

        // 新規ユーザを登録し、生成された UserId を返す
        public async Task<int> CreateUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            // Oracle では INSERT 文に RETURNING 句を利用して ID を取得可能
            string sql = @"
                INSERT INTO Users (UserName, Email, PasswordHash, CreatedAt)
                VALUES (:UserName, :Email, :PasswordHash, :CreatedAt)
                RETURNING UserId INTO :UserId";

            var parameters = new DynamicParameters();
            parameters.Add("UserName", user.UserName, DbType.String, size: 50);
            parameters.Add("Email", user.Email, DbType.String, size: 100);
            parameters.Add("PasswordHash", user.PasswordHash, DbType.String, size: 256);
            parameters.Add("CreatedAt", user.CreatedAt, DbType.DateTime);
            parameters.Add("UserId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await _connection.ExecuteAsync(sql, parameters, _transaction);

            return parameters.Get<int>("UserId");
        }
    }
}
