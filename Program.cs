using Microsoft.AspNetCore.Authentication;

// アプリケーションビルダーのインスタンスを作成
var builder = WebApplication.CreateBuilder(args);

// AppSettingsのバインド
builder.Services.Configure<PersonalWebApi.Models.Config.AppSettings>(builder.Configuration);

// FileOutput設定を取得
var fileOutputSection = builder.Configuration.GetSection("Logging:FileOutput");
var fileOutput = fileOutputSection.Get<PersonalWebApi.Models.Config.FileOutput>() ?? new();

// FileLoggerProviderを設定
if (fileOutput.Enabled) {
    builder.Logging.AddProvider(new PersonalWebApi.Utilities.FileLoggerProvider(
        fileOutput.FilePath,
        Enum.TryParse<LogLevel>(builder.Configuration["Logging:LogLevel:Default"], out var level) ? level : LogLevel.Information,
        fileOutput.MaxFileSizeKB * 1024 // KB → Bytes
    ));
}

// コントローラーサービスをDIコンテナに追加
builder.Services.AddControllers();

// Swagger/OpenAPIサービスを登録（APIドキュメント・テスト用）
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c => {
    c.DocInclusionPredicate((docName, apiDesc) => {
        // BaseControllerはSwaggerに含めない
        return apiDesc.ActionDescriptor?.DisplayName?.Contains("BaseController") != true;
    });
});

// CORSポリシーを登録
builder.Services
    .AddCors(options => {
        options.AddPolicy("AllowAll", policy => {
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });

// カスタムAPIキー認証サービスをDIコンテナに追加
builder.Services
    .AddAuthentication("ApiKey")
    .AddScheme<AuthenticationSchemeOptions, PersonalWebApi.Utilities.ApiKeyAuthenticationHandler>("ApiKey", null);

// HTTPクライアントをDIコンテナに追加
builder.Services.AddHttpClient();

// アプリケーションインスタンスをビルド
var app = builder.Build();

// リッスンするURLを設定
app.Urls.Add("https://localhost:5001");
app.Urls.Add("http://localhost:5000");

// CORSを有効化
app.UseCors("AllowAll");

// HTTPリクエストパイプラインの構成
if (app.Environment.IsDevelopment()) {
    // 開発環境のみSwagger UIを有効化
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPSリダイレクトミドルウェアを有効化
app.UseHttpsRedirection();

// 認証・認可ミドルウェアを追加
app.UseAuthentication();
app.UseAuthorization();

// コントローラーのエンドポイントをマッピング
app.MapControllers();

// アプリケーションを起動
app.Run();
