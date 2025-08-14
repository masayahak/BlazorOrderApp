using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorOrderApp.Components.Commons
{
    public abstract class InitialFocusComponent : ComponentBase, IDisposable, IAsyncDisposable
    {
        [Inject] protected IJSRuntime JS { get; set; } = default!;

        // 最初にフォーカスしたい data-tab（各ページで override 可）
        protected virtual int FirstTabOrder => 0;

        // 初回レンダ後に追加でやりたい処理（任意）
        protected virtual Task OnFirstRenderAsync() => Task.CompletedTask;

        protected sealed override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender) return;

            await Task.Yield(); // DOM安定待ち
            try
            {
                await JS.InvokeVoidAsync("focusByTabOrder", FirstTabOrder);
            }
            catch
            {
                // focusByTabOrder 未読込でも落ちないように握りつぶす
            }
            await OnFirstRenderAsync();
        }

        public void Dispose() => GC.SuppressFinalize(this);
        public ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);
            return ValueTask.CompletedTask;
        }
    }
}
