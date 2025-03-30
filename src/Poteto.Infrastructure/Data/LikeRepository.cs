using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

using Dapper;

using Poteto.Domain.Entities;

namespace Poteto.Infrastructure.Data
{
    // Like に対する DB 操作を抽象化するインターフェース
    public interface ILikeRepository
    {
        Task<Like?> GetLikeAsync(int tweetId, int userId);
        Task<IEnumerable<Like>> GetLikesByTweetIdAsync(int tweetId);
        Task<int> CreateLikeAsync(Like like);
        Task DeleteLikeAsync(int tweetId, int userId);
    }

    // Dapper を用いた LikeRepository の実装
    public class LikeRepository : ILikeRepository
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        // IUnitOfWork から接続およびトランザクションを取得
        public LikeRepository(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            _connection = unitOfWork.Transaction.Connection ?? throw new InvalidOperationException("Connection is null");
            _transaction = unitOfWork.Transaction ?? throw new InvalidOperationException("Transaction is null");
        }

        // 指定のツイートとユーザの Like 関係を取得
        public async Task<Like?> GetLikeAsync(int tweetId, int userId)
        {
            string sql = "SELECT * FROM Likes WHERE TweetId = :TweetId AND UserId = :UserId";
            return await _connection.QueryFirstOrDefaultAsync<Like>(
                sql,
                new { TweetId = tweetId, UserId = userId },
                _transaction
            );
        }

        // 特定のツイートに対する Like の一覧を取得（最新順）
        public async Task<IEnumerable<Like>> GetLikesByTweetIdAsync(int tweetId)
        {
            string sql = "SELECT * FROM Likes WHERE TweetId = :TweetId ORDER BY CreatedAt DESC";
            return await _connection.QueryAsync<Like>(
                sql,
                new { TweetId = tweetId },
                _transaction
            );
        }

        // 新規 Like を登録し、生成された LikeId を返す
        public async Task<int> CreateLikeAsync(Like like)
        {
            if (like == null)
                throw new ArgumentNullException(nameof(like));

            string sql = @"
                INSERT INTO Likes (TweetId, UserId, CreatedAt)
                VALUES (:TweetId, :UserId, :CreatedAt)
                RETURNING LikeId INTO :LikeId";

            var parameters = new DynamicParameters();
            parameters.Add("TweetId", like.TweetId, DbType.Int32);
            parameters.Add("UserId", like.UserId, DbType.Int32);
            parameters.Add("CreatedAt", like.CreatedAt, DbType.DateTime);
            parameters.Add("LikeId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await _connection.ExecuteAsync(sql, parameters, _transaction);
            return parameters.Get<int>("LikeId");
        }

        // 指定の Like を削除
        public async Task DeleteLikeAsync(int tweetId, int userId)
        {
            string sql = "DELETE FROM Likes WHERE TweetId = :TweetId AND UserId = :UserId";
            await _connection.ExecuteAsync(
                sql,
                new { TweetId = tweetId, UserId = userId },
                _transaction
            );
        }
    }
}
