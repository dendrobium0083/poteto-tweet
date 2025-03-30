using System;

namespace Poteto.Domain.Entities
{
    public class Like
    {
        // いいねの一意な識別子
        public int LikeId { get; private set; }
        
        // 対象のツイートID
        public int TweetId { get; private set; }
        
        // いいねをしたユーザのID
        public int UserId { get; private set; }
        
        // いいね登録日時（UTC）
        public DateTime CreatedAt { get; private set; }

        // コンストラクタ：新規いいね作成時に利用
        public Like(int tweetId, int userId)
        {
            if (tweetId <= 0)
                throw new ArgumentException("TweetIdは正の値でなければなりません。", nameof(tweetId));
            if (userId <= 0)
                throw new ArgumentException("UserIdは正の値でなければなりません。", nameof(userId));
            
            TweetId = tweetId;
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
