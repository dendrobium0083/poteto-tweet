using System.Collections.Generic;
using System.Threading.Tasks;

using Poteto.Domain.Entities;

namespace Poteto.Application.Interfaces.Repositories
{
    // Like に対する DB 操作を抽象化するインターフェース
    public interface ILikeRepository
    {
        Task<Like?> GetLikeAsync(int tweetId, int userId);
        Task<IEnumerable<Like>> GetLikesByTweetIdAsync(int tweetId);
        Task<int> CreateLikeAsync(Like like);
        Task DeleteLikeAsync(int tweetId, int userId);
    }
}
