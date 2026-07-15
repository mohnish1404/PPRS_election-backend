# 🗳️ Election Employee Management System — Backend API

![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-8.0-purple?logo=dotnet)
![MySQL](https://img.shields.io/badge/MySQL-8.0-orange?logo=mysql)
![EF Core](https://img.shields.io/badge/EF_Core-8-blue)
![JWT](https://img.shields.io/badge/Auth-JWT-green)
![License](https://img.shields.io/badge/License-Private-red)

A production-ready **ASP.NET Core 8 Web API** for the Election Employee Management System of the **Government of Chhattisgarh**. Handles authentication, user management, election duty randomization, exemptions, and audit logging.

---

## 📋 Table of Contents

- [Features](#-features)
- [Tech Stack](#-tech-stack)
- [Project Structure](#-project-structure)
- [Getting Started](#-getting-started)
- [Environment Configuration](#-environment-configuration)
- [Database Setup](#-database-setup)
- [API Documentation](#-api-documentation)
- [Authentication](#-authentication)
- [Database Schema](#-database-schema)
- [Security](#-security)

---

## ✨ Features

### 🔐 Authentication & Authorization
- JWT Bearer token authentication
- OTP-based login (Email & Mobile)
- Password login with CAPTCHA validation
- Role-based access control (Admin / User)
- Password expiry tracking (365 days)
- Forgot password with OTP flow
- Account activation workflow

### 👨‍💼 Admin Features
- Pending approval management (approve/reject with remarks)
- User management (list, activate, deactivate)
- User audit log tracking (who activated/deactivated and when)
- Election duty randomization by AC, designation
- Duty assignment report with AC-wise filtering
- Duty removal with reason logging
- Duty exemption with mandatory document upload
- Exemption history with restore capability
- Recent removal history (last 30 days)

### 👤 User Features
- User registration with admin approval workflow
- Profile management (view + update mobile/email)
- Application status tracking
- Duplicate mobile/email validation on update

### 📊 Master Data
- AC (Assembly Constituency) list
- District, Block master data
- Department, Designation, Office master data
- Duty posts, Election work master
- PWD type master, Employee type master
- Bank, Branch master

### 🗳️ Election Randomization
- Polling team assignment
- AC-wise duty count and availability
- Designation-wise assignment
- Bulk polling personnel save
- Part-wise booth assignment

### 🛡️ Duty Exemption System
- Grant exemption with 15 predefined reasons
- Mandatory document upload (PDF/DOC/DOCX, max 5MB)
- Server-side file validation (type + size)
- Document stored in local server folder
- Exemption status tracking (Active/Restored)
- Restore exemption to available pool
- Automatic available count recalculation

---

## 🛠️ Tech Stack

| Technology | Version | Purpose |
|---|---|---|
| **ASP.NET Core** | 8.0 | Web API Framework |
| **Entity Framework Core** | 8.x | ORM |
| **Pomelo MySQL EF Core** | 8.x | MySQL Provider |
| **MySQL** | 8.0 | Database |
| **JWT Bearer** | 8.x | Authentication |
| **Swagger (Swashbuckle)** | 6.x | API Documentation |
| **C#** | 12 | Programming Language |

---

## 📁 Project Structure

```
ElectionEmployeeAPI/
├── Controllers/
│   ├── AdminController.cs          # Admin operations (users, approvals, duties, exemptions)
│   ├── AuthController.cs           # Authentication (login, register, OTP, profile)
│   ├── EpicController.cs           # EPIC voter data
│   ├── MastersController.cs        # Master data (AC, districts, designations, etc.)
│   ├── PollingPersonnelController.cs  # Bulk employee data management
│   └── RandomizationController.cs  # Election duty randomization
│
├── Data/
│   ├── ApplicationDbContext.cs     # EF Core DbContext
│   ├── DutyExemption.cs           # Duty exemption model
│   ├── ExemptionReason.cs         # Exemption reason master model
│   └── UserAuditLog.cs            # User audit log model
│
├── DTOs/
│   ├── BulkPollingPersonnelDto.cs  # Bulk employee upload DTO
│   ├── ExemptionDtos.cs           # Exemption request/response DTOs
│   ├── LoginDto.cs                # Login request DTO
│   ├── LoginRequest.cs            # Login form DTO
│   ├── OtpDto.cs                  # OTP request DTO
│   ├── PollingPersonnelDto.cs     # Employee DTO
│   ├── PollingPersonnelUploadDto.cs  # Upload DTO
│   ├── RegisterDto.cs             # Registration DTO
│   ├── RemoveDutyRequest.cs       # Duty removal DTO
│   └── UpdateProfileDto.cs        # Profile update DTO
│
├── Models/
│   ├── User.cs                    # Application user model
│   ├── AdminUser.cs               # Admin user model
│   ├── AdminApproval.cs           # Approval request model
│   ├── OTP.cs                     # OTP model
│   └── [other models...]
│
├── Services/
│   └── AuthService.cs             # Authentication business logic
│
├── wwwroot/
│   └── uploads/
│       └── exemptions/            # Uploaded exemption documents
│
├── appsettings.json               # Configuration (DO NOT COMMIT)
├── appsettings.example.json       # Configuration template (safe to commit)
└── Program.cs                     # Application entry point & DI configuration
```

---

## 🚀 Getting Started

### Prerequisites

- **.NET 8 SDK** — [Download](https://dotnet.microsoft.com/download/dotnet/8)
- **MySQL 8.0** — [Download](https://dev.mysql.com/downloads/)
- **MySQL Workbench** (recommended) — [Download](https://dev.mysql.com/downloads/workbench/)
- **Visual Studio 2022** or **VS Code** with C# extension

### Installation

```bash
# 1. Clone the repository
git clone https://github.com/YOUR_USERNAME/election-backend.git

# 2. Navigate to project folder
cd election-backend/ElectionEmployeeAPI

# 3. Copy config template
copy appsettings.example.json appsettings.json

# 4. Update appsettings.json with your DB credentials
#    (see Environment Configuration section)

# 5. Restore NuGet packages
dotnet restore

# 6. Apply database migrations
dotnet ef database update

# 7. Run the application
dotnet run
```

API will be available at:
- **HTTP:** `http://localhost:5103`
- **Swagger UI:** `http://localhost:5103/swagger`

---

## 🔧 Environment Configuration

Copy `appsettings.example.json` to `appsettings.json` and fill in your values:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;database=election_db;user=root;password=YOUR_MYSQL_PASSWORD;port=3306"
  },
  "Jwt": {
    "Key": "YOUR_SUPER_SECRET_JWT_KEY_MIN_32_CHARACTERS",
    "Issuer": "ElectionEmployeeAPI",
    "Audience": "ElectionEmployeeClient",
    "ExpiryDays": 7
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "AllowedOrigins": "http://localhost:5173"
}
```

> ⚠️ **Never commit `appsettings.json`** — it contains your database password and JWT secret. It is already in `.gitignore`.

---

## 🗄️ Database Setup

### Step 1: Create Database

Open MySQL Workbench and run:

```sql
CREATE DATABASE election_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

### Step 2: Run EF Core Migrations

```bash
# Install EF Core tools (if not already installed)
dotnet tool install --global dotnet-ef

# Apply all migrations
dotnet ef database update
```

This will create all tables automatically.

### Step 3: Run Manual SQL (Exemption Tables)

Run this SQL in MySQL Workbench to create exemption tables and seed reasons:

```sql
CREATE TABLE ExemptionReasons (
    ReasonId      INT AUTO_INCREMENT PRIMARY KEY,
    ReasonText    VARCHAR(200) NOT NULL,
    IsActive      TINYINT(1) NOT NULL DEFAULT 1
);

INSERT INTO ExemptionReasons (ReasonText) VALUES
('Medical / Health Issue'),
('Pregnancy'),
('Death in Family (Bereavement)'),
('Transferred Out of District'),
('Already on Election Duty Elsewhere (Double Booking)'),
('Court / Legal Obligation'),
('Retired from Service'),
('Resigned from Service'),
('Long Leave (Approved)'),
('Disciplinary Action / Suspension'),
('Physically Unfit (PWD related, post-assignment)'),
('Family Emergency'),
('Out of State / Unreachable'),
('Administrative Error (Wrong Assignment)'),
('Other (Admin Discretion)');

CREATE TABLE DutyExemptions (
    ExemptionId           INT AUTO_INCREMENT PRIMARY KEY,
    MemberId               INT NOT NULL,
    EmpCode                VARCHAR(50) NOT NULL,
    AC_No                  INT NOT NULL,
    Part_No                INT NOT NULL,
    DutyPost               VARCHAR(100) NULL,
    ReasonId               INT NOT NULL,
    Remarks                VARCHAR(500) NULL,
    DocumentPath           VARCHAR(300) NOT NULL,
    DocumentOriginalName   VARCHAR(260) NULL,
    ExemptedBy             VARCHAR(100) NOT NULL,
    ExemptionDateTime      DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Status                 VARCHAR(20) NOT NULL DEFAULT 'Active',
    RestoredBy             VARCHAR(100) NULL,
    RestoredDateTime       DATETIME NULL,
    CONSTRAINT FK_DutyExemptions_Reason FOREIGN KEY (ReasonId) REFERENCES ExemptionReasons(ReasonId)
);

CREATE TABLE UserAuditLogs (
    LogId           INT AUTO_INCREMENT PRIMARY KEY,
    TargetUserId    VARCHAR(50) NOT NULL,
    ActionType      VARCHAR(30) NOT NULL,
    PerformedBy     VARCHAR(100) NOT NULL,
    PerformedAt     DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Remarks         VARCHAR(300) NULL
);
```

---

## 📚 API Documentation

### Swagger UI

Once running, visit: **`http://localhost:5103/swagger`**

All endpoints are documented with request/response schemas.

---

### 🔐 Auth Endpoints

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/api/auth/login` | None | Login with password |
| POST | `/api/auth/login-otp` | None | Login with OTP |
| POST | `/api/auth/register` | None | Register new user |
| POST | `/api/auth/send-otp` | None | Send OTP to email/mobile |
| POST | `/api/auth/verify-otp` | None | Verify OTP |
| POST | `/api/auth/forgot-password` | None | Forgot password flow |
| GET | `/api/auth/my-approval-status` | JWT | Get approval status |
| GET | `/api/auth/my-profile` | JWT | Get full user profile |
| PUT | `/api/auth/update-profile` | JWT | Update mobile + email |

---

### 👨‍💼 Admin Endpoints

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/Admin/pending-approvals` | Admin | Get pending requests |
| POST | `/api/Admin/approve/{id}` | Admin | Approve a request |
| POST | `/api/Admin/reject/{id}` | Admin | Reject with reason |
| GET | `/api/Admin/users` | Admin | Get all users |
| POST | `/api/Admin/toggle-user/{userId}` | Admin | Activate/Deactivate user |
| GET | `/api/Admin/assigned-duties` | Admin | Get duty assignments |
| POST | `/api/Admin/remove-duty` | Admin | Remove duty assignment |
| GET | `/api/Admin/recent-removals` | Admin | Recent removals (30 days) |
| GET | `/api/Admin/exemption-reasons` | Admin | Get 15 exemption reasons |
| POST | `/api/Admin/exemptions` | Admin | Grant duty exemption |
| GET | `/api/Admin/exemptions` | Admin | List all exemptions |
| POST | `/api/Admin/exemptions/{id}/restore` | Admin | Restore exemption |
| GET | `/api/Admin/audit-logs/{userId}` | Admin | Get user audit logs |

---

### 📊 Master Data Endpoints

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/masters/ac-list` | JWT | Assembly Constituency list |
| GET | `/api/masters/districts` | JWT | District list |
| GET | `/api/masters/designations` | JWT | Designation list |
| GET | `/api/masters/departments` | JWT | Department list |
| GET | `/api/masters/offices` | JWT | Office list |
| GET | `/api/masters/duty-posts` | JWT | Duty post list |
| GET | `/api/masters/banks` | JWT | Bank list |

---

### 🗳️ Randomization Endpoints

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/randomization/ac-stats` | Admin | AC-wise duty stats |
| POST | `/api/randomization/assign` | Admin | Assign duties |
| GET | `/api/randomization/assigned-count` | Admin | Count assigned duties |

---

## 🔐 Authentication

All protected endpoints require a **JWT Bearer token** in the Authorization header:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### JWT Token Claims

| Claim | Description |
|---|---|
| `nameidentifier` | User ID (userId field) |
| `name` | Full name |
| `emailaddress` | Email |
| `role` | `Admin` or `User` |

### Token Expiry

Tokens expire after **7 days** (configurable in `appsettings.json`).

---

## 🗃️ Database Schema

### Key Tables

| Table | Description |
|---|---|
| `admin_users` | Admin accounts |
| `Users` | Registered user accounts |
| `AdminApprovals` | Registration/activation approval requests |
| `OTPs` | OTP records for login/forgot password |
| `PollingPersonnel` | Employee master data |
| `polling_team` | Assigned polling teams |
| `polling_team_members` | Team member assignments |
| `DutyPosts` | Polling duty post types |
| `ExemptionReasons` | 15 predefined exemption reasons |
| `DutyExemptions` | Duty exemption records with documents |
| `duty_removal_log` | Removed duty history |
| `UserAuditLogs` | User activate/deactivate audit trail |
| `newpartlist` | AC/Part booth master data |
| `Districts`, `Blocks` | Location master data |
| `Departments`, `Designations` | HR master data |

---

## 🔒 Security

- ✅ **JWT Authentication** — Stateless token-based auth
- ✅ **Role-based Authorization** — `[Authorize(Roles = "Admin")]` on admin endpoints
- ✅ **Password Hashing** — BCrypt via custom hash implementation
- ✅ **CORS** — Restricted to allowed frontend origins only
- ✅ **File Upload Validation** — Type (PDF/DOC/DOCX) + Size (5MB) checked server-side
- ✅ **Sensitive file protection** — `appsettings.json` in `.gitignore`
- ✅ **Input Validation** — DTO validation attributes on all inputs
- ✅ **Duplicate check** — Mobile + email uniqueness enforced on update

---

## 🚀 Deployment

### Build for Production

```bash
dotnet publish -c Release -o ./publish
```

### Recommended Hosting

| Option | Cost | Notes |
|---|---|---|
| **Railway** | Free tier | Easy .NET deployment |
| **Render** | Free tier | Docker-based |
| **Azure App Service** | Paid | Best for .NET |
| **VPS (Ubuntu)** | Low cost | Full control |

### Environment Variables for Production

Set these in your hosting platform (not in files):

```
ConnectionStrings__DefaultConnection=server=...;database=...;user=...;password=...
Jwt__Key=your-production-secret-key
AllowedOrigins=https://your-frontend-domain.vercel.app
```

---

## 👨‍💻 Developer Notes

- **EF Core Code-First** — Models define DB schema, not the other way around
- **MySQL** with Pomelo provider — Column names follow C# conventions
- **File uploads** saved to `wwwroot/uploads/exemptions/` with GUID filename
- **Admin user** must be created directly in DB (no self-registration for admin)
- **OTP** is 6-digit, valid for 10 minutes, stored in `OTPs` table
- **dflag** field in `PollingPersonnel` — `false` means active employee

---

## 📞 Support

For issues, contact the development team or raise a GitHub issue.

---

*Built with ❤️ for Government of Chhattisgarh Election Commission*
