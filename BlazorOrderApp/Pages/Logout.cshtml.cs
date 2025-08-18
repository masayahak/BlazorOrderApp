using BlazorOrderApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlazorOrderApp.Pages
{
    public class LogoutModel : PageModel
    {
        private readonly MyAuthenticationService _authService;
        public LogoutModel(MyAuthenticationService authService)
            => _authService = authService;

        public async Task<IActionResult> OnGetAsync()
        {
            await _authService.LogoutAsync();
            return RedirectToPage("/login");
        }
    }
}
