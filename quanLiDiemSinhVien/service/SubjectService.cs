using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient; // Thư viện dành riêng cho MySQL
using model;
using utils;

namespace Service
{
    public class SubjectService
    {
        // Lấy tất cả môn học
        public List<Subject> GetAll()
        {
            List<Subject> list = new List<Subject>();
            string sql = "SELECT * FROM subjects";

            try
            {
                // Lưu ý: DatabaseConnection.GetConnection() phải trả về MySqlConnection
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        using (MySqlDataReader rs = cmd.ExecuteReader())
                        {
                            while (rs.Read())
                            {
                                list.Add(new Subject(
                                    rs.GetString("id"),       // Lấy chuỗi theo tên cột
                                    rs.GetString("name"),
                                    rs.GetInt32("credits")    // Lấy số nguyên theo tên cột
                                ));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Lỗi: " + e.Message);
            }
            return list;
        }

        // Thêm môn học (Trả về bool: True = thành công, False = thất bại)
        public bool AddSubject(Subject s)
        {
            // MySQL trong C# vẫn hỗ trợ tham số dạng @name, rất tiện lợi
            string sqlCheck = "SELECT COUNT(*) FROM subjects WHERE name = @name";
            string sqlInsert = "INSERT INTO subjects (id, name, credits) VALUES (@id, @name, @credits)";

            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    // --- BƯỚC 1: KIỂM TRA TÊN MÔN HỌC ---
                    using (MySqlCommand checkCmd = new MySqlCommand(sqlCheck, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@name", s.Name);

                        // ExecuteScalar trả về object, cần ép kiểu về int (Int64 cho count lớn hoặc Int32)
                        long count = Convert.ToInt64(checkCmd.ExecuteScalar());

                        if (count > 0)
                        {
                            Console.WriteLine("❌ Tên môn học '" + s.Name + "' đã tồn tại!");
                            return false;
                        }
                    }

                    // --- BƯỚC 2: THÊM MỚI ---
                    using (MySqlCommand insertCmd = new MySqlCommand(sqlInsert, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@id", s.Id);
                        insertCmd.Parameters.AddWithValue("@name", s.Name);
                        insertCmd.Parameters.AddWithValue("@credits", s.Credits);

                        int rows = insertCmd.ExecuteNonQuery();
                        return rows > 0;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("❌ Lỗi thêm môn học: " + e.Message);
                return false;
            }
        }

        public bool UpdateSubject(string id, string newName, int newCredits)
        {
            // Kiểm tra tên xem có bị trùng với môn KHÁC không (trừ chính nó ra)
            // Logic: Tìm môn nào có tên giống newName NHƯNG id khác id hiện tại
            string sqlCheck = "SELECT COUNT(*) FROM subjects WHERE name = @name AND id != @id";
            string sqlUpdate = "UPDATE subjects SET name = @name, credits = @credits WHERE id = @id";

            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    // Kiểm tra trùng tên
                    using (MySqlCommand checkCmd = new MySqlCommand(sqlCheck, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@name", newName);
                        checkCmd.Parameters.AddWithValue("@id", id);
                        long count = Convert.ToInt64(checkCmd.ExecuteScalar());
                        if (count > 0)
                        {
                            throw new Exception("Tên môn học đã tồn tại ở một mã môn khác!");
                        }
                    }

                    // Thực hiện Update
                    using (MySqlCommand cmd = new MySqlCommand(sqlUpdate, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", newName);
                        cmd.Parameters.AddWithValue("@credits", newCredits);
                        cmd.Parameters.AddWithValue("@id", id);

                        int rows = cmd.ExecuteNonQuery();
                        return rows > 0; // Trả về true nếu sửa được ít nhất 1 dòng
                    }
                }
            }
            catch (Exception e)
            {
                // Ném lỗi ra để bên View hiện thông báo
                throw new Exception(e.Message);
            }
        }

        // Xóa môn học
        // Xóa môn học (Sửa từ void -> bool để báo kết quả về cho View)
        public bool DeleteSubject(string id)
        {
            string sql = "DELETE FROM subjects WHERE id = @id";

            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);

                        // ExecuteNonQuery trả về số dòng bị ảnh hưởng
                        int rows = cmd.ExecuteNonQuery();

                        // Nếu số dòng > 0 nghĩa là đã xóa được
                        return rows > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                // Lỗi 1451: Cannot delete or update a parent row (Dính khóa ngoại - Môn học đang có điểm)
                if (ex.Number == 1451)
                {
                    // Ném lỗi ra để bên View hiện thông báo cụ thể
                    throw new Exception("Không thể xóa môn này vì đã có sinh viên đăng ký/có điểm!");
                }

                // Các lỗi khác
                Console.WriteLine("Lỗi SQL: " + ex.Message);
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("Lỗi hệ thống: " + e.Message);
                return false;
            }
        }

        // Lấy tên môn học theo ID
        public string GetSubjectNameById(string id)
        {
            string sql = "SELECT name FROM subjects WHERE id = @id";

            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);

                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            return result.ToString();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Lỗi: " + e.Message);
            }
            return "Không rõ";
        }

        // --- 2. HÀM TÌM KIẾM (SearchSubject) ---
        public List<Subject> SearchSubject(string keyword)
        {
            List<Subject> list = new List<Subject>();
            // Tìm theo Mã môn HOẶC Tên môn
            string sql = "SELECT * FROM subjects WHERE id LIKE @key OR name LIKE @key";

            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        // Thêm dấu % để tìm kiếm tương đối (gần đúng)
                        cmd.Parameters.AddWithValue("@key", "%" + keyword + "%");

                        using (MySqlDataReader rs = cmd.ExecuteReader())
                        {
                            while (rs.Read())
                            {
                                list.Add(new Subject(
                                    rs.GetString("id"),
                                    rs.GetString("name"),
                                    rs.GetInt32("credits")
                                ));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Lỗi tìm kiếm: " + e.Message);
            }
            return list;
        }

    }
}