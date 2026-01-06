# 🎓 Student Grade Management System

## 📌 Project Overview
This project is a **Student Grade Management System** developed using **C# (WinForms)** with **MySQL** as the database.  
The system focuses on **backend logic, database operations, and role-based access control**, while the user interface is implemented at a basic level using WinForms.

The project was developed for **academic learning purposes**, emphasizing system analysis, backend design, and data management rather than UI complexity.

---

## ✨ Key Features
- User authentication with **role-based access control** (Admin / Teacher / Student)
- Separate menus and functionalities for **teachers and students**
- **CRUD operations** for:
  - Students
  - Subjects
  - Grades
- Backend logic for grade management and data validation
- Database integration using MySQL
- Basic WinForms UI built with drag-and-drop components

---

## 🧠 System Workflow
1. User logs in with username and password
2. The system verifies the user role from the database
3. Role-based menu is displayed (Teacher or Student)
4. Teachers can manage students, subjects, and grades
5. Students can view their academic information
6. All data operations are processed through backend logic and stored in the database

---

## 🧰 Technologies Used
- **Programming Language:** C#
- **Framework:** .NET (WinForms)
- **Database:** MySQL
- **Tools:** Visual Studio 2022, XAMPP

---

## ⚙️ Installation & Setup

### 1. Software Requirements
- Visual Studio 2022
- XAMPP (Apache & MySQL)

### 2. Database Setup (Using XAMPP)
1. Download XAMPP: https://www.apachefriends.org/download.html  
2. Install and open **XAMPP Control Panel**
3. Start **Apache** and **MySQL**
4. Open browser and go to:  
http://localhost/phpmyadmin
5. Create a new database named: quanlysinhvien

### 3. Database Schema
Run the following SQL script in phpMyAdmin:

```sql
CREATE TABLE IF NOT EXISTS users (
 id INT AUTO_INCREMENT PRIMARY KEY,
 username VARCHAR(50) NOT NULL UNIQUE,
 password VARCHAR(100) NOT NULL,
 role VARCHAR(20) DEFAULT 'student'
);

CREATE TABLE IF NOT EXISTS students (
 id VARCHAR(20) PRIMARY KEY,
 name VARCHAR(100) NOT NULL,
 email VARCHAR(100)
);

CREATE TABLE IF NOT EXISTS subjects (
 id VARCHAR(20) PRIMARY KEY,
 name VARCHAR(100) NOT NULL,
 credits INT DEFAULT 1
);

CREATE TABLE IF NOT EXISTS grades (
 student_id VARCHAR(20),
 subject_id VARCHAR(20),
 score DOUBLE,
 PRIMARY KEY (student_id, subject_id),
 FOREIGN KEY (student_id) REFERENCES students(id),
 FOREIGN KEY (subject_id) REFERENCES subjects(id)
);

INSERT INTO users (username, password, role) 
VALUES (
 'admin',
 '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92',
 'admin'
);
```

⚠️ Note: Please update the database connection credentials in
DatabaseConnection.cs before running the application.
-

---

### ▶️ How to Run
Clone the repository:

git clone https://github.com/DatHe146/quanLyDiemSinhVien.git

Open the solution file in Visual Studio 2022

Configure database connection settings

Build and run the project

---

### 🎯 Learning Outcomes

Improved understanding of backend system design

Hands-on experience with CRUD operations

Practical use of role-based authentication

Experience working with C# and relational databases

Better understanding of system workflow and data management
