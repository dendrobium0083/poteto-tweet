using System;

namespace Poteto.Infrastructure.Configurations
{
    /// <summary>
    /// Oracleデータベースの設定を管理するクラス
    /// </summary>
    public class OracleConfiguration
    {
        /// <summary>
        /// Oracleデータベースへの接続文字列
        /// </summary>
        public string ConnectionString { get; }

        /// <summary>
        /// コマンドのタイムアウト時間（秒）
        /// </summary>
        public int CommandTimeout { get; }

        /// <summary>
        /// 接続プールの最小サイズ
        /// </summary>
        public int MinPoolSize { get; }

        /// <summary>
        /// 接続プールの最大サイズ
        /// </summary>
        public int MaxPoolSize { get; }

        /// <summary>
        /// OracleConfigurationの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="connectionString">接続文字列</param>
        /// <param name="commandTimeout">コマンドのタイムアウト時間（秒）</param>
        /// <param name="minPoolSize">接続プールの最小サイズ</param>
        /// <param name="maxPoolSize">接続プールの最大サイズ</param>
        /// <exception cref="ArgumentException">接続文字列が無効な場合</exception>
        /// <exception cref="ArgumentOutOfRangeException">タイムアウト時間またはプールサイズが無効な場合</exception>
        public OracleConfiguration(
            string connectionString,
            int commandTimeout = 30,
            int minPoolSize = 1,
            int maxPoolSize = 100)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("接続文字列は必須です。", nameof(connectionString));
            }

            if (commandTimeout <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(commandTimeout), "タイムアウト時間は正の値である必要があります。");
            }

            if (minPoolSize < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minPoolSize), "最小プールサイズは0以上である必要があります。");
            }

            if (maxPoolSize <= 0 || maxPoolSize < minPoolSize)
            {
                throw new ArgumentOutOfRangeException(nameof(maxPoolSize), "最大プールサイズは最小プールサイズより大きく、正の値である必要があります。");
            }

            ConnectionString = connectionString;
            CommandTimeout = commandTimeout;
            MinPoolSize = minPoolSize;
            MaxPoolSize = maxPoolSize;
        }
    }
}
