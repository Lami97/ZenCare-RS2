using ZenCare.Model.Enums;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI.Forms;

public partial class AppointmentForm : Form
{
    private readonly APIService _apiService = new APIService();
    private List<ServiceResponse> _services = new();

    public AppointmentForm()
    {
        InitializeComponent();
    }

    private async void AppointmentForm_Load(object sender, EventArgs e)
    {
        LoadStatusLookup();
        await LoadLookups();
        await LoadAppointments();
    }

    private async void btnSearch_Click(object sender, EventArgs e)
    {
        await LoadAppointments();
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
        OpenDetailsForm(new AppointmentDetailsForm());
    }

    private void btnEdit_Click(object sender, EventArgs e)
    {
        var selectedId = GetSelectedAppointmentId();

        if (selectedId == null)
        {
            MessageBox.Show("Please select an appointment.");
            return;
        }

        OpenDetailsForm(new AppointmentDetailsForm(selectedId.Value));
    }

    private async void btnDelete_Click(object sender, EventArgs e)
    {
        var selectedId = GetSelectedAppointmentId();

        if (selectedId == null)
        {
            MessageBox.Show("Please select an appointment.");
            return;
        }

        var result = MessageBox.Show(
            "Are you sure you want to delete this appointment?",
            "Confirm delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result != DialogResult.Yes)
        {
            return;
        }

        var deleted = await _apiService.Delete($"Appointment/{selectedId.Value}");

        if (!deleted)
        {
            MessageBox.Show(GetApiErrorMessage("Unable to delete appointment."));
            return;
        }

        await LoadAppointments();
    }

    private async void btnRefresh_Click(object sender, EventArgs e)
    {
        txtSearch.Clear();
        SelectLookupItem(cmbUser, 0);
        SelectLookupItem(cmbEmployee, 0);
        SelectLookupItem(cmbServiceCategory, 0);
        SelectLookupItem(cmbService, 0);
        cmbStatus.SelectedIndex = 0;
        chkAppointmentDate.Checked = false;
        await LoadAppointments();
    }

    private async Task LoadLookups()
    {
        var users = await _apiService.Get<PagedResult<UserResponse>>("User");
        cmbUser.DataSource = CreateLookupItems(users?.Items.Select(x => new LookupItem(x.Id, GetUserDisplayName(x))), "All");

        var employees = await _apiService.Get<PagedResult<EmployeeResponse>>("Employee");
        cmbEmployee.DataSource = CreateLookupItems(employees?.Items.Select(x => new LookupItem(x.Id, x.UserName)), "All");

        var serviceCategories = await _apiService.Get<PagedResult<ServiceCategoryResponse>>("ServiceCategory");
        cmbServiceCategory.DataSource = CreateLookupItems(serviceCategories?.Items.Select(x => new LookupItem(x.Id, x.Name)), "All");

        var services = await _apiService.Get<PagedResult<ServiceResponse>>("Service");
        _services = services?.Items ?? new List<ServiceResponse>();
        cmbService.DataSource = CreateLookupItems(_services.Select(x => new LookupItem(x.Id, x.Name)), "All");
    }

    private void LoadStatusLookup()
    {
        var statuses = new List<StatusLookupItem> { new StatusLookupItem(null, "All") };
        statuses.AddRange(Enum.GetValues<AppointmentStatus>().Select(x => new StatusLookupItem(x, x.ToString())));
        cmbStatus.DataSource = statuses;
    }

    private async Task LoadAppointments()
    {
        var search = new AppointmentSearchObject
        {
            UserId = GetSelectedLookupId(cmbUser),
            EmployeeId = GetSelectedLookupId(cmbEmployee),
            WellnessServiceId = GetSelectedLookupId(cmbService),
            Status = GetSelectedStatus(),
            AppointmentDate = chkAppointmentDate.Checked ? dtpAppointmentDate.Value.Date : null
        };

        var query = new List<string>();

        if (search.UserId.HasValue)
        {
            query.Add($"UserId={search.UserId.Value}");
        }

        if (search.EmployeeId.HasValue)
        {
            query.Add($"EmployeeId={search.EmployeeId.Value}");
        }

        if (search.WellnessServiceId.HasValue)
        {
            query.Add($"WellnessServiceId={search.WellnessServiceId.Value}");
        }

        if (search.Status.HasValue)
        {
            query.Add($"Status={search.Status.Value}");
        }

        if (search.AppointmentDate.HasValue)
        {
            query.Add($"AppointmentDate={Uri.EscapeDataString(search.AppointmentDate.Value.ToString("O"))}");
        }

        var endpoint = query.Count == 0 ? "Appointment" : $"Appointment?{string.Join("&", query)}";
        var result = await _apiService.Get<PagedResult<AppointmentResponse>>(endpoint);

        if (result == null)
        {
            MessageBox.Show(GetApiErrorMessage("Unable to load appointments."));
            dgvAppointments.DataSource = new List<AppointmentResponse>();
            return;
        }

        var items = result.Items ?? new List<AppointmentResponse>();

        if (!string.IsNullOrWhiteSpace(txtSearch.Text))
        {
            items = items
                .Where(x => x.UserName.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase)
                    || x.EmployeeName.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase)
                    || x.ServiceName.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase)
                    || x.Status.ToString().Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase)
                    || (!string.IsNullOrWhiteSpace(x.Notes) && x.Notes.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        var selectedServiceCategoryId = GetSelectedLookupId(cmbServiceCategory);

        if (selectedServiceCategoryId.HasValue)
        {
            var serviceIds = _services
                .Where(x => x.ServiceCategoryId == selectedServiceCategoryId.Value)
                .Select(x => x.Id)
                .ToHashSet();

            items = items
                .Where(x => serviceIds.Contains(x.WellnessServiceId))
                .ToList();
        }

        dgvAppointments.DataSource = items;
    }

