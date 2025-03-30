using System;

namespace Poteto.Application.DTOs
{
    /// <summary>
    /// いいね情報を転送するためのデータ転送オブジェクト
    /// </summary>
    public class LikeDTO
    {
        /// <summary>
        /// いいねの一意な識別子
        /// </summary>
        public int LikeId { get; set; }
        
        /// <summary>
        /// 対象のツイートID
        /// </summary>
        public int TweetId { get; set; }
        
        /// <summary>
        /// いいねを行ったユーザのID
        /// </summary>
        public int UserId { get; set; }
        
        /// <summary>
        /// いいね登録日時
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
