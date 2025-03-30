using System;
using System.Text.RegularExpressions;

namespace Poteto.Domain.ValueObjects
{
    public class Email : IEquatable<Email>
    {
        // メールアドレスの値
        public string Value { get; }

        // コンストラクタ：メールアドレスの生成時に入力値を検証
        public Email(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("メールアドレスは必須です。", nameof(email));

            // 簡易なメールアドレスの正規表現によるバリデーション
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(email))
                throw new ArgumentException("メールアドレスの形式が正しくありません。", nameof(email));

            Value = email;
        }

        // 等価性の比較：大文字小文字を区別せずに比較
        public bool Equals(Email other)
        {
            if (other == null) return false;
            return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Email);
        }

        public override int GetHashCode()
        {
            return Value.ToLowerInvariant().GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
