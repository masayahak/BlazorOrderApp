// ログインページでは何もしない
if (location.pathname.startsWith("/login")) {
    return;
}

let timeoutMinutes = 1; // 例：1分（本番は20などに）
let timeoutHandle = null;

// ------------------------------------------
// タイムアウト表示処理
// ------------------------------------------
function showTimeoutOverlay() {

    // 既存オーバーレイがあれば削除
    const old = document.getElementById('timeout-overlay');
    if (old) old.remove();

    // オーバーレイ本体
    const overlay = document.createElement('div');
    overlay.id = 'timeout-overlay';
    overlay.style = `
        position: fixed; z-index: 9999; top: 0; left: 0; width: 100vw; height: 100vh;
        background: rgba(0,0,0,0.4); display: flex; align-items: center; justify-content: center;
    `;

    // モーダル内容
    overlay.innerHTML = `
        <div style="
            background: #fff;
            padding: 2.5rem 2rem;
            border-radius: 16px;
            box-shadow: 0 8px 24px rgba(0,0,0,0.18);
            min-width: 320px;
            text-align: center;">
            <div style="font-size:2.2rem; color:#F55; margin-bottom:0.5em;">
                <svg xmlns="http://www.w3.org/2000/svg" width="38" height="38" fill="currentColor" class="bi bi-clock-history" viewBox="0 0 16 16">
                  <path d="M8.515 3.547a.5.5 0 0 0-.53.847l2.0 1.25V8a.5.5 0 0 0 1 0V5.293a.5.5 0 0 0-.255-.434l-2-1.25z"/>
                  <path d="M8 15A7 7 0 1 1 8 1a7 7 0 0 1 0 14zm0-1A6 6 0 1 0 8 2a6 6 0 0 0 0 12z"/>
                </svg>
                <br>タイムアウトしました
            </div>
            <div style="font-size:1.15rem; color:#555;">しばらく操作がなかったため<br>自動的にログアウトします。</div>
            <div style="margin-top:1.2em;">
                <span style="font-size:1.1rem; color:#666;" id="timeout-count">3</span> 秒後にログイン画面へ戻ります...
            </div>
        </div>
    `;
    document.body.appendChild(overlay);

    // カウントダウン→リダイレクト
    let remain = 5;
    const timer = setInterval(() => {
        remain--;
        document.getElementById('timeout-count').innerText = remain;
        if (remain <= 0) {
            clearInterval(timer);
            window.location.href = '/login?expired=1';
        }
    }, 1000);
}

// ------------------------------------------
// タイムアウト判定処理
// ------------------------------------------
function resetTimeout() {
    if (timeoutHandle) clearTimeout(timeoutHandle);
    timeoutHandle = setTimeout(() => {
        showTimeoutOverlay();
    }, timeoutMinutes * 60 * 1000);
}

document.addEventListener('keydown', resetTimeout); // キー操作
document.addEventListener('mousedown', resetTimeout); // クリック
//document.addEventListener('mousemove', resetTimeout); // マウス操作
//document.addEventListener('touchstart', resetTimeout); // スマホ

// 起動時初期化
resetTimeout();
