# E-Commerce Web Application

A full-stack, enterprise-grade E-Commerce Web Application built with a modern **Clean Architecture** pattern. This solution provides a seamless shopping experience for clients alongside robust management capabilities, automated deployment setups, and comprehensive monitoring systems.

---

## ✨ Key Features

* **Product Catalog & Management:**
  * Browse, search, and filter products by categories and price.
  * Paginated product fetching for optimal performance.
  * Product detail pages with dynamic image galleries and ratings/reviews.
* **Shopping Cart & Checkout Workflow:**
  * Real-time basket operations stored in high-performance cache (**Redis**).
  * Multi-step checkout process including shipping address assignment and delivery method selection.
  * Secure online payment processing integrated with **Stripe**.
* **Authentication & User Management:**
  * Secure registration, login, and password reset functionalities.
  * **JWT (JSON Web Tokens)** authentication paired with **Refresh Tokens** for persistent and secure sessions.
  * Role-Based Access Control (RBAC) allowing admins to manage user claims, roles, and permissions.
* **DevOps & Observability:**
  * Full containerization via **Docker** and multi-service orchestrations via **Docker Compose**.
  * Scalable Kubernetes deployment resources formatted using **Helm Charts**.
  * Full telemetry and observability stack configured with **Prometheus**, **Grafana**, **Loki**, and **Alloy**.

---

## 🛠️ Tech Stack & Architecture

The solution adheres to **Clean Architecture** principles, isolating domain logic, infrastructure concerns, and presentation layers.

```
┌─────────────────────────────────────────────────────────────┐
│                      AppProject.Client                      │
│                   (Blazor WebAssembly UI)                   │
└──────────────────────────────┬──────────────────────────────┘
                               │ HTTP / REST API
┌──────────────────────────────▼──────────────────────────────┐
│                       AppProject.API                        │
│                   (ASP.NET Core Controllers)                │
└──────────────────────────────┬──────────────────────────────┘
                               │
┌──────────────────────────────▼──────────────────────────────┐
│                            Core                             │
│       (Entities, DTOs, Interfaces, Mappings, Services)       │
└──────────────────────────────▲──────────────────────────────┘
                               │
┌──────────────────────────────┴──────────────────────────────┐
│                        Infrastructure                       │
│    (Database Context, Repositories, Identity, Redis Cache)   │
└─────────────────────────────────────────────────────────────┘
```

* **Backend Framework:** ASP.NET Core (.NET)
* **Frontend Framework:** Blazor WebAssembly
* **Database & Caching:** SQL Server, Redis
* **Authentication:** ASP.NET Core Identity, JWT, Refresh Tokens
* **Payment Processing:** Stripe API
* **Mapping Tool:** AutoMapper
* **API Testing & Documentation:** Postman Collection Included
* **Containerization & Monitoring:** Docker, Kubernetes, Helm, Prometheus, Grafana, Loki, Alloy

---

## 📂 Project Structure

```text
AppProject/
├── Core/                      # Domain entities, DTOs, Interfaces, Business Logic
├── Infrastructure/            # Database Context, Repositories, Authentication implementations
├── AppProject.API/            # ASP.NET Core Web API Controllers & Middleware
├── AppProject.Client/         # Blazor WebAssembly Frontend UI
├── chart-ecom/                # Helm charts for Kubernetes deployment
├── docker-compose.yml         # Multi-container local orchestration
└── Dockerfile                 # Application build & package definition
```

---

## 🚀 Getting Started

### Prerequisites

Ensure you have the following installed on your local environment:
* [.NET SDK](https://dotnet.microsoft.com/download) (.NET 8 or higher recommended)
* [SQL Server](https://www.microsoft.com/sql-server/)
* [Redis](https://redis.io/)
* [Docker Desktop](https://www.docker.com/products/docker-desktop/) (Optional, for containerized run)
* [Postman](https://www.postman.com/) (For API testing)

### Local Setup Instructions

1. **Clone the Repository:**
   ```bash
   git clone https://github.com/samerrazzaqh/appecom.git
   cd appecom
   ```

2. **Configure Database & Connection Strings:**
   Update the connection strings in `AppProject.API/appsettings.json` for SQL Server and Redis:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=AppEcomDb;Trusted_Connection=True;TrustServerCertificate=True;",
       "Redis": "localhost:6379"
     }
   }
   ```

3. **Apply Database Migrations:**
   ```bash
   dotnet ef database update --project Infrastructure --startup-project AppProject.API
   ```

4. **Run the Application:**
  * Start the Backend API:
    ```bash
    dotnet run --project AppProject.API
    ```
  * Start the Blazor Client:
    ```bash
    dotnet run --project AppProject.Client
    ```

---

## 📌 API Endpoints Overview

Below is a reference guide for the main RESTful endpoints configured in the included Postman collection.

### 🔑 Account & Authentication
| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `POST` | `/api/Account/Register` | Register a new user account |
| `POST` | `/api/Account/Login` | Authenticate user and issue JWT token |
| `GET`  | `/api/Account/IsUserAuth` | Verify authentication status |
| `GET`  | `/api/Account/loginByRefreshToken` | Obtain a new JWT using a refresh token |
| `GET`  | `/api/Account/get-address-for-user` | Retrieve shipping address of the user |
| `PUT`  | `/api/Account/update-address` | Update user shipping address |
| `POST` | `/api/Account/reset-password` | Reset account password |

### 📦 Products
| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `GET`  | `/api/Products/get-all` | Fetch paginated products list |
| `GET`  | `/api/Products/get-by-id/{id}` | Get product details by ID |
| `POST` | `/api/Products/Add-Product` | Create a new product (with multipart image upload) |
| `PUT`  | `/api/Products/Update-Product` | Update product info and images |
| `DELETE`| `/api/Products/Delete-Product/{id}`| Remove a product |

### 🏷️ Categories
| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `GET`  | `/api/Categories/get-all` | Retrieve all product categories |
| `GET`  | `/api/Categories/get-by-id/{id}` | Get category by ID |
| `POST` | `/api/Categories/add-category` | Create a new category |
| `PUT`  | `/api/Categories/update-category` | Modify category details |
| `GET`  | `/api/Categories/delete-category/{id}` | Delete a category |

### 🛒 Baskets
| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `GET`  | `/api/Baskets/get-basket-item/{id}` | Get basket items by Basket ID |
| `POST` | `/api/Baskets/update-basket` | Create or update basket in Redis cache |
| `DELETE`| `/api/Baskets/delete-basket-item/{id}`| Clear or delete basket |

### 🚚 Orders & Payments
| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `GET`  | `/api/Orders/get-delivery` | Fetch available delivery methods |
| `POST` | `/api/Orders/create-order` | Place a new order |
| `GET`  | `/api/Orders/get-orders-for-user` | Get order history for authenticated user |
| `GET`  | `/api/Orders/get-order-by-id/{id}` | Fetch order details by ID |
| `POST` | `/api/Payments/Create` | Initialize Stripe payment intent |

### 🛠️ Administration
| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `GET`  | `/api/Administration/GetListRoles` | List all system roles |
| `POST` | `/api/Administration/CreateRole` | Create a new role |
| `PUT`  | `/api/Administration/EditRole` | Assign users to roles |
| `POST` | `/api/Administration/GetListUsers` | Paginated users search and list |
| `PUT`  | `/api/Administration/UpdateUserClaimsAndRoles` | Manage claims & permissions for a user |

---

## ⚡ Postman Collection

A pre-configured Postman Collection file (`AppEcomBlazor.postman_collection.json`) is included in the project repository. You can import this file directly into Postman to test all endpoints.
