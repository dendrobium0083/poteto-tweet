using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

using Dapper;

using Poteto.Domain.Entities;

namespace Poteto.Infrastructure.Data
{
    // Comment に対する DB 操作を抽象化するインターフェース
    public interface ICommentRepository
    {
        Task<Comment?> GetCommentByIdAsync(int commentId);
        Task<IEnumerable<Comment>> GetCommentsByTweetIdAsync(int tweetId);
        Task<int> CreateCommentAsync(Comment comment);
        Task UpdateCommentAsync(Comment comment);
    }

    // Dapper を用いた CommentRepository の実装
    public class CommentRepository : ICommentRepository
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        // IUnitOfWork から接続およびトランザクションを取得
        public CommentRepository(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            _connection = unitOfWork.Transaction.Connection ?? throw new InvalidOperationException("Connection is null");
            _transaction = unitOfWork.Transaction ?? throw new InvalidOperationException("Transaction is null");
        }

        // CommentId によるコメントの取得
        public async Task<Comment?> GetCommentByIdAsync(int commentId)
        {
            string sql = "SELECT * FROM Comments WHERE CommentId = :CommentId";
            return await _connection.QueryFirstOrDefaultAsync<Comment>(
                sql,
                new { CommentId = commentId },
                _transaction
            );
        }

        // 指定ツイートのコメント一覧を取得（投稿順）
        public async Task<IEnumerable<Comment>> GetCommentsByTweetIdAsync(int tweetId)
        {
            string sql = "SELECT * FROM Comments WHERE TweetId = :TweetId ORDER BY CreatedAt ASC";
            return await _connection.QueryAsync<Comment>(
                sql,
                new { TweetId = tweetId },
                _transaction
            );
        }

        // 新規コメントを登録し、生成された CommentId を返す
        public async Task<int> CreateCommentAsync(Comment comment)
        {
            if (comment == null)
                throw new ArgumentNullException(nameof(comment));

            // Oracle の RETURNING 句を利用して、自動採番された CommentId を取得
            string sql = @"
                INSERT INTO Comments (TweetId, UserId, Content, CreatedAt)
                VALUES (:TweetId, :UserId, :Content, :CreatedAt)
                RETURNING CommentId INTO :CommentId";

            var parameters = new DynamicParameters();
            parameters.Add("TweetId", comment.TweetId, DbType.Int32);
            parameters.Add("UserId", comment.UserId, DbType.Int32);
            parameters.Add("Content", comment.Content, DbType.String, size: 500);
            parameters.Add("CreatedAt", comment.CreatedAt, DbType.DateTime);
            parameters.Add("CommentId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await _connection.ExecuteAsync(sql, parameters, _transaction);
            return parameters.Get<int>("CommentId");
        }

        // コメント内容の更新
        public async Task UpdateCommentAsync(Comment comment)
        {
            if (comment == null)
                throw new ArgumentNullException(nameof(comment));

            string sql = @"
                UPDATE Comments
                SET Content = :Content,
                    UpdatedAt = :UpdatedAt
                WHERE CommentId = :CommentId";

            await _connection.ExecuteAsync(sql,
                new
                {
                    Content = comment.Content,
                    UpdatedAt = comment.UpdatedAt,
                    CommentId = comment.CommentId
                },
                _transaction);
        }
    }
}
