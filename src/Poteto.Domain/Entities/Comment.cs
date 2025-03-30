using System;

namespace Poteto.Domain.Entities
{
    public class Comment
    {
        // コメントの一意な識別子
        public int CommentId { get; private set; }
        
        // 対象のツイートID
        public int TweetId { get; private set; }
        
        // コメント投稿者のユーザID
        public int UserId { get; private set; }
        
        // コメント内容（例として最大500文字）
        public string Content { get; private set; }
        
        // 作成日時（UTC）
        public DateTime CreatedAt { get; private set; }
        
        // 更新日時（UTC、更新時のみ設定）
        public DateTime? UpdatedAt { get; private set; }

        // コンストラクタ：新規コメント作成時に利用
        public Comment(int tweetId, int userId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("コメント内容は必須です。", nameof(content));
            if (content.Length > 500)
                throw new ArgumentException("コメントは500文字以内で入力してください。", nameof(content));

            TweetId = tweetId;
            UserId = userId;
            Content = content;
            CreatedAt = DateTime.UtcNow;
        }
        
        // コメント内容更新時の処理
        public void UpdateContent(string newContent)
        {
            if (string.IsNullOrWhiteSpace(newContent))
                throw new ArgumentException("新しいコメント内容は必須です。", nameof(newContent));
            if (newContent.Length > 500)
                throw new ArgumentException("コメントは500文字以内で入力してください。", nameof(newContent));

            Content = newContent;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
