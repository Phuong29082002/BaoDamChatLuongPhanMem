namespace OrangeHRM_AutoTest;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(1280, 750);
        this.Text = "OrangeHRM - Kiểm Thử Tự Động";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Font = new Font("Segoe UI", 9.5F);
        this.MinimumSize = new Size(1100, 650);

        // === Panel bên trái - chọn module ===
        panelLeft = new Panel();
        panelLeft.Dock = DockStyle.Left;
        panelLeft.Width = 260;
        panelLeft.BackColor = Color.FromArgb(245, 245, 245);
        panelLeft.Padding = new Padding(10);

        lblTieuDe = new Label();
        lblTieuDe.Text = "CHỌN MODULE KIỂM THỬ";
        lblTieuDe.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        lblTieuDe.Dock = DockStyle.Top;
        lblTieuDe.Height = 40;
        lblTieuDe.TextAlign = ContentAlignment.MiddleLeft;

        chkLeave = new CheckBox();
        chkLeave.Text = "Module Nghỉ Phép (25 TCs)";
        chkLeave.Dock = DockStyle.Top;
        chkLeave.Height = 30;
        chkLeave.Checked = true;

        chkRecruitment = new CheckBox();
        chkRecruitment.Text = "Module Tuyển Dụng (24 TCs)";
        chkRecruitment.Dock = DockStyle.Top;
        chkRecruitment.Height = 30;
        chkRecruitment.Checked = true;

        chkMyInfo = new CheckBox();
        chkMyInfo.Text = "Module Thông Tin CN (23 TCs)";
        chkMyInfo.Dock = DockStyle.Top;
        chkMyInfo.Height = 30;
        chkMyInfo.Checked = true;

        // Separator
        var sep1 = new Panel();
        sep1.Dock = DockStyle.Top;
        sep1.Height = 15;

        btnChayTatCa = new Button();
        btnChayTatCa.Text = "▶ CHẠY KIỂM THỬ";
        btnChayTatCa.Dock = DockStyle.Top;
        btnChayTatCa.Height = 45;
        btnChayTatCa.BackColor = Color.FromArgb(76, 175, 80);
        btnChayTatCa.ForeColor = Color.White;
        btnChayTatCa.FlatStyle = FlatStyle.Flat;
        btnChayTatCa.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        btnChayTatCa.Cursor = Cursors.Hand;
        btnChayTatCa.Click += BtnChayTatCa_Click;

        btnDungLai = new Button();
        btnDungLai.Text = "⏹ DỪNG LẠI";
        btnDungLai.Dock = DockStyle.Top;
        btnDungLai.Height = 35;
        btnDungLai.BackColor = Color.FromArgb(244, 67, 54);
        btnDungLai.ForeColor = Color.White;
        btnDungLai.FlatStyle = FlatStyle.Flat;
        btnDungLai.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        btnDungLai.Cursor = Cursors.Hand;
        btnDungLai.Enabled = false;
        btnDungLai.Click += BtnDungLai_Click;

        var sep2 = new Panel();
        sep2.Dock = DockStyle.Top;
        sep2.Height = 15;

        // Thống kê
        lblThongKe = new Label();
        lblThongKe.Text = "Tổng: 0 | Pass: 0 | Fail: 0 | Error: 0";
        lblThongKe.Dock = DockStyle.Top;
        lblThongKe.Height = 25;
        lblThongKe.Font = new Font("Segoe UI", 9);

        lblTiLe = new Label();
        lblTiLe.Text = "Tỉ lệ Pass: 0%";
        lblTiLe.Dock = DockStyle.Top;
        lblTiLe.Height = 25;
        lblTiLe.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        lblTiLe.ForeColor = Color.FromArgb(33, 150, 243);

        var sep3 = new Panel();
        sep3.Dock = DockStyle.Top;
        sep3.Height = 10;

        btnXuatReport = new Button();
        btnXuatReport.Text = "📄 Xuất báo cáo HTML";
        btnXuatReport.Dock = DockStyle.Top;
        btnXuatReport.Height = 35;
        btnXuatReport.BackColor = Color.FromArgb(33, 150, 243);
        btnXuatReport.ForeColor = Color.White;
        btnXuatReport.FlatStyle = FlatStyle.Flat;
        btnXuatReport.Cursor = Cursors.Hand;
        btnXuatReport.Click += BtnXuatReport_Click;

        // Thêm controls vào panel trái (ngược thứ tự vì Dock=Top)
        panelLeft.Controls.Add(btnXuatReport);
        panelLeft.Controls.Add(sep3);
        panelLeft.Controls.Add(lblTiLe);
        panelLeft.Controls.Add(lblThongKe);
        panelLeft.Controls.Add(sep2);
        panelLeft.Controls.Add(btnDungLai);
        panelLeft.Controls.Add(btnChayTatCa);
        panelLeft.Controls.Add(sep1);
        panelLeft.Controls.Add(chkMyInfo);
        panelLeft.Controls.Add(chkRecruitment);
        panelLeft.Controls.Add(chkLeave);
        panelLeft.Controls.Add(lblTieuDe);

        // === Panel phải - kết quả ===
        panelRight = new Panel();
        panelRight.Dock = DockStyle.Fill;
        panelRight.Padding = new Padding(10);

        // Progress bar
        progressBar = new ProgressBar();
        progressBar.Dock = DockStyle.Top;
        progressBar.Height = 25;
        progressBar.Style = ProgressBarStyle.Continuous;

        lblTienTrinh = new Label();
        lblTienTrinh.Text = "Sẵn sàng ki���m thử...";
        lblTienTrinh.Dock = DockStyle.Top;
        lblTienTrinh.Height = 28;
        lblTienTrinh.Font = new Font("Segoe UI", 9.5F);
        lblTienTrinh.TextAlign = ContentAlignment.MiddleLeft;

        // DataGridView - bảng kết quả
        dgvKetQua = new DataGridView();
        dgvKetQua.Dock = DockStyle.Fill;
        dgvKetQua.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvKetQua.AllowUserToAddRows = false;
        dgvKetQua.AllowUserToDeleteRows = false;
        dgvKetQua.ReadOnly = true;
        dgvKetQua.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvKetQua.RowHeadersVisible = false;
        dgvKetQua.BackgroundColor = Color.White;
        dgvKetQua.BorderStyle = BorderStyle.None;
        dgvKetQua.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        dgvKetQua.DefaultCellStyle.SelectionBackColor = Color.FromArgb(232, 240, 254);
        dgvKetQua.DefaultCellStyle.SelectionForeColor = Color.Black;
        dgvKetQua.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(63, 81, 181);
        dgvKetQua.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgvKetQua.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        dgvKetQua.EnableHeadersVisualStyles = false;
        dgvKetQua.ColumnHeadersHeight = 35;
        dgvKetQua.RowTemplate.Height = 30;

        // Thêm các cột
        dgvKetQua.Columns.Add("colSTT", "STT");
        dgvKetQua.Columns.Add("colMa", "Mã TC");
        dgvKetQua.Columns.Add("colTen", "Tên Test Case");
        dgvKetQua.Columns.Add("colModule", "Module");
        dgvKetQua.Columns.Add("colTrangThai", "Trạng Thái");
        dgvKetQua.Columns.Add("colGhiChu", "Ghi Chú");
        dgvKetQua.Columns.Add("colThoiGian", "Thời Gian");

        dgvKetQua.Columns["colSTT"].Width = 40;
        dgvKetQua.Columns["colSTT"].FillWeight = 5;
        dgvKetQua.Columns["colMa"].FillWeight = 8;
        dgvKetQua.Columns["colTen"].FillWeight = 30;
        dgvKetQua.Columns["colModule"].FillWeight = 12;
        dgvKetQua.Columns["colTrangThai"].FillWeight = 10;
        dgvKetQua.Columns["colGhiChu"].FillWeight = 25;
        dgvKetQua.Columns["colThoiGian"].FillWeight = 10;

        // Log textbox ở dưới
        txtLog = new RichTextBox();
        txtLog.Dock = DockStyle.Bottom;
        txtLog.Height = 120;
        txtLog.ReadOnly = true;
        txtLog.BackColor = Color.FromArgb(30, 30, 30);
        txtLog.ForeColor = Color.LightGreen;
        txtLog.Font = new Font("Consolas", 9);
        txtLog.BorderStyle = BorderStyle.None;

        var splitter = new Splitter();
        splitter.Dock = DockStyle.Bottom;
        splitter.Height = 3;
        splitter.BackColor = Color.Gray;

        panelRight.Controls.Add(dgvKetQua);
        panelRight.Controls.Add(lblTienTrinh);
        panelRight.Controls.Add(progressBar);
        panelRight.Controls.Add(splitter);
        panelRight.Controls.Add(txtLog);

        // Thêm panel vào form
        this.Controls.Add(panelRight);
        this.Controls.Add(panelLeft);
    }

    #endregion

    // Khai báo controls
    private Panel panelLeft;
    private Panel panelRight;
    private Label lblTieuDe;
    private CheckBox chkLeave;
    private CheckBox chkRecruitment;
    private CheckBox chkMyInfo;
    private Button btnChayTatCa;
    private Button btnDungLai;
    private Button btnXuatReport;
    private Label lblThongKe;
    private Label lblTiLe;
    private ProgressBar progressBar;
    private Label lblTienTrinh;
    private DataGridView dgvKetQua;
    private RichTextBox txtLog;
}
