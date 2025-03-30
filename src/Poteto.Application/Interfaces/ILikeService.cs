using System.Collections.Generic;
using System.Threading.Tasks;
using Poteto.Application.DTOs;

namespace Poteto.Application.Interfaces
{
    /// <summary>
    /// ツイートのいいね操作に関するユースケースを定義するインターフェース
    /// </summary>
    public interface ILikeService
    {
        /// <summary>
        /// 新規いいねを登録し、登録されたいいね情報の DTO を返します。
        /// </summary>
        /// <param name="tweetId">対象のツイートID</param>
        /// <param name="userId">いいねを行ったユーザのID</param>
        /// <returns>登録されたいいね情報の DTO</returns>
        Task<LikeDTO> CreateLikeAsync(int tweetId, int userId);

        /// <summary>
        /// 指定のツイートに対するいいねの一覧を取得します。
        /// </summary>
        /// <param name="tweetId">対象のツイートID</param>
        /// <returns>いいね情報の DTO の一覧</returns>
        Task<IEnumerable<LikeDTO>> GetLikesByTweetIdAsync(int tweetId);

        /// <summary>
        /// 指定されたいいね関係を解除します。
        /// </summary>
        /// <param name="tweetId">対象のツイートID</param>
        /// <param name="userId">いいねを解除するユーザのID</param>
        Task DeleteLikeAsync(int tweetId, int userId);
    }
}
