using System;
using System.Collections.Generic;

using Serilog;
using Serilog.Events;
using Serilog.Sinks.File;

namespace Poteto.Infrastructure.Logging
{
    /// <summary>
    /// Serilogの設定を管理するクラス
    /// </summary>
    public static class SerilogConfig
    {
        /// <summary>
        /// デフォルトのログ出力テンプレート
        /// </summary>
        private const string DefaultOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

        /// <summary>
        /// Serilogのロガーを環境に応じて設定します
        /// </summary>
        /// <param name="environment">実行環境（"Development"または"Production"）</param>
        /// <param name="logFilePathPattern">ログファイルの出力パスパターン</param>
        /// <param name="minimumLevel">最小ログレベル（デフォルト: Debug）</param>
        /// <param name="fileMinimumLevel">ファイル出力の最小ログレベル（デフォルト: Information）</param>
        /// <param name="outputTemplate">ログ出力テンプレート（デフォルト: 標準テンプレート）</param>
        /// <exception cref="ArgumentException">環境名またはログファイルパスパターンが無効な場合</exception>
        public static void ConfigureLogger(
            string environment,
            string logFilePathPattern,
            LogEventLevel minimumLevel = LogEventLevel.Debug,
            LogEventLevel fileMinimumLevel = LogEventLevel.Information,
            string? outputTemplate = null)
        {
            ValidateParameters(environment, logFilePathPattern);

            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.Is(minimumLevel)
                .Enrich.FromLogContext();

            // 開発環境の場合、コンソール出力を追加
            if (IsDevelopmentEnvironment(environment))
            {
                loggerConfig = loggerConfig.WriteTo.Console(
                    outputTemplate: outputTemplate ?? DefaultOutputTemplate);
            }

            // ファイル出力の設定
            ConfigureFileSink(loggerConfig, logFilePathPattern, fileMinimumLevel, outputTemplate);

            // ロガーの作成と設定
            Log.Logger = loggerConfig.CreateLogger();
        }

        /// <summary>
        /// パラメータの検証を行います
        /// </summary>
        /// <param name="environment">実行環境</param>
        /// <param name="logFilePathPattern">ログファイルパスパターン</param>
        /// <exception cref="ArgumentException">パラメータが無効な場合</exception>
        private static void ValidateParameters(string environment, string logFilePathPattern)
        {
            if (string.IsNullOrWhiteSpace(environment))
            {
                throw new ArgumentException("環境名は必須です。", nameof(environment));
            }

            if (string.IsNullOrWhiteSpace(logFilePathPattern))
            {
                throw new ArgumentException("ログファイルパスパターンは必須です。", nameof(logFilePathPattern));
            }
        }

        /// <summary>
        /// 開発環境かどうかを判定します
        /// </summary>
        /// <param name="environment">実行環境</param>
        /// <returns>開発環境の場合はtrue、それ以外はfalse</returns>
        private static bool IsDevelopmentEnvironment(string environment)
        {
            return environment.Equals("Development", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// ファイルシンクの設定を行います
        /// </summary>
        /// <param name="loggerConfig">ロガー設定</param>
        /// <param name="logFilePathPattern">ログファイルパスパターン</param>
        /// <param name="minimumLevel">最小ログレベル</param>
        /// <param name="outputTemplate">ログ出力テンプレート</param>
        private static void ConfigureFileSink(
            LoggerConfiguration loggerConfig,
            string logFilePathPattern,
            LogEventLevel minimumLevel,
            string? outputTemplate)
        {
            loggerConfig.WriteTo.File(
                path: logFilePathPattern,
                rollingInterval: RollingInterval.Day,
                restrictedToMinimumLevel: minimumLevel,
                outputTemplate: outputTemplate ?? DefaultOutputTemplate,
                retainedFileCountLimit: 31, // 約1ヶ月分のログを保持
                fileSizeLimitBytes: 10 * 1024 * 1024, // 10MB
                rollOnFileSizeLimit: true,
                shared: true
            );
        }
    }
}
