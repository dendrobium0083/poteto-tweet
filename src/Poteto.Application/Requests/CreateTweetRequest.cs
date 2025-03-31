namespace Poteto.Application.Requests
{
    /// <summary>
    /// ツイート投稿用のリクエストDTO
    /// </summary>
    public class CreateTweetRequest
    {
        /// <summary>
        /// ツイート投稿者のユーザID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// ツイートの内容
        /// </summary>
        public required string Content { get; set; }
    }
}
