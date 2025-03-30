using System;
using System.Collections.Generic;

namespace Poteto.Domain.Entities
{
    public class Tweet
    {
        // ツイートの一意な識別子
        public int TweetId { get; private set; }

        // ツイート投稿者のユーザID
        public int UserId { get; private set; }

        // ツイート内容（最大280文字）
        public string Content { get; private set; }

        // 作成日時（UTC）
        public DateTime CreatedAt { get; private set; }

        // 更新日時（UTC、更新時のみ設定）
        public DateTime? UpdatedAt { get; private set; }

        // ツイートに対するコメント一覧
        private readonly List<Comment> _comments = new List<Comment>();
        public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();

        // コンストラクタ：新規ツイート作成時に利用
        public Tweet(int userId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("ツイート内容は必須です。", nameof(content));
            if (content.Length > 280)
                throw new ArgumentException("ツイート内容は280文字以内で入力してください。", nameof(content));

            UserId = userId;
            Content = content;
            CreatedAt = DateTime.UtcNow;
        }

        // ツイート内容更新時の処理
        public void UpdateContent(string newContent)
        {
            if (string.IsNullOrWhiteSpace(newContent))
                throw new ArgumentException("新しいツイート内容は必須です。", nameof(newContent));
            if (newContent.Length > 280)
                throw new ArgumentException("ツイート内容は280文字以内で入力してください。", nameof(newContent));

            Content = newContent;
            UpdatedAt = DateTime.UtcNow;
        }

        // コメント追加処理
        public void AddComment(Comment comment)
        {
            if (comment == null)
                throw new ArgumentNullException(nameof(comment));

            _comments.Add(comment);
        }
    }
}
