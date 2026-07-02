using ZenCare.Model.Enums;
using ZenCare.Model.Responses;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI.Forms;

public partial class ReportsForm : Form
{
    private readonly APIService _apiService = new APIService();
    private List<UserResponse> _users = new();
    private List<EmployeeResponse> _employees = new();
    private List<AppointmentResponse> _appointments = new();
    private List<ServiceResponse> _services = new();
    private List<ProductResponse> _products = new();
    private List<ServiceReportRow> _popularServices = new();
    private List<EmployeeReportRow> _employeeWorkload = new();
    private List<UserActivityReportRow> _userActivity = new();

    public ReportsForm()
    {
        InitializeComponent();
    }

    private async void ReportsForm_Load(object sender, EventArgs e)
    {
        await LoadReports();
    }

    private async void btnRefresh_Click(object sender, EventArgs e)
    {
        await LoadReports();
    }

    private void btnExportAppointmentSummary_Click(object sender, EventArgs e)
    {
        ExportAppointmentSummaryPdf();
    }

    private void btnExportBusinessStatistics_Click(object sender, EventArgs e)
    {
        ExportBusinessStatisticsPdf();
    }

    private async Task LoadReports()
    {
        btnRefresh.Enabled = false;
        lblStatus.Text = "Loading reports...";

        try
        {
            _users = await GetItems<UserResponse>("User");
            _employees = await GetItems<EmployeeResponse>("Employee");
            _appointments = await GetItems<AppointmentResponse>("Appointment");
            _services = await GetItems<ServiceResponse>("Service");
            _products = await GetItems<ProductResponse>("Product");

            LoadGeneralStatistics(_users, _employees, _appointments, _services, _products);
            LoadAppointmentStatistics(_appointments);
            LoadMostPopularServices(_appointments);
            LoadEmployeeWorkload(_appointments);
            LoadUserActivity(_appointments);

            lblStatus.Text = "Reports generated from existing CRUD endpoint data.";
        }
        catch (Exception ex)
        {
            lblStatus.Text = "Unable to load reports.";
            MessageBox.Show(ex.Message);
        }
        finally
        {
            btnRefresh.Enabled = true;
        }
    }

    private async Task<List<T>> GetItems<T>(string endpoint)
    {
        var result = await _apiService.Get<PagedResult<T>>(endpoint);
        return result?.Items ?? new List<T>();
    }

    private void LoadGeneralStatistics(
        List<UserResponse> users,
        List<EmployeeResponse> employees,
        List<AppointmentResponse> appointments,
        List<ServiceResponse> services,
        List<ProductResponse> products)
    {
        lblTotalUsersValue.Text = users.Count.ToString();
        lblTotalEmployeesValue.Text = employees.Count.ToString();
        lblTotalAppointmentsValue.Text = appointments.Count.ToString();
        lblTotalServicesValue.Text = services.Count.ToString();
        lblTotalProductsValue.Text = products.Count.ToString();
    }

    private void LoadAppointmentStatistics(List<AppointmentResponse> appointments)
    {
        lblPendingAppointmentsValue.Text = CountAppointmentsByStatus(appointments, AppointmentStatus.Pending).ToString();
        lblConfirmedAppointmentsValue.Text = CountAppointmentsByStatus(appointments, AppointmentStatus.Confirmed).ToString();
        lblCompletedAppointmentsValue.Text = CountAppointmentsByStatus(appointments, AppointmentStatus.Completed).ToString();
        lblCancelledAppointmentsValue.Text = CountAppointmentsByStatus(appointments, AppointmentStatus.Cancelled).ToString();
    }

    private static int CountAppointmentsByStatus(List<AppointmentResponse> appointments, AppointmentStatus status)
    {
        return appointments.Count(x => x.Status == status);
    }

    private void LoadMostPopularServices(List<AppointmentResponse> appointments)
    {
        _popularServices = appointments
            .GroupBy(x => new { x.WellnessServiceId, x.ServiceName })
            .Select(x => new ServiceReportRow(x.Key.ServiceName, x.Count()))
            .OrderByDescending(x => x.Appointments)
            .ToList();

        dgvPopularServices.DataSource = _popularServices;
        lblPopularServicesEmpty.Visible = _popularServices.Count == 0;
    }

    private void LoadEmployeeWorkload(List<AppointmentResponse> appointments)
    {
        _employeeWorkload = appointments
            .GroupBy(x => new { x.EmployeeId, x.EmployeeName })
            .Select(x => new EmployeeReportRow(x.Key.EmployeeName, x.Count()))
            .OrderByDescending(x => x.Appointments)
            .ToList();

        dgvEmployeeWorkload.DataSource = _employeeWorkload;
        lblEmployeeWorkloadEmpty.Visible = _employeeWorkload.Count == 0;
    }

