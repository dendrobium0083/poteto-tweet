namespace Poteto.Application.Requests
{
    /// <summary>
    /// ユーザ認証用のリクエスト DTO
    /// </summary>
    public class AuthenticateUserRequest
    {
        /// <summary>
        /// メールアドレス（必須）
        /// </summary>
        public required string Email { get; set; }

        /// <summary>
        /// 平文パスワード（必須）
        /// </summary>
        public required string Password { get; set; }
    }
}
