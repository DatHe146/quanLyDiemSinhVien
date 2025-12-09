# 🎓 HƯỚNG DẪN CÀI ĐẶT & CHẠY DỰ ÁN QUẢN LÝ SINH VIÊN

Dự án này là phần mềm quản lý sinh viên được viết bằng **C# (WinForms)** và sử dụng cơ sở dữ liệu **MySQL**. 
Tài liệu này sẽ hướng dẫn bạn cách cài đặt môi trường bằng **XAMPP** để chạy dự án dễ dàng nhất.

---

## 🛠️ 1. Yêu cầu phần mềm
Để chạy được dự án, máy tính của bạn cần cài:
1.  **Visual Studio 2022** (Để chạy code C#).
2.  **XAMPP** (Để tạo server Database MySQL nhanh gọn).

---

## 🚀 2. Hướng dẫn cài đặt Database (Dùng XAMPP)

Thay vì cài MySQL Server phức tạp, chúng ta sẽ dùng XAMPP.

### Bước 1: Cài đặt và Bật XAMPP
1.  Tải XAMPP tại: [https://www.apachefriends.org/download.html](https://www.apachefriends.org/download.html)
2.  Cài đặt như bình thường (Cứ nhấn Next).
3.  Mở **XAMPP Control Panel**.
4.  Bấm nút **Start** ở 2 dòng:
    * **Apache** (Web server)
    * **MySQL** (Database server)
    *(Khi 2 dòng chuyển sang màu xanh lá là OK)*.

### Bước 2: Tạo Cơ sở dữ liệu
1.  Mở trình duyệt web, truy cập: [http://localhost/phpmyadmin](http://localhost/phpmyadmin)
2.  Nhìn cột bên trái, bấm **New**.
3.  Ô "Database name" nhập chính xác: `quanlysinhvien`
4.  Bấm **Create**.

### Bước 3: Chạy script SQL
1.  Bấm vào tab **SQL** ở thanh menu trên cùng.
2.  Copy toàn bộ đoạn code SQL dưới đây và dán vào ô trống:

```sql
-- Tạo bảng Users (Tài khoản)
CREATE TABLE IF NOT EXISTS users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    password VARCHAR(100) NOT NULL, -- Quan trọng: Độ dài 100 để lưu mã hóa SHA256
    role VARCHAR(20) DEFAULT 'student'
);

-- Tạo bảng Students (Sinh viên)
CREATE TABLE IF NOT EXISTS students (
    id VARCHAR(20) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    email VARCHAR(100)
);

-- Tạo bảng Subjects (Môn học)
CREATE TABLE IF NOT EXISTS subjects (
    id VARCHAR(20) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    credits INT DEFAULT 1
);

-- Tạo bảng Grades (Điểm số)
CREATE TABLE IF NOT EXISTS grades (
    student_id VARCHAR(20),
    subject_id VARCHAR(20),
    score DOUBLE,
    PRIMARY KEY (student_id, subject_id),
    FOREIGN KEY (student_id) REFERENCES students(id),
    FOREIGN KEY (subject_id) REFERENCES subjects(id)
);

-- Thêm một tài khoản Admin mặc định (Pass: 123456)
-- Lưu ý: Mật khẩu này đã được mã hóa SHA256
INSERT INTO users (username, password, role) 
VALUES ('admin', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 'admin');
-- lưu ý: trước khi runtest vào trong file databaseConnection ở folder utils chỉnh mk thành mk do ae tạo( trường hợp ae next hết không tạo mật khẩu thì để rỗng "" ).
# quanLyDiemSinhVien
