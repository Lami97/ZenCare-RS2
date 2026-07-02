using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI.Forms;

public partial class EmployeeDetailsForm : Form
{
    private readonly APIService _apiService = new APIService();
    private readonly int? _employeeId;

    public EmployeeDetailsForm()
    {
        InitializeComponent();
        chkIsAvailable.Checked = true;
        dtpHireDate.Value = DateTime.Today;
        dtpHireDate.Enabled = false;
    }

    public EmployeeDetailsForm(int id)
        : this()
    {
        _employeeId = id;
    }

    private async void EmployeeDetailsForm_Load(object sender, EventArgs e)
    {
        await LoadLookups();

        if (_employeeId.HasValue)
        {
            await LoadEmployee();
        }
    }

    private async Task LoadLookups()
    {
        var users = await _apiService.Get<PagedResult<UserResponse>>("User");
        var employees = await _apiService.Get<PagedResult<EmployeeResponse>>("Employee");
        var currentEmployee = _employeeId.HasValue
            ? employees?.Items.FirstOrDefault(x => x.Id == _employeeId.Value)
            : null;

        var assignedUserIds = employees?.Items
            .Where(x => !_employeeId.HasValue || x.Id != _employeeId.Value)
            .Select(x => x.UserId)
            .ToHashSet() ?? new HashSet<int>();

        var availableUsers = users?.Items
            .Where(x => !assignedUserIds.Contains(x.Id) || x.Id == currentEmployee?.UserId)
            .Select(x => new LookupItem(x.Id, x.Username));

        cmbUser.DataSource = CreateLookupItems(availableUsers);
    }

    private async Task LoadEmployee()
    {
        var employee = await _apiService.Get<EmployeeResponse>($"Employee/{_employeeId}");

        if (employee == null)
        {
            MessageBox.Show("Employee was not found.");
            DialogResult = DialogResult.Cancel;
            Close();
            return;
        }

        SelectLookupItem(cmbUser, employee.UserId);
        txtSpecialization.Text = employee.Specialization;
        txtBio.Text = employee.Bio;
        chkHasHireDate.Checked = employee.HireDate.HasValue;
        dtpHireDate.Enabled = employee.HireDate.HasValue;
        dtpHireDate.Value = employee.HireDate ?? DateTime.Today;
        chkIsAvailable.Checked = employee.IsAvailable;
    }

    private async void btnSave_Click(object sender, EventArgs e)
    {
        if (!ValidateInput())
        {
            return;
        }

        if (_employeeId.HasValue)
        {
            var request = new EmployeeUpdateRequest
            {
                Id = _employeeId.Value,
                UserId = GetSelectedLookupId(cmbUser),
                Specialization = string.IsNullOrWhiteSpace(txtSpecialization.Text) ? null : txtSpecialization.Text,
                Bio = string.IsNullOrWhiteSpace(txtBio.Text) ? null : txtBio.Text,
                HireDate = chkHasHireDate.Checked ? dtpHireDate.Value.Date : null,
                IsAvailable = chkIsAvailable.Checked
            };

            var response = await _apiService.Put<EmployeeResponse>($"Employee/{_employeeId.Value}", request);

            if (response == null)
            {
                MessageBox.Show("Unable to save employee.");
                return;
            }
        }
        else
        {
            var request = new EmployeeInsertRequest
            {
                UserId = GetSelectedLookupId(cmbUser),
                Specialization = string.IsNullOrWhiteSpace(txtSpecialization.Text) ? null : txtSpecialization.Text,
                Bio = string.IsNullOrWhiteSpace(txtBio.Text) ? null : txtBio.Text,
                HireDate = chkHasHireDate.Checked ? dtpHireDate.Value.Date : null,
                IsAvailable = chkIsAvailable.Checked
            };

            var response = await _apiService.Post<EmployeeResponse>("Employee", request);

            if (response == null)
            {
                MessageBox.Show("Unable to save employee.");
                return;
            }
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void chkHasHireDate_CheckedChanged(object sender, EventArgs e)
    {
        dtpHireDate.Enabled = chkHasHireDate.Checked;
    }

    private bool ValidateInput()
    {
        if (GetSelectedLookupId(cmbUser) <= 0)
        {
            MessageBox.Show("User is required.");
            return false;
        }

        return true;
    }

    private static int GetSelectedLookupId(ComboBox comboBox)
    {
        return comboBox.SelectedItem is LookupItem item ? item.Id : 0;
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

    private static List<LookupItem> CreateLookupItems(IEnumerable<LookupItem>? items)
    {
        var lookupItems = new List<LookupItem> { new LookupItem(0, "Select") };
        lookupItems.AddRange(items ?? Enumerable.Empty<LookupItem>());
        return lookupItems;
    }

    private sealed record LookupItem(int Id, string Name)
    {
        public override string ToString()
        {
            return Name;
        }
    }
}
