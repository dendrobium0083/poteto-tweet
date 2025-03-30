using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Poteto.Application.DTOs;
using Poteto.Application.Interfaces;
using Poteto.Domain.Entities;
using Poteto.Infrastructure.Data;

namespace Poteto.Application.Services
{
    public class FollowService : IFollowService
    {
        private readonly IFollowRepository _followRepository;

        public FollowService(IFollowRepository followRepository)
        {
            _followRepository = followRepository ?? throw new ArgumentNullException(nameof(followRepository));
        }

        /// <summary>
        /// 新規フォローを登録し、登録されたフォロー情報の DTO を返します。
        /// </summary>
        public async Task<FollowDTO> CreateFollowAsync(int followerId, int followeeId)
        {
            // ドメインエンティティの生成（リポジトリ内でバリデーションも実施可能）
            var follow = new Follow(followerId, followeeId);

            // リポジトリを介してフォローを登録し、生成された FollowId を取得
            int followId = await _followRepository.CreateFollowAsync(follow);

            // 登録後のフォロー情報の取得（または、生成したエンティティに followId を設定しても良い）
            follow = await _followRepository.GetFollowAsync(followerId, followeeId);

            // DTO への変換
            return new FollowDTO
            {
                FollowId = follow.FollowId,
                FollowerId = follow.FollowerId,
                FolloweeId = follow.FolloweeId,
                CreatedAt = follow.CreatedAt
            };
        }

        /// <summary>
        /// 指定したユーザ（フォロー対象）のフォロワー一覧を取得します。
        /// </summary>
        public async Task<IEnumerable<FollowDTO>> GetFollowersAsync(int followeeId)
        {
            var followers = await _followRepository.GetFollowersAsync(followeeId);

            // 各フォロー関係を DTO に変換
            return followers.Select(f => new FollowDTO
            {
                FollowId = f.FollowId,
                FollowerId = f.FollowerId,
                FolloweeId = f.FolloweeId,
                CreatedAt = f.CreatedAt
            });
        }

        /// <summary>
        /// 指定したユーザ（フォローを行う側）がフォロー中のユーザ一覧を取得します。
        /// </summary>
        public async Task<IEnumerable<FollowDTO>> GetFolloweesAsync(int followerId)
        {
            var followees = await _followRepository.GetFolloweesAsync(followerId);

            // 各フォロー関係を DTO に変換
            return followees.Select(f => new FollowDTO
            {
                FollowId = f.FollowId,
                FollowerId = f.FollowerId,
                FolloweeId = f.FolloweeId,
                CreatedAt = f.CreatedAt
            });
        }

        /// <summary>
        /// 指定されたフォロー関係を解除します。
        /// </summary>
        public async Task DeleteFollowAsync(int followerId, int followeeId)
        {
            await _followRepository.DeleteFollowAsync(followerId, followeeId);
        }
    }
}
