using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Poteto.Application.DTOs
{
    /// <summary>
    /// ツイート情報のDTO
    /// </summary>
    public class TweetDTO
    {
        /// <summary>
        /// ツイートID
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// ユーザーID
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// ユーザー名
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// ツイート内容
        /// </summary>
        [Required]
        [StringLength(280, MinimumLength = 1)]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 作成日時
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新日時
        /// </summary>
        [Required]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// いいね数
        /// </summary>
        [Required]
        public int LikeCount { get; set; }

        /// <summary>
        /// コメント数
        /// </summary>
        [Required]
        public int CommentCount { get; set; }

        /// <summary>
        /// ツイートに対するコメントの一覧
        /// </summary>
        public IEnumerable<CommentDTO>? Comments { get; set; }
    }
}
