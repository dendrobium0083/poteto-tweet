using System.Data;

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
}
