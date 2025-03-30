using System;

namespace Poteto.Infrastructure.Configurations
{
    public class OracleConfiguration
    {
        // Oracle用の接続文字列
        public string ConnectionString { get; }

        // 他に必要なOracle固有の設定があれば、プロパティとして追加可能です
        // 例: タイムアウト設定、キャッシュ設定など

        public OracleConfiguration(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("接続文字列は必須です。", nameof(connectionString));

            ConnectionString = connectionString;
        }
    }
}