    private void LoadUserActivity(List<AppointmentResponse> appointments)
    {
        _userActivity = appointments
            .GroupBy(x => new { x.UserId, x.UserName })
            .Select(x => new UserActivityReportRow(x.Key.UserName, x.Count()))
            .OrderByDescending(x => x.Appointments)
            .ToList();

        dgvUserActivity.DataSource = _userActivity;
        lblUserActivityEmpty.Visible = _userActivity.Count == 0;
    }

    private void ExportAppointmentSummaryPdf()
    {
        using var dialog = CreateSaveFileDialog("AppointmentSummary.pdf");

        if (dialog.ShowDialog() != DialogResult.OK)
        {
            return;
        }

        try
        {
            var document = new SimplePdfDocument();
            document.AddTitle("ZenCare Appointment Summary");
            document.AddText($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm}");

            document.AddSection("Appointment status counts");
            document.AddTable(new[]
            {
                new[] { "Status", "Count" },
                new[] { "Pending", CountAppointmentsByStatus(_appointments, AppointmentStatus.Pending).ToString() },
                new[] { "Confirmed", CountAppointmentsByStatus(_appointments, AppointmentStatus.Confirmed).ToString() },
                new[] { "Completed", CountAppointmentsByStatus(_appointments, AppointmentStatus.Completed).ToString() },
                new[] { "Cancelled", CountAppointmentsByStatus(_appointments, AppointmentStatus.Cancelled).ToString() }
            });

            document.AddSection("Most popular services");
            AddServiceRows(document);

            document.AddSection("Employee workload");
            AddEmployeeRows(document);

            document.Save(dialog.FileName);
            MessageBox.Show("Appointment Summary PDF exported successfully.");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Unable to export Appointment Summary PDF. {ex.Message}");
        }
    }

    private void ExportBusinessStatisticsPdf()
    {
        using var dialog = CreateSaveFileDialog("BusinessStatistics.pdf");

        if (dialog.ShowDialog() != DialogResult.OK)
        {
            return;
        }

        try
        {
            var document = new SimplePdfDocument();
            document.AddTitle("ZenCare Business Statistics");
            document.AddText($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm}");

            document.AddSection("General statistics");
            document.AddTable(new[]
            {
                new[] { "Metric", "Value" },
                new[] { "Total users", _users.Count.ToString() },
                new[] { "Total employees", _employees.Count.ToString() },
                new[] { "Total appointments", _appointments.Count.ToString() },
                new[] { "Total services", _services.Count.ToString() },
                new[] { "Total products", _products.Count.ToString() }
            });

            document.AddSection("User activity");
            AddUserRows(document);

            document.Save(dialog.FileName);
            MessageBox.Show("Business Statistics PDF exported successfully.");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Unable to export Business Statistics PDF. {ex.Message}");
        }
    }

    private static SaveFileDialog CreateSaveFileDialog(string fileName)
    {
        return new SaveFileDialog
        {
            FileName = fileName,
            Filter = "PDF files (*.pdf)|*.pdf",
            DefaultExt = "pdf",
            AddExtension = true,
            OverwritePrompt = true
        };
    }

    private void AddServiceRows(SimplePdfDocument document)
    {
        if (_popularServices.Count == 0)
        {
            document.AddText("No data available.");
            return;
        }

        document.AddTable(new[] { new[] { "Service", "Appointments" } }
            .Concat(_popularServices.Select(x => new[] { x.ServiceName, x.Appointments.ToString() })));
    }

    private void AddEmployeeRows(SimplePdfDocument document)
    {
        if (_employeeWorkload.Count == 0)
        {
            document.AddText("No data available.");
            return;
        }

        document.AddTable(new[] { new[] { "Employee", "Appointments" } }
            .Concat(_employeeWorkload.Select(x => new[] { x.Employee, x.Appointments.ToString() })));
    }

    private void AddUserRows(SimplePdfDocument document)
    {
        if (_userActivity.Count == 0)
        {
            document.AddText("No data available.");
            return;
        }

        document.AddTable(new[] { new[] { "User", "Appointments" } }
            .Concat(_userActivity.Select(x => new[] { x.User, x.Appointments.ToString() })));
    }

    private sealed record ServiceReportRow(string ServiceName, int Appointments);
    private sealed record EmployeeReportRow(string Employee, int Appointments);
    private sealed record UserActivityReportRow(string User, int Appointments);
}
