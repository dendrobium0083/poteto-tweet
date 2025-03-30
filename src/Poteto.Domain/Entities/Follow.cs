using System;

namespace Poteto.Domain.Entities
{
    public class Follow
    {
        // フォロー関係の一意な識別子
        public int FollowId { get; private set; }
        
        // フォローを行うユーザのID
        public int FollowerId { get; private set; }
        
        // フォロー対象のユーザのID
        public int FolloweeId { get; private set; }
        
        // フォロー登録日時（UTC）
        public DateTime CreatedAt { get; private set; }

        // コンストラクタ：新規フォロー関係の作成時に利用
        public Follow(int followerId, int followeeId)
        {
            if (followerId <= 0)
                throw new ArgumentException("FollowerIdは正の値でなければなりません。", nameof(followerId));
            if (followeeId <= 0)
                throw new ArgumentException("FolloweeIdは正の値でなければなりません。", nameof(followeeId));
            if (followerId == followeeId)
                throw new ArgumentException("自分自身をフォローすることはできません。", nameof(followeeId));
            
            FollowerId = followerId;
            FolloweeId = followeeId;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
