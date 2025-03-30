using System;

using Poteto.Domain.Entities;

namespace Poteto.Domain.DomainServices
{
    public class TweetDomainService
    {
        /// <summary>
        /// ツイート内容のバリデーションを行います。
        /// </summary>
        /// <param name="content">ツイート内容</param>
        public void ValidateTweetContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("ツイート内容は必須です。", nameof(content));
            if (content.Length > 280)
                throw new ArgumentException("ツイート内容は280文字以内で入力してください。", nameof(content));
        }

        /// <summary>
        /// ツイートがスパムとみなされるかを判定します。（例: 単純なチェック）
        /// </summary>
        /// <param name="tweet">対象のツイート</param>
        /// <returns>スパムなら true、そうでなければ false</returns>
        public bool IsSpamTweet(Tweet tweet)
        {
            if (tweet == null)
                throw new ArgumentNullException(nameof(tweet));

            // 例として、ツイート内容に「spam」という文字列が含まれていればスパムと判定する簡易ロジック
            return tweet.Content.IndexOf("spam", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// ツイート内容の更新処理に対するビジネスルールを適用します。
        /// 例: ツイート作成後一定時間を過ぎたツイートは更新不可とする。
        /// </summary>
        /// <param name="tweet">対象のツイート</param>
        /// <param name="newContent">新しいツイート内容</param>
        public void UpdateTweetContent(Tweet tweet, string newContent)
        {
            if (tweet == null)
                throw new ArgumentNullException(nameof(tweet));

            // 新しい内容のバリデーション
            ValidateTweetContent(newContent);

            // 例: ツイート作成から15分以上経過している場合は更新を許可しない
            TimeSpan updateWindow = TimeSpan.FromMinutes(15);
            if (DateTime.UtcNow - tweet.CreatedAt > updateWindow)
                throw new InvalidOperationException("ツイートは作成から15分以上経過しているため、更新できません。");

            // バリデーションを通過した場合、エンティティ側の更新処理を呼び出す
            tweet.UpdateContent(newContent);
        }
    }
}
