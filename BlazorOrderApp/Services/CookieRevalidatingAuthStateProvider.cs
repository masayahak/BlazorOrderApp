using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.Extensions.Options;

namespace BlazorOrderApp.Services
{
    // =============================================================
    // サーバー側で定期的にセッションが有効か監視する
    // BlazorのSignalR通信は一度開始されると、セッションがタイムアウトしても
    // 自動では切断されない。
    // =============================================================

    public sealed class CookieRevalidatingAuthStateProvider
    : RevalidatingServerAuthenticationStateProvider
    {
        private readonly UserActivityService _activity;
        private readonly IdleTimeoutOptions _options;

        public CookieRevalidatingAuthStateProvider(
            ILoggerFactory loggerFactory,
            UserActivityService activity,
            IOptions<IdleTimeoutOptions> options) : base(loggerFactory)
        {
            _activity = activity;
            _options = options.Value;
        }

        // サーバー上でセッションが有効か監視する間隔（1分毎）
        protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(1);

        // ---------------------------
        // 定期監視イベント
        // ---------------------------
        protected override Task<bool> ValidateAuthenticationStateAsync(
            AuthenticationState state, CancellationToken token)
        {
            // この処理がflaseを返すと、Blazorはセッションが終わったと認識する
            // つまりここでflaseを返すことはlogoutと同じ
 
            // そもそもCookieセッションが無効か？
            var user = state.User;
            if (user?.Identity?.IsAuthenticated != true)
                return Task.FromResult(false);

            var name = user.Identity.Name ?? string.Empty;
            var last = _activity.GetLast(name);
            if (last is null) return Task.FromResult(true);

            // 有効なアイドル時間か判定
            var idle = DateTimeOffset.UtcNow - last.Value;
            var ok = idle < TimeSpan.FromMinutes(_options.IdleTimeoutMinutes);
            return Task.FromResult(ok);
        }
    }

}