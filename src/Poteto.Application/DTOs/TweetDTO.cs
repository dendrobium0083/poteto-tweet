using System;
using System.Collections.Generic;

namespace Poteto.Application.DTOs
{
    /// <summary>
    /// ツイート情報を転送するためのデータ転送オブジェクト
    /// </summary>
    public class TweetDTO
    {
        /// <summary>
        /// ツイートの一意な識別子
        /// </summary>
        public int TweetId { get; set; }

        /// <summary>
        /// ツイート投稿者のユーザID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// ツイートの内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// ツイートの作成日時（UTC）
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// ツイートの更新日時（UTC、更新時のみ設定）
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// ツイートに対するコメントの一覧（必要に応じて）
        /// </summary>
        public IEnumerable<CommentDTO> Comments { get; set; }
    }
}
