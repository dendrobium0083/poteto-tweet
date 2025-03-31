namespace Poteto.Application.Requests
{
    /// <summary>
    /// コメント投稿用のリクエストDTO
    /// </summary>
    public class CreateCommentRequest
    {
        /// <summary>
        /// 対象ツイートのID
        /// </summary>
        public int TweetId { get; set; }

        /// <summary>
        /// コメント投稿者のユーザID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// コメント内容
        /// </summary>
        public required string Content { get; set; }
    }
}
