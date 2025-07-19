using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BlazorOrderApp.Services
{
    public class MyAuthenticationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MyAuthenticationService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<bool> LoginAsync(LoginModel loginUser)
        {

            var claims = new List<Claim>();

            // 本来の認証判定はもっと複雑だが、ここではテスト用にシンプルに認証
            if (loginUser.UserName == "admin" && loginUser.Password == "admin")
            {
                claims.Add(new Claim(ClaimTypes.Name, loginUser.UserName));
                claims.Add(new Claim(ClaimTypes.Role, "Administrator"));
            }
            else if (loginUser.UserName == "test" && loginUser.Password == "test")
            {
                claims.Add(new Claim(ClaimTypes.Name, loginUser.UserName));
                claims.Add(new Claim(ClaimTypes.Role, "User"));
            }
            else
            {
                return false;
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                throw new InvalidOperationException("HttpContext is not available.");

            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));
            return true;

        }
        public async Task LogoutAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return;

            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }

    public class LoginModel
    {
        public string UserName { get; set; } = "";
        public string Password { get; set; } = "";
    }

}