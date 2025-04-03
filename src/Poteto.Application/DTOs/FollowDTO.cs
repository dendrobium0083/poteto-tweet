using System;
using System.ComponentModel.DataAnnotations;

namespace Poteto.Application.DTOs
{
    /// <summary>
    /// フォロー情報のDTO
    /// </summary>
    public class FollowDTO
    {
        /// <summary>
        /// フォローID
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// フォロワーID（フォローするユーザー）
        /// </summary>
        [Required]
        public int FollowerId { get; set; }

        /// <summary>
        /// フォロワー名
        /// </summary>
        [Required]
        [StringLength(50)]
        public string FollowerUsername { get; set; } = string.Empty;

        /// <summary>
        /// フォロー対象ID（フォローされるユーザー）
        /// </summary>
        [Required]
        public int FollowingId { get; set; }

        /// <summary>
        /// フォロー対象名
        /// </summary>
        [Required]
        [StringLength(50)]
        public string FollowingUsername { get; set; } = string.Empty;

        /// <summary>
        /// 作成日時
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
