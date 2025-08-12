using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace BlazorOrderApp.Components.Commons
{
    public abstract class FocusNavigableComponent : ComponentBase
    {
        [Inject] protected IJSRuntime JS { get; set; } = default!;

        // 先頭フォーカスさせたいタブ番号（必要に応じて派生側で override）
        protected virtual int FirstTabOrder => 0;

        // 派生側で描画後に追加処理したい場合のフック
        protected virtual Task OnFirstRenderAsync() => Task.CompletedTask;
        protected virtual Task AfterEveryRenderAsync(bool firstRender) => Task.CompletedTask;

        // ここを sealed にして、必ず初期フォーカスが走るように固定
        protected sealed override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("focusByTabOrder", FirstTabOrder);
                await OnFirstRenderAsync();
            }
            await AfterEveryRenderAsync(firstRender);
        }

        // 共通 KeyDown（Enterで次へ）
        protected Task HandleKeyDown(KeyboardEventArgs e, int currentTabOrder)
            => (e.Key == "Enter")
                ? JS.InvokeVoidAsync("focusByTabOrder", currentTabOrder).AsTask()
                : Task.CompletedTask;
    }
}
