using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

using Dapper;

using Poteto.Application.Interfaces.Repositories;
using Poteto.Domain.Entities;

namespace Poteto.Infrastructure.Data
{

    // Dapper を利用した FollowRepository の実装
    public class FollowRepository : IFollowRepository
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        // IUnitOfWork から接続およびトランザクションを取得
        public FollowRepository(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            _connection = unitOfWork.Transaction.Connection ?? throw new InvalidOperationException("Connection is null");
            _transaction = unitOfWork.Transaction ?? throw new InvalidOperationException("Transaction is null");
        }

        // 指定のフォロワーとフォロー対象の関係を取得
        public async Task<Follow?> GetFollowAsync(int followerId, int followeeId)
        {
            string sql = "SELECT * FROM Follows WHERE FollowerId = :FollowerId AND FolloweeId = :FolloweeId";
            return await _connection.QueryFirstOrDefaultAsync<Follow>(
                sql,
                new { FollowerId = followerId, FolloweeId = followeeId },
                _transaction
            );
        }

        // 特定のユーザ（Followee）のフォロワー一覧を取得
        public async Task<IEnumerable<Follow>> GetFollowersAsync(int followeeId)
        {
            string sql = "SELECT * FROM Follows WHERE FolloweeId = :FolloweeId ORDER BY CreatedAt DESC";
            return await _connection.QueryAsync<Follow>(
                sql,
                new { FolloweeId = followeeId },
                _transaction
            );
        }

        // 特定のユーザ（Follower）がフォローしているユーザ一覧を取得
        public async Task<IEnumerable<Follow>> GetFolloweesAsync(int followerId)
        {
            string sql = "SELECT * FROM Follows WHERE FollowerId = :FollowerId ORDER BY CreatedAt DESC";
            return await _connection.QueryAsync<Follow>(
                sql,
                new { FollowerId = followerId },
                _transaction
            );
        }

        // 新規フォローを登録し、生成された FollowId を返す
        public async Task<int> CreateFollowAsync(Follow follow)
        {
            if (follow == null)
                throw new ArgumentNullException(nameof(follow));

            // Oracle の RETURNING 句を利用して、自動採番された FollowId を取得
            string sql = @"
                INSERT INTO Follows (FollowerId, FolloweeId, CreatedAt)
                VALUES (:FollowerId, :FolloweeId, :CreatedAt)
                RETURNING FollowId INTO :FollowId";

            var parameters = new DynamicParameters();
            parameters.Add("FollowerId", follow.FollowerId, DbType.Int32);
            parameters.Add("FolloweeId", follow.FolloweeId, DbType.Int32);
            parameters.Add("CreatedAt", follow.CreatedAt, DbType.DateTime);
            parameters.Add("FollowId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await _connection.ExecuteAsync(sql, parameters, _transaction);
            return parameters.Get<int>("FollowId");
        }

        // 指定のフォロー関係を削除
        public async Task DeleteFollowAsync(int followerId, int followeeId)
        {
            string sql = "DELETE FROM Follows WHERE FollowerId = :FollowerId AND FolloweeId = :FolloweeId";
            await _connection.ExecuteAsync(
                sql,
                new { FollowerId = followerId, FolloweeId = followeeId },
                _transaction
            );
        }
    }
}
