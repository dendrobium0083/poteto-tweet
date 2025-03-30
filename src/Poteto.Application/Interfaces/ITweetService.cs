using System.Collections.Generic;
using System.Threading.Tasks;

using Poteto.Application.DTOs;

namespace Poteto.Application.Interfaces
{
    /// <summary>
    /// ツイートに関するユースケースを定義するインターフェース
    /// </summary>
    public interface ITweetService
    {
        /// <summary>
        /// 新規ツイートを投稿し、登録されたツイート情報を返します。
        /// </summary>
        /// <param name="userId">投稿者のユーザID</param>
        /// <param name="content">ツイート内容</param>
        /// <returns>登録されたツイートの DTO</returns>
        Task<TweetDTO> CreateTweetAsync(int userId, string content);

        /// <summary>
        /// 指定したツイートID のツイート情報を取得します。
        /// </summary>
        /// <param name="tweetId">ツイートの一意な識別子</param>
        /// <returns>ツイートの DTO</returns>
        Task<TweetDTO> GetTweetByIdAsync(int tweetId);

        /// <summary>
        /// 指定したユーザの全ツイート一覧を取得します。
        /// </summary>
        /// <param name="userId">ユーザの一意な識別子</param>
        /// <returns>ツイートの DTO の一覧</returns>
        Task<IEnumerable<TweetDTO>> GetTweetsByUserIdAsync(int userId);

        /// <summary>
        /// 指定したツイートの内容を更新します。
        /// </summary>
        /// <param name="tweetId">更新対象のツイートID</param>
        /// <param name="newContent">新しいツイート内容</param>
        Task UpdateTweetAsync(int tweetId, string newContent);
    }
}
