using System;

namespace Poteto.Domain.Entities
{
    /// <summary>
    /// ユーザーエンティティ
    /// </summary>
    public class User
    {
        /// <summary>
        /// ユーザーID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ユーザー名
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// メールアドレス
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// パスワードハッシュ
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        public User() { }

        // コンストラクタ：ユーザ登録時に利用
        public User(string userName, string email, string passwordHash)
        {
            // バリデーション処理など必要に応じて追加
            Username = userName ?? throw new ArgumentNullException(nameof(userName));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
            CreatedAt = DateTime.UtcNow;
        }

        // パスワード更新時の処理
        public void UpdatePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
            {
                throw new ArgumentException("新しいパスワードのハッシュは必須です。", nameof(newPasswordHash));
            }

            PasswordHash = newPasswordHash;
            UpdatedAt = DateTime.UtcNow;
        }

        // ユーザ名更新時の処理（必要に応じて）
        public void UpdateUserName(string newUserName)
        {
            if (string.IsNullOrWhiteSpace(newUserName))
            {
                throw new ArgumentException("新しいユーザ名は必須です。", nameof(newUserName));
            }

            Username = newUserName;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