    private int? GetSelectedAppointmentId()
    {
        return dgvAppointments.CurrentRow?.DataBoundItem is AppointmentResponse appointment
            ? appointment.Id
            : null;
    }

    private static int? GetSelectedLookupId(ComboBox comboBox)
    {
        return comboBox.SelectedItem is LookupItem { Id: > 0 } item ? item.Id : null;
    }

    private AppointmentStatus? GetSelectedStatus()
    {
        return cmbStatus.SelectedItem is StatusLookupItem item ? item.Status : null;
    }

    private static void SelectLookupItem(ComboBox comboBox, int id)
    {
        for (var i = 0; i < comboBox.Items.Count; i++)
        {
            if (comboBox.Items[i] is LookupItem item && item.Id == id)
            {
                comboBox.SelectedIndex = i;
                return;
            }
        }
    }

    private static List<LookupItem> CreateLookupItems(IEnumerable<LookupItem>? items, string emptyText)
    {
        var lookupItems = new List<LookupItem> { new LookupItem(0, emptyText) };
        lookupItems.AddRange(items ?? Enumerable.Empty<LookupItem>());
        return lookupItems;
    }

    private static string GetUserDisplayName(UserResponse user)
    {
        var fullName = $"{user.FirstName} {user.LastName}".Trim();
        return string.IsNullOrWhiteSpace(fullName)
            ? user.Username
            : $"{user.Username} - {fullName}";
    }

    private string GetApiErrorMessage(string fallback)
    {
        return string.IsNullOrWhiteSpace(_apiService.LastErrorMessage)
            ? fallback
            : _apiService.LastErrorMessage;
    }

    private async void OpenDetailsForm(AppointmentDetailsForm form)
    {
        if (form.ShowDialog() == DialogResult.OK)
        {
            await LoadAppointments();
        }
    }

    private sealed record LookupItem(int Id, string Name)
    {
        public override string ToString()
        {
            return Name;
        }
    }

    private sealed record StatusLookupItem(AppointmentStatus? Status, string Name)
    {
        public override string ToString()
        {
            return Name;
        }
    }
}
