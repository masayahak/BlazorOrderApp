using BlazorOrderApp.Components;
using BlazorOrderApp.Repositories;
using BlazorOrderApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<I商品Repository, 商品Repository>();
builder.Services.AddScoped<I得意先Repository, 得意先Repository>();
builder.Services.AddScoped<I受注Repository, 受注Repository>();

// ----------- Cookie認証 --------------------------------
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // 20分で自動的にログアウト
        options.ExpireTimeSpan = TimeSpan.FromMinutes(1);
        // アクセスごとに延長
        options.SlidingExpiration = true; 

        options.LoginPath = "/login";
        options.Events.OnRedirectToLogin = context =>
        {
            // 直接アクセス判定（Refererが無い、または空の場合は直接アクセスとみなす）
            var hasReferer = context.Request.Headers.ContainsKey("Referer")
                && !string.IsNullOrWhiteSpace(context.Request.Headers["Referer"]);

            // クエリ文字列にexpiredが無い、かつ直接アクセスではないリダイレクトは全てタイムアウト扱い
            var redirectUri = context.RedirectUri;
            if (!redirectUri.Contains("expired"))
            {
                var uri = new UriBuilder(redirectUri);
                var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                query["expired"] = hasReferer ? "1" : "0"; // 直接なら0、それ以外（遷移）は1
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
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
