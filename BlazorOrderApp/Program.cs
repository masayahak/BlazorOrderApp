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

// �O���t�p
builder.Services.AddRadzenComponents();

// ----------- Cookie�F�� --------------------------------
// ���O�C���^���O�A�E�g�p��Razor Pages
builder.Services.AddRazorPages();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        // �^�C���A�E�g�̓T�[�o�[���ŊǗ�����̂ŗ]�T����������
        o.ExpireTimeSpan = TimeSpan.FromHours(10);
        o.LoginPath = "/login";
        o.AccessDeniedPath = "/login";
    });
// appsetting�̃^�C���A�E�g�ݒ�
builder.Services.Configure<IdleTimeoutOptions>(
    builder.Configuration.GetSection("IdleTimeout"));

// �Ǝ��F�ؔ���i�f���p�ȈՔŁj
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<MyAuthenticationService>();

builder.Services.AddAuthorization();
builder.Services.AddSingleton<UserActivityService>(); // �����L�^
// �T�[�o�[����Cookie�Z�b�V�����̒���Ď�
builder.Services.AddScoped<AuthenticationStateProvider, CookieRevalidatingAuthStateProvider>();
// -------------------------------------------------------

// �ڍׂȃG���[��L����
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

// �F�؁E�F��
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();
app.MapStaticAssets();

// ���O�C���^���O�A�E�g�p��Razor Pages �̃G���h�|�C���g�ǉ�
// Blazor�̃}�b�s���O����Ɏ��s
app.MapRazorPages();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
