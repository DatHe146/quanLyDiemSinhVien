using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient; // Thư viện MySQL
using model; // Namespace chứa class Grade
using utils;  // Namespace chứa class DatabaseConnection

namespace Service
{
    public class GradeService
    {
        // Thêm hoặc Cập nhật điểm
        // Giữ nguyên logic ON DUPLICATE KEY UPDATE của MySQL
        public void AddOrUpdateGrade(string sid, string subid, double score)
        {
            // Trong C# MySQL, ta dùng tham số @ thay vì ? để rõ ràng hơn
            string sql = @"INSERT INTO grades (student_id, subject_id, score) 
                           VALUES (@sid, @subid, @score) 
                           ON DUPLICATE KEY UPDATE score = @score";

            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        // Gán giá trị cho tham số
                        cmd.Parameters.AddWithValue("@sid", sid);
                        cmd.Parameters.AddWithValue("@subid", subid);
                        cmd.Parameters.AddWithValue("@score", score);

                        cmd.ExecuteNonQuery();
                        Console.WriteLine("✅ Đã lưu điểm vào Database!");
                    }
                }
            }
            catch (MySqlException e)
            {
                // Bắt lỗi ràng buộc khóa ngoại (Foreign Key) 
                // Mã lỗi 1452 thường là lỗi foreign key trong MySQL
                if (e.Number == 1452)
                {
                    Console.WriteLine("sai ma sinh vien hoac ma mon hoc");
                    throw new Exception("Sai mã sinh viên hoặc mã môn học!");
                }
                else
                {
                    Console.WriteLine("❌ Lỗi Database: " + e.Message);
                    throw new Exception("Lỗi Database!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("❌ Lỗi hệ thống: " + e.Message);
                throw new Exception(e.Message);
            }
        }

        // Lấy danh sách điểm của 1 sinh viên
        public List<Grade> GetGradesByStudent(string sid)
        {
            List<Grade> list = new List<Grade>();
            string sql = "SELECT * FROM grades WHERE student_id = @sid";

            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@sid", sid);

                        using (MySqlDataReader rs = cmd.ExecuteReader())
                        {
                            while (rs.Read())
                            {
                                // Giả sử Model Grade có constructor tương ứng
                                list.Add(new Grade(
                                    rs.GetString("student_id"),
                                    rs.GetString("subject_id"),
                                    rs.GetDouble("score")
                                ));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Lỗi lấy danh sách điểm: " + e.Message);
            }
            return list;
        }

        // Tính điểm trung bình (GPA)
        public double CalculateGPA(string sid)
        {
            string sql = "SELECT AVG(score) FROM grades WHERE student_id = @sid";

            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@sid", sid);

                        // Sử dụng ExecuteScalar vì kết quả trả về chỉ là 1 ô dữ liệu duy nhất
                        object result = cmd.ExecuteScalar();

                        // Kiểm tra null và DBNull (Trường hợp SV chưa có điểm nào thì kết quả là DBNull)
                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToDouble(result);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Lỗi tính GPA: " + e.Message);
            }

            return 0.0;
        }
    }
}