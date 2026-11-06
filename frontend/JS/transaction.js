document.addEventListener("DOMContentLoaded", function () {
    // 1. Check Session (Route Guard)
    const sessionData = localStorage.getItem("userSession");
    if (!sessionData) {
        window.location.href = "login.html";
        return;
    }

    const userSession = JSON.parse(sessionData);

    // Elements
    const sessionInfo = document.getElementById("sessionInfo");
    const btnLogout = document.getElementById("btnLogout");
    const invoiceNoEl = document.getElementById("invoiceNo");
    const searchInput = document.getElementById("searchInput");
    const searchResultsContainer = document.getElementById("searchResultsContainer");
    const searchResultsTable = document.getElementById("searchResultsTable");
    const selectedProductBox = document.getElementById("selectedProductBox");
    const selectedProductName = document.getElementById("selectedProductName");
    const selectedProductPrice = document.getElementById("selectedProductPrice");
    const selectedProductStock = document.getElementById("selectedProductStock");
    const inputQty = document.getElementById("inputQty");
    const btnAddToCart = document.getElementById("btnAddToCart");
    const cartTableBody = document.getElementById("cartTableBody");
    const totalPriceText = document.getElementById("totalPriceText");
    const paymentMethodSelect = document.getElementById("paymentMethodSelect");
    const btnSubmitTransaction = document.getElementById("btnSubmitTransaction");

    let cart = [];
    let selectedProduct = null;

    // Set Header Info & Clock
    function updateHeaderClock() {
        const now = new Date();
        const dateStr = now.toLocaleDateString("id-ID");
        const timeStr = now.toLocaleTimeString("id-ID");
        sessionInfo.innerText = `${dateStr}, ${timeStr} | ${userSession.role}: ${userSession.username}`;
    }
    setInterval(updateHeaderClock, 1000);
    updateHeaderClock();

    // Logout
    btnLogout.addEventListener("click", function () {
        localStorage.removeItem("userSession");
        window.location.href = "login.html";
    });

    // Helper Format Rupiah
    function formatRupiah(amount) {
        return "Rp " + new Intl.NumberFormat("id-ID").format(amount);
    }

    // Generate Preview Invoice Number
    function updateInvoicePreview() {
        const dateNow = new Date();
        const dd = String(dateNow.getDate()).padStart(2, '0');
        const mm = String(dateNow.getMonth() + 1).padStart(2, '0');
        const yy = String(dateNow.getFullYear()).slice(-2);
        const dateStr = `${dd}${mm}${yy}`;
        const userPrefix = userSession.idUser.padStart(5, '0');
        const initialRunningNo = "001";
        invoiceNoEl.innerText = `${userPrefix}${dateStr}${initialRunningNo}`;
    }
    updateInvoicePreview();

    // Fetch Payment Methods
    async function loadPaymentMethods() {
        try {
            const response = await fetch(`${BASE_API_URL}/api/payment-methods`);
            if (response.ok) {
                const methods = await response.json();
                paymentMethodSelect.innerHTML = ""; // Kosongkan placeholder dropdown
                
                methods.forEach(m => {
                    const opt = document.createElement("option");
                    opt.value = m.idPaymentMethod;
                    opt.innerText = m.methodName;

                    // 'Cash' default opt
                    if (m.methodName.toLowerCase() === "cash") {
                        opt.selected = true;
                    }

                    paymentMethodSelect.appendChild(opt);
                });
            }
        } catch (err) {
            console.error("Gagal memuat metode pembayaran:", err);
        }
    }
    loadPaymentMethods();

    // Search Product Event
    searchInput.addEventListener("keypress", async function (e) {
        if (e.key === "Enter") {
            e.preventDefault();
            const query = searchInput.value.trim();
            if (!query) return;

            try {
                const response = await fetch(`${BASE_API_URL}/api/product/search?query=${encodeURIComponent(query)}`);
                if (response.ok) {
                    const products = await response.json();
                    renderSearchResults(products);
                }
            } catch (err) {
                alert("Gagal terhubung ke API pencarian produk.");
            }
        }
    });

    // Render Search Results
    function renderSearchResults(products) {
        searchResultsTable.innerHTML = "";
        if (products.length === 0) {
            searchResultsContainer.style.display = "none";
            return;
        }

        products.forEach(p => {
            const tr = document.createElement("tr");
            tr.innerHTML = `
                <td>${p.name}</td>
                <td>${p.weight} Kg</td>
                <td>${p.stock}</td>
                <td>${formatRupiah(p.price)}</td>
                <td>${p.discount}%</td>
                <td style="text-align: center;"><button class="btn-select" data-id="${p.idProduct}">Pilih</button></td>
            `;

            tr.querySelector(".btn-select").addEventListener("click", function () {
                openSelectBox(p);
            });

            searchResultsTable.appendChild(tr);
        });

        searchResultsContainer.style.display = "block";
    }

    // Select Product Box
    function openSelectBox(product) {
        selectedProduct = product;
        selectedProductName.innerText = `Barang Terpilih: ${product.name}`;
        selectedProductPrice.innerText = formatRupiah(product.price);
        selectedProductStock.innerText = product.stock;
        inputQty.value = 1;
        selectedProductBox.style.display = "block";
    }

    // Add to Cart
    btnAddToCart.addEventListener("click", function () {
        if (!selectedProduct) return;

        const qty = parseInt(inputQty.value) || 0;
        if (qty <= 0) {
            alert("Jumlah kuantitas harus lebih dari 0.");
            return;
        }

        // Validasi Sisa Stok
        if (qty > selectedProduct.stock) {
            alert("Jumlah barang yang dimasukkan melebihi sisa stok.");
            return;
        }

        // Hitung Subtotal
        const discountAmount = selectedProduct.price * (selectedProduct.discount / 100);
        const finalUnitPrice = selectedProduct.price - discountAmount;
        const subtotal = finalUnitPrice * qty;

        // Cek jika produk sudah ada di keranjang
        const existingIndex = cart.findIndex(item => item.idProduct === selectedProduct.idProduct);
        if (existingIndex > -1) {
            const totalQty = cart[existingIndex].qty + qty;
            if (totalQty > selectedProduct.stock) {
                alert("Jumlah barang yang dimasukkan melebihi sisa stok.");
                return;
            }
            cart[existingIndex].qty = totalQty;
            cart[existingIndex].subtotal = finalUnitPrice * totalQty;
        } else {
            cart.push({
                idProduct: selectedProduct.idProduct,
                name: selectedProduct.name,
                price: selectedProduct.price,
                discount: selectedProduct.discount,
                qty: qty,
                subtotal: subtotal
            });
        }

        // Reset Box & Search Result
        selectedProductBox.style.display = "none";
        searchResultsContainer.style.display = "none";
        searchInput.value = "";
        selectedProduct = null;

        renderCart();
    });

    // Render Cart Table
    function renderCart() {
        cartTableBody.innerHTML = "";
        let total = 0;

        cart.forEach((item, index) => {
            total += item.subtotal;
            const tr = document.createElement("tr");
            tr.innerHTML = `
                <td>${item.name}</td>
                <td>${formatRupiah(item.price)}</td>
                <td>${item.discount}%</td>
                <td>${item.qty}</td>
                <td>${formatRupiah(item.subtotal)}</td>
                <td style="text-align: center;"><button class="btn-delete" data-index="${index}">Hapus</button></td>
            `;

            tr.querySelector(".btn-delete").addEventListener("click", function () {
                cart.splice(index, 1);
                renderCart();
            });

            cartTableBody.appendChild(tr);
        });

        totalPriceText.innerText = formatRupiah(total);
    }

    // Submit Transaction
    btnSubmitTransaction.addEventListener("click", async function () {

        if (cart.length === 0) {
            alert("Daftar belanjaan masih kosong!");
            return;
        }

        const idPaymentMethod = paymentMethodSelect.value;

        const payload = {
            idUser: userSession.idUser,
            idPaymentMethod: parseInt(idPaymentMethod),
            items: cart.map(item => ({
                idProduct: item.idProduct,
                qty: item.qty
            }))
        };

        try {
            const response = await fetch(`${BASE_API_URL}/api/transaction`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(payload)
            });

            const data = await response.json();

            if (response.ok) {
                alert(`Transaksi berhasil disubmit! Invoice: ${data.invoiceNo}`);
                // Reset State
                cart = [];
                renderCart();
                paymentMethodSelect.selectedIndex = 0;
                
                
                const prefix = data.invoiceNo.slice(0, -3); // Ambil [userID][ddmmyy] (11 digit pertama)
                const lastRunningNo = parseInt(data.invoiceNo.slice(-3), 10); // Ambil 3 digit terakhir
                    
                // Increment +1 untuk transaksi berikutnya
                const nextRunningNo = String(lastRunningNo + 1).padStart(3, '0');
                    
                // Tampilkan nomor invoice baru untuk transaksi berikutnya
                invoiceNoEl.innerText = `${prefix}${nextRunningNo}`;
                

            } else {
                alert(data.message || "Gagal memproses transaksi.");
            }
        } catch (err) {
            alert("Gagal terhubung ke server Backend API.");
        }
    });
});