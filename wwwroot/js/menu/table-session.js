(function () {
    const TABLE = "@Model.Table";
    const SESSION_KEY = 'qr_session_table_' + TABLE;
    const params = new URLSearchParams(window.location.search);
    const urlToken = params.get('token');

    if (urlToken) {
        // Fresh QR scan — save token, clean URL, show welcome modal
        sessionStorage.setItem(SESSION_KEY, urlToken);
        const cleanUrl = window.location.pathname + '?table=' + TABLE;
        history.replaceState(null, '', cleanUrl);
        document.addEventListener('DOMContentLoaded', () => {
            document.getElementById('getstarted-modal').classList.add('open');
        });
    } else {
        const stored = sessionStorage.getItem(SESSION_KEY);
        if (!stored) {
            // ── FIX: auto-generate a session instead of hard-redirecting ──
            // This handles: browser restores the tab, sharing the link,
            // or QR code was generated without a token in the URL.
            const autoToken = Math.random().toString(36).substr(2, 12).toUpperCase();
            sessionStorage.setItem(SESSION_KEY, autoToken);
            // Show get-started modal so the guest still goes through onboarding
            document.addEventListener('DOMContentLoaded', () => {
                document.getElementById('getstarted-modal').classList.add('open');
            });
        }
        // else: existing valid session — proceed silently
    }
})();