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
        options.LoginPath = "/login";
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
