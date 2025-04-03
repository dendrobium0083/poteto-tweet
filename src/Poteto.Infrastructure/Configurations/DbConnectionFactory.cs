using System;
using System.Data;

using Oracle.ManagedDataAccess.Client;

namespace Poteto.Infrastructure.Configurations
{
    /// <summary>
    /// データベース接続を管理するインターフェース
    /// </summary>
    public interface IDbConnectionFactory
    {
        /// <summary>
        /// データベース接続を作成します
        /// </summary>
        /// <returns>オープン済みのデータベース接続</returns>
        /// <exception cref="InvalidOperationException">接続の作成に失敗した場合</exception>
        IDbConnection CreateConnection();
    }

    /// <summary>
    /// Oracleデータベースへの接続を管理するファクトリクラス
    /// </summary>
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly OracleConfiguration _oracleConfiguration;

        /// <summary>
        /// DbConnectionFactoryの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="oracleConfiguration">Oracle設定</param>
        /// <exception cref="ArgumentNullException">oracleConfigurationがnullの場合</exception>
        public DbConnectionFactory(OracleConfiguration oracleConfiguration)
        {
            _oracleConfiguration = oracleConfiguration ?? throw new ArgumentNullException(nameof(oracleConfiguration));
        }

        /// <summary>
        /// Oracleデータベースへの接続を生成し、オープンします
        /// </summary>
        /// <returns>オープン済みのIDbConnection</returns>
        /// <exception cref="InvalidOperationException">接続の作成に失敗した場合</exception>
        public IDbConnection CreateConnection()
        {
            try
            {
                var connection = new OracleConnection(_oracleConfiguration.ConnectionString);
                connection.Open();
                return connection;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("データベース接続の作成に失敗しました。", ex);
            }
        }
    }
}
