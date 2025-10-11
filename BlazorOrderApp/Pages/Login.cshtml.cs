using BlazorOrderApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlazorOrderApp.Pages
{
    public class LoginModel : PageModel
    {
        // 自作認証サービスのDI
        private readonly MyAuthenticationService _authService;
        public LoginModel(MyAuthenticationService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        public string UserName { get; set; } = "";
        [BindProperty]
        public string Password { get; set; } = "";

        // VIEWへ状態を表示するためにpublic
        public bool LoginError { get; set; } = false;
        public bool Expired { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // expired=1 のときだけ true
            Expired = Request.Query["expired"] == "1";
            if (Expired)
                await _authService.LogoutAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string handler)
        {
            switch (handler)
            {
                case "Login":
                    return await Login(UserName, Password);
                case "DemoAdmin":
                    return await Login("admin", "admin765");
                case "DemoUser":
                    return await Login("test", "test326");
                default:
                    return Page();
            }
        }

        private async Task<IActionResult> Login(string user, string pass)
        {
            var loginUser = new Services.LoginModel
            {
                UserName = user,
                Password = pass
            };

            var success = await _authService.LoginAsync(loginUser);

            if (success)
                return Redirect("/dashboard");

            LoginError = true;
            return Page();
        }

    }
}