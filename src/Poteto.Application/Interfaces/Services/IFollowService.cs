using System.Collections.Generic;
using System.Threading.Tasks;

using Poteto.Application.DTOs;

namespace Poteto.Application.Interfaces.Services
{
    /// <summary>
    /// ユーザ間のフォロー操作に関するユースケースを定義するインターフェース
    /// </summary>
    public interface IFollowService
    {
        /// <summary>
        /// 新規フォローを登録し、登録されたフォロー情報の DTO を返します。
        /// </summary>
        /// <param name="followerId">フォローを行うユーザのID</param>
        /// <param name="followeeId">フォロー対象のユーザのID</param>
        /// <returns>登録されたフォロー情報の DTO</returns>
        Task<FollowDTO> CreateFollowAsync(int followerId, int followeeId);

        /// <summary>
        /// 指定したユーザ（フォロー対象）のフォロワー一覧を取得します。
        /// </summary>
        /// <param name="followeeId">フォロー対象ユーザのID</param>
        /// <returns>フォロワー情報の DTO の一覧</returns>
        Task<IEnumerable<FollowDTO>> GetFollowersAsync(int followeeId);

        /// <summary>
        /// 指定したユーザ（フォローを行う側）がフォロー中のユーザ一覧を取得します。
        /// </summary>
        /// <param name="followerId">フォローを行うユーザのID</param>
        /// <returns>フォロー中のユーザ情報の DTO の一覧</returns>
        Task<IEnumerable<FollowDTO>> GetFolloweesAsync(int followerId);

        /// <summary>
        /// 指定されたフォロー関係を解除します。
        /// </summary>
        /// <param name="followerId">フォローを行うユーザのID</param>
        /// <param name="followeeId">フォロー解除する対象ユーザのID</param>
        Task DeleteFollowAsync(int followerId, int followeeId);
    }
}
