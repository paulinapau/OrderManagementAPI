# Order Management API

## Overview

This project implements a **RESTful Order Management API** for a retailer company.

The system allows:

- managing products
- applying product discounts
- creating orders
- generating invoices
- generating reports for discounted products

The API is implemented using **.NET 10** and follows **Onion Architecture**, separating:

- Domain
- Application
- Infrastructure
- API

This separation keeps business logic independent from infrastructure frameworks such as Entity Framework.

---

# Prerequisites

To run the project locally you need:

- .NET SDK
- Docker
- Git

PostgreSQL is provided through **Docker Compose** for easier local setup.

---

# Running the Project

## 1 Clone the repository

git clone https://github.com/your-repository/order-management-api.git

cd order-management-api

---

## 2 Start PostgreSQL

The database runs in a Docker container.
docker compose up -d

---

## 3 Restore dependencies

dotnet restore

---

## 4 Apply database migrations

dotnet ef databse update

---

## 5 Run the API

dotnet run

# Swagger API Documentation

Once the API is running, open Swagger to explore and test endpoints:
http://localhost:8080/swagger/index.html

or different port can be found in the Containers window → Ports tab.

# Database Configuration

If the API cannot connect to PostgreSQL inside Docker, update the connection string in **appsettings.json**.

Example:
"ConnectionStrings": {
"DefaultConnection": "Host=localhost;Port=5432;Database=mydb;Username=postgres;Password=postgres"
}

# Notes

solution implements these:

- Automated tests;
- API Documentation generated from code (hint - Swagger);
- Containerization ( Docker Compose for easier local setup/testing);
- RESTful API;
- Code is structured using some known architecture (e.g., NTier, Onion, etc.);
- Pagination support;

# Spent time

4h setting up project/database/Docker
4h implementing APIs
1h Writing and running tests
