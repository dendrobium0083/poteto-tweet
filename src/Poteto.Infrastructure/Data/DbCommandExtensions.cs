using System;
using System.Data;

namespace Poteto.Infrastructure.Data
{
    /// <summary>
    /// IDbCommandの拡張メソッド
    /// </summary>
    public static class DbCommandExtensions
    {
        /// <summary>
        /// パラメータを追加します
        /// </summary>
        /// <param name="command">コマンド</param>
        /// <param name="name">パラメータ名</param>
        /// <param name="value">パラメータ値</param>
        public static void AddParameter(this IDbCommand command, string name, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }
    }
}
