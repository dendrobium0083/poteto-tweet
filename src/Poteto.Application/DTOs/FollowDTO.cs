using System;

namespace Poteto.Application.DTOs
{
    /// <summary>
    /// フォロー情報を転送するためのデータ転送オブジェクト
    /// </summary>
    public class FollowDTO
    {
        /// <summary>
        /// フォロー関係の一意な識別子
        /// </summary>
        public int FollowId { get; set; }
        
        /// <summary>
        /// フォローを行うユーザのID
        /// </summary>
        public int FollowerId { get; set; }
        
        /// <summary>
        /// フォロー対象のユーザのID
        /// </summary>
        public int FolloweeId { get; set; }
        
        /// <summary>
        /// フォロー登録日時（UTC）
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
