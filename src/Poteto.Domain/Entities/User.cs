using System;

namespace Poteto.Domain.Entities
{
    public class User
    {
        // ユーザの一意な識別子
        public int UserId { get; private set; }
        
        // ユーザ名（ユニーク）
        public string UserName { get; private set; }
        
        // メールアドレス（ユニーク）
        public string Email { get; private set; }
        
        // ハッシュ化されたパスワード
        public string PasswordHash { get; private set; }
        
        // 作成日時（UTC）
        public DateTime CreatedAt { get; private set; }
        
        // 更新日時（UTC, 更新時のみ設定）
        public DateTime? UpdatedAt { get; private set; }

        public User() { }

        // コンストラクタ：ユーザ登録時に利用
        public User(string userName, string email, string passwordHash)
        {
            // バリデーション処理など必要に応じて追加
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
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
            
            UserName = newUserName;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
