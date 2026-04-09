using System.Diagnostics;
using System.Text;
using Bogus;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OrangeHRM_AutoTest.Helpers;
using OrangeHRM_AutoTest.Pages;
using OrangeHRM_AutoTest.TestCases;

namespace OrangeHRM_AutoTest;

public partial class Form1 : Form
{
    // Danh sách kết quả test
    private List<KetQuaTest> danhSachKetQua = new List<KetQuaTest>();
    private CancellationTokenSource? ctsHuyBo;
    private bool dangChay = false;

    // Tên nhân viên tạo mới mỗi lần chạy - dùng xuyên suốt
    public static string TenNV_Ho = "Nguyễn";
    public static string TenNV_Dem = "Văn";
    public static string TenNV_Ten = "A";
    public static string TenNV_TimKiem = "Nguyễn"; // dùng cho autocomplete

    // URL và tài khoản đăng nhập
    // URL lấy từ AppConfig
    
    

    public Form1()
    {
        InitializeComponent();
        TaoDanhSachTestCase();
    }

    // Tạo danh sách test case ban đầu và hiển thị lên bảng
    private void TaoDanhSachTestCase()
    {
        danhSachKetQua.Clear();
        dgvKetQua.Rows.Clear();

        // Lấy test case từ mỗi module
        var leaveTC = new LeaveTestCases();
        var recruitmentTC = new RecruitmentTestCases();
        var myInfoTC = new MyInfoTestCases();

        danhSachKetQua.AddRange(leaveTC.LayDanhSachTestCase());
        danhSachKetQua.AddRange(recruitmentTC.LayDanhSachTestCase());
        danhSachKetQua.AddRange(myInfoTC.LayDanhSachTestCase());

        // Hiển thị lên DataGridView
        int stt = 1;
        foreach (var tc in danhSachKetQua)
        {
            dgvKetQua.Rows.Add(stt, tc.MaTestCase, tc.TenTestCase, tc.Module, tc.TrangThai, tc.GhiChu, tc.ThoiGianChay);
            stt++;
        }

        CapNhatThongKe();
        GhiLog("Đã tải " + danhSachKetQua.Count + " test case.");
    }

