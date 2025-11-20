# Assignment Manager --- Fullstack School System

A complete Admin--Teacher--Student management platform built with
**React** and **ASP.NET Core**, featuring authentication, role-based
permissions, assignments, submissions, file uploads, and dashboards.\
Fully deployed on **Vercel (Frontend)** and **Azure (API + SQL
Server)**.

------------------------------------------------------------------------

## 📌 Live Demo

### Frontend (Vercel)

https://assignment-manager-client-j9x3.vercel.app/

------------------------------------------------------------------------

## 📂 Repositories

-   **Frontend:** https://github.com/ReactRay/assignment-manager-Client
-   **Backend:** [https://github.com/ReactRay/assignment-manager-API](https://github.com/ReactRay/assignment-manager-API)

------------------------------------------------------------------------

## 🖼️ Preview

![Preview](https://res.cloudinary.com/danlxus36/image/upload/v1763597207/Screenshot_1_urdpjp.png)

------------------------------------------------------------------------

## 🚀 Features

### Admin

-   Full user management (create, edit, delete)
-   Manage teachers, students, and admins
-   System-wide permissions and access control

### Teacher

-   Create assignments
-   View and grade submissions
-   Upload files (PDF/doc/images)
-   update , delete assignments

### Student

-   View assignments
-   Submit homework with file upload
-   Track submission status and grades

------------------------------------------------------------------------

## 🛠️ Tech Stack

### Frontend

-   React (Vite)
-   Axios
-   React Router
-   Custom CSS styling
-   Role-protected routing

### Backend

-   ASP.NET Core 8 Web API
-   Entity Framework Core
-   SQL Server (Azure)
-   ASP.NET Identity + JWT
-   AutoMapper
-   Repository Pattern

### Deployment

-   Vercel (React client)
-   Azure App Service (API)
-   Azure SQL Database

------------------------------------------------------------------------

## 🔐 Authentication & Authorization

-   JWT token-based authentication
-   ASP.NET Identity user management
-   Role-based authorization:
    -   `Admin`
    -   `Teacher`
    -   `Student`
-   Protected API + protected frontend routes

------------------------------------------------------------------------

## 📁 File Uploads

Supports uploading: - PDF\
- Images\
- Word docs\
- Any file stored as byte\[\] in SQL Server

Teachers and admins can download submissions.

------------------------------------------------------------------------

## 📘 Pages Overview

### Public

-   Home
-   Guide
-   Login
-   Register

### Protected

-   Admin Dashboard
-   Teacher Dashboard
-   Student Dashboard
-   Assignment Create / Submit
-   Submissions Viewer

------------------------------------------------------------------------

## 🌐 API Endpoints (Examples)

### Authentication

``` http
POST /api/auth/login
POST /api/auth/register
```

### Assignments

``` http
GET /api/assignments
POST /api/assignments
GET /api/assignments/{id}
```

### Submissions

``` http
POST /api/submissions
GET /api/submissions/by-assignment/{assignmentId}
```

------------------------------------------------------------------------

## 📁 Project Structure

### Frontend

    src/
     ├─ pages/
     ├─ components/
     ├─ api/axiosClient.js
     ├─ utils/
     └─ styles/

### Backend

    Controllers/
    Models/
    DTOs/
    Repositories/
    Services/
    Migrations/
    Permissions/

------------------------------------------------------------------------

## 🚀 Deployment Notes

### Frontend (Vercel)

-   Vite bundler
-   Axios interceptors
-   Auto-redirect based on user role

### Backend (Azure)

-   Published via Visual Studio
-   Azure SQL Database
-   CORS configured for Vercel
-   ASP.NET Identity + JWT configured

------------------------------------------------------------------------

## 📘 System Guide

Available at `/guide` inside the deployed app.

------------------------------------------------------------------------

## 👨‍💻 Author

**Radwan Mansur**\
Fullstack Developer (React • .NET • SQL)

-   Portfolio: https://www.radwan-mansur.com\
-   GitHub: https://github.com/ReactRay\
-   LinkedIn: https://www.linkedin.com/in/radwan-mansur-1368b7232/\
-   Email: raydiaz1899@gmail.com

------------------------------------------------------------------------

## ⭐ Support


