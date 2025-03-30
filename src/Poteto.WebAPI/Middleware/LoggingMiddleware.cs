using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using Serilog;

namespace Poteto.WebAPI.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // リクエスト処理開始前にタイマー開始
            var stopwatch = Stopwatch.StartNew();

            // リクエスト情報のログ出力
            Log.Information("Handling request: {Method} {Path}", context.Request.Method, context.Request.Path);

            // 次のミドルウェアまたはエンドポイントへ処理を委譲
            await _next(context);

            // リクエスト処理終了後にタイマー停止
            stopwatch.Stop();

            // レスポンス情報と実行時間のログ出力
            Log.Information("Finished handling request: {StatusCode} in {ElapsedMilliseconds} ms",
                context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
        }
    }
}
