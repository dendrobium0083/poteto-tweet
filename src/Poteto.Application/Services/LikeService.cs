using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Poteto.Application.DTOs;
using Poteto.Application.Interfaces.Repositories;
using Poteto.Application.Interfaces.Services;
using Poteto.Domain.Entities;

namespace Poteto.Application.Services
{
    public class LikeService : ILikeService
    {
        private readonly ILikeRepository _likeRepository;

        public LikeService(ILikeRepository likeRepository)
        {
            _likeRepository = likeRepository ?? throw new ArgumentNullException(nameof(likeRepository));
        }

        /// <summary>
        /// 新規いいねを登録し、登録されたいいね情報の DTO を返します。
        /// </summary>
        public async Task<LikeDTO> CreateLikeAsync(int tweetId, int userId)
        {
            // ドメインエンティティとして新規いいねを生成
            var like = new Like(tweetId, userId);

            // リポジトリを通じていいねを登録し、生成された LikeId を取得
            int likeId = await _likeRepository.CreateLikeAsync(like);

            // 登録後のいいね情報を取得（または、生成したエンティティに likeId を設定しても良い）
            like = await _likeRepository.GetLikeAsync(tweetId, userId);
            if (like == null)
            {
                throw new InvalidOperationException("いいねの登録に失敗しました。");
            }

            // DTO への変換
            return new LikeDTO
            {
                LikeId = like.LikeId,
                TweetId = like.TweetId,
                UserId = like.UserId,
                CreatedAt = like.CreatedAt
            };
        }

        /// <summary>
        /// 指定のツイートに対するいいねの一覧を取得します。
        /// </summary>
        public async Task<IEnumerable<LikeDTO>> GetLikesByTweetIdAsync(int tweetId)
        {
            var likes = await _likeRepository.GetLikesByTweetIdAsync(tweetId);

            // 各いいね情報を DTO に変換して返却
            return likes.Select(like => new LikeDTO
            {
                LikeId = like.LikeId,
                TweetId = like.TweetId,
                UserId = like.UserId,
                CreatedAt = like.CreatedAt
            });
        }

        /// <summary>
        /// 指定されたいいね関係を解除します。
        /// </summary>
        public async Task DeleteLikeAsync(int tweetId, int userId)
        {
            await _likeRepository.DeleteLikeAsync(tweetId, userId);
        }
    }
}
