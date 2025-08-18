using System.Collections.Concurrent;

namespace BlazorOrderApp.Services
{
    public sealed class UserActivityService
    {
        private readonly ConcurrentDictionary<string, DateTimeOffset> _last = new();

        public void Touch(string userName)
            => _last[userName] = DateTimeOffset.UtcNow;

        public DateTimeOffset? GetLast(string userName)
            => _last.TryGetValue(userName, out var ts) ? ts : (DateTimeOffset?)null;
    }
}
