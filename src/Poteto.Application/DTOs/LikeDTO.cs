using System;
using System.ComponentModel.DataAnnotations;

namespace Poteto.Application.DTOs
{
    /// <summary>
    /// いいね情報のDTO
    /// </summary>
    public class LikeDTO
    {
        /// <summary>
        /// いいねID
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
        /// 作成日時
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
