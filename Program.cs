using Microsoft.AspNetCore.Authentication;

// アプリケーションビルダーのインスタンスを作成
var builder = WebApplication.CreateBuilder(args);

// 環境ごとにカスタムファイルロガープロバイダーを登録
if (builder.Environment.IsDevelopment()) {
    // 開発環境：デバッグ用ログファイル
    builder.Logging.AddProvider(new PersonalWebApi.Utilities.FileLoggerProvider(
        "logs/dev.log",         // 開発用ログファイルパス
        LogLevel.Debug,         // 最小ログレベル：Debug
        10 * 1024 * 1024        // 最大ファイルサイズ：10MB
    ));
} else {
    // 本番環境：本番用ログファイル
    builder.Logging.AddProvider(new PersonalWebApi.Utilities.FileLoggerProvider(
        "logs/prd.log",         // 本番用ログファイルパス
        LogLevel.Information,   // 最小ログレベル：Information
        10 * 1024 * 1024        // 最大ファイルサイズ：10MB
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
                .AllowAnyMethod()
            ;
    });
});

// アプリケーション設定 "appsettings.json" をPOCOクラスにバインド
builder.Services
    .Configure<PersonalWebApi.Models.Config.AppSettings>(builder.Configuration);

// カスタムAPIキー認証サービスをDIコンテナに追加
builder.Services
    .AddAuthentication("ApiKey")
    .AddScheme<AuthenticationSchemeOptions, PersonalWebApi.Utilities.ApiKeyAuthenticationHandler>("ApiKey", null);

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
