namespace Poteto.Application.Requests
{
    /// <summary>
    /// ユーザ登録用のリクエスト DTO
    /// </summary>
    public class RegisterUserRequest
    {
        /// <summary>
        /// ユーザ名（必須）
        /// </summary>
        public required string UserName { get; set; }

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
