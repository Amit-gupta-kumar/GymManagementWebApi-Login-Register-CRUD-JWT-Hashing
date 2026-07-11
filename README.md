# Gym Management System API (with JWT Authentication & Dapper)

This repository contains a fully functional **Gym Management System Web API** built with **ASP.NET Core (.NET 8)**, **Dapper**, and **SQL Server**. It includes secure JWT-based authentication, user registration with hashed passwords, model-state validations, and full CRUD operations for managing gym membership plans and members.

---

## 🚀 Key Features

- **User Authentication Layer**: Secure sign-up (`/api/auth/register`) and sign-in (`/api/auth/login`) capabilities.
- **Password Security**: Uses modern password hashing through the **BCrypt.Net-Next** package to ensure passwords are never stored in plain text.
- **JWT Authorization Safeguards**: High-security endpoints (like adding or modifying gym members) are strictly guarded via JWT tokens.
- **Micro-ORM Efficiency**: Utilizes **Dapper** for high-performance raw SQL queries and data mapping, optimizing execution directly from the application layer.
- **Automatic Data Validation**: Protects database integrity using strict C# Data Annotation rules (e.g., verifying active plan durations, valid 10-digit phones, and checking that a date of birth cannot be in the future).
- **Interactive API Documentation**: Full **Swagger UI** integration with customized Bearer token lock interfaces to test endpoints natively.

---

## 🛠️ Technology Stack

- **Backend Framework**: ASP.NET Core Web API (.NET 8.0)
- **Data Access Layer**: Dapper (Micro-ORM) + Microsoft.Data.SqlClient
- **Database Engine**: Microsoft SQL Server (Local Express Edition. "im here usning Developer Edition so i don't need to inculde that ."//Ecpress" in my server name" )
- **Security & Authorization**: System.IdentityModel.Tokens.Jwt + BCrypt.Net-Next
- **API Explorer & Documentation**: Swagger / OpenAPI Support

---

## 💾 Database Setup & Architecture

### 1. Create the Database
Open **SQL Server Management Studio (SSMS)**, connect to your local server instance (e.g., `.\\SQLEXPRESS`), open a **New Query** window, and execute the following initialization script:

```sql
CREATE DATABASE GymManagementDB;
GO

USE GymManagementDB;
GO

-- 1. Create Users Table
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    Role NVARCHAR(30) NOT NULL,
    IsActive BIT DEFAULT 1
);

-- 2. Create MembershipPlans Table
CREATE TABLE MembershipPlans (
    MembershipPlanId INT IDENTITY(1,1) PRIMARY KEY,
    PlanName NVARCHAR(100) NOT NULL,
    DurationInMonths INT NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    IsActive BIT DEFAULT 1
);

-- 3. Create Members Table
CREATE TABLE Members (
    MemberId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(150) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    Gender NVARCHAR(20) NOT NULL,
    DateOfBirth DATE NOT NULL,
    JoinDate DATE NOT NULL,
    MembershipPlanId INT FOREIGN KEY REFERENCES MembershipPlans(MembershipPlanId),
    IsActive BIT DEFAULT 1
);
GO
```

# 💻 Running & Configuring the API

## 1. Configure the Database Connection

Open the `appsettings.json` file in the root of your project and update the `DefaultConnection` string to match your local SQL Server instance.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=GymManagementDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "YourSuperSecretSecretKeyThatIsVeryLong123!",
    "Issuer": "GymManagementAPI",
    "Audience": "GymManagementAPI"
  }
}
```

---

## 2. Run the Application

1. Open the **GymManagement.sln** solution in **Visual Studio 2022**.
2. Restore all required NuGet packages if prompted.
3. Ensure the following packages are installed:
   - Dapper
   - Microsoft.Data.SqlClient
   - Microsoft.AspNetCore.Authentication.JwtBearer
   - BCrypt.Net-Next
4. Press **F5** or click the **Start** (▶) button to run the project.
5. Swagger UI will automatically open in your browser.

```
https://localhost:<port>/swagger
```

---

# 🧪 Testing the API Using Swagger

## Step 1: Register a New User

Expand the following endpoint:

```
POST /api/auth/register
```

1. Click **Try it out**
2. Enter your registration details.
3. Click **Execute**.

---

## Step 2: Login

Open:

```
POST /api/auth/login
```

1. Click **Try it out**
2. Enter the registered email and password.
3. Click **Execute**.
4. Copy the generated JWT Token from the response.

Example:

```text
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## Step 3: Authorize Swagger

Click the **Authorize** 🔒 button located in the top-right corner of Swagger.

Paste your token in the following format:

```text
Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

> **Note:** There must be **one space** between `Bearer` and the token.

Click **Authorize**, then **Close**.

---

## Step 4: Test Protected Endpoints

After authorization, you can access all secured endpoints, including:

- **Member Management**
  - GET `/api/member`
  - GET `/api/member/{id}`
  - POST `/api/member`
  - PUT `/api/member/{id}`
  - DELETE `/api/member/{id}`

- **Membership Plan Management**
  - GET `/api/membershipplan`
  - GET `/api/membershipplan/{id}`
  - POST `/api/membershipplan`
  - PUT `/api/membershipplan/{id}`
  - DELETE `/api/membershipplan/{id}`

---

# ✅ Validation Rules

The API includes built-in validation.

Examples:

- ❌ Future dates are **not allowed** for a member's **Date of Birth**.
- ❌ Invalid **MembershipPlanId** values are rejected.
- ❌ Invalid requests return a structured **400 Bad Request** response.

---

# 🎉 Project Completed

Congratulations! 🎉

You have successfully built a complete **ASP.NET Core 8 Web API** featuring:

- 🔐 JWT Authentication
- 🔒 BCrypt Password Hashing
- 👤 User Registration & Login
- 📦 CRUD Operations
- 🗄️ SQL Server Database Integration
- ⚡ Dapper ORM
- 📖 Swagger API Documentation
- ✅ Model Validation
- 🏗️ Clean Project Structure
