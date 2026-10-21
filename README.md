# E-Commerce Web Application

A full-stack, feature-rich **E-Commerce Web Application** built with a modern Clean Architecture pattern. The platform provides a seamless shopping experience featuring product catalog filtering, secure user authentication, order tracking, and online payment processing.

---

## 🚀 Key Features

* **Product Catalog & Details:**
  * Browse and search items with sorting options (e.g., Price: High to Low).
  * Filter products by categories such as **PC** and **Phone**.
  * Detailed product pages featuring image zooming, customer reviews, ratings, and estimated delivery times.
* **Shopping Cart & Checkout Workflow:**
  * Add, remove, and adjust quantities of items in the cart.
  * Multi-step checkout process including a shipping address form, delivery method selection, and payment options (including Stripe).
* **Authentication & User Management:**
  * Secure user Login, Registration (`Sign Up`), and Password Recovery (`Forgot Password`).
  * User profile dashboard to manage account information and view order history (pending, completed, or failed orders).
* **UI Themes:**
  * Fully responsive interface supporting both **Light and Dark modes**.
* **Payment Integration:**
  * Integrated with **Stripe** for secure online transactions.

---

## 🛠️ Tech Stack & Architecture

The application is structured following **Clean Architecture** principles to ensure high maintainability and scalability.

* **Backend:** ASP.NET Core (.NET)
* **Frontend:** Blazor
* **Databases & Caching:** 
  * **Microsoft SQL Server** (Relational Database)
  * **Redis** (Caching and session management)
* **DevOps & Deployment:**
  * **Docker & Docker Compose**
  * **Helm Charts** (Kubernetes packaging)
  * **ArgoCD** (GitOps continuous deployment)

---

## 📁 Project Structure

```text
AppProject/
│
├── Core/                   # Domain entities, business logic, and interfaces
├── Infrastructure/         # Database contexts, repositories, and services
├── AppProject.API/         # ASP.NET Core Web API backend
├── AppProject.Client/      # Blazor Frontend client application
├── chart-core/             # Helm charts for Kubernetes deployment
├── compose.yaml            # Docker Compose configuration
└── README.md               # Project documentation
