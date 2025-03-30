using System;

namespace Poteto.Application.DTOs
{
    /// <summary>
    /// ユーザ情報を転送するためのデータ転送オブジェクト
    /// </summary>
    public class UserDTO
    {
        /// <summary>
        /// ユーザの一意な識別子
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// ユーザ名（ユニーク）
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// ユーザのメールアドレス
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// ユーザ登録日時（UTC）
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
