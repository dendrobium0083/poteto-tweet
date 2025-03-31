using System.Collections.Generic;
using System.Threading.Tasks;

using Poteto.Domain.Entities;

namespace Poteto.Application.Interfaces.Repositories
{
    /// <summary>
    /// Comment に対する DB 操作を抽象化するインターフェース
    /// </summary>
    public interface ICommentRepository
    {
        Task<Comment?> GetCommentByIdAsync(int commentId);
        Task<IEnumerable<Comment>> GetCommentsByTweetIdAsync(int tweetId);
        Task<int> CreateCommentAsync(Comment comment);
        Task UpdateCommentAsync(Comment comment);
    }
}
