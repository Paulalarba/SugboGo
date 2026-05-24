# 📍 SugboGo: The Ultimate Cebu Experience Platform

[![Framework](https://img.shields.io/badge/Framework-ASP.NET%20Core%20MVC%2010.0-512bd4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/en-us/apps/aspnet)
[![Database](https://img.shields.io/badge/Database-PostgreSQL-336791?style=flat-square&logo=postgresql)](https://www.postgresql.org/)
[![License](https://img.shields.io/badge/License-MIT-green.svg?style=flat-square)](LICENSE)

SugboGo is a high-performance travel ecosystem built to transform how travelers discover, plan, and experience the island of Cebu. It serves as a bridge between **Explorers** looking for authentic local gems, **B2B Partners** providing elite logistics/services, and **Administrators** orchestrating the entire island's operations.

---

## 🌟 Core Ecosystem

### 🧭 For Travelers (Explorers)
- **Vibe-Based Curation:** Beyond simple lists—discover spots based on your "Travel Vibe" (Solo, Family, Adventure).
- **Checkout 2.0:** A personalized booking flow that captures preferences, activities, and special requests.
- **Personal Dashboard:** Manage upcoming itineraries, saved "hidden gems," and travel history in one hub.

### 🤝 For B2B Partners (Vendors)
- **Operations Command Center:** A dedicated portal for transport providers, guides, and hotels.
- **Assignment Ledger:** Real-time visibility into assigned bookings and traveler specifics.
- **Trip Lifecycle Management:** Update trip statuses (Active, Completed) to keep the network synchronized.
- **Verification Engine:** QR-code based traveler verification for on-site operations.

### 🏛️ For Administrators (Command Center)
- **Master Booking Ledger:** End-to-end visibility of all financial and operational data.
- **Partner Network Management:** Register, audit, and link user accounts to B2B vendor entities.
- **Inventory Curation:** Add and verify "Hidden Gems" across all 50+ regions of Cebu.
- **KPI Analytics:** Real-time metrics on user growth, revenue pipeline, and trending travel vibes.

---

## 🚀 Tech Stack & Architecture

- **Backend:** C# 13, ASP.NET Core 10.0 MVC
- **Data Layer:** Entity Framework Core with a **Hybrid Factory Strategy** (PostgreSQL, Supabase, or Local JSON).
- **Security:** PBKDF2 Password Hashing, Role-Based Access Control (RBAC), and Anti-Forgery Protection.
- **Frontend:** Razor Pages with specialized "Command Center" CSS architectures for high-density data.
- **Documentation:** Modern Markdown with region-specific "Cebu Intelligence" datasets.

---

## 🔑 Portal Access & User Registration

SugboGo uses a Role-Based Access Control (RBAC) system. Here is how to set up specialized accounts:

### 🏛️ How to Register as an Administrator
Administrators are designated via high-level security configuration.
1.  **Configure Admin Email:** Open `appsettings.json` and add your email to the `Authentication:AdminEmails` list:
    ```json
    "Authentication": {
      "AdminEmails": ["yourname@sugbogo.ph"]
    }
    ```
2.  **Register:** Go to the application and register a new account using that exact email address.
3.  **Automatic Escalation:** The system will automatically detect the email and assign the `Admin` role upon registration.
4.  **Access:** You can now access `http://localhost:5115/Admin` to manage the platform.

### 🤝 How to Register as a B2B Partner
B2B Partners require a two-step "Verified Link" process to ensure network integrity.
1.  **Traveler Registration:** The partner representative must first register a standard traveler account at `http://localhost:5115/Account`.
2.  **Admin Authorization:** An existing Administrator must log in and navigate to the **[Vendor Partners](http://localhost:5115/Admin/Partners)** ledger.
3.  **Entity Linking:**
    *   Find the B2B Partner entity (e.g., "Mactan Island Boat Charters").
    *   Click **Edit**.
    *   Paste the representative's **User ID** (found in the Admin's "Traveler Profiles" section) into the **"Linked Account User ID"** field.
4.  **Access:** Once linked, the representative will lose standard traveler access and be redirected to their private **Partner Command Center** at `http://localhost:5115/Partner` upon login.

---

## 🛠️ Developer Setup Guide

### 1. Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/) (Recommended for full operational features)
- [VS Code](https://code.visualstudio.com/) or [Visual Studio 2022+](https://visualstudio.microsoft.com/)

### 2. Installation & Setup
```bash
# Clone the repository
git clone https://github.com/yourusername/SugboGo.git
cd SugboGo

# Restore dependencies
dotnet restore

# Configure Environment
cp .env.example .env
```

### 3. Database Initialization
SugboGo features **Auto-Sync Migration**. On the first run, the system will:
1. Detect your PostgreSQL instance.
2. Apply all schema migrations.
3. **Seed Data:** Inject 19+ B2B Partners and curated Cebu Hidden Gems automatically.

### 4. Launching the App
```bash
dotnet run
```
Access the portals at:
- **Client App:** `http://localhost:5115`
- **Admin Portal:** `http://localhost:5115/Admin`
- **Partner Portal:** `http://localhost:5115/Partner`

---

## 📊 Project Statistics (Live Metrics)

| Metric | Count | Context |
| :--- | :--- | :--- |
| **B2B Partners** | 19+ | Verified Transport, Lodging, and Guides |
| **Curated Gems** | 7+ | Off-the-beaten-path destinations |
| **Active Regions** | 12+ | Cebu City, Mactan, Oslob, Bantayan, etc. |
| **Core Contributors** | 1 | Senior Software Engineer |
| **Architecture** | Hybrid | Local-first with Cloud Scalability |

---

## 🤝 Contribution Guidelines

We welcome contributions that help make Cebu the premier travel destination in Asia!

1. **Fork** the repository.
2. **Create a Feature Branch** (`git checkout -b feature/AmazingFeature`).
3. **Commit your changes** (`git commit -m 'Add some AmazingFeature'`).
4. **Push to the Branch** (`git push origin feature/AmazingFeature`).
5. **Open a Pull Request**.

*Note: Ensure your code adheres to the project's C# Clean Coding standards and includes necessary migrations for model changes.*

---

## 📄 License
Distributed under the MIT License. See `LICENSE` for more information.

## 📞 Support & Contact
- **Project Lead:** [Your Name/Handle]
- **Region:** Cebu, Philippines
- **Issue Tracker:** [GitHub Issues](https://github.com/yourusername/SugboGo/issues)

---
*Created with ❤️ in Cebu for the World.*