    // Nút chạy kiểm thử
    private async void BtnChayTatCa_Click(object? sender, EventArgs e)
    {
        if (dangChay) return;

        dangChay = true;
        ctsHuyBo = new CancellationTokenSource();
        btnChayTatCa.Enabled = false;
        btnDungLai.Enabled = true;

        // Lọc test case theo module được chọn
        var dsChay = new List<KetQuaTest>();
        foreach (var tc in danhSachKetQua)
        {
            if (tc.Module == "Nghỉ Phép" && chkLeave.Checked) dsChay.Add(tc);
            else if (tc.Module == "Thông Tin CN" && chkMyInfo.Checked) dsChay.Add(tc);
            else if (tc.Module == "Tuyển Dụng" && chkRecruitment.Checked) dsChay.Add(tc);
        
        }

        if (dsChay.Count == 0)
        {
            MessageBox.Show("Vui lòng chọn ít nhất 1 module!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            dangChay = false;
            btnChayTatCa.Enabled = true;
            btnDungLai.Enabled = false;
            return;
        }

        progressBar.Maximum = dsChay.Count;
        progressBar.Value = 0;
        GhiLog("Bắt đầu chạy " + dsChay.Count + " test case...");

        // Chạy từng test case
        await Task.Run(() =>
        {
            IWebDriver? driver = null;

            try
            {
                // Khởi tạo browser
                this.Invoke(() => lblTienTrinh.Text = "Đang mở trình duyệt Chrome...");
                GhiLog("Khởi tạo Chrome browser...");

                var options = new ChromeOptions();
                options.AddArgument("--start-maximized");
                options.AddArgument("--disable-notifications");
                driver = new ChromeDriver(options);
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

                // Đăng nhập
                this.Invoke(() => lblTienTrinh.Text = "Đang đăng nhập OrangeHRM...");
                GhiLog("Đang đăng nhập...");
                DangNhap(driver);

                // Bước 1: Tạo nhân viên mới bằng Bogus (mỗi lần chạy tên khác)
                var faker = new Faker("vi");
                TenNV_Ho = faker.Name.LastName();
                TenNV_Dem = faker.Name.FirstName();
                TenNV_Ten = faker.Name.FirstName();
                TenNV_TimKiem = TenNV_Ho;

                string tenDayDu = $"{TenNV_Ho} {TenNV_Dem} {TenNV_Ten}";
                this.Invoke(() => lblTienTrinh.Text = $"Đang tạo nhân viên {tenDayDu}...");
                GhiLog($"Setup: Tạo nhân viên mới {tenDayDu}");
                TaoNhanVien(driver, TenNV_Ho, TenNV_Dem, TenNV_Ten);

                // Khởi tạo các class test
                var leaveTC = new LeaveTestCases();
                var recruitmentTC = new RecruitmentTestCases();
                var myInfoTC = new MyInfoTestCases();

                // Leave Balance đã có sẵn trong DB - không cần setup
                GhiLog("Leave Balance đã được cấu hình sẵn trong DB");

                int dem = 0;
                foreach (var tc in dsChay)
                {
                    if (ctsHuyBo.Token.IsCancellationRequested) break;

                    dem++;
                    this.Invoke(() =>
                    {
                        lblTienTrinh.Text = $"[{dem}/{dsChay.Count}] Đang chạy: {tc.MaTestCase} - {tc.TenTestCase}";
                        progressBar.Value = dem;
                    });

                    GhiLog($">> Chạy {tc.MaTestCase}: {tc.TenTestCase}");

                    // Đo thời gian
                    var sw = Stopwatch.StartNew();
                    KetQuaTest ketQua;

                    try
                    {
                        // Gọi hàm test tương ứng
                        if (tc.Module == "Nghỉ Phép")
                            ketQua = leaveTC.ChayTestCase(tc.MaTestCase, driver);
                        else if (tc.Module != "Tuyển Dụng")
                            ketQua = myInfoTC.ChayTestCase(tc.MaTestCase, driver);
                        
                        else
                            ketQua = recruitmentTC.ChayTestCase(tc.MaTestCase, driver);

                    }
                    catch (Exception ex)
                    {
                        ketQua = new KetQuaTest(tc.MaTestCase, tc.TenTestCase, tc.Module);
                        ketQua.TrangThai = "Error";
                        ketQua.GhiChu = ex.Message;
                    }

                    sw.Stop();
                    ketQua.ThoiGianChay = sw.Elapsed.TotalSeconds.ToString("F1") + "s";

                    // Cập nhật kết quả
                    tc.TrangThai = ketQua.TrangThai;
                    tc.GhiChu = ketQua.GhiChu;
                    tc.ThoiGianChay = ketQua.ThoiGianChay;

                    // Cập nhật giao diện
                    this.Invoke(() => CapNhatDongKetQua(tc));

                    GhiLog($"   Kết quả: {ketQua.TrangThai} ({ketQua.ThoiGianChay}) {ketQua.GhiChu}");
                }
            }
            catch (Exception ex)
            {
                GhiLog("LỖI: " + ex.Message);
                MessageBox.Show("Lỗi khi chạy test: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Đóng browser
                if (driver != null)
                {
                    try { driver.Quit(); } catch { }
                }

                this.Invoke(() =>
                {
                    dangChay = false;
                    btnChayTatCa.Enabled = true;
                    btnDungLai.Enabled = false;
                    lblTienTrinh.Text = "Hoàn thành kiểm thử!";
                    CapNhatThongKe();
                });
                GhiLog("=== Hoàn thành kiểm thử ===");
            }
        });
    }

    // Đăng nhập vào hệ thống
    private void DangNhap(IWebDriver driver)
    {
        driver.Navigate().GoToUrl(Helpers.AppConfig.LOGIN_URL);
        Thread.Sleep(1000);

        driver.FindElement(By.Name("username")).SendKeys(Helpers.AppConfig.USERNAME);
        driver.FindElement(By.Name("password")).SendKeys(Helpers.AppConfig.PASSWORD);
        driver.FindElement(By.CssSelector("button[type='submit']")).Click();
        Thread.Sleep(1000);
    }

    // Tạo nhân viên mới qua PIM > Add Employee
    private void TaoNhanVien(IWebDriver driver, string ho, string dem, string ten)
    {
        try
        {
            driver.Navigate().GoToUrl(Helpers.AppConfig.BASE_URL + "/pim/addEmployee");
            Thread.Sleep(1500);

            // Đợi form load
            var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var fnInput = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(
                By.Name("firstName")));
            fnInput.Clear();
            fnInput.SendKeys(ho);

            var mnInput = driver.FindElement(By.Name("middleName"));
            mnInput.Clear();
            mnInput.SendKeys(dem);

            var lnInput = driver.FindElement(By.Name("lastName"));
            lnInput.Clear();
            lnInput.SendKeys(ten);

            // Xóa Employee ID cũ và nhập ID unique
            var empIdInputs = driver.FindElements(By.CssSelector("input.oxd-input"));
            foreach (var inp in empIdInputs)
            {
                var val = inp.GetAttribute("value") ?? "";
                var name = inp.GetAttribute("name") ?? "";
                if (name != "firstName" && name != "middleName" && name != "lastName" && val != "")
                {
                    inp.Click();
                    inp.SendKeys(OpenQA.Selenium.Keys.Control + "a");
                    inp.SendKeys(DateTime.Now.ToString("HHmmss"));
                    break;
                }
            }

            Thread.Sleep(300);

            // Bấm Save
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();
            Thread.Sleep(2000);

            GhiLog("Đã tạo nhân viên: " + ho + " " + dem + " " + ten);
        }
        catch (Exception ex)
        {
            GhiLog("Lỗi tạo nhân viên: " + ex.Message);
        }
    }

    // Nút dừng
    private void BtnDungLai_Click(object? sender, EventArgs e)
    {
        ctsHuyBo?.Cancel();
        GhiLog("Đã yêu cầu dừng kiểm thử...");
        lblTienTrinh.Text = "Đang dừng...";
    }

    // Cập nhật 1 dòng kết quả trong bảng
    private void CapNhatDongKetQua(KetQuaTest tc)
    {
        for (int i = 0; i < dgvKetQua.Rows.Count; i++)
        {
            if (dgvKetQua.Rows[i].Cells["colMa"].Value?.ToString() == tc.MaTestCase)
            {
                dgvKetQua.Rows[i].Cells["colTrangThai"].Value = tc.TrangThai;
                dgvKetQua.Rows[i].Cells["colGhiChu"].Value = tc.GhiChu;
                dgvKetQua.Rows[i].Cells["colThoiGian"].Value = tc.ThoiGianChay;

                // Đổi màu theo trạng thái
                var row = dgvKetQua.Rows[i];
                if (tc.TrangThai == "Pass")
                {
                    row.Cells["colTrangThai"].Style.BackColor = Color.FromArgb(200, 230, 201);
                    row.Cells["colTrangThai"].Style.ForeColor = Color.FromArgb(27, 94, 32);
                }
                else if (tc.TrangThai == "Fail")
                {
                    row.Cells["colTrangThai"].Style.BackColor = Color.FromArgb(255, 205, 210);
                    row.Cells["colTrangThai"].Style.ForeColor = Color.FromArgb(183, 28, 28);
                }
                else if (tc.TrangThai == "Error")
                {
                    row.Cells["colTrangThai"].Style.BackColor = Color.FromArgb(255, 224, 178);
                    row.Cells["colTrangThai"].Style.ForeColor = Color.FromArgb(230, 81, 0);
                }

                dgvKetQua.FirstDisplayedScrollingRowIndex = i;
                break;
            }
        }
        CapNhatThongKe();
    }

    // Cập nhật thống kê
    private void CapNhatThongKe()
    {
        int tong = danhSachKetQua.Count;
        int pass = danhSachKetQua.Count(x => x.TrangThai == "Pass");
        int fail = danhSachKetQua.Count(x => x.TrangThai == "Fail");
        int error = danhSachKetQua.Count(x => x.TrangThai == "Error");
        int daChay = pass + fail + error;

        lblThongKe.Text = $"Tổng: {tong} | Pass: {pass} | Fail: {fail} | Error: {error}";

        if (daChay > 0)
        {
            double tiLe = (double)pass / daChay * 100;
            lblTiLe.Text = $"Tỉ lệ Pass: {tiLe:F1}%";
            lblTiLe.ForeColor = tiLe >= 70 ? Color.FromArgb(27, 94, 32) : Color.FromArgb(183, 28, 28);
        }
        else
        {
            lblTiLe.Text = "Tỉ lệ Pass: 0%";
        }
    }

    // Ghi log
    private void GhiLog(string noiDung)
    {
        if (txtLog.InvokeRequired)
        {
            txtLog.Invoke(() => GhiLog(noiDung));
            return;
        }

        string dong = $"[{DateTime.Now:HH:mm:ss}] {noiDung}\n";
        txtLog.AppendText(dong);
        txtLog.ScrollToCaret();
    }

    // Xuất báo cáo HTML
    private void BtnXuatReport_Click(object? sender, EventArgs e)
    {
        int pass = danhSachKetQua.Count(x => x.TrangThai == "Pass");
        int fail = danhSachKetQua.Count(x => x.TrangThai == "Fail");
        int error = danhSachKetQua.Count(x => x.TrangThai == "Error");
        int chuaChay = danhSachKetQua.Count(x => x.TrangThai == "Chưa chạy");

        var sb = new StringBuilder();
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang='vi'><head><meta charset='UTF-8'>");
        sb.AppendLine("<title>OrangeHRM - Báo Cáo Kiểm Thử Tự Động</title>");
        sb.AppendLine("<style>");
        sb.AppendLine("* { margin: 0; padding: 0; box-sizing: border-box; }");
        sb.AppendLine("body { font-family: 'Segoe UI', sans-serif; background: #f5f5f5; padding: 20px; }");
        sb.AppendLine(".header { background: linear-gradient(135deg, #1a237e, #283593); color: white; padding: 30px; border-radius: 10px; margin-bottom: 20px; }");
        sb.AppendLine(".header h1 { font-size: 24px; margin-bottom: 8px; }");
        sb.AppendLine(".header p { opacity: 0.9; font-size: 14px; }");
        sb.AppendLine(".stats { display: flex; gap: 15px; margin-bottom: 20px; }");
        sb.AppendLine(".stat-card { flex: 1; padding: 20px; border-radius: 8px; color: white; text-align: center; }");
        sb.AppendLine(".stat-card h2 { font-size: 32px; }");
        sb.AppendLine(".stat-card p { font-size: 13px; opacity: 0.9; }");
        sb.AppendLine(".bg-pass { background: #43a047; }");
        sb.AppendLine(".bg-fail { background: #e53935; }");
        sb.AppendLine(".bg-error { background: #fb8c00; }");
        sb.AppendLine(".bg-total { background: #1e88e5; }");
        sb.AppendLine("table { width: 100%; border-collapse: collapse; background: white; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.1); }");
        sb.AppendLine("th { background: #3f51b5; color: white; padding: 12px 10px; text-align: left; font-size: 13px; }");
        sb.AppendLine("td { padding: 10px; border-bottom: 1px solid #eee; font-size: 13px; }");
        sb.AppendLine("tr:hover { background: #f5f5f5; }");
        sb.AppendLine(".badge { padding: 4px 12px; border-radius: 12px; font-size: 12px; font-weight: bold; }");
        sb.AppendLine(".badge-pass { background: #e8f5e9; color: #2e7d32; }");
        sb.AppendLine(".badge-fail { background: #ffebee; color: #c62828; }");
        sb.AppendLine(".badge-error { background: #fff3e0; color: #e65100; }");
        sb.AppendLine(".badge-skip { background: #e3f2fd; color: #1565c0; }");
        sb.AppendLine(".chart-container { background: white; padding: 20px; border-radius: 8px; margin-bottom: 20px; box-shadow: 0 2px 8px rgba(0,0,0,0.1); }");
        sb.AppendLine("</style></head><body>");

        // Header
        sb.AppendLine("<div class='header'>");
        sb.AppendLine("<h1>BÁO CÁO KIỂM THỬ TỰ ĐỘNG - OrangeHRM</h1>");
        sb.AppendLine($"<p>Ngày thực hiện: {DateTime.Now:dd/MM/yyyy HH:mm} | URL: {Helpers.AppConfig.LOGIN_URL}</p>");
        sb.AppendLine($"<p>Tài khoản: {Helpers.AppConfig.USERNAME} | Trình duyệt: Chrome</p>");
        sb.AppendLine("</div>");

        // Thống kê
        sb.AppendLine("<div class='stats'>");
        sb.AppendLine($"<div class='stat-card bg-total'><h2>{danhSachKetQua.Count}</h2><p>Tổng Test Case</p></div>");
        sb.AppendLine($"<div class='stat-card bg-pass'><h2>{pass}</h2><p>Pass</p></div>");
        sb.AppendLine($"<div class='stat-card bg-fail'><h2>{fail}</h2><p>Fail</p></div>");
        sb.AppendLine($"<div class='stat-card bg-error'><h2>{error}</h2><p>Error / Skip</p></div>");
        sb.AppendLine("</div>");

        // Tỉ lệ
        int daChay = pass + fail + error;
        double tiLe = daChay > 0 ? (double)pass / daChay * 100 : 0;
        sb.AppendLine("<div class='chart-container'>");
        sb.AppendLine($"<h3>Tỉ lệ Pass: {tiLe:F1}% ({pass}/{daChay} test case đã chạy)</h3>");
        // Thanh progress đơn giản bằng CSS
        sb.AppendLine($"<div style='background:#eee;border-radius:10px;height:30px;margin-top:10px;overflow:hidden;'>");
        sb.AppendLine($"<div style='background:#43a047;height:100%;width:{tiLe:F0}%;border-radius:10px;display:flex;align-items:center;justify-content:center;color:white;font-weight:bold;font-size:13px;'>{tiLe:F1}%</div>");
        sb.AppendLine("</div></div>");

        // Bảng kết quả theo module
        string[] modules = { "Nghỉ Phép", "Tuyển Dụng", "Thông Tin CN" };
        foreach (var mod in modules)
        {
            var dsModule = danhSachKetQua.Where(x => x.Module == mod).ToList();
            if (dsModule.Count == 0) continue;

            int modPass = dsModule.Count(x => x.TrangThai == "Pass");
            sb.AppendLine($"<h3 style='margin:20px 0 10px;'>Module: {mod} ({modPass}/{dsModule.Count} Pass)</h3>");
            sb.AppendLine("<table>");
            sb.AppendLine("<tr><th>STT</th><th>Mã TC</th><th>Tên Test Case</th><th>Trạng Thái</th><th>Ghi Chú</th><th>Thời Gian</th></tr>");

            int stt = 1;
            foreach (var tc in dsModule)
            {
                string badgeClass = tc.TrangThai == "Pass" ? "badge-pass" :
                                    tc.TrangThai == "Fail" ? "badge-fail" :
                                    tc.TrangThai == "Error" ? "badge-error" : "badge-skip";

                sb.AppendLine($"<tr><td>{stt}</td><td>{tc.MaTestCase}</td><td>{tc.TenTestCase}</td>");
                sb.AppendLine($"<td><span class='badge {badgeClass}'>{tc.TrangThai}</span></td>");
                sb.AppendLine($"<td>{tc.GhiChu}</td><td>{tc.ThoiGianChay}</td></tr>");
                stt++;
            }
            sb.AppendLine("</table>");
        }

        sb.AppendLine("<p style='text-align:center;margin-top:30px;color:#999;font-size:12px;'>Báo cáo được tạo tự động bởi OrangeHRM AutoTest</p>");
        sb.AppendLine("</body></html>");

        // Lưu file
        string reportDir = Path.Combine(Application.StartupPath, "Reports");
        Directory.CreateDirectory(reportDir);
        string filePath = Path.Combine(reportDir, $"BaoCao_{DateTime.Now:yyyyMMdd_HHmmss}.html");

        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);

        GhiLog("Đã xuất báo cáo: " + filePath);

        // Mở file trong trình duyệt
        var result = MessageBox.Show(
            "Đã xuất báo cáo thành công!\nBạn có muốn mở xem ngay?",
            "Thông báo",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Information);

        if (result == DialogResult.Yes)
        {
            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
        }
    }
}
