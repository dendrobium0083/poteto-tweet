namespace Poteto.Application.Requests
{
    /// <summary>
    /// ツイート更新用のリクエストDTO
    /// </summary>
    public class UpdateTweetRequest
    {
        /// <summary>
        /// 新しいツイート内容
        /// </summary>
        public required string NewContent { get; set; }
    }
}
