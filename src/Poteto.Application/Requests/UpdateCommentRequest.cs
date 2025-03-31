namespace Poteto.Application.Requests
{
    /// <summary>
    /// コメント更新用のリクエストDTO
    /// </summary>
    public class UpdateCommentRequest
    {
        /// <summary>
        /// 新しいコメント内容
        /// </summary>
        public required string NewContent { get; set; }
    }
}
