using System;
using System.Data;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Poteto.Infrastructure.Configurations;

namespace Poteto.Infrastructure.Data
{
    /// <summary>
    /// リポジトリの共通機能を提供するベースクラス
    /// </summary>
    public abstract class BaseRepository
    {
        /// <summary>
        /// データベース接続ファクトリ
        /// </summary>
        protected readonly IDbConnectionFactory _connectionFactory;

        /// <summary>
        /// ロガー
        /// </summary>
        protected readonly ILogger _logger;

        /// <summary>
        /// BaseRepositoryの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="connectionFactory">データベース接続ファクトリ</param>
        /// <param name="logger">ロガー</param>
        /// <exception cref="ArgumentNullException">connectionFactoryまたはloggerがnullの場合</exception>
        protected BaseRepository(IDbConnectionFactory connectionFactory, ILogger logger)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// データベース接続を取得します
        /// </summary>
        /// <returns>データベース接続</returns>
        protected IDbConnection GetConnection()
        {
            return _connectionFactory.CreateConnection();
        }

        /// <summary>
        /// トランザクション内で処理を実行します
        /// </summary>
        /// <typeparam name="T">戻り値の型</typeparam>
        /// <param name="action">実行する処理</param>
        /// <returns>処理の結果</returns>
        protected async Task<T> ExecuteInTransactionAsync<T>(Func<IDbConnection, IDbTransaction, Task<T>> action)
        {
            using var connection = GetConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                var result = await action(connection, transaction);
                transaction.Commit();
                return result;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError(ex, "トランザクションの実行中にエラーが発生しました");
                throw;
            }
        }

        /// <summary>
        /// トランザクション内で処理を実行します
        /// </summary>
        /// <param name="action">実行する処理</param>
        protected async Task ExecuteInTransactionAsync(Func<IDbConnection, IDbTransaction, Task> action)
        {
            using var connection = GetConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                await action(connection, transaction);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError(ex, "トランザクションの実行中にエラーが発生しました");
                throw;
            }
        }
    }
}
