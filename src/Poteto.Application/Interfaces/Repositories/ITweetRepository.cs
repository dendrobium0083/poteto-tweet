using System.Collections.Generic;
using System.Threading.Tasks;
using Poteto.Domain.Entities;

namespace Poteto.Application.Interfaces.Repositories
{
    // Tweet に対する DB 操作を抽象化するインターフェース
    public interface ITweetRepository
    {
        Task<Tweet?> GetTweetByIdAsync(int tweetId);
        Task<IEnumerable<Tweet>> GetTweetsByUserIdAsync(int userId);
        Task<int> CreateTweetAsync(Tweet tweet);
        Task UpdateTweetAsync(Tweet tweet);
    }

}
