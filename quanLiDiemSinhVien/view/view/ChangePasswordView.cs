using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using model;   // Nơi chứa class User
using Service; // Nơi chứa AuthService

namespace quanLiDiemSinhVien.view
{
    public partial class ChangePasswordView : Form
    {
        private AuthService authService = new AuthService();
        private User currentUser; // Biến lưu người dùng đang đăng nhập

        // Constructor nhận vào User từ Form chính truyền sang
        public ChangePasswordView(User u)
        {
            InitializeComponent();
            this.currentUser = u;
        }

        // Constructor mặc định (để tránh lỗi Designer nếu lỡ mở xem)
        public ChangePasswordView()
        {
            InitializeComponent();
        }

        // --- Nút HỦY ---
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close(); // Đóng cửa sổ
        }

        // --- Nút LƯU THAY ĐỔI ---
        private void btnSave_Click(object sender, EventArgs e)
        {
            string oldPass = txtOldPass.Text;
            string newPass = txtNewPass.Text;
            string confirm = txtConfirmPass.Text;

            // 1. Kiểm tra rỗng
            if (string.IsNullOrEmpty(oldPass) || string.IsNullOrEmpty(newPass))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Kiểm tra mật khẩu cũ có đúng không
            // Gọi hàm CheckPassword bên Service
            if (!authService.Check(currentUser, oldPass))
            {
                MessageBox.Show("❌ Mật khẩu cũ không đúng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 3. Kiểm tra mật khẩu xác nhận
            if (newPass != confirm)
            {
                MessageBox.Show("❌ Mật khẩu xác nhận không khớp!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 4. Tiến hành đổi mật khẩu
            if (authService.ChangePassword(currentUser, newPass))
            {
                MessageBox.Show("✅ Đổi mật khẩu thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close(); // Đóng form sau khi đổi xong
            }
            else
            {
                MessageBox.Show("❌ Lỗi hệ thống, không thể đổi mật khẩu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}