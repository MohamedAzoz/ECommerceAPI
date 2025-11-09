🛒 E-Commerce API – ASP.NET Core Web API

A complete and production-ready E-Commerce API built using ASP.NET Core, following clean architecture principles, deep separation of concerns, authentication/authorization, product management, orders, reviews, addresses, payment integration, and more.

This project was designed to simulate a real-world backend system with modern practices and scalable architecture.

✅ Features Overview

This API includes a full set of features required for any modern e-commerce backend:

🔐 Authentication & Authorization

Register / Login

Email Verification (send code to user’s email)

Role Management (Admin, User…)

JWT Authentication (Bearer token)

Microsoft Identity Integration

🛒 Cart Management

Add items

Update quantities

Delete items

Get user cart

📦 Products

Add / Update / Delete products

Get all products

Search by keyword

Get product by ID

Upload product images

Download / Delete images

⭐ Reviews

Add review

Get reviews by ProductId

Delete review

📍 Addresses

Add / Update / Delete / Get addresses

🧾 Orders

Create new order

Get all orders

Get orders by user

Update order status

Delete order

💳 Payment Integration (NEW)

Integrated Paymob payment gateway:

Generate payment link

Handle callback endpoint

Update order after successful payment

🏗️ Architecture

The project follows a clean, maintainable, and scalable structure:

ECommerce.API           → Presentation Layer (Controllers)
ECommerce.Core          → Domain Layer (Interfaces, Entities, DTOs)
ECommerce.Infrastructure → Data Access Layer (EF Core, Repositories)

✅ Applied Principles

Separation of Concerns

Dependency Injection

Service Layer Pattern (move logic out of controllers)

Repository Pattern (optional)

AutoMapper for object mapping

Result Pattern for unified API responses

Global Exception Handler to catch all unhandled exceptions

🧠 Result Pattern

All endpoints return a consistent API response format:

{
  "success": true,
  "message": "Product created successfully",
  "data": { ... }
}


This improves readability, debugging, and frontend integration.

🔥 Global Exception Handling

A global middleware catches all unhandled exceptions and returns uniform error responses.
This ensures:

Cleaner controllers

No duplicated try/catch

Better production error handling

🗂️ Technologies Used

✅ ASP.NET Core Web API

✅ Entity Framework Core

✅ SQL Server

✅ LINQ

✅ Microsoft Identity

✅ JWT Authentication

✅ AutoMapper

✅ Paymob API Integration

✅ Validation using Data Annotations

✅ Clean Architecture Structure

✅ Result Pattern

✅ Global Exception Handling

📁 API Endpoints
✅ Auth

POST /api/Auth/Register

POST /api/Auth/Login

POST /api/Auth/VerifyEmail

✅ Products

GET /api/Products

GET /api/Products/{id}

GET /api/Products/{word}

POST /api/Products

PUT /api/Products

DELETE /api/Products/{id}

✅ Product Images

POST /api/ProductImage/upload

GET /api/ProductImage/download/{fileName}

DELETE /api/ProductImage/{fileName}

✅ Cart

GET /api/Cart

POST /api/Cart

PUT /api/Cart

DELETE /api/Cart/{id}

✅ Reviews

GET /api/Reviews/{productId}

POST /api/Reviews

DELETE /api/Reviews/{id}

✅ Orders

GET /api/Orders

GET /api/Orders/GetByUserId

GET /api/Orders/{id}

POST /api/Orders/NewOrder

PUT /api/Orders/{id}/status

DELETE /api/Orders/{id}

✅ Payment

POST /api/Payment/create-link

GET /api/paymob/callback

✅ Addresses

POST /api/Addresses

PUT /api/Addresses

DELETE /api/Addresses/{id}

GET /api/Addresses

✅ Roles

POST /api/Role

📸 Swagger UI

The entire API is fully documented using Swagger:
✅ All endpoints
✅ Models
✅ Authorization

🚀 Future Enhancements

Caching layer

Unit tests + Integration tests

Redis for cart & session

Docker deployment

Notification service (email/SMS)

✅ How to Run

Clone the repository

Update appsettings.json with your SQL Server connection

Run migrations:

update-database


Run the API

Open Swagger:

https://localhost:{port}/swagger

🧑‍💻 Author

Mohamed Azoz
Full Stack .NET & Angular Developer
