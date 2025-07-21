using Microsoft.AspNetCore.Mvc.RazorPages;
using BlazorOrderApp.Services;

namespace BlazorOrderApp.Pages
{
    public class LogoutModel : PageModel
    {
        private readonly MyAuthenticationService _authService;
        public LogoutModel(MyAuthenticationService authService)
            => _authService = authService;

        public async Task OnGetAsync()
        {
            await _authService.LogoutAsync();
            // ��ʕ\����Ameta�^�O�� /login �ɑJ��
        }
    }
}