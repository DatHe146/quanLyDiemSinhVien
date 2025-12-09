    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Data;
    using MySql.Data.MySqlClient; // Cần thư viện MySql.Data
    using System.Text.RegularExpressions;
    using model; // Namespace chứa class Student của bạn
    using utils;  // Namespace chứa class DatabaseConnection của bạn

    namespace Service
    {
        public class StudentService
        {
            // 1. Định nghĩa mẫu Regex chuẩn cho Email
            // @ dùng để khai báo verbatim string trong C#
            private static readonly string EMAIL_PATTERN = @"^[A-Za-z0-9+_.-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,6}$";

            // 2. Hàm phụ để kiểm tra tính hợp lệ
            private bool IsValidEmail(string email)
            {
                if (string.IsNullOrEmpty(email)) return false;
                return Regex.IsMatch(email, EMAIL_PATTERN);
            }

            // --- 1. LOGIC LẤY DỮ LIỆU (GetAll) ---
            public List<Student> GetAll()
            {
                List<Student> list = new List<Student>();
                string sql = "SELECT * FROM students";

                try
                {
                    // Sử dụng 'using' để tự động đóng kết nối (tương tự try-with-resources của Java)
                    using (MySqlConnection conn = DatabaseConnection.GetConnection())
                    {
                        conn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                        {
                            using (MySqlDataReader rs = cmd.ExecuteReader())
                            {
                                while (rs.Read())
                                {
                                    list.Add(new Student(
                                        rs["id"].ToString(),
                                        rs["name"].ToString(),
                                        rs["email"].ToString()
                                    ));
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                return list;
            }

            // --- 2. LOGIC THÊM (AddStudent) ---
            public void AddStudent(Student s)
            {
                // 1. Check Email trước
                if (!IsValidEmail(s.Email))
                {
                    throw new Exception("Email không đúng định dạng (ví dụ: abc@gmail.com)!");
                }

                string sql = "INSERT INTO students (id, name, email) VALUES (@id, @name, @email)";

                try
                {
                    using (MySqlConnection conn = DatabaseConnection.GetConnection())
                    {
                        conn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                        {
                            // Thêm tham số để tránh SQL Injection
                            cmd.Parameters.AddWithValue("@id", s.Id);
                            cmd.Parameters.AddWithValue("@name", s.Name);
                            cmd.Parameters.AddWithValue("@email", s.Email);

                            cmd.ExecuteNonQuery(); // Thực thi lệnh INSERT
                        }
                    }
                }
                catch (MySqlException e)
                {
                    // Mã lỗi 1062 trong MySQL tương ứng với Duplicate Entry (Trùng khóa chính)
                    if (e.Number == 1062)
                    {
                        throw new Exception($"Mã sinh viên {s.Id} đã tồn tại!");
                    }
                    throw new Exception("Lỗi kết nối cơ sở dữ liệu: " + e.Message);
                }
                catch (Exception e)
                {
                    throw new Exception("Lỗi hệ thống: " + e.Message);
                }
            }

            // --- 3. LOGIC SỬA (UpdateStudent) ---
            public void UpdateStudent(string id, string newName, string newEmail)
            {
                // 1. Validate Email trước khi sửa
                if (!IsValidEmail(newEmail))
                {
                    throw new Exception("Email mới không đúng định dạng!");
                }

                string sql = "UPDATE students SET name = @name, email = @email WHERE id = @id";

                try
                {
                    using (MySqlConnection conn = DatabaseConnection.GetConnection())
                    {
                        conn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@name", newName);
                            cmd.Parameters.AddWithValue("@email", newEmail);
                            cmd.Parameters.AddWithValue("@id", id);

                            int rows = cmd.ExecuteNonQuery();
                            if (rows == 0)
                            {
                                throw new Exception("Không tìm thấy sinh viên có ID: " + id);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Lỗi khi cập nhật: " + e.Message);
                }
            }

            // --- 4. LOGIC XÓA (DeleteStudent) ---
            public bool DeleteStudent(string id)
            {
                string sql = "DELETE FROM students WHERE id = @id";
                try
                {
                    using (MySqlConnection conn = DatabaseConnection.GetConnection())
                    {
                        conn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", id);
                            return cmd.ExecuteNonQuery() > 0;
                        }
                    }
                }
                catch (Exception)
                {
                    // Lỗi thường gặp: Ràng buộc khóa ngoại
                    Console.WriteLine("❌ Không thể xóa sinh viên này (có thể do đã có bảng điểm).");
                    return false;
                }
            }

            // --- 5. LOGIC TÌM KIẾM (FindStudent) ---
            public Student FindStudent(string id)
            {
                string sql = "SELECT * FROM students WHERE id = @id";
                try
                {
                    using (MySqlConnection conn = DatabaseConnection.GetConnection())
                    {
                        conn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", id);
                            using (MySqlDataReader rs = cmd.ExecuteReader())
                            {
                                if (rs.Read())
                                {
                                    return new Student(
                                        rs["id"].ToString(),
                                        rs["name"].ToString(),
                                        rs["email"].ToString()
                                    );
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                return null;
            }

            // --- HÀM MỚI: TÌM KIẾM (SearchStudent) ---
            public List<Student> SearchStudent(string keyword)
            {
                List<Student> list = new List<Student>();
                string sql = "SELECT * FROM students WHERE id LIKE @keyword OR name LIKE @keyword";

                try
                {
                    using (MySqlConnection conn = DatabaseConnection.GetConnection())
                    {
                        conn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                        {
                            string query = "%" + keyword + "%";
                            // MySQL connector cho phép dùng tên tham số nhiều lần
                            cmd.Parameters.AddWithValue("@keyword", query);

                            using (MySqlDataReader rs = cmd.ExecuteReader())
                            {
                                while (rs.Read())
                                {
                                    list.Add(new Student(
                                        rs["id"].ToString(),
                                        rs["name"].ToString(),
                                        rs["email"].ToString()
                                    ));
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                return list;
            }

            // Lấy tên theo ID
            public string GetStudentNameById(string id)
            {
                Student s = FindStudent(id);
                return (s != null) ? s.Name : null;
            }

            // --- 6. LOGIC SẮP XẾP (GetStudentsSortedByName) ---
            public List<Student> GetStudentsSortedByName()
            {
                List<Student> list = new List<Student>();
            
                // Giữ nguyên câu lệnh SQL vì SUBSTRING_INDEX là hàm đặc thù của MySQL
                string sql = "SELECT * FROM students ORDER BY SUBSTRING_INDEX(name, ' ', -1) ASC, name ASC";

                try
                {
                    using (MySqlConnection conn = DatabaseConnection.GetConnection())
                    {
                        conn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                        {
                            using (MySqlDataReader rs = cmd.ExecuteReader())
                            {
                                while (rs.Read())
                                {
                                    list.Add(new Student(
                                        rs["id"].ToString(),
                                        rs["name"].ToString(),
                                        rs["email"].ToString()
                                    ));
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                return list;
            }
        }
    }