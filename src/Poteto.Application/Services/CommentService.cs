using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Poteto.Application.DTOs;
using Poteto.Application.Interfaces.Repositories;
using Poteto.Application.Interfaces.Services;
using Poteto.Domain.Entities;

namespace Poteto.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;

        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
        }

        /// <summary>
        /// 新規コメントを投稿し、登録されたコメント情報の DTO を返します。
        /// </summary>
        public async Task<CommentDTO> CreateCommentAsync(int tweetId, int userId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("コメント内容は必須です。", nameof(content));

            // ドメインエンティティとして新規コメントを生成
            var comment = new Comment(tweetId, userId, content);

            // リポジトリ経由でコメントを登録し、自動採番された CommentId を取得
            int commentId = await _commentRepository.CreateCommentAsync(comment);

            // 登録後のコメント情報を取得（または comment エンティティに commentId を設定しても良い）
            comment = await _commentRepository.GetCommentByIdAsync(commentId);
            if (comment == null)
            {
                throw new InvalidOperationException("コメントの登録に失敗しました。");
            }

            // DTO への変換
            return new CommentDTO
            {
                CommentId = comment.CommentId,
                TweetId = comment.TweetId,
                UserId = comment.UserId,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt
            };
        }

        /// <summary>
        /// 指定したコメントID のコメント情報を取得し、DTO に変換して返します。
        /// </summary>
        public async Task<CommentDTO> GetCommentByIdAsync(int commentId)
        {
            var comment = await _commentRepository.GetCommentByIdAsync(commentId);
            if (comment == null)
            {
                throw new InvalidOperationException("Block data could not be found after creation.");
            }

            return new CommentDTO
            {
                CommentId = comment.CommentId,
                TweetId = comment.TweetId,
                UserId = comment.UserId,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt
            };
        }

        /// <summary>
        /// 指定したツイートに対する全コメント一覧を取得し、DTO の一覧に変換して返します。
        /// </summary>
        public async Task<IEnumerable<CommentDTO>> GetCommentsByTweetIdAsync(int tweetId)
        {
            var comments = await _commentRepository.GetCommentsByTweetIdAsync(tweetId);

            return comments.Select(comment => new CommentDTO
            {
                CommentId = comment.CommentId,
                TweetId = comment.TweetId,
                UserId = comment.UserId,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt
            });
        }

        /// <summary>
        /// 指定したコメントの内容を更新します。
        /// </summary>
        public async Task UpdateCommentAsync(int commentId, string newContent)
        {
            if (string.IsNullOrWhiteSpace(newContent))
                throw new ArgumentException("新しいコメント内容は必須です。", nameof(newContent));

            // コメントを取得
            var comment = await _commentRepository.GetCommentByIdAsync(commentId);
            if (comment == null)
            {
                throw new Exception("コメントが見つかりません。");
            }

            // ドメインエンティティ側の更新処理を呼び出し（バリデーションもここで実施可能）
            comment.UpdateContent(newContent);

            // 更新内容を永続化
            await _commentRepository.UpdateCommentAsync(comment);
        }
    }
}
