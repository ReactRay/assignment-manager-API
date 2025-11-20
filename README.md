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

-   Frontend: https://github.com/ReactRay/assignment-manager-Client\
-   Backend: https://github.com/ReactRay/assignment-manager-API

------------------------------------------------------------------------

## 🖼️ Preview

![Preview](https://res.cloudinary.com/danlxus36/image/upload/v1763597207/Screenshot_1_urdpjp.png)

------------------------------------------------------------------------

## 🚀 Features

### Admin

-   Full user management\
-   Assign roles (admin/teacher/student)\
-   Create teachers, students, admins\
-   Manage system-wide permissions

### Teacher

-   Create assignments\
-   Update / delete assignments\
-   Grade submissions\
-   Manage classes & submissions

### Student

-   View assignments\
-   Submit homework with file upload\
-   Download feedback + graded files\
-   Track progress

------------------------------------------------------------------------

## 🛠️ Tech Stack

### Frontend

-   React (Vite)
-   Axios
-   React Router
-   Custom CSS
-   Role-based routing

### Backend

-   ASP.NET Core 8 Web API
-   Entity Framework Core
-   SQL Server
-   ASP.NET Identity + JWT
-   AutoMapper
-   Repository Pattern

### Deployment

-   Vercel (Frontend)
-   Azure App Service (Backend)
-   Azure SQL Database

------------------------------------------------------------------------

## 🔐 Authentication & Authorization

Roles: - `Admin` - `Teacher` - `Student`

The backend uses: - ASP.NET Identity\
- JWT Authentication\
- Policy-based authorization

------------------------------------------------------------------------

# 📘 Full API Reference

## 🔑 Auth Endpoints

### Register

`POST /api/Auth/register`

### Login

`POST /api/Auth/login`

### Seed Roles

`POST /api/Auth/seed-roles`

------------------------------------------------------------------------

## 🛂 Admin Endpoints

### Get all users

`GET /api/Admin/users`

### Assign role to user

`POST /api/Admin/users/{userId}/roles/{roleName}`

### Remove role from user

`DELETE /api/Admin/users/{userId}/roles/{roleName}`

### Create admin

`POST /api/Admin/create-admin`

### Create teacher

`POST /api/Admin/create-teacher`

### Create student

`POST /api/Admin/create-student`

------------------------------------------------------------------------

## 📝 Assignment Endpoints

### Create

`POST /api/Assignments`

### Get all

`GET /api/Assignments`

### Get by id

`GET /api/Assignments/{id}`

### Update

`PUT /api/Assignments/{id}`

### Delete

`DELETE /api/Assignments/{id}`

------------------------------------------------------------------------

## 📤 Submission Endpoints

### Submit assignment

`POST /api/Submissions`

### Get submission by id

`GET /api/Submissions/{id}`

### Get submissions for assignment

`GET /api/Submissions/assignment/{assignmentId}`

### Grade submission

`PUT /api/Submissions/{id}/grade`

### Get my submissions

`GET /api/Submissions/mine`

### Download file

`GET /api/Submissions/{id}/download`

------------------------------------------------------------------------

# ⚙️ Local Development Setup

## 1️⃣ Clone repos

``` bash
git clone https://github.com/ReactRay/assignment-manager-Client
git clone https://github.com/ReactRay/assignment-manager-API
```

------------------------------------------------------------------------

## 2️⃣ Backend Setup

``` bash
cd assignment-manager-API
dotnet restore
```

### Edit `appsettings.json`:

``` json
"DefaultConnection": "Server=YOUR_SERVER;Database=AssignmentManagerDB;Trusted_Connection=True;TrustServerCertificate=True"
```

### Apply migrations:

``` bash
dotnet ef database update
```

### Run API:

``` bash
dotnet run
```

------------------------------------------------------------------------

## 3️⃣ Frontend Setup

``` bash
cd assignment-manager-Client
npm install
```


### Start:

``` bash
npm run dev
```

------------------------------------------------------------------------

# 👨‍💻 Author

**Radwan Mansur**\
Portfolio: https://www.radwan-mansur.com\
GitHub: https://github.com/ReactRay\
LinkedIn: https://www.linkedin.com/in/radwan-mansur-1368b7232/\
Email: raydiaz1899@gmail.com

------------------------------------------------------------------------

## ⭐ Support

If you like this project, please ⭐ the repos!
