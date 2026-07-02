using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI.Forms;

public partial class UserForm : Form
{
    private readonly APIService _apiService = new APIService();

    public UserForm()
    {
        InitializeComponent();
    }

    private async void UserForm_Load(object sender, EventArgs e)
    {
        await LoadUsers();
    }

    private async void btnSearch_Click(object sender, EventArgs e)
    {
        await LoadUsers();
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
        OpenDetailsForm(new UserDetailsForm());
    }

    private void btnEdit_Click(object sender, EventArgs e)
    {
        var selectedId = GetSelectedUserId();

        if (selectedId == null)
        {
            MessageBox.Show("Please select a user.");
            return;
        }

        OpenDetailsForm(new UserDetailsForm(selectedId.Value));
    }

    private async void btnDelete_Click(object sender, EventArgs e)
    {
        var selectedId = GetSelectedUserId();

        if (selectedId == null)
        {
            MessageBox.Show("Please select a user.");
            return;
        }

        var result = MessageBox.Show(
            "Are you sure you want to delete this user?",
            "Confirm delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result != DialogResult.Yes)
        {
            return;
        }

        var deleted = await _apiService.Delete($"User/{selectedId.Value}");

        if (!deleted)
        {
            MessageBox.Show(GetUserDeleteErrorMessage());
            return;
        }

        MessageBox.Show("User was deleted successfully.");
        await LoadUsers();
    }

    private async void btnRefresh_Click(object sender, EventArgs e)
    {
        txtSearch.Clear();
        chkIsActive.CheckState = CheckState.Indeterminate;
        await LoadUsers();
    }

    private async Task LoadUsers()
    {
        var search = new UserSearchObject
        {
            Username = string.IsNullOrWhiteSpace(txtSearch.Text) ? null : txtSearch.Text,
            IsActive = chkIsActive.CheckState == CheckState.Indeterminate ? null : chkIsActive.Checked
        };

        var query = new List<string>();

        if (!string.IsNullOrWhiteSpace(search.Username))
        {
            query.Add($"Username={Uri.EscapeDataString(search.Username)}");
        }

        if (search.IsActive.HasValue)
        {
            query.Add($"IsActive={search.IsActive.Value}");
        }

        var endpoint = query.Count == 0 ? "User" : $"User?{string.Join("&", query)}";
        var result = await _apiService.Get<PagedResult<UserResponse>>(endpoint);

        if (result == null)
        {
            MessageBox.Show(GetApiErrorMessage("Unable to load users."));
            dgvUsers.DataSource = new List<UserResponse>();
            return;
        }

        dgvUsers.DataSource = result.Items ?? new List<UserResponse>();
    }

    private int? GetSelectedUserId()
    {
        return dgvUsers.CurrentRow?.DataBoundItem is UserResponse user
            ? user.Id
            : null;
    }

    private string GetApiErrorMessage(string fallback)
    {
        return string.IsNullOrWhiteSpace(_apiService.LastErrorMessage)
            ? fallback
            : _apiService.LastErrorMessage;
    }

    private string GetUserDeleteErrorMessage()
    {
        const string linkedRecordsMessage = "User cannot be deleted because it is linked to appointments, roles, purchases, or other records.";

        if (string.IsNullOrWhiteSpace(_apiService.LastErrorMessage))
        {
            return linkedRecordsMessage;
        }

        var errorMessage = _apiService.LastErrorMessage;

        if (errorMessage.Contains("REFERENCE constraint", StringComparison.OrdinalIgnoreCase)
            || errorMessage.Contains("FOREIGN KEY", StringComparison.OrdinalIgnoreCase)
            || errorMessage.Contains("conflicted with", StringComparison.OrdinalIgnoreCase)
            || errorMessage.Contains("DbUpdateException", StringComparison.OrdinalIgnoreCase)
            || errorMessage.Contains("SqlException", StringComparison.OrdinalIgnoreCase)
            || errorMessage.Contains("Exception", StringComparison.OrdinalIgnoreCase)
            || errorMessage.Contains("StackTrace", StringComparison.OrdinalIgnoreCase)
            || errorMessage.Contains("Microsoft.EntityFrameworkCore", StringComparison.OrdinalIgnoreCase)
            || errorMessage.Contains(" at ", StringComparison.OrdinalIgnoreCase)
            || errorMessage.Contains("<html", StringComparison.OrdinalIgnoreCase))
        {
            return linkedRecordsMessage;
        }

        return errorMessage;
    }

    private async void OpenDetailsForm(UserDetailsForm form)
    {
        if (form.ShowDialog() == DialogResult.OK)
        {
            await LoadUsers();
        }
    }
}
