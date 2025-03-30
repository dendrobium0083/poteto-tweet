using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Poteto.Application.DTOs;
using Poteto.Application.Interfaces;
using Poteto.Domain.DomainServices;
using Poteto.Domain.Entities;
using Poteto.Infrastructure.Data;

namespace Poteto.Application.Services
{
    public class TweetService : ITweetService
    {
        private readonly ITweetRepository _tweetRepository;
        private readonly TweetDomainService _tweetDomainService;

        public TweetService(ITweetRepository tweetRepository, TweetDomainService tweetDomainService)
        {
            _tweetRepository = tweetRepository ?? throw new ArgumentNullException(nameof(tweetRepository));
            _tweetDomainService = tweetDomainService ?? throw new ArgumentNullException(nameof(tweetDomainService));
        }

        /// <summary>
        /// 新規ツイートを投稿し、登録されたツイート情報の DTO を返します。
        /// </summary>
        public async Task<TweetDTO> CreateTweetAsync(int userId, string content)
        {
            // ドメインサービスによる内容のバリデーション
            _tweetDomainService.ValidateTweetContent(content);

            // 新規ツイートエンティティの生成
            var tweet = new Tweet(userId, content);

            // リポジトリを介してツイートの登録
            int tweetId = await _tweetRepository.CreateTweetAsync(tweet);

            // 登録されたツイートの取得（または、tweet エンティティに tweetId を設定しても良い）
            tweet = await _tweetRepository.GetTweetByIdAsync(tweetId);

            // ドメインエンティティを DTO に変換して返却
            return new TweetDTO
            {
                TweetId = tweet.TweetId,
                UserId = tweet.UserId,
                Content = tweet.Content,
                CreatedAt = tweet.CreatedAt,
                UpdatedAt = tweet.UpdatedAt,
                Comments = tweet.Comments?.Select(c => new CommentDTO
                {
                    CommentId = c.CommentId,
                    TweetId = c.TweetId,
                    UserId = c.UserId,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
            };
        }

        /// <summary>
        /// 指定したツイートID のツイート情報を取得します。
        /// </summary>
        public async Task<TweetDTO> GetTweetByIdAsync(int tweetId)
        {
            var tweet = await _tweetRepository.GetTweetByIdAsync(tweetId);
            if (tweet == null)
            {
                return null;
            }

            return new TweetDTO
            {
                TweetId = tweet.TweetId,
                UserId = tweet.UserId,
                Content = tweet.Content,
                CreatedAt = tweet.CreatedAt,
                UpdatedAt = tweet.UpdatedAt,
                Comments = tweet.Comments?.Select(c => new CommentDTO
                {
                    CommentId = c.CommentId,
                    TweetId = c.TweetId,
                    UserId = c.UserId,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
            };
        }

        /// <summary>
        /// 指定したユーザの全ツイート一覧を取得します。
        /// </summary>
        public async Task<IEnumerable<TweetDTO>> GetTweetsByUserIdAsync(int userId)
        {
            var tweets = await _tweetRepository.GetTweetsByUserIdAsync(userId);

            // 各ツイートを DTO に変換
            return tweets.Select(tweet => new TweetDTO
            {
                TweetId = tweet.TweetId,
                UserId = tweet.UserId,
                Content = tweet.Content,
                CreatedAt = tweet.CreatedAt,
                UpdatedAt = tweet.UpdatedAt,
                Comments = tweet.Comments?.Select(c => new CommentDTO
                {
                    CommentId = c.CommentId,
                    TweetId = c.TweetId,
                    UserId = c.UserId,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
            });
        }

        /// <summary>
        /// 指定したツイートの内容を更新します。
        /// </summary>
        public async Task UpdateTweetAsync(int tweetId, string newContent)
        {
            // ツイート取得
            var tweet = await _tweetRepository.GetTweetByIdAsync(tweetId);
            if (tweet == null)
            {
                throw new Exception("Tweet not found");
            }

            // ドメインサービスを利用して更新時のビジネスルールを適用
            _tweetDomainService.UpdateTweetContent(tweet, newContent);

            // 更新されたツイートをリポジトリ経由で永続化
            await _tweetRepository.UpdateTweetAsync(tweet);
        }
    }
}
