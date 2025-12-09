using Service;
using System;
using System.Windows.Forms;
// Thêm dòng này để code bên dưới gọn hơn
using MySql.Data.MySqlClient;

namespace quanLiDiemSinhVien.view
{
    public partial class RegisterView : Form
    {
        // Nên dùng Interface nếu có thể, nhưng để class cụ thể cũng không sao
        private readonly AuthService authService = new AuthService();

        public RegisterView()
        {
            InitializeComponent();
            // Đặt mặc định cho Role để tránh null
            if (cbRole.Items.Count > 0) cbRole.SelectedIndex = 0;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string user = txtUser.Text.Trim();
            string pass = txtPass.Text; // Mật khẩu thường không Trim() vì Space cũng là ký tự
            string confirm = txtConfirmPass.Text;

            // Lấy role an toàn
            string role = cbRole.SelectedItem?.ToString() ?? "student";

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Vui lòng nhập đủ thông tin!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (pass != confirm)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                if (authService.Add(user, pass, role))
                {
                    MessageBox.Show("✅ Tạo tài khoản thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Không thể tạo tài khoản. Vui lòng thử lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (MySqlException ex) // Đã có using ở trên nên viết gọn được
            {
                if (ex.Number == 1062)
                {
                    MessageBox.Show($"❌ Tên đăng nhập '{user}' đã tồn tại!", "Trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtUser.Focus(); // Đưa con trỏ chuột về ô User để nhập lại
                }
                else
                {
                    MessageBox.Show("❌ Lỗi Database: " + ex.Message, "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi hệ thống: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RegisterView_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void cbRole_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txtConfirmPass_TextChanged(object sender, EventArgs e)
        {

        }
    }
}