using System;
using Poteto.Domain.Entities;
using Poteto.Domain.ValueObjects;

namespace Poteto.Domain.DomainServices
{
    public class UserDomainService
    {
        // 例: ユーザ登録時に許可するメールドメインを検証する処理
        public bool IsAllowedEmailDomain(Email email)
        {
            if (email == null)
                throw new ArgumentNullException(nameof(email));
            
            // 例として、"example.com" ドメインのみ許可するルール
            return email.Value.EndsWith("@example.com", StringComparison.OrdinalIgnoreCase);
        }
        
        // 例: ユーザのパスワードがドメイン固有のビジネスルールを満たしているか検証する処理
        public bool IsValidPassword(Password password)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));
            
            // ここでは、Password クラスで基本バリデーションは実施しているため
            // 追加のビジネスルールがあれば実装する
            // 例: ハッシュ値のパターンチェックなど（今回は単純にtrueを返す）
            return true;
        }
        
        // 例: ユーザ登録前にその他のビジネスルールを統合的に検証する処理
        public void ValidateNewUser(User user, Email email, Password password)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (!IsAllowedEmailDomain(email))
                throw new InvalidOperationException("指定されたメールドメインは許可されていません。");
            if (!IsValidPassword(password))
                throw new InvalidOperationException("パスワードがビジネスルールに適合していません。");
            
            // 必要に応じて他の検証処理を追加
        }
    }
}
