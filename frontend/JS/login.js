document.addEventListener("DOMContentLoaded", function () {
    const loginForm = document.getElementById("loginForm");
    const errorMessage = document.getElementById("errorMessage");

    if (localStorage.getItem("userSession")) {
        window.location.href = "transaction.html";
        return;
    }

    loginForm.addEventListener("submit", async function (e) {
        e.preventDefault();

        errorMessage.style.display = "none";
        errorMessage.innerText = "";

        const username = document.getElementById("username").value.trim();
        const password = document.getElementById("password").value;
        const role = document.getElementById("role").value;

        // Payload Request sesuai LoginRequestDto
        const payload = {
            username: username,
            password: password,
            role: role
        };

        try {
            const response = await fetch(`${BASE_API_URL}/api/auth/login`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(payload)
            });

            const data = await response.json();

            if (response.ok) {
                // Simpan Sesi User ke localStorage
                const userSession = {
                    idUser: data.idUser,
                    username: data.username,
                    fullName: data.fullName,
                    role: data.role,
                    token: data.token
                };
                localStorage.setItem("userSession", JSON.stringify(userSession));

                // Redirect ke Halaman Transaksi Baru
                window.location.href = "transaction.html";
            } else {
                // Tampilkan Pesan Error dari API ("Invalid login" / Alert Info)
                errorMessage.innerText = data.message || "Invalid login";
                errorMessage.style.display = "block";
            }
        } catch (error) {
            console.error("Login Error:", error);
            errorMessage.innerText = "Gagal terhubung ke server Backend API.";
            errorMessage.style.display = "block";
        }
    });
});