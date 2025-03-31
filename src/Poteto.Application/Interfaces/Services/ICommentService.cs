using System.Collections.Generic;
using System.Threading.Tasks;

using Poteto.Application.DTOs;

namespace Poteto.Application.Interfaces.Services
{
    /// <summary>
    /// コメントに関するユースケースを定義するインターフェース
    /// </summary>
    public interface ICommentService
    {
        /// <summary>
        /// 新規コメントを投稿し、登録されたコメント情報を返します。
        /// </summary>
        /// <param name="tweetId">対象のツイートID</param>
        /// <param name="userId">コメント投稿者のユーザID</param>
        /// <param name="content">コメント内容</param>
        /// <returns>登録されたコメントの DTO</returns>
        Task<CommentDTO> CreateCommentAsync(int tweetId, int userId, string content);

        /// <summary>
        /// 指定したコメントID のコメント情報を取得します。
        /// </summary>
        /// <param name="commentId">コメントの一意な識別子</param>
        /// <returns>コメントの DTO</returns>
        Task<CommentDTO> GetCommentByIdAsync(int commentId);

        /// <summary>
        /// 指定したツイートに対する全コメント一覧を取得します。
        /// </summary>
        /// <param name="tweetId">対象のツイートID</param>
        /// <returns>コメントの DTO の一覧</returns>
        Task<IEnumerable<CommentDTO>> GetCommentsByTweetIdAsync(int tweetId);

        /// <summary>
        /// 指定したコメントの内容を更新します。
        /// </summary>
        /// <param name="commentId">更新対象のコメントID</param>
        /// <param name="newContent">新しいコメント内容</param>
        Task UpdateCommentAsync(int commentId, string newContent);
    }
}
