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
            await _authService.LogoutAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var loginUser = new Services.LoginModel
            {
                UserName = UserName,
                Password = Password
            };
            var success = await _authService.LoginAsync(loginUser);

            if (success)
            {
                return Redirect("/dashboard");
            }
            else
            {
                LoginError = true;
                return Page();
            }
        }

    }
}