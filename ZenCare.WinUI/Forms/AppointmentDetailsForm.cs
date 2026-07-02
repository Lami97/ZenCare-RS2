using ZenCare.Model.Enums;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI.Forms;

public partial class AppointmentDetailsForm : Form
{
    private readonly APIService _apiService = new APIService();
    private readonly int? _appointmentId;
    private List<ServiceResponse> _services = new();
    private bool _lookupsLoaded;

    public AppointmentDetailsForm()
    {
        InitializeComponent();
    }

    public AppointmentDetailsForm(int appointmentId)
        : this()
    {
        _appointmentId = appointmentId;
    }

    private async void AppointmentDetailsForm_Load(object sender, EventArgs e)
    {
        LoadStatusLookup();
        await LoadLookups();

        if (_appointmentId.HasValue)
        {
            Text = "Edit appointment";
            await LoadAppointment();
        }
        else
        {
            Text = "New appointment";
            cmbStatus.SelectedItem = cmbStatus.Items
                .OfType<StatusLookupItem>()
                .FirstOrDefault(x => x.Status == AppointmentStatus.Pending);
        }
    }

    private async Task LoadLookups()
    {
        var users = await _apiService.Get<PagedResult<UserResponse>>("User");
        var userRoles = await _apiService.Get<PagedResult<UserRoleResponse>>("UserRole");
        var roles = userRoles?.Items ?? new List<UserRoleResponse>();

        var clientUsers = GetClientUsers(users?.Items ?? new List<UserResponse>(), roles);
        cmbUser.DataSource = CreateLookupItems(clientUsers.Select(x => new LookupItem(x.Id, GetUserDisplayName(x), x.Id)));

        var employees = await _apiService.Get<PagedResult<EmployeeResponse>>("Employee");
        var roleFilteredEmployees = GetEmployeeRoleEmployees(employees?.Items ?? new List<EmployeeResponse>(), roles);
        cmbEmployee.DataSource = CreateLookupItems(roleFilteredEmployees.Select(x => new LookupItem(x.Id, x.UserName, x.UserId)));

        var serviceCategories = await _apiService.Get<PagedResult<ServiceCategoryResponse>>("ServiceCategory");
        cmbServiceCategory.DataSource = CreateLookupItems(serviceCategories?.Items.Select(x => new LookupItem(x.Id, x.Name)), "All");

        var services = await _apiService.Get<PagedResult<ServiceResponse>>("Service");
        _services = services?.Items ?? new List<ServiceResponse>();
        PopulateServices();
        _lookupsLoaded = true;
    }

    private static List<UserResponse> GetClientUsers(List<UserResponse> users, List<UserRoleResponse> userRoles)
    {
        var clientUserIds = userRoles
            .Where(x => string.Equals(x.RoleName, "Client", StringComparison.OrdinalIgnoreCase))
            .Select(x => x.UserId)
            .ToHashSet();

        return users
            .Where(x => clientUserIds.Contains(x.Id))
            .ToList();
    }

    private static List<EmployeeResponse> GetEmployeeRoleEmployees(List<EmployeeResponse> employees, List<UserRoleResponse> userRoles)
    {
        var employeeUserIds = userRoles
            .Where(x => string.Equals(x.RoleName, "Employee", StringComparison.OrdinalIgnoreCase))
            .Select(x => x.UserId)
            .ToHashSet();

        return employees
            .Where(x => employeeUserIds.Contains(x.UserId))
            .ToList();
    }

    private void LoadStatusLookup()
    {
        cmbStatus.DataSource = Enum.GetValues<AppointmentStatus>()
            .Select(x => new StatusLookupItem(x, x.ToString()))
            .ToList();
    }

    private async Task LoadAppointment()
    {
        var appointment = await _apiService.Get<AppointmentResponse>($"Appointment/{_appointmentId}");

        if (appointment == null)
        {
            MessageBox.Show(GetApiErrorMessage("Unable to load appointment."));
            DialogResult = DialogResult.Cancel;
            Close();
            return;
        }

        SelectLookupItem(cmbUser, appointment.UserId);
        SelectLookupItem(cmbEmployee, appointment.EmployeeId);

        var selectedService = _services.FirstOrDefault(x => x.Id == appointment.WellnessServiceId);
        SelectLookupItem(cmbServiceCategory, selectedService?.ServiceCategoryId ?? 0);
        PopulateServices();
        SelectLookupItem(cmbService, appointment.WellnessServiceId);

        SelectStatus(appointment.Status);
        dtpAppointmentDate.Value = appointment.AppointmentDate.Date;
        dtpStartTime.Value = DateTime.Today.Add(appointment.StartTime);
        dtpEndTime.Value = DateTime.Today.Add(appointment.EndTime);
        txtNotes.Text = appointment.Notes;
        txtCancellationReason.Text = appointment.CancellationReason;
    }

    private async void btnSave_Click(object sender, EventArgs e)
    {
        if (!ValidateInput())
        {
            return;
        }

        if (_appointmentId.HasValue)
        {
            await UpdateAppointment();
        }
        else
        {
            await InsertAppointment();
        }
    }

    private async Task InsertAppointment()
    {
        var request = new AppointmentInsertRequest
        {
            UserId = GetSelectedLookupId(cmbUser),
            EmployeeId = GetSelectedLookupId(cmbEmployee),
            WellnessServiceId = GetSelectedLookupId(cmbService),
            AppointmentDate = dtpAppointmentDate.Value.Date,
            StartTime = dtpStartTime.Value.TimeOfDay,
            EndTime = dtpEndTime.Value.TimeOfDay,
            Status = GetSelectedStatus(),
            Notes = string.IsNullOrWhiteSpace(txtNotes.Text) ? null : txtNotes.Text.Trim(),
            CancellationReason = string.IsNullOrWhiteSpace(txtCancellationReason.Text) ? null : txtCancellationReason.Text.Trim()
        };

        var response = await _apiService.Post<AppointmentResponse>("Appointment", request);

        if (response == null)
        {
            MessageBox.Show(GetApiErrorMessage("Unable to save appointment."));
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    private async Task UpdateAppointment()
    {
        var request = new AppointmentUpdateRequest
        {
            Id = _appointmentId!.Value,
            UserId = GetSelectedLookupId(cmbUser),
            EmployeeId = GetSelectedLookupId(cmbEmployee),
            WellnessServiceId = GetSelectedLookupId(cmbService),
            AppointmentDate = dtpAppointmentDate.Value.Date,
            StartTime = dtpStartTime.Value.TimeOfDay,
            EndTime = dtpEndTime.Value.TimeOfDay,
            Status = GetSelectedStatus(),
            Notes = string.IsNullOrWhiteSpace(txtNotes.Text) ? null : txtNotes.Text.Trim(),
            CancellationReason = string.IsNullOrWhiteSpace(txtCancellationReason.Text) ? null : txtCancellationReason.Text.Trim()
        };

        var response = await _apiService.Put<AppointmentResponse>($"Appointment/{_appointmentId.Value}", request);

        if (response == null)
        {
            MessageBox.Show(GetApiErrorMessage("Unable to save appointment."));
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    private bool ValidateInput()
    {
        if (GetSelectedLookupId(cmbUser) <= 0)
        {
            MessageBox.Show("User is required.");
            cmbUser.Focus();
            return false;
        }

        if (GetSelectedLookupId(cmbEmployee) <= 0)
        {
            MessageBox.Show("Employee is required.");
            cmbEmployee.Focus();
            return false;
        }

        if (GetSelectedLookupId(cmbService) <= 0)
        {
            MessageBox.Show("Service is required.");
            cmbService.Focus();
            return false;
        }

        if (GetSelectedRelatedUserId(cmbUser) > 0
            && GetSelectedRelatedUserId(cmbUser) == GetSelectedRelatedUserId(cmbEmployee))
        {
            MessageBox.Show("Client and employee cannot be the same person.");
            cmbEmployee.Focus();
            return false;
        }

        if (dtpEndTime.Value.TimeOfDay <= dtpStartTime.Value.TimeOfDay)
        {
            MessageBox.Show("End time must be after start time.");
            dtpEndTime.Focus();
            return false;
        }

        if (txtNotes.Text.Length > 1000)
        {
            MessageBox.Show("Notes can contain up to 1000 characters.");
            txtNotes.Focus();
            return false;
        }

        if (txtCancellationReason.Text.Length > 500)
        {
            MessageBox.Show("Cancellation reason can contain up to 500 characters.");
            txtCancellationReason.Focus();
            return false;
        }

        return true;
    }

    private void cmbServiceCategory_SelectedIndexChanged(object sender, EventArgs e)
    {
        PopulateServices(_lookupsLoaded);
    }

    private void PopulateServices(bool showEmptyMessage = false)
    {
        var selectedServiceId = GetSelectedLookupId(cmbService);
        var selectedServiceCategoryId = GetSelectedLookupId(cmbServiceCategory);

        var services = _services.AsEnumerable();

        if (selectedServiceCategoryId > 0)
        {
            services = services.Where(x => x.ServiceCategoryId == selectedServiceCategoryId);
        }

        var serviceLookupItems = services
            .Select(x => new LookupItem(x.Id, x.Name))
            .ToList();

        cmbService.Enabled = serviceLookupItems.Count > 0;
        cmbService.DataSource = CreateLookupItems(serviceLookupItems);

        if (showEmptyMessage && selectedServiceCategoryId > 0 && serviceLookupItems.Count == 0)
        {
            MessageBox.Show("No services are available for the selected category.");
        }

        if (selectedServiceId > 0)
        {
            SelectLookupItem(cmbService, selectedServiceId);
        }
    }

    private static int GetSelectedLookupId(ComboBox comboBox)
    {
        return comboBox.SelectedItem is LookupItem item ? item.Id : 0;
    }

    private static int GetSelectedRelatedUserId(ComboBox comboBox)
    {
        return comboBox.SelectedItem is LookupItem item ? item.RelatedUserId : 0;
    }

    private AppointmentStatus GetSelectedStatus()
    {
        return cmbStatus.SelectedItem is StatusLookupItem item
            ? item.Status
            : AppointmentStatus.Pending;
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

    private void SelectStatus(AppointmentStatus status)
    {
        for (var i = 0; i < cmbStatus.Items.Count; i++)
        {
            if (cmbStatus.Items[i] is StatusLookupItem item && item.Status == status)
            {
                cmbStatus.SelectedIndex = i;
                return;
            }
        }
    }

    private static List<LookupItem> CreateLookupItems(IEnumerable<LookupItem>? items)
    {
        var lookupItems = new List<LookupItem> { new LookupItem(0, "Select") };
        lookupItems.AddRange(items ?? Enumerable.Empty<LookupItem>());
        return lookupItems;
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

    private void btnCancel_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private sealed record LookupItem(int Id, string Name, int RelatedUserId = 0)
    {
        public override string ToString()
        {
            return Name;
        }
    }

    private sealed record StatusLookupItem(AppointmentStatus Status, string Name)
    {
        public override string ToString()
        {
            return Name;
        }
    }
}
