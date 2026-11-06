document.addEventListener("DOMContentLoaded", function () {
    const registerForm = document.getElementById("registerForm");
    const errorMessage = document.getElementById("errorMessage");
    const successMessage = document.getElementById("successMessage");

    registerForm.addEventListener("submit", async function (e) {
        e.preventDefault();

        // Reset Tampilan Message
        errorMessage.style.display = "none";
        errorMessage.innerText = "";
        successMessage.style.display = "none";
        successMessage.innerText = "";

        const username = document.getElementById("username").value.trim();
        const fullName = document.getElementById("fullName").value.trim();
        const password = document.getElementById("password").value;
        const rePassword = document.getElementById("rePassword").value;
        const role = document.getElementById("role").value;

        // Validasi Sisi Klien: Password & Re-Password
        if (password !== rePassword) {
            errorMessage.innerText = "Password dan Re-Password tidak cocok!";
            errorMessage.style.display = "block";
            return;
        }

        // Payload Request sesuai RegisterRequestDto
        const payload = {
            username: username,
            fullName: fullName,
            password: password,
            rePassword: rePassword,
            role: role
        };

        try {
            const response = await fetch(`${BASE_API_URL}/api/auth/register`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(payload)
            });

            const data = await response.json();

            if (response.ok) {
                // Tampilkan pesan sukses
                successMessage.innerText = data.message || "Registrasi berhasil! Mengalihkan ke halaman login...";
                successMessage.style.display = "block";

                // Redirect ke Halaman Login setelah 1.5 detik
                setTimeout(() => {
                    window.location.href = "login.html";
                }, 1500);
            } else {
                // Tampilkan Pesan Error dari API
                errorMessage.innerText = data.message || "Registrasi gagal.";
                errorMessage.style.display = "block";
            }
        } catch (error) {
            console.error("Register Error:", error);
            errorMessage.innerText = "Gagal terhubung ke server Backend API.";
            errorMessage.style.display = "block";
        }
    });
});