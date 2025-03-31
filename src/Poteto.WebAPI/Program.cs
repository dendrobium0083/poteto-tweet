using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Poteto.Application.Interfaces.Repositories;
using Poteto.Application.Interfaces.Services;
using Poteto.Application.Services;
using Poteto.Domain.DomainServices;
using Poteto.Infrastructure.Configurations;
using Poteto.Infrastructure.Data;
using Poteto.Infrastructure.Logging;
using Poteto.WebAPI.Middleware;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

// appsettings.json から接続文字列を取得
string? connectionString = builder.Configuration.GetConnectionString("OracleConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("OracleConnection の接続文字列が設定されていません。");
}

// Serilog の設定（環境に応じたログ出力の設定例）
var environment = builder.Environment.EnvironmentName;
var logFilePathPattern = "logs/{Date}.log";
SerilogConfig.ConfigureLogger(environment, logFilePathPattern);
builder.Host.UseSerilog();

// コントローラー、Swagger などのサービスを DI コンテナに登録
builder.Services.AddControllers();
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

// アプリケーション層のサービス登録
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITweetService, TweetService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IFollowService, FollowService>();
builder.Services.AddScoped<IBlockService, BlockService>();
builder.Services.AddScoped<ILikeService, LikeService>();

// ドメイン層のサービス登録
builder.Services.AddScoped<TweetDomainService>();

// インフラ層のリポジトリ登録
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITweetRepository, TweetRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IFollowRepository, FollowRepository>();
builder.Services.AddScoped<IBlockRepository, BlockRepository>();
builder.Services.AddScoped<ILikeRepository, LikeRepository>();

// OracleConfiguration をシングルトンで登録し、接続文字列を注入
builder.Services.AddSingleton(new OracleConfiguration(connectionString));

// DbConnectionFactory は OracleConfiguration を受け取るので引数なしで登録できる
builder.Services.AddScoped<DbConnectionFactory>();

// Unit of Work の登録
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

// 開発環境の場合、Swagger UI を有効化
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// カスタムミドルウェア (例: LoggingMiddleware) の登録
app.UseMiddleware<LoggingMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
