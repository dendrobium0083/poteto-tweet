using System.ComponentModel.DataAnnotations;

namespace Poteto.WebAPI.Requests;

/// <summary>
/// ユーザー登録リクエスト
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// ユーザー名
    /// </summary>
    [Required(ErrorMessage = "ユーザー名は必須です")]
    [StringLength(50, ErrorMessage = "ユーザー名は50文字以内で入力してください")]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// メールアドレス
    /// </summary>
    [Required(ErrorMessage = "メールアドレスは必須です")]
    [EmailAddress(ErrorMessage = "有効なメールアドレスを入力してください")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// パスワード
    /// </summary>
    [Required(ErrorMessage = "パスワードは必須です")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "パスワードは8文字以上100文字以内で入力してください")]
    public string Password { get; set; } = string.Empty;
}
