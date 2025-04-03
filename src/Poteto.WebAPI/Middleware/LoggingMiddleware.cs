using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Events;

namespace Poteto.WebAPI.Middleware
{
    /// <summary>
    /// HTTPリクエストとレスポンスのログを記録するミドルウェア
    /// </summary>
    /// <remarks>
    /// このミドルウェアは以下の情報をログに記録します：
    /// - リクエストの開始と終了
    /// - リクエスト/レスポンスのヘッダー
    /// - リクエスト/レスポンスのボディ（JSONの場合）
    /// - 処理時間
    /// - エラー情報
    /// </remarks>
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        /// <summary>
        /// LoggingMiddlewareの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="next">次のミドルウェアへのデリゲート</param>
        /// <param name="logger">ロガーインスタンス</param>
        /// <exception cref="ArgumentNullException">nextまたはloggerがnullの場合</exception>
        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// リクエストを処理し、ログを記録します
        /// </summary>
        /// <param name="context">HTTPコンテキスト</param>
        /// <returns>非同期操作を表すタスク</returns>
        /// <exception cref="Exception">リクエスト処理中にエラーが発生した場合</exception>
        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestId = Guid.NewGuid().ToString();

            try
            {
                // リクエスト情報のログ出力
                LogRequest(context, requestId);

                // リクエストボディの読み取り（必要な場合）
                if (context.Request.ContentLength > 0 &&
                    context.Request.ContentType?.Contains("application/json") == true)
                {
                    context.Request.EnableBuffering();
                    var requestBody = await ReadRequestBodyAsync(context.Request);
                    _logger.LogInformation("Request Body: {RequestBody}", requestBody);
                    context.Request.Body.Position = 0;
                }

                // 元のレスポンスストリームを保持
                var originalBodyStream = context.Response.Body;
                using var responseBody = new MemoryStream();
                context.Response.Body = responseBody;

                // 次のミドルウェアまたはエンドポイントへ処理を委譲
                await _next(context);

                // レスポンス情報のログ出力
                await LogResponseAsync(context, responseBody, originalBodyStream, requestId, stopwatch);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "リクエスト処理中にエラーが発生しました: {RequestId}", requestId);
                throw;
            }
        }

        /// <summary>
        /// リクエスト情報をログに記録します
        /// </summary>
        /// <param name="context">HTTPコンテキスト</param>
        /// <param name="requestId">リクエストID</param>
        private void LogRequest(HttpContext context, string requestId)
        {
            var request = context.Request;
            var logLevel = GetLogLevelForRequest(request);

            _logger.Log(logLevel,
                "リクエスト開始: {RequestId} {Method} {Path} {QueryString}",
                requestId,
                request.Method,
                request.Path,
                request.QueryString);

            _logger.LogDebug("リクエストヘッダー: {Headers}",
                string.Join(", ", request.Headers.Select(h => $"{h.Key}: {h.Value}")));
        }

        /// <summary>
        /// レスポンス情報をログに記録します
        /// </summary>
        /// <param name="context">HTTPコンテキスト</param>
        /// <param name="responseBody">レスポンスボディのストリーム</param>
        /// <param name="originalBodyStream">元のレスポンスストリーム</param>
        /// <param name="requestId">リクエストID</param>
        /// <param name="stopwatch">処理時間計測用のストップウォッチ</param>
        /// <returns>非同期操作を表すタスク</returns>
        private async Task LogResponseAsync(
            HttpContext context,
            MemoryStream responseBody,
            Stream originalBodyStream,
            string requestId,
            Stopwatch stopwatch)
        {
            stopwatch.Stop();

            // レスポンスボディの読み取り
            responseBody.Seek(0, SeekOrigin.Begin);
            var responseBodyText = await new StreamReader(responseBody).ReadToEndAsync();
            responseBody.Seek(0, SeekOrigin.Begin);

            // レスポンスを元のストリームにコピー
            await responseBody.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;

            var logLevel = GetLogLevelForResponse(context.Response.StatusCode);

            _logger.Log(logLevel,
                "リクエスト完了: {RequestId} {StatusCode} {ElapsedMilliseconds}ms",
                requestId,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);

            if (!string.IsNullOrEmpty(responseBodyText))
            {
                _logger.LogDebug("レスポンスボディ: {ResponseBody}", responseBodyText);
            }
        }

        /// <summary>
        /// リクエストメソッドに基づいて適切なログレベルを決定します
        /// </summary>
        /// <param name="request">HTTPリクエスト</param>
        /// <returns>決定されたログレベル</returns>
        private static LogLevel GetLogLevelForRequest(HttpRequest request)
        {
            // 重要な操作はInfoレベルでログ
            if (request.Method == HttpMethods.Post ||
                request.Method == HttpMethods.Put ||
                request.Method == HttpMethods.Delete)
            {
                return LogLevel.Information;
            }
            return LogLevel.Debug;
        }

        /// <summary>
        /// レスポンスステータスコードに基づいて適切なログレベルを決定します
        /// </summary>
        /// <param name="statusCode">HTTPステータスコード</param>
        /// <returns>決定されたログレベル</returns>
        private static LogLevel GetLogLevelForResponse(int statusCode)
        {
            if (statusCode >= 500)
                return LogLevel.Error;
            if (statusCode >= 400)
                return LogLevel.Warning;
            return LogLevel.Information;
        }

        /// <summary>
        /// リクエストボディを読み取り、文字列として返します
        /// </summary>
        /// <param name="request">HTTPリクエスト</param>
        /// <returns>リクエストボディの文字列</returns>
        private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            request.EnableBuffering();
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            request.Body.Position = 0;
            return bodyAsText;
        }
    }
}
