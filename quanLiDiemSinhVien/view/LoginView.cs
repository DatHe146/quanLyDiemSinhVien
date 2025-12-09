using Microsoft.Win32;
using Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace quanLiDiemSinhVien.view
{
    public partial class LoginView : Form
    {
        public LoginView()
        {
            InitializeComponent(); // Lệnh này vẽ giao diện lên màn hình
        }
        private AuthService authService = new AuthService();
        private void btnLogin_Click(object sender, EventArgs e)
        {
            // Lấy dữ liệu từ 2 cái TextBox bạn vừa đặt tên ở Bước 3
            string uName = txtUserName.Text;
            string pwd = txtPassword.Text;

            var u = authService.Login(uName, pwd);

            if (u != null)
            {
                this.Hide();
                if (u.Role == "admin") // Hoặc so sánh kỹ hơn tuỳ logic của bạn
                {
                    new AdminMainView(u).Show();
                }
                else
                {
                    new StudentPortalView(u).Show();
                }
            }
            else
            {
                lblStatus.Text = "❌ Sai tài khoản hoặc mật khẩu!";
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            new RegisterView().ShowDialog();
        }

        private void LoginView_Load(object sender, EventArgs e)
        {

        }
    }
}
