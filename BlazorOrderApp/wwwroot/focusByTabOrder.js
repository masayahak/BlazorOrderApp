(function () {
    // 既に導入済みなら二重登録しない
    if (window._enterNavInstalled) return;
    window._enterNavInstalled = true;

    // ---- focusByTabOrder を window に公開 ----
    window.focusByTabOrder = function (current, dir = 1) {
        const isFocusable = el =>
            !el.hasAttribute('disabled') &&
            getComputedStyle(el).visibility !== 'hidden' &&
            getComputedStyle(el).display !== 'none' &&
            el.getClientRects().length > 0;

        const all = Array.from(document.querySelectorAll('[data-tab]'))
            .filter(isFocusable)
            .map(el => ({ el, order: Number(el.getAttribute('data-tab')) }))
            .filter(x => Number.isFinite(x.order))
            .sort((a, b) => (a.order - b.order) ||
                ((a.el.compareDocumentPosition(b.el) & Node.DOCUMENT_POSITION_FOLLOWING) ? -1 : 1));

        if (all.length === 0) return;

        const forward = () => {
            const i = all.findIndex(x => x.order > current);
            return i >= 0 ? i : 0;
        };
        const backward = () => {
            let i = -1;
            for (let k = 0; k < all.length; k++) if (all[k].order < current) i = k;
            return i >= 0 ? i : all.length - 1;
        };

        const pos = dir > 0 ? forward() : backward();
        const target = all[pos]?.el;
        if (!target) return;

        target.focus({ preventScroll: false });
        if (typeof target.select === 'function') { try { target.select(); } catch { } }
        target.scrollIntoView({ block: 'nearest', inline: 'nearest', behavior: 'smooth' });
    };

    // ---- Enterキー制御 ----
    let reentry = false;
    document.addEventListener('keydown', e => {
        if (e.key !== 'Enter' || e.isComposing) return;

        const t = e.target;
        if (!t) return;
        if (t.tagName === 'TEXTAREA' || t.isContentEditable ||
            t.tagName === 'BUTTON' || (t.type && t.type.toLowerCase() === 'submit')) return;

        const cur = Number(t.getAttribute('data-tab'));
        if (!Number.isFinite(cur)) return;

        e.preventDefault();
        e.stopPropagation();
        e.stopImmediatePropagation();

        if (reentry) return;
        reentry = true;
        requestAnimationFrame(() => {
            window.focusByTabOrder(cur, e.shiftKey ? -1 : +1);
            reentry = false;
        });
    }, true);
})();