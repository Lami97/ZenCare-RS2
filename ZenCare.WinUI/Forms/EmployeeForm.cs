using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI.Forms;

public partial class EmployeeForm : Form
{
    private readonly APIService _apiService = new APIService();

    public EmployeeForm()
    {
        InitializeComponent();
    }

    private async void EmployeeForm_Load(object sender, EventArgs e)
    {
        await LoadLookups();
        await LoadEmployees();
    }

    private async void btnSearch_Click(object sender, EventArgs e)
    {
        await LoadEmployees();
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
        OpenDetailsForm(new EmployeeDetailsForm());
    }

    private void btnEdit_Click(object sender, EventArgs e)
    {
        var selectedId = GetSelectedEmployeeId();

        if (selectedId == null)
        {
            MessageBox.Show("Please select an employee.");
            return;
        }

        OpenDetailsForm(new EmployeeDetailsForm(selectedId.Value));
    }

    private async void btnDelete_Click(object sender, EventArgs e)
    {
        var selectedId = GetSelectedEmployeeId();

        if (selectedId == null)
        {
            MessageBox.Show("Please select an employee.");
            return;
        }

        var result = MessageBox.Show(
            "Are you sure you want to delete this employee?",
            "Confirm delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result != DialogResult.Yes)
        {
            return;
        }

        await _apiService.Delete($"Employee/{selectedId.Value}");
        await LoadEmployees();
    }

    private async void btnRefresh_Click(object sender, EventArgs e)
    {
        cmbUser.SelectedIndex = 0;
        txtSpecialization.Clear();
        chkIsAvailable.CheckState = CheckState.Indeterminate;
        await LoadEmployees();
    }

    private async Task LoadLookups()
    {
        var users = await _apiService.Get<PagedResult<UserResponse>>("User");
        cmbUser.DataSource = CreateLookupItems(users?.Items.Select(x => new LookupItem(x.Id, x.Username)), "All");
    }

    private async Task LoadEmployees()
    {
        var search = new EmployeeSearchObject
        {
            UserId = GetSelectedLookupId(cmbUser),
            Specialization = string.IsNullOrWhiteSpace(txtSpecialization.Text) ? null : txtSpecialization.Text,
            IsAvailable = chkIsAvailable.CheckState == CheckState.Indeterminate ? null : chkIsAvailable.Checked
        };

        var query = new List<string>();

        if (search.UserId.HasValue)
        {
            query.Add($"UserId={search.UserId.Value}");
        }

        if (!string.IsNullOrWhiteSpace(search.Specialization))
        {
            query.Add($"Specialization={Uri.EscapeDataString(search.Specialization)}");
        }

        if (search.IsAvailable.HasValue)
        {
            query.Add($"IsAvailable={search.IsAvailable.Value}");
        }

        var endpoint = query.Count == 0 ? "Employee" : $"Employee?{string.Join("&", query)}";
        var result = await _apiService.Get<PagedResult<EmployeeResponse>>(endpoint);
        dgvEmployees.DataSource = result?.Items ?? new List<EmployeeResponse>();
    }

    private int? GetSelectedEmployeeId()
    {
        return dgvEmployees.CurrentRow?.DataBoundItem is EmployeeResponse employee
            ? employee.Id
            : null;
    }

    private static int? GetSelectedLookupId(ComboBox comboBox)
    {
        return comboBox.SelectedItem is LookupItem { Id: > 0 } item ? item.Id : null;
    }

    private static List<LookupItem> CreateLookupItems(IEnumerable<LookupItem>? items, string emptyText)
    {
        var lookupItems = new List<LookupItem> { new LookupItem(0, emptyText) };
        lookupItems.AddRange(items ?? Enumerable.Empty<LookupItem>());
        return lookupItems;
    }

    private async void OpenDetailsForm(EmployeeDetailsForm form)
    {
        if (form.ShowDialog() == DialogResult.OK)
        {
            await LoadEmployees();
        }
    }

    private sealed record LookupItem(int Id, string Name)
    {
        public override string ToString()
        {
            return Name;
        }
    }
}
