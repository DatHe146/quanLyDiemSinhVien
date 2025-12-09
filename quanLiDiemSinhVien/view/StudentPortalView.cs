using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using model;   // Namespace chứa class User, Grade
using Service; // Namespace chứa các Service

namespace quanLiDiemSinhVien.view
{
    public partial class StudentPortalView : Form
    {

        // Khởi tạo các Service
        private StudentService studentService = new StudentService();
        private GradeService gradeService = new GradeService();
        private SubjectService subjectService = new SubjectService();

        // Biến lưu thông tin người dùng hiện tại
        private User currentUser;

        // Constructor nhận vào một User
        public StudentPortalView(User u)
        {
            InitializeComponent();
            this.currentUser = u;

            // 1. Cấu hình bảng (Tạo 3 cột)
            SetupTable();

            // 2. Tải dữ liệu lên giao diện
            LoadData();
        }

        private void SetupTable()
        {
            dgvGrades.ColumnCount = 3; // Tổng cộng 3 cột

            dgvGrades.Columns[0].Name = "Mã Môn";
            dgvGrades.Columns[0].Width = 100; // Độ rộng cột

            dgvGrades.Columns[1].Name = "Tên Môn Học";
            dgvGrades.Columns[1].Width = 250; // Cột tên môn rộng hơn chút

            dgvGrades.Columns[2].Name = "Điểm Số";

            // Tự động chỉnh độ rộng cột cho đẹp
            dgvGrades.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void LoadData()
        {
            // --- A. Hiển thị tên sinh viên ---
            // Username chính là Mã Sinh Viên (ID)
            string studentId = currentUser.UserName;

            string realName = studentService.GetStudentNameById(studentId);

            // Nếu chưa có tên thật (ví dụ mới tạo acc) thì hiện username
            if (string.IsNullOrEmpty(realName))
            {
                realName = currentUser.UserName;
            }

            lblWelcome.Text = "Xin chào sinh viên: " + realName;

            // --- B. Hiển thị danh sách điểm ---
            List<Grade> grades = gradeService.GetGradesByStudent(studentId);

            // Xóa dữ liệu cũ (nếu có)
            dgvGrades.Rows.Clear();

            foreach (var g in grades)
            {
                // Lấy tên môn học từ ID môn
                string subName = subjectService.GetSubjectNameById(g.SubjectId);

                // Thêm một dòng vào DataGridView
                // Thứ tự phải khớp với lúc khai báo cột: Mã -> Tên -> Điểm
                dgvGrades.Rows.Add(g.SubjectId, subName, g.Score);
            }

            // --- C. Hiển thị GPA ---
            double gpa = gradeService.CalculateGPA(studentId);

            // Format số thập phân: F2 nghĩa là lấy 2 số sau dấu phẩy (ví dụ: 8.50)
            lblGPA.Text = "GPA Tích Lũy: " + gpa.ToString("F2");
        }

        // --- SỰ KIỆN NÚT BẤM ---

        // Nút Đăng Xuất (Bạn nhớ Double Click vào nút Logout để tạo hàm này)
        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Close(); // Đóng form này lại
            // Tùy logic Main của bạn, có thể cần hiện lại Form Login:
            new LoginView().Show();
        }

        // Nút Đổi Mật Khẩu (Double Click vào nút Đổi Pass)
        private void btnChangePass_Click(object sender, EventArgs e)
        {
            // Mở form đổi mật khẩu (truyền user hiện tại sang)
            new ChangePasswordView(currentUser).ShowDialog();
        }

        private void lblGPA_Click(object sender, EventArgs e)
        {

        }
    }
}