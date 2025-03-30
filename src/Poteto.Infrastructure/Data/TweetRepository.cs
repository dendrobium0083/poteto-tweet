using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

using Dapper;

using Poteto.Domain.Entities;

namespace Poteto.Infrastructure.Data
{
    // Tweet に対する DB 操作を抽象化するインターフェース
    public interface ITweetRepository
    {
        Task<Tweet> GetTweetByIdAsync(int tweetId);
        Task<IEnumerable<Tweet>> GetTweetsByUserIdAsync(int userId);
        Task<int> CreateTweetAsync(Tweet tweet);
        Task UpdateTweetAsync(Tweet tweet);
    }

    // Dapper を用いた TweetRepository の実装
    public class TweetRepository : ITweetRepository
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        // IUnitOfWork から接続およびトランザクションを取得
        public TweetRepository(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            _connection = unitOfWork.Transaction.Connection;
            _transaction = unitOfWork.Transaction;
        }

        // TweetId によるツイートの取得
        public async Task<Tweet> GetTweetByIdAsync(int tweetId)
        {
            string sql = "SELECT * FROM Tweets WHERE TweetId = :TweetId";
            return await _connection.QueryFirstOrDefaultAsync<Tweet>(
                sql,
                new { TweetId = tweetId },
                _transaction
            );
        }

        // 指定ユーザのツイート一覧を取得（最新順）
        public async Task<IEnumerable<Tweet>> GetTweetsByUserIdAsync(int userId)
        {
            string sql = "SELECT * FROM Tweets WHERE UserId = :UserId ORDER BY CreatedAt DESC";
            return await _connection.QueryAsync<Tweet>(
                sql,
                new { UserId = userId },
                _transaction
            );
        }

        // 新規ツイートを登録し、生成された TweetId を返す
        public async Task<int> CreateTweetAsync(Tweet tweet)
        {
            if (tweet == null)
                throw new ArgumentNullException(nameof(tweet));

            // Oracle の RETURNING 句を利用して、自動採番された TweetId を取得
            string sql = @"
                INSERT INTO Tweets (UserId, Content, CreatedAt)
                VALUES (:UserId, :Content, :CreatedAt)
                RETURNING TweetId INTO :TweetId";

            var parameters = new DynamicParameters();
            parameters.Add("UserId", tweet.UserId, DbType.Int32);
            parameters.Add("Content", tweet.Content, DbType.String, size: 280);
            parameters.Add("CreatedAt", tweet.CreatedAt, DbType.DateTime);
            parameters.Add("TweetId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await _connection.ExecuteAsync(sql, parameters, _transaction);
            return parameters.Get<int>("TweetId");
        }

        // ツイート内容の更新
        public async Task UpdateTweetAsync(Tweet tweet)
        {
            if (tweet == null)
                throw new ArgumentNullException(nameof(tweet));

            string sql = @"
                UPDATE Tweets
                SET Content = :Content,
                    UpdatedAt = :UpdatedAt
                WHERE TweetId = :TweetId";

            await _connection.ExecuteAsync(sql,
                new
                {
                    Content = tweet.Content,
                    UpdatedAt = tweet.UpdatedAt,
                    TweetId = tweet.TweetId
                },
                _transaction);
        }
    }
}
