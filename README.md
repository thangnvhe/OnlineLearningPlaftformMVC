# 🎓 OnlineLearning Platform

**OnlineLearning** is a comprehensive e-learning ecosystem built on the **ASP.NET Core 8** framework. The project focuses on bridging the gap between Instructors (**Mentors**) and Students (**Mentees**) through real-time interaction tools, structured learning paths, and modern payment integrations.

---

## 🌟 Core Features

### 1. Training & Course Management
* **Learning Paths:** Organize lessons into chapters, supporting embedded videos (YouTube API) and downloadable attachments.
* **Mentorship Management:** Advanced Role-Based Access Control (RBAC) for **Admin - Mentor - Mentee**.

---

## 🏗 System Architecture

The project follows a **Layered/Modular Architecture** to ensure scalability and maintainability:
* **Framework:** .NET 8 (LTS).
* **Data Access:** Entity Framework Core (SQL Server).
* **Design Patterns:** Repository Pattern & Dependency Injection.

---

## 🏗 Project Structure

The project is organized into clear layers, ensuring a strict Separation of Concerns (SoC) between Business Logic, Data Access, and the Presentation layer.

### 📂 Key Directory Overview

* **`Areas/`**: Segregates workspaces for different roles (Admin, Mentor, Mentee). Each area contains its own Controllers and Views.
* **`Configurations/`**: Centralized system service setup (Dependency Injection), DbContext configuration, Authentication, Sessions, and Middlewares.
* **`Controllers/`**: Handles incoming HTTP requests, orchestrates services, and returns Data/Views to the user.
* **`Data/`**: Contains the `OnlLearnDBContext` and Entity Configurations for database mapping.
* **`Repositories/`**: Implements the Repository Pattern to abstract data access, facilitating Unit Testing and database flexibility.
* **`Services/`**: The **Business Logic Layer (BLL)**. Contains the core logic for all features, including calculations, data processing, and payment integrations.
* **`Models/` & `Enums/`**: Defines Database Entities, Data Transfer Objects (DTOs), and global constants.
* **`Migrations/`**: Stores database schema history (Code-First approach).
* **`Attribute/`**: Custom attributes for specialized authorization and data validation.
* **`ViewComponents/`**: Reusable UI components (e.g., Sidebars, Navbars) to keep Views clean and modular.
* **`Utils/` & `Common/`**: Utility classes for string manipulation, date formatting, and helpers for MoMo or Google APIs.
* **`wwwroot/`**: Static assets including CSS, JavaScript, images, and frontend libraries.

---

## 🛠 Quickstart (Local Setup)

### Prerequisites:
* .NET SDK 8.x
* SQL Server (Local or Docker)

### Installation Steps:
1. **Clone the repository:**
   ```bash
   git clone [https://github.com/thangnvhe/OnlineLearningPlaftformMVC.git](https://github.com/thangnvhe/OnlineLearningPlaftformMVC.git)
