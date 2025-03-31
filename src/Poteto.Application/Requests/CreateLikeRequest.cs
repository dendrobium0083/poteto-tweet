namespace Poteto.Application.Requests
{
    /// <summary>
    /// いいね登録用のリクエストDTO
    /// </summary>
    public class CreateLikeRequest
    {
        /// <summary>
        /// 対象のツイートID
        /// </summary>
        public int TweetId { get; set; }

        /// <summary>
        /// いいねを行ったユーザのID
        /// </summary>
        public int UserId { get; set; }
    }
}
