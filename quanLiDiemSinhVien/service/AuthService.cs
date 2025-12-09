using System;
using System.Security.Cryptography; // Thư viện để mã hóa
using System.Text;
using MySql.Data.MySqlClient;
using model;
using utils;

namespace Service
{
    public class AuthService
    {
        // --- HÀM MÃ HÓA MẬT KHẨU (SHA256) ---
        // Hàm này biến "123456" thành một chuỗi ký tự loằng ngoằng không thể dịch ngược
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Chuyển chuỗi thành mảng byte
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Chuyển mảng byte thành chuỗi Hex để lưu vào DB
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // 1. ĐĂNG NHẬP
        public User Login(string username, string password)
        {
            // Mã hóa mật khẩu người dùng nhập vào để so sánh với cái đã lưu trong DB
            string passwordHash = HashPassword(password);

            string sql = "SELECT * FROM users WHERE username = @u AND password = @p";

            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@u", username);
                        cmd.Parameters.AddWithValue("@p", passwordHash); // So sánh Hash

                        using (MySqlDataReader rs = cmd.ExecuteReader())
                        {
                            if (rs.Read())
                            {
                                return new User(
                                    rs.GetString("username"),
                                    rs.GetString("password"), // Đây là chuỗi đã mã hóa
                                    rs.GetString("role")
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // Nên ghi log lỗi ra file hoặc debug console thay vì Console.WriteLine nếu là App WinForm
                System.Diagnostics.Debug.WriteLine("Lỗi đăng nhập: " + e.Message);
            }
            return null;
        }

        // 2. ĐĂNG KÝ (ADD)
        public bool Add(string username, string password, string role)
        {
            // Mã hóa mật khẩu trước khi lưu
            string passwordHash = HashPassword(password);

            string sql = "INSERT INTO users (username, password, role) VALUES (@u, @p, @r)";

            // Không cần try-catch ở đây để lỗi (ví dụ trùng username) 
            // được ném ra ngoài cho RegisterView xử lý (như code View bạn gửi trước đó)
            using (MySqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@p", passwordHash); // Lưu Hash
                    cmd.Parameters.AddWithValue("@r", role);

                    int rowsInserted = cmd.ExecuteNonQuery();
                    return rowsInserted > 0;
                }
            }
        }

        // 3. KIỂM TRA MẬT KHẨU CŨ
        public bool Check(User currentUser, string oldPassInput)
        {
            // Mã hóa cái người dùng vừa nhập xem có khớp với cái hash đang lưu trong object không
            string oldPassHash = HashPassword(oldPassInput);
            return currentUser.Password == oldPassHash;
        }

        // 4. ĐỔI MẬT KHẨU
        public bool ChangePassword(User currentUser, string newPass)
        {
            string newPassHash = HashPassword(newPass); // Mã hóa mật khẩu mới
            string sql = "UPDATE users SET password = @newPass WHERE username = @username";

            try
            {
                using (MySqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@newPass", newPassHash);
                        cmd.Parameters.AddWithValue("@username", currentUser.UserName);

                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            // Cập nhật lại thông tin trong RAM để không phải query lại
                            currentUser.Password = newPassHash;
                            return true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi đổi mật khẩu: " + e.Message);
            }
            return false;
        }
    }
}