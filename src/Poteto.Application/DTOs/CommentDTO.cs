using System;

namespace Poteto.Application.DTOs
{
    /// <summary>
    /// コメント情報を転送するためのデータ転送オブジェクト
    /// </summary>
    public class CommentDTO
    {
        /// <summary>
        /// コメントの一意な識別子
        /// </summary>
        public int CommentId { get; set; }

        /// <summary>
        /// 対象のツイートID
        /// </summary>
        public int TweetId { get; set; }

        /// <summary>
        /// コメント投稿者のユーザID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// コメントの内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// コメントの作成日時
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// コメントの更新日時（更新時のみ設定）
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
