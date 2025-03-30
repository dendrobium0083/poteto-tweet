using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Poteto.Domain.ValueObjects
{
    public class Password : IEquatable<Password>
    {
        // ハッシュ化されたパスワードの値
        public string HashedValue { get; }

        // コンストラクタ：平文パスワードを受け取り、バリデーション後にハッシュ化
        public Password(string plainPassword)
        {
            if (string.IsNullOrWhiteSpace(plainPassword))
                throw new ArgumentException("パスワードは必須です。", nameof(plainPassword));

            // パスワードの長さチェック（例：最低8文字）
            if (plainPassword.Length < 8)
                throw new ArgumentException("パスワードは8文字以上でなければなりません。", nameof(plainPassword));

            // 複雑性チェック（例：大文字、小文字、数字をそれぞれ1文字以上含む）
            if (!Regex.IsMatch(plainPassword, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$"))
                throw new ArgumentException("パスワードは大文字、小文字、数字を含む必要があります。", nameof(plainPassword));

            HashedValue = ComputeHash(plainPassword);
        }

        // SHA256を利用してパスワードをハッシュ化
        private string ComputeHash(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = sha256.ComputeHash(bytes);
                var sb = new StringBuilder();
                foreach (var b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        // 等価性の比較：ハッシュ値同士で比較
        public bool Equals(Password other)
        {
            if (other == null) return false;
            return string.Equals(HashedValue, other.HashedValue, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Password);
        }

        public override int GetHashCode()
        {
            return HashedValue.GetHashCode();
        }

        public override string ToString()
        {
            return HashedValue;
        }
    }
}
