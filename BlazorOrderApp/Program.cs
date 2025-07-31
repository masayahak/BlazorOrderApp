using ApexCharts;
using BlazorOrderApp.Components;
using BlazorOrderApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// DI
builder.Services.AddApplicationServices();

// グラフ用
builder.Services.AddApexCharts();

// ----------- Cookie認証 --------------------------------
// ログイン／ログアウト用のRazor Pages
builder.Services.AddRazorPages();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // 20分で自動的にログアウト
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        // アクセスごとに延長
        options.SlidingExpiration = true;

        options.LoginPath = "/login";
        options.Events.OnRedirectToLogin = context =>
        {
            // 直接アクセス判定（Refererが無い、または空の場合は直接アクセスとみなす）
            var referer = context.Request.Headers.Referer;
            var hasReferer = !string.IsNullOrWhiteSpace(referer);

            // クエリ文字列にexpiredが無い、かつ直接アクセスではないリダイレクトは全てタイムアウト扱い
            var redirectUri = context.RedirectUri;
            if (!redirectUri.Contains("expired"))
            {
                var uri = new UriBuilder(redirectUri);
                var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                query["expired"] = hasReferer ? "1" : "0";
                uri.Query = query.ToString();
                context.Response.Redirect(uri.ToString());
            }
            else
            {
                context.Response.Redirect(redirectUri);
            }
            return Task.CompletedTask;
        };
    });


builder.Services.AddCascadingAuthenticationState();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<MyAuthenticationService>();
// -------------------------------------------------------

// 詳細なエラーを有効化
builder.Services.AddServerSideBlazor(options =>
{
    options.DetailedErrors = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

// 認証・認可
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// ログイン／ログアウト用のRazor Pages のエンドポイント追加
// Blazorのマッピングより先に実行
app.MapRazorPages();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
