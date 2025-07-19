using BlazorOrderApp.Components;
using BlazorOrderApp.Repositories;
using BlazorOrderApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<I���iRepository, ���iRepository>();
builder.Services.AddScoped<I���Ӑ�Repository, ���Ӑ�Repository>();
builder.Services.AddScoped<I��Repository, ��Repository>();

// ----------- Cookie�F�� --------------------------------
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // 20���Ŏ����I�Ƀ��O�A�E�g
        options.ExpireTimeSpan = TimeSpan.FromMinutes(1);
        // �A�N�Z�X���Ƃɉ���
        options.SlidingExpiration = true; 

        options.LoginPath = "/login";
        options.Events.OnRedirectToLogin = context =>
        {
            // ���ڃA�N�Z�X����iReferer�������A�܂��͋�̏ꍇ�͒��ڃA�N�Z�X�Ƃ݂Ȃ��j
            var hasReferer = context.Request.Headers.ContainsKey("Referer")
                && !string.IsNullOrWhiteSpace(context.Request.Headers["Referer"]);

            // �N�G���������expired�������A�����ڃA�N�Z�X�ł͂Ȃ����_�C���N�g�͑S�ă^�C���A�E�g����
            var redirectUri = context.RedirectUri;
            if (!redirectUri.Contains("expired"))
            {
                var uri = new UriBuilder(redirectUri);
                var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                query["expired"] = hasReferer ? "1" : "0"; // ���ڂȂ�0�A����ȊO�i�J�ځj��1
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

// �ڍׂȃG���[��L����
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

// �F�؁E�F��
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
