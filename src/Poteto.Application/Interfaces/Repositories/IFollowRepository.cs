using System.Collections.Generic;
using System.Threading.Tasks;

using Poteto.Domain.Entities;

namespace Poteto.Application.Interfaces.Repositories
{

    // Follow に対する DB 操作を抽象化するインターフェース
    public interface IFollowRepository
    {
        Task<Follow?> GetFollowAsync(int followerId, int followeeId);
        Task<IEnumerable<Follow>> GetFollowersAsync(int followeeId);
        Task<IEnumerable<Follow>> GetFolloweesAsync(int followerId);
        Task<int> CreateFollowAsync(Follow follow);
        Task DeleteFollowAsync(int followerId, int followeeId);
    }
}
