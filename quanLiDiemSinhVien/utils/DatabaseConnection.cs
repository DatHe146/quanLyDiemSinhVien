using System;
using MySql.Data.MySqlClient;

namespace utils
{
    public class DatabaseConnection
    {
        private static string host = "localhost";
        private static int port = 3306;
        private static string database = "quanlysinhvien";
        private static string username = "root";
        private static string password = "";

        private static string connectionString = $"Server={host};Database={database};Port={port};User ID={username};Password={password};";

        public static MySqlConnection GetConnection()
        {
            // Chỉ tạo đối tượng kết nối và trả về. KHÔNG Open() ở đây.
            return new MySqlConnection(connectionString);
        }
    }
}