# BeeMart POS - Integrated Point of Sale Web Application

A full-stack Point of Sale (POS) Web Application built for **BeeMart**, adhering strictly to the **Separation of Concerns** architectural pattern. The project consists of a stateless WebClient (Frontend) consuming a RESTful Backend API powered by **.NET 8** and **Entity Framework Core** with **SQL Server**.

---

## 🛠️ Tech Stack & Architecture

### **Backend API (`/backend`)**
* **Framework:** ASP.NET Core Web API (.NET 8)
* **ORM:** Entity Framework Core 8[cite: 2]
* **Database:** Microsoft SQL Server[cite: 2]
* **Security:** Password Hashing (BCrypt / SHA256)[cite: 2]
* **API Documentation:** Swagger / OpenAPI[cite: 5]

### **WebClient Frontend (`/frontend`)**
* **Core:** Vanilla HTML5, JavaScript (ES6+), Modern Plain CSS
* **Styling:** Custom CSS Grid & Flexbox (Strictly no CSS frameworks used)[cite: 2]
* **API Communication:** Asynchronous Fetch API / AJAX

### **Backend API (`/backend`)**
* **Framework:** ASP.NET Core Web API (.NET 8)
* **ORM:** Entity Framework Core 8 (SQL Server Provider)
* **Security & Auth:** Password Hashing (SHA-256 / BCrypt)
* **API Documentation:** Swagger UI / OpenAPI specification

### **WebClient Frontend (`/frontend`)**
* **Core:** HTML5, Vanilla JavaScript (ES6+), Modern Plain CSS
* **Design Standard:** Custom CSS Grid & Flexbox (**Strictly no CSS Frameworks like Bootstrap**)
* **HTTP Client:** Asynchronous Fetch API / AJAX

---

## ✨ Features & Business Logic

1. **Authentication & Authorization Layer**
   * **Registration:** Account creation with role selection (`Admin` or `Cashier`), password hashing, and duplicate username validation[cite: 2, 6].
   * **Login:** Role-based login authentication with local session state management (`localStorage`)[cite: 2, 6].
   * **Route Protection & Guard:** Restricts unauthenticated access to transactional pages and handles user logouts.

2. **New Transaction System (`New Transaction`)**
   * **Real-time Clock & Header:** Live timestamp display with user session details[cite: 6, 12].
   * **Product Search:** Real-time search by product name (limited to top 10 results for optimization).
   * **Cart & Stock Validation:** Interactive shopping cart with subtotal calculation, discount processing, and real-time stock availability verification[cite: 2, 11, 12].
   * **Auto Invoice Generation:** Automatic 14-digit Invoice Number generation using the formula:  
     `[5-digit User ID][DDMMYY][3-digit Running Number]`[cite: 2, 4, 11, 12].
   * **Dynamic Payment Methods:** Dynamically loads available payment options (Cash, Debit, Credit) from the database[cite: 2, 11, 12].
   * **Atomic Transaction Processing:** Automatically updates stock counts upon successful checkout.

3. **Transaction History (`History Transaction`)**
   * **Date Filtering:** Date picker control for querying sales records[cite: 2, 13].
   * **Role-Based Visibility:**
     * **Cashier:** Restricted to viewing their own submitted transactions[cite: 2, 11, 13].
     * **Admin:** Privileged view to inspect transactions submitted across all cashiers[cite: 2, 11, 13].
   * **Server-Side Pagination:** Sorted in ascending order by transaction date with a limit of 20 records per page[cite: 2, 11, 13].

---

### 3. Transaction History (`History Transaction`)
* **Date Filter:** Date picker input for querying daily transaction logs.
* **Role-Based Supervision:**
  * **Cashier:** Restricted to viewing only transactions executed by their own account.
  * **Admin:** Privileged supervision view showing transactions across all cashiers.
* **Pagination & Sorting:** Sorted in **Ascending order** by transaction timestamp, limited to **20 records per page** (displays page navigation controls even for 1 or zero records).

---

## 📁 Project Repository Structure

```text
.
├── script.sql                 # DDL (Tables) & DML (Initial Product/User/Payment Seeding)
├── backend/                   # ASP.NET Core API (.NET 8)
│   └── WebApp/
│       ├── Controllers/       # AuthController, ProductController, PaymentMethodController, TransactionController
│       ├── Data/              # AppDbContext & EF Core Relationships Configuration
│       ├── DTOs/              # Request & Response Data Transfer Objects
│       ├── Models/            # Database Entity Class Definitions
│       ├── appsettings.json   # Server connection string configuration
│       └── Program.cs         # Service DI Container & Middleware Pipeline
└── frontend/                  # WebClient (Vanilla HTML/JS/CSS)
    ├── css/                   # Login, Transaction, and History Stylesheets
    ├── js/                    # Client-side Scripts (config.js, auth.js, transaction.js, history.js)
    ├── login.html             # Login Page
    ├── register.html          # Registration Page
    ├── transaction.html       # Point of Sale Checkout Interface
    └── history.html           # Transaction History & Supervision View
```

---

## 🚀 Setup & Installation Guide

### **Prerequisites**
1. [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
2. Microsoft SQL Server & SQL Server Management Studio (SSMS)
3. Any modern web browser (Google Chrome, Microsoft Edge, Firefox)

---

### **Step 1: Database Initialization**
1. Open **SQL Server Management Studio (SSMS)** and connect to your local SQL Server instance.
2. Create a target database named `BeeMartDB`:
   ```sql
   CREATE DATABASE BeeMartDB;
   GO
   ```
3. Open and execute the **`script.sql`** file provided in the root directory. This will execute all DDL table creation queries and populate the initial seed data (Payment Methods, Master Products, Default Accounts).

---

### **Step 2: Backend API Configuration & Execution**
1. Open a terminal and navigate to the API project directory:
   ```bash
   cd backend/WebApp
   ```
2. Open `appsettings.json` and adjust the connection string to match your local SQL Server instance:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=.\\SQLEXPRESS;Database=BeeMartDB;Trusted_Connection=True;TrustServerCertificate=True;"
     }
   }
   ```
3. Run the Backend API:
   ```bash
   dotnet run
   ```
4. Access the Swagger documentation at `http://localhost:5284/swagger` or `https://localhost:7123/swagger`.

---

### **Step 3: Launching the Frontend WebClient**
1. Open `frontend/js/config.js` and verify that `BASE_API_URL` points to your active backend port:
   ```javascript
   const BASE_API_URL = "http://localhost:5284";
   ```
2. Open `frontend/login.html` in your browser (or serve it using VS Code Live Server).
3. Authenticate using default seed credentials:
   * **Admin Account:** Username `mark` | Password `password123`
   * **Cashier Account:** Username `john` | Password `password123`

---

## 🌐 API Specifications

| Module | Method | Endpoint | Description |
| :--- | :--- | :--- | :--- |
| **Auth** | `POST` | `/api/auth/register` | Register new user account with hashed password |
| **Auth** | `POST` | `/api/auth/login` | Authenticate credentials and return user role session |
| **Payment** | `GET` | `/api/payment-methods` | Retrieve active dynamic payment options |
| **Product** | `GET` | `/api/product/search?query={keyword}` | Search products by keyword (Max 10 results) |
| **Transaction**| `POST` | `/api/transaction` | Process transaction checkout, deduct stock, and generate invoice |
| **History** | `GET` | `/api/transaction/history` | Retrieve paginated transactions (Filter by Date/Role, Max 20/page) |

---
