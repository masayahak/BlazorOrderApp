using BlazorOrderApp.Components;
using BlazorOrderApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState();

// DI
builder.Services.AddApplicationServices();

// グラフ用
builder.Services.AddRadzenComponents();

// ----------- Cookie認証 --------------------------------
// ログイン／ログアウト用のRazor Pages
builder.Services.AddRazorPages();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        // タイムアウトはサーバー側で管理するので余裕を持たせる
        o.ExpireTimeSpan = TimeSpan.FromHours(10);
        o.LoginPath = "/login";
        o.AccessDeniedPath = "/login";
    });
// appsettingのタイムアウト設定
builder.Services.Configure<IdleTimeoutOptions>(
    builder.Configuration.GetSection("IdleTimeout"));

// 独自認証判定（デモ用簡易版）
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<MyAuthenticationService>();

builder.Services.AddAuthorization();
builder.Services.AddSingleton<UserActivityService>(); // 活動記録
// サーバー側でCookieセッションの定期監視
builder.Services.AddScoped<AuthenticationStateProvider, CookieRevalidatingAuthStateProvider>();
// -------------------------------------------------------

// 詳細なエラーを有効化
builder.Services.AddServerSideBlazor(options =>
{
    options.DetailedErrors = true;
});

// Azure Application Insights
builder.Services.AddApplicationInsightsTelemetry();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// 認証・認可
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();
app.MapStaticAssets();

// ログイン／ログアウト用のRazor Pages のエンドポイント追加
// Blazorのマッピングより先に実行
app.MapRazorPages();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
