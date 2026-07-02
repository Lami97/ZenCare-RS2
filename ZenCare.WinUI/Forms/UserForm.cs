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
            MessageBox.Show(GetApiErrorMessage("Unable to delete user."));
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

    private async void OpenDetailsForm(UserDetailsForm form)
    {
        if (form.ShowDialog() == DialogResult.OK)
        {
            await LoadUsers();
        }
    }
}
