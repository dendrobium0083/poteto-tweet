using System.ComponentModel.DataAnnotations;

namespace Poteto.Application.DTOs
{
    /// <summary>
    /// ユーザー情報のDTO
    /// </summary>
    public class UserDTO
    {
        /// <summary>
        /// ユーザーID
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// ユーザー名
        /// </summary>
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// メールアドレス
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 作成日時
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
