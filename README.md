# Cinema Management System

## 🎬 Overview

Cinema Management System is a web application built with ASP.NET Core MVC that allows for managing a cinema's operations, including movies, screenings, screening rooms, tickets, and user accounts. The system supports roles such as admin, employees and users, and includes full JWT authentication, a ticketing module, employee management panel, and seat selection logic.

---

## 📁 Project Structure

```
Cinema_Management_System/
├── Controllers/          # ASP.NET MVC Controllers
├── DTOs/                 # Data Transfer Objects for views and API
├── Models/               # EF Core models for Movie, Screening, Ticket, etc.
├── Services/             # Business logic (e.g., AddMovieService, ScreeningService)
├── Views/                # Razor views for MVC frontend
├── wwwroot/              # Static content (images, CSS, JS)
├── Data/                 # DbContext and database initialization
├── Mappers/              # Manual mapping from models to DTOs
├── appsettings.json      # Config files
```

---

## 🔐 Authentication and Roles

The system uses **ASP.NET Core Identity** with **JWT authentication**. There are two main roles:

* **Employee**: Can manage movies, screenings, screening rooms
* **User**: Can browse movies, book tickets, view their purchases

---

## 🎥 Features

### ✅ Movie Management

* Employees can add, edit, and delete movies.
* Poster upload support.
* Displayed on the main page with basic info (title, duration, poster).

### ✅ Screening Room Management

* Employees can add and edit screening rooms.
* Room format selection (e.g., 2D, 3D, IMAX).
* Seat layout visualized in the interface.

### ✅ Screening Management

* Add new screenings with:

  * Selected movie
  * Start time (end time auto-calculated)
  * Assigned room
* Validate overlapping screenings in the same room
* Edit/delete only if no tickets have been sold

### ✅ Seat Selection & Ticket Purchase

* Customers can select a screening and view available seats
* Visual seat map displayed (disabled for taken ones)
* Seat selection with base price
* Upon purchase, ticket stored in DB with user ID and purchase time

### ✅ Ticket Display

* Users can go to "My Tickets" page to see all tickets:

  * Movie title
  * Screening time
  * Seat number
  * Final price
  * QR code

### ✅ PDF Ticket Generator

* \[GET] `/PDF/{ticketId}` endpoint returns PDF version of the ticket
* Includes a QR code and movie info

---

## 🧰 Employee Panel

Accessible only to logged-in users with the **Employee** role:

* Add/Edit/Delete Movies
* Add/Edit Screening Rooms
* Add/Edit Screenings
* Delete screenings only if no tickets are sold
* View screenings per date (with a DatePicker UI)

---

## 📊 Technologies Used

* ASP.NET Core MVC
* Entity Framework Core
* JWT Authentication
* Identity Roles
* NLog for error logging
* Razor Views
* Bootstrap for UI
* Moq + xUnit/NUnit for tests

---

## ⚙️ Database Indexes (Performance Optimization)

Custom indexes were added to optimize frequent queries:

```sql
CREATE NONCLUSTERED INDEX IX_Screenings_DateStartTime ON Screenings (DateStartTime);

CREATE NONCLUSTERED INDEX IX_Tickets_UserId_Covering_Full
ON Tickets(UserId)
INCLUDE (Id, ScreeningId, SeatId, PurchaseDate, FinalPrice);
```

These indexes help speed up:

* Searching screenings by date
* Retrieving all tickets for a given user

---

## 🚀 CI/CD

The solution includes a GitHub Actions workflow (`dotnet-ci.yml`) that:

* Builds the solution (`dotnet build`)
* Runs tests (`dotnet test`)
* Optionally builds and pushes a Docker image (if configured)

---

## 📋 Future Improvements

* Role-based dashboards
* Online payment integration
* Admin-level statistics
* Archival of past screenings
