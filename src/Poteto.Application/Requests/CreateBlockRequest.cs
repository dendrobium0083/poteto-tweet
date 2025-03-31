namespace Poteto.Application.Requests
{
    /// <summary>
    /// ブロック登録用のリクエストDTO
    /// </summary>
    public class CreateBlockRequest
    {
        /// <summary>
        /// ブロックを行うユーザのID
        /// </summary>
        public int BlockerId { get; set; }

        /// <summary>
        /// ブロック対象のユーザのID
        /// </summary>
        public int BlockedId { get; set; }
    }
}
