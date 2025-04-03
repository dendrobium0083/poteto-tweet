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

// 設定の検証
ValidateConfiguration(builder.Configuration);

// ロギングの設定
ConfigureLogging(builder);

// サービスの登録
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// ミドルウェアの設定
ConfigureMiddleware(app, builder.Environment);

app.Run();

/// <summary>
/// 設定の検証を行います
/// </summary>
/// <param name="configuration">アプリケーション設定</param>
/// <exception cref="InvalidOperationException">必須の設定が不足している場合</exception>
static void ValidateConfiguration(IConfiguration configuration)
{
    string? connectionString = configuration.GetConnectionString("OracleConnection");
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException("OracleConnection の接続文字列が設定されていません。");
    }
}

/// <summary>
/// ロギングの設定を行います
/// </summary>
/// <param name="builder">WebApplicationBuilder</param>
static void ConfigureLogging(WebApplicationBuilder builder)
{
    var environment = builder.Environment.EnvironmentName;
    var logFilePathPattern = "logs/{Date}.log";
    SerilogConfig.ConfigureLogger(environment, logFilePathPattern);
    builder.Host.UseSerilog();
}

/// <summary>
/// サービスの登録を行います
/// </summary>
/// <param name="services">IServiceCollection</param>
/// <param name="configuration">IConfiguration</param>
static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // コントローラー、Swagger などのサービスを登録
    services.AddControllers();
    if (configuration.GetValue<bool>("EnableSwagger"))
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    // アプリケーション層のサービス登録
    RegisterApplicationServices(services);

    // ドメイン層のサービス登録
    RegisterDomainServices(services);

    // インフラ層のリポジトリ登録
    RegisterInfrastructureServices(services, configuration);
}

/// <summary>
/// アプリケーション層のサービスを登録します
/// </summary>
/// <param name="services">IServiceCollection</param>
static void RegisterApplicationServices(IServiceCollection services)
{
    services.AddScoped<IUserService, UserService>();
    services.AddScoped<ITweetService, TweetService>();
    services.AddScoped<ICommentService, CommentService>();
    services.AddScoped<IFollowService, FollowService>();
    services.AddScoped<IBlockService, BlockService>();
    services.AddScoped<ILikeService, LikeService>();
}

/// <summary>
/// ドメイン層のサービスを登録します
/// </summary>
/// <param name="services">IServiceCollection</param>
static void RegisterDomainServices(IServiceCollection services)
{
    services.AddScoped<TweetDomainService>();
}

/// <summary>
/// インフラ層のサービスを登録します
/// </summary>
/// <param name="services">IServiceCollection</param>
/// <param name="configuration">IConfiguration</param>
static void RegisterInfrastructureServices(IServiceCollection services, IConfiguration configuration)
{
    // リポジトリの登録
    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<ITweetRepository, TweetRepository>();
    services.AddScoped<ICommentRepository, CommentRepository>();
    services.AddScoped<IFollowRepository, FollowRepository>();
    services.AddScoped<IBlockRepository, BlockRepository>();
    services.AddScoped<ILikeRepository, LikeRepository>();

    // データベース関連の設定
    var connectionString = configuration.GetConnectionString("OracleConnection")!;
    services.AddSingleton(new OracleConfiguration(connectionString));
    services.AddScoped<DbConnectionFactory>();
    services.AddScoped<IUnitOfWork, UnitOfWork>();
}

/// <summary>
/// ミドルウェアの設定を行います
/// </summary>
/// <param name="app">WebApplication</param>
/// <param name="environment">IHostEnvironment</param>
static void ConfigureMiddleware(WebApplication app, IHostEnvironment environment)
{
    if (environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseMiddleware<LoggingMiddleware>();
    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
}
