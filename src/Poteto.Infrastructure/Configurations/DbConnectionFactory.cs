using System;
using System.Data;

using Oracle.ManagedDataAccess.Client;

namespace Poteto.Infrastructure.Configurations
{
    public class DbConnectionFactory
    {
        private readonly OracleConfiguration _oracleConfiguration;

        public DbConnectionFactory(OracleConfiguration oracleConfiguration)
        {
            _oracleConfiguration = oracleConfiguration ?? throw new ArgumentNullException(nameof(oracleConfiguration));
        }

        /// <summary>
        /// Oracle データベースへの接続を生成し、オープンします。
        /// </summary>
        /// <returns>オープン済みの IDbConnection</returns>
        public IDbConnection CreateConnection()
        {
            var connection = new OracleConnection(_oracleConfiguration.ConnectionString);
            connection.Open();
            return connection;
        }
    }
}
