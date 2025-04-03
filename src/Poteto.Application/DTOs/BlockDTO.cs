using System;
using System.ComponentModel.DataAnnotations;

namespace Poteto.Application.DTOs
{
    /// <summary>
    /// ブロック情報のDTO
    /// </summary>
    public class BlockDTO
    {
        /// <summary>
        /// ブロックID
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// ブロックするユーザーID
        /// </summary>
        [Required]
        public int BlockerId { get; set; }

        /// <summary>
        /// ブロックするユーザー名
        /// </summary>
        [Required]
        [StringLength(50)]
        public string BlockerUsername { get; set; } = string.Empty;

        /// <summary>
        /// ブロック対象のユーザーID
        /// </summary>
        [Required]
        public int BlockedId { get; set; }

        /// <summary>
        /// ブロック対象のユーザー名
        /// </summary>
        [Required]
        [StringLength(50)]
        public string BlockedUsername { get; set; } = string.Empty;

        /// <summary>
        /// 作成日時
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
