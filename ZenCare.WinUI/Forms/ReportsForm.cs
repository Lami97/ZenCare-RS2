using ZenCare.Model.Enums;
using ZenCare.Model.Responses;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI.Forms;

public partial class ReportsForm : Form
{
    private readonly APIService _apiService = new APIService();

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

    private async Task LoadReports()
    {
        btnRefresh.Enabled = false;
        lblStatus.Text = "Loading reports...";

        try
        {
            var users = await GetItems<UserResponse>("User");
            var employees = await GetItems<EmployeeResponse>("Employee");
            var appointments = await GetItems<AppointmentResponse>("Appointment");
            var services = await GetItems<ServiceResponse>("Service");
            var products = await GetItems<ProductResponse>("Product");

            LoadGeneralStatistics(users, employees, appointments, services, products);
            LoadAppointmentStatistics(appointments);
            LoadMostPopularServices(appointments);
            LoadEmployeeWorkload(appointments);
            LoadUserActivity(appointments);

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
        var rows = appointments
            .GroupBy(x => new { x.WellnessServiceId, x.ServiceName })
            .Select(x => new ServiceReportRow(x.Key.ServiceName, x.Count()))
            .OrderByDescending(x => x.Appointments)
            .ToList();

        dgvPopularServices.DataSource = rows;
        lblPopularServicesEmpty.Visible = rows.Count == 0;
    }

    private void LoadEmployeeWorkload(List<AppointmentResponse> appointments)
    {
        var rows = appointments
            .GroupBy(x => new { x.EmployeeId, x.EmployeeName })
            .Select(x => new EmployeeReportRow(x.Key.EmployeeName, x.Count()))
            .OrderByDescending(x => x.Appointments)
            .ToList();

        dgvEmployeeWorkload.DataSource = rows;
        lblEmployeeWorkloadEmpty.Visible = rows.Count == 0;
    }

    private void LoadUserActivity(List<AppointmentResponse> appointments)
    {
        var rows = appointments
            .GroupBy(x => new { x.UserId, x.UserName })
            .Select(x => new UserActivityReportRow(x.Key.UserName, x.Count()))
            .OrderByDescending(x => x.Appointments)
            .ToList();

        dgvUserActivity.DataSource = rows;
        lblUserActivityEmpty.Visible = rows.Count == 0;
    }

    private sealed record ServiceReportRow(string ServiceName, int Appointments);
    private sealed record EmployeeReportRow(string Employee, int Appointments);
    private sealed record UserActivityReportRow(string User, int Appointments);
}
