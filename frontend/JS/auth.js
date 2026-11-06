document.addEventListener("DOMContentLoaded", function () {
    const btnLogout = document.getElementById("btnLogout");

    if (btnLogout) {
        btnLogout.addEventListener("click", function () {
            // 1. Hapus data sesi/token dari browser
            localStorage.removeItem("userSession");

            // 2. Redirect kembali ke halaman login
            window.location.href = "login.html";
        });
    }
});