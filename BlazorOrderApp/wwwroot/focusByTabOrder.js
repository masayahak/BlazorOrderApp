window.focusByTabOrder = function (current) {
    const nodes = Array.from(document.querySelectorAll('[data-tab]'))
        .filter(el => !el.hasAttribute('disabled') && el.offsetParent !== null);
    if (nodes.length === 0) return;

    const orderList = Array.from(new Set(
        nodes.map(el => Number(el.getAttribute('data-tab'))).filter(n => !Number.isNaN(n))
    )).sort((a, b) => a - b);

    let next = orderList.find(n => n > current);
    if (next == null) next = orderList[0];

    const target = nodes.find(el => Number(el.getAttribute('data-tab')) === next);
    if (target) {
        target.focus();
        if (typeof target.select === 'function') { try { target.select(); } catch { } }
        target.scrollIntoView({ block: 'nearest' });
    }
};