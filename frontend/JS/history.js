document.addEventListener("DOMContentLoaded", function () {
    // 1. Route Guard
    const sessionData = localStorage.getItem("userSession");
    if (!sessionData) {
        window.location.href = "login.html";
        return;
    }

    const userSession = JSON.parse(sessionData);

    // Elements
    const sessionInfo = document.getElementById("sessionInfo");
    const btnLogout = document.getElementById("btnLogout");
    const searchRoleLabel = document.getElementById("searchRoleLabel");
    const filterForm = document.getElementById("filterForm");
    const transactionDateInput = document.getElementById("transactionDate");
    const historyTableBody = document.getElementById("historyTableBody");
    const paginationContainer = document.getElementById("paginationContainer");
    const errorMessage = document.getElementById("errorMessage");

    // Dynamic Header Info dengan warna kuning pada Role
    sessionInfo.innerHTML = `Role: <span class="role-text">${userSession.role}</span> | Login: <strong>${userSession.username}</strong>`;

    // Logout Event
    btnLogout.addEventListener("click", function () {
        localStorage.removeItem("userSession");
        window.location.href = "login.html";
    });

    // Helper Format Rupiah
    function formatRupiah(amount) {
        return "Rp " + new Intl.NumberFormat("id-ID").format(amount);
    }

    // Helper Display Message
    function showError(msg) {
        errorMessage.innerText = msg;
        errorMessage.style.display = "block";
    }

    function hideError() {
        errorMessage.style.display = "none";
    }

    // Setup Role Label Header
    if (userSession.role.toLowerCase() === "cashier") {
        searchRoleLabel.innerHTML = `Pencarian Data Transaksi <span class="role-label">(Hanya Kasir: ${userSession.username})</span>`;
    } else {
        searchRoleLabel.innerHTML = `Pencarian Data Transaksi <span class="role-label">(Menampilkan Semua Kasir)</span>`;
    }

    // Set Default Date Picker ke Tanggal Hari Ini (Format YYYY-MM-DD)
    const today = new Date();
    const yyyy = today.getFullYear();
    const mm = String(today.getMonth() + 1).padStart(2, '0');
    const dd = String(today.getDate()).padStart(2, '0');
    const todayStr = `${yyyy}-${mm}-${dd}`;
    transactionDateInput.value = todayStr;

    // Fetch History Function
    async function loadHistory(page = 1) {
        hideError();
        const selectedDate = transactionDateInput.value;

        if (!selectedDate) {
            showError("Silakan pilih tanggal terlebih dahulu.");
            return;
        }

        // Construction Query Param sesuai API Backend
        const url = `${BASE_API_URL}/api/transaction/history?userId=${encodeURIComponent(userSession.idUser)}&role=${encodeURIComponent(userSession.role)}&date=${encodeURIComponent(selectedDate)}&page=${page}`;

        try {
            const response = await fetch(url);
            if (!response.ok) {
                const errData = await response.json();
                showError(errData.message || "Gagal mengambil data riwayat.");
                renderTable([]);
                renderPagination(1, 1);
                return;
            }

            const result = await response.json();
            renderTable(result.data);
            renderPagination(result.currentPage || 1, result.totalPages || 1);
        } catch (err) {
            console.error(err);
            showError("Gagal terhubung ke server Backend API.");
            renderTable([]);
            renderPagination(1, 1);
        }
    }

    // Render Table Data
    function renderTable(data) {
        historyTableBody.innerHTML = "";

        if (!data || data.length === 0) {
            historyTableBody.innerHTML = `
                <tr>
                    <td colspan="6" style="text-align: center; color: #777;">Data transaksi tidak ditemukan pada tanggal ini.</td>
                </tr>
            `;
            return;
        }

        data.forEach(item => {
            const tr = document.createElement("tr");
            tr.innerHTML = `
                <td>${item.invoiceNo}</td>
                <td>${item.transactionDate}</td>
                <td>${item.kasir}</td>
                <td>${item.totalItems}</td>
                <td>${item.paymentMethod}</td>
                <td>${formatRupiah(item.totalPrice)}</td>
            `;
            historyTableBody.appendChild(tr);
        });
    }

    // Selalu Tampil Minimal Tombol '1'
    function renderPagination(currentPage = 1, totalPages = 1) {
        paginationContainer.innerHTML = "";

        // Jika totalPages bernilai kurang dari 1 (misal 0/null), paksa minimal 1 halaman
        const maxPages = Math.max(1, totalPages);

        for (let i = 1; i <= maxPages; i++) {
            const btn = document.createElement("button");
            btn.className = `page-btn ${i === currentPage ? 'active' : ''}`;
            btn.innerText = i;
            btn.addEventListener("click", function () {
                if (i !== currentPage) {
                    loadHistory(i);
                }
            });
            paginationContainer.appendChild(btn);
        }
    }

    // Submit Filter Event
    filterForm.addEventListener("submit", function (e) {
        e.preventDefault();
        loadHistory(1);
    });

    // Inisialisasi awal tarik data hari ini
    loadHistory(1);
});