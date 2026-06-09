export function fmt(n) {
    return "₱" + parseFloat(n).toFixed(2);
}

export function genId() {
    return Math.random()
        .toString(36)
        .substr(2, 9)
        .toUpperCase();
}

export function toast(msg) {
    const el = document.createElement("div");

    el.className = "toast";
    el.textContent = msg;

    document
        .getElementById("toast-container")
        .appendChild(el);

    setTimeout(() => el.remove(), 2900);
}