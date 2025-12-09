using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using model;
using Service;

namespace quanLiDiemSinhVien.view
{
    public partial class AdminMainView : Form
    {
        // Khởi tạo các Service
        private StudentService studentService = new StudentService();
        private SubjectService subjectService = new SubjectService();
        private GradeService gradeService = new GradeService();
        
        private User currentUser;

        public AdminMainView(User u)
        {
            InitializeComponent();
            this.currentUser = u;
            this.Text = "Hệ Thống Quản Lý (Admin: " + u.UserName + ")";

            // 1. Cấu hình bảng (Tạo cột cho 3 bảng)
            SetupTables();

            // 2. Load dữ liệu ban đầu
            LoadStudentData();
            LoadSubjectData();
            
            // Tab điểm thì để trống, khi nào tìm mới hiện
        }

        private void SetupTables()
        {
            // --- Bảng Sinh Viên ---
            dgvStudents.ColumnCount = 3;
            dgvStudents.Columns[0].Name = "Mã SV";
            dgvStudents.Columns[1].Name = "Họ Tên";
            dgvStudents.Columns[2].Name = "Email";
            dgvStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // --- Bảng Môn Học ---
            dgvSubjects.ColumnCount = 3;
            dgvSubjects.Columns[0].Name = "Mã MH";
            dgvSubjects.Columns[1].Name = "Tên Môn Học";
            dgvSubjects.Columns[2].Name = "Tín Chỉ";
            dgvSubjects.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // --- Bảng Điểm ---
            dgvGrades.ColumnCount = 4;
            dgvGrades.Columns[0].Name = "Mã SV";
            dgvGrades.Columns[1].Name = "Mã MH";
            dgvGrades.Columns[2].Name = "Tên Môn Học";
            dgvGrades.Columns[3].Name = "Điểm";
            dgvGrades.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        // ==========================================
        //        TAB 1: QUẢN LÝ SINH VIÊN
        // ==========================================
        
        // Hàm tải danh sách SV lên bảng
        private void LoadStudentData()
        {
            dgvStudents.Rows.Clear();
            List<Student> list = studentService.GetAll(); // Cần đảm bảo Service có hàm GetAll()
            foreach (var s in list)
            {
                dgvStudents.Rows.Add(s.Id, s.Name, s.Email);
            }
        }

        // Sự kiện: Click vào bảng SV -> Đổ dữ liệu lên ô nhập
        private void dgvStudents_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvStudents.Rows[e.RowIndex];
                txtStudentId.Text = row.Cells[0].Value.ToString();
                txtStudentName.Text = row.Cells[1].Value.ToString();
                txtStudentEmail.Text = row.Cells[2].Value.ToString();
            }
        }

        // Nút THÊM SV
        private void btnAddSt_Click(object sender, EventArgs e)
        {
            try
            {
                Student s = new Student(txtStudentId.Text, txtStudentName.Text, txtStudentEmail.Text);
                studentService.AddStudent(s); // Service kiểm tra trùng lặp và add
                
                MessageBox.Show("✅ Thêm thành công!");
                LoadStudentData();
                ClearStudentInput();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ " + ex.Message);
            }
        }

        // Nút SỬA SV
        private void btnEditSt_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtStudentId.Text)) return;
                
                studentService.UpdateStudent(txtStudentId.Text, txtStudentName.Text, txtStudentEmail.Text);
                MessageBox.Show("✅ Cập nhật thành công!");
                LoadStudentData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ " + ex.Message);
            }
        }

        // Nút XÓA SV
        private void btnDelSt_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn chắc chắn muốn xóa?", "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (studentService.DeleteStudent(txtStudentId.Text))
                {
                    MessageBox.Show("✅ Đã xóa!");
                    LoadStudentData();
                    ClearStudentInput();
                }
                else
                {
                    MessageBox.Show("❌ Không xóa được (Có thể SV đang có điểm).");
                }
            }
        }

        // Nút SẮP XẾP TÊN
        private void btnSortSt_Click(object sender, EventArgs e)
        {
            List<Student> sortedList = studentService.GetStudentsSortedByName();
            dgvStudents.Rows.Clear();
            foreach (var s in sortedList)
            {
                dgvStudents.Rows.Add(s.Id, s.Name, s.Email);
            }
            MessageBox.Show("✅ Đã sắp xếp theo tên!");
        }

        // Nút TÌM KIẾM SV
        private void btnSearchSt_Click(object sender, EventArgs e)
        {
            string keyword = txtSearchSt.Text.Trim();
            if (string.IsNullOrEmpty(keyword))
            {
                LoadStudentData(); // Nếu rỗng thì load lại tất cả
                return;
            }

            List<Student> results = studentService.SearchStudent(keyword);
            dgvStudents.Rows.Clear();
            if (results.Count == 0)
            {
                MessageBox.Show("Không tìm thấy sinh viên nào!");
            }
            else
            {
                foreach (var s in results)
                {
                    dgvStudents.Rows.Add(s.Id, s.Name, s.Email);
                }
            }
        }
        
        // Nút LÀM MỚI
        private void btnRefreshSt_Click(object sender, EventArgs e)
        {
            LoadStudentData();
            ClearStudentInput();
        }

        private void ClearStudentInput()
        {
            txtStudentId.Text = "";
            txtStudentName.Text = "";
            txtStudentEmail.Text = "";
            txtSearchSt.Text = "";
        }

        // ==========================================
        //        TAB 2: QUẢN LÝ MÔN HỌC
        // ==========================================
        
        private void LoadSubjectData()
        {
            dgvSubjects.Rows.Clear();
            foreach (var s in subjectService.GetAll())
            {
                dgvSubjects.Rows.Add(s.Id, s.Name, s.Credits);
            }
        }

        // Sự kiện: Khi bấm vào bảng Môn học -> Đổ dữ liệu lên các ô nhập
        private void dgvSubjects_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra nếu bấm vào hàng hợp lệ (không phải tiêu đề cột)
            if (e.RowIndex >= 0)
            {
                // Lấy dòng hiện tại
                DataGridViewRow row = dgvSubjects.Rows[e.RowIndex];

                // Đổ dữ liệu vào các ô TextBox
                txtSubjectId.Text = row.Cells[0].Value.ToString();   // Cột 0: Mã môn
                txtSubjectName.Text = row.Cells[1].Value.ToString(); // Cột 1: Tên môn
                txtCredit.Text = row.Cells[2].Value.ToString();      // Cột 2: Tín chỉ
            }
        }

        // Nút THÊM Môn Học
        private void btnAddSub_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtSubjectId.Text) || string.IsNullOrWhiteSpace(txtSubjectName.Text))
                {
                    MessageBox.Show("⚠️ Vui lòng nhập đủ thông tin!");
                    return;
                }

                int cred = int.Parse(txtCredit.Text);
                Subject s = new Subject(txtSubjectId.Text, txtSubjectName.Text, cred);

                if (subjectService.AddSubject(s))
                {
                    MessageBox.Show("✅ Đã thêm môn học!");
                    LoadSubjectData();
                    txtSubjectId.Text = ""; txtSubjectName.Text = ""; txtCredit.Text = "";
                }
                else
                {
                    MessageBox.Show("❌ Trùng mã môn học!");
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("❌ Tín chỉ phải là số!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi: " + ex.Message);
            }
        }

        // Nút XÓA Môn Học
        private void btnDelSub_Click(object sender, EventArgs e)
        {
            // 1. KIỂM TRA: Người dùng đã chọn dòng nào chưa?
            if (dgvSubjects.CurrentRow == null || dgvSubjects.CurrentRow.Index < 0)
            {
                MessageBox.Show("⚠️ Vui lòng chọn môn học cần xóa trên bảng!", "Chưa chọn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. LẤY DỮ LIỆU: Lấy Mã môn học từ dòng đang chọn (Cột 0)
            string subjectId = dgvSubjects.CurrentRow.Cells[0].Value.ToString();
            string subjectName = dgvSubjects.CurrentRow.Cells[1].Value.ToString(); // Lấy thêm tên để hiện thông báo cho rõ

            // 3. HỎI XÁC NHẬN: Để tránh lỡ tay xóa nhầm
            DialogResult result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa môn: {subjectName} ({subjectId})?\n\nLưu ý: Hành động này không thể hoàn tác!",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // 4. GỌI SERVICE:
                    // Vì hàm trả về bool nên ta kiểm tra if
                    if (subjectService.DeleteSubject(subjectId))
                    {
                        // -- TRƯỜNG HỢP THÀNH CÔNG --
                        MessageBox.Show("✅ Đã xóa môn học thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Quan trọng: Tải lại bảng để dòng đó biến mất
                        LoadSubjectData();

                        // Xóa trắng các ô nhập liệu (để tránh người dùng bấm nhầm nút Sửa sau khi Xóa)
                        txtSubjectId.Text = "";
                        txtSubjectName.Text = "";
                        txtCredit.Text = "";
                    }
                    else
                    {
                        // Trường hợp trả về false (ít khi xảy ra nếu đã bắt try-catch, nhưng cứ để cho chắc)
                        MessageBox.Show("❌ Xóa thất bại. Vui lòng thử lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    // -- TRƯỜNG HỢP CÓ LỖI (Ví dụ: Môn học đang có điểm) --
                    // Service ném lỗi gì thì mình hiện lỗi đó lên
                    MessageBox.Show("❌ " + ex.Message, "Không thể xóa", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // --- XỬ LÝ NÚT SỬA MÔN HỌC ---
        private void btnEditSub_Click(object sender, EventArgs e)
        {
            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrEmpty(txtSubjectId.Text) || string.IsNullOrEmpty(txtSubjectName.Text))
            {
                MessageBox.Show("⚠️ Vui lòng chọn môn cần sửa từ bảng!", "Chưa chọn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Lấy dữ liệu từ ô nhập
                string id = txtSubjectId.Text;
                string name = txtSubjectName.Text;
                int credits = int.Parse(txtCredit.Text); // Có thể lỗi nếu nhập chữ cái vào ô tín chỉ

                if (credits < 1)
                {
                    MessageBox.Show("⚠️ Tín chỉ phải lớn hơn 0!");
                    return;
                }

                // Gọi Service
                if (subjectService.UpdateSubject(id, name, credits))
                {
                    MessageBox.Show("✅ Cập nhật thành công!");
                    LoadSubjectData(); // Load lại bảng

                    // (Tùy chọn) Xóa trắng ô nhập để tránh bấm nhầm
                    txtSubjectId.Text = ""; txtSubjectName.Text = ""; txtCredit.Text = "";
                }
                else
                {
                    MessageBox.Show("❌ Không tìm thấy mã môn để sửa (Có thể bạn đã sửa nhầm mã môn).");
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("❌ Tín chỉ phải là số nguyên!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi: " + ex.Message);
            }
        }

        // --- XỬ LÝ NÚT TÌM KIẾM MÔN HỌC ---
        private void btnSearchSub_Click(object sender, EventArgs e)
        {
            string keyword = txtSearchSub.Text.Trim();

            // Gọi Service tìm kiếm
            List<Subject> results = subjectService.SearchSubject(keyword);

            // Xóa bảng cũ đi và điền kết quả mới vào
            dgvSubjects.Rows.Clear();

            if (results.Count > 0)
            {
                foreach (var s in results)
                {
                    dgvSubjects.Rows.Add(s.Id, s.Name, s.Credits);
                }
            }
            else
            {
                MessageBox.Show("🔍 Không tìm thấy môn học nào phù hợp!");
                // Nếu không thấy thì load lại toàn bộ cho tiện
                LoadSubjectData();
            }
        }

        // --- XỬ LÝ NÚT LÀM MỚI (Hiện lại tất cả) ---
        private void btnRefreshSub_Click(object sender, EventArgs e)
        {
            // 1. Tải lại toàn bộ danh sách từ Database
            LoadSubjectData();

            // 2. Xóa chữ trong ô tìm kiếm đi
            txtSearchSub.Text = "";

            // 3. Xóa trắng các ô nhập liệu (cho sạch sẽ)
            txtSubjectId.Text = "";
            txtSubjectName.Text = "";
            txtCredit.Text = "";
        }

        // ==========================================
        //        TAB 3: QUẢN LÝ ĐIỂM
        // ==========================================

        // Nút LƯU ĐIỂM
        private void btnSaveGrade_Click(object sender, EventArgs e)
        {
            try
            {
                double score = double.Parse(txtScore.Text);
                if (score < 0 || score > 10)
                {
                    MessageBox.Show("❌ Điểm phải từ 0 - 10!");
                    return;
                }

                gradeService.AddOrUpdateGrade(txtGradeSid.Text, txtGradeSubId.Text, score);
                MessageBox.Show("✅ Đã lưu điểm!");
                txtScore.Text = "";
                
                // Nếu đang xem bảng điểm của SV đó thì load lại luôn cho tiện
                if (txtSearchGrade.Text == txtGradeSid.Text)
                {
                    btnSearchGrade_Click(null, null);
                }
            }
            catch
            {
                MessageBox.Show("❌ Mã không tồn tại hoặc điểm sai định dạng!");
            }
        }

        // Nút TÌM KIẾM ĐIỂM & TÍNH GPA
        private void btnSearchGrade_Click(object sender, EventArgs e)
        {
            string sid = txtSearchGrade.Text.Trim();
            if (string.IsNullOrEmpty(sid))
            {
                MessageBox.Show("Nhập mã SV cần xem!");
                return;
            }

            dgvGrades.Rows.Clear();
            List<Grade> list = gradeService.GetGradesByStudent(sid);

            if (list.Count == 0)
            {
                MessageBox.Show("Không tìm thấy điểm của SV này!");
                lblGPA.Text = "GPA: ...";
            }
            else
            {
                foreach (var g in list)
                {
                    string subName = subjectService.GetSubjectNameById(g.SubjectId);
                    if (subName == null) subName = "Unknown";

                    dgvGrades.Rows.Add(g.StudentId, g.SubjectId, subName, g.Score);
                }
                
                // Tính GPA
                double gpa = gradeService.CalculateGPA(sid);
                lblGPA.Text = "GPA Tích Lũy: " + gpa.ToString("F2");
            }
        }
        
        // Click bảng điểm -> điền ngược lên form
        private void dgvGrades_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvGrades.Rows[e.RowIndex];
                txtGradeSid.Text = row.Cells[0].Value.ToString();
                txtGradeSubId.Text = row.Cells[1].Value.ToString();
                txtScore.Text = row.Cells[3].Value.ToString();
            }
        }

        // ==========================================
        //        CHỨC NĂNG CHUNG (FOOTER)
        // ==========================================

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Close();
            new LoginView().Show();
        }

        private void btnChangePass_Click(object sender, EventArgs e)
        {
            new ChangePasswordView(currentUser).ShowDialog();
        }

        private void dgvSubjects_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txtSubjectName_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtSearchSub_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtSearchGrade_TextChanged(object sender, EventArgs e)
        {

        }
    }
}