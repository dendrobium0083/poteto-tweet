using System;
using System.ComponentModel.DataAnnotations;

namespace Poteto.Application.DTOs
{
    /// <summary>
    /// コメント情報のDTO
    /// </summary>
    public class CommentDTO
    {
        /// <summary>
        /// コメントID
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// ツイートID
        /// </summary>
        [Required]
        public int TweetId { get; set; }

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
        /// コメント内容
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
    }
}
