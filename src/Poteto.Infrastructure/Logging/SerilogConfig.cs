using System;
using Serilog;
using Serilog.Events;

namespace Poteto.Infrastructure.Logging
{
    public static class SerilogConfig
    {
        /// <summary>
        /// Serilog のロガーを環境に応じて設定します。
        /// </summary>
        /// <param name="environment">実行環境 ("Development" or "Production")</param>
        /// <param name="logFilePathPattern">
        /// ログファイルの出力パス。コントローラ毎のログファイルの場合は、プレースホルダ（例: {Controller}）を利用可能。
        /// 例: "logs/{Controller}-.log"
        /// </param>
        public static void ConfigureLogger(string environment, string logFilePathPattern)
        {
            if (string.IsNullOrWhiteSpace(environment))
                throw new ArgumentException("環境名は必須です。", nameof(environment));

            if (string.IsNullOrWhiteSpace(logFilePathPattern))
                throw new ArgumentException("ログファイルパスパターンは必須です。", nameof(logFilePathPattern));

            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.Debug();

            // 開発環境の場合、コンソール出力を追加
            if (environment.Equals("Development", StringComparison.OrdinalIgnoreCase))
            {
                loggerConfig = loggerConfig.WriteTo.Console();
            }

            // 常にファイル出力（コントローラ毎に出力するため、ファイル名にプレースホルダを利用する前提）
            loggerConfig = loggerConfig.WriteTo.File(
                path: logFilePathPattern,
                rollingInterval: RollingInterval.Day,
                restrictedToMinimumLevel: LogEventLevel.Information,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
            );

            Log.Logger = loggerConfig.CreateLogger();
        }
    }
}
