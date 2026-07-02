using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI.Forms;

public partial class UserRoleForm : Form
{
    private readonly APIService _apiService = new APIService();

    public UserRoleForm()
    {
        InitializeComponent();
    }

    private async void UserRoleForm_Load(object sender, EventArgs e)
    {
        await LoadLookups();
        await LoadUserRoles();
    }

    private async void btnSearch_Click(object sender, EventArgs e)
    {
        await LoadUserRoles();
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
        OpenDetailsForm(new UserRoleDetailsForm());
    }

    private void btnEdit_Click(object sender, EventArgs e)
    {
        var selectedId = GetSelectedUserRoleId();

        if (selectedId == null)
        {
            MessageBox.Show("Please select a user role.");
            return;
        }

        OpenDetailsForm(new UserRoleDetailsForm(selectedId.Value));
    }

    private async void btnDelete_Click(object sender, EventArgs e)
    {
        var selectedId = GetSelectedUserRoleId();

        if (selectedId == null)
        {
            MessageBox.Show("Please select a user role.");
            return;
        }

        var result = MessageBox.Show(
            "Are you sure you want to delete this assigned role?",
            "Confirm delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result != DialogResult.Yes)
        {
            return;
        }

        var deleted = await _apiService.Delete($"UserRole/{selectedId.Value}");

        if (!deleted)
        {
            MessageBox.Show(GetApiErrorMessage("Unable to delete assigned role."));
            return;
        }

        MessageBox.Show("Assigned role was deleted successfully.");
        await LoadUserRoles();
    }

    private async void btnRefresh_Click(object sender, EventArgs e)
    {
        SelectLookupItem(cmbUser, 0);
        SelectLookupItem(cmbRole, 0);
        txtSearch.Clear();
        await LoadUserRoles();
    }

    private async Task LoadLookups()
    {
        var users = await _apiService.Get<PagedResult<UserResponse>>("User");
        cmbUser.DataSource = CreateLookupItems(users?.Items.Select(x => new LookupItem(x.Id, GetUserDisplayName(x))));

        var roles = await _apiService.Get<PagedResult<RoleResponse>>("Role");
        cmbRole.DataSource = CreateLookupItems(roles?.Items.Select(x => new LookupItem(x.Id, x.Name)));
    }

    private async Task LoadUserRoles()
    {
        var search = new UserRoleSearchObject
        {
            UserId = GetSelectedLookupId(cmbUser) > 0 ? GetSelectedLookupId(cmbUser) : null,
            RoleId = GetSelectedLookupId(cmbRole) > 0 ? GetSelectedLookupId(cmbRole) : null
        };

        var query = new List<string>();

        if (search.UserId.HasValue)
        {
            query.Add($"UserId={search.UserId.Value}");
        }

        if (search.RoleId.HasValue)
        {
            query.Add($"RoleId={search.RoleId.Value}");
        }

        var endpoint = query.Count == 0 ? "UserRole" : $"UserRole?{string.Join("&", query)}";
        var result = await _apiService.Get<PagedResult<UserRoleResponse>>(endpoint);

        if (result == null)
        {
            MessageBox.Show(GetApiErrorMessage("Unable to load assigned roles."));
            dgvUserRoles.DataSource = new List<UserRoleResponse>();
            return;
        }

        var items = result.Items ?? new List<UserRoleResponse>();

        if (!string.IsNullOrWhiteSpace(txtSearch.Text))
        {
            items = items
                .Where(x => x.UserName.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        dgvUserRoles.DataSource = items;
    }

    private int? GetSelectedUserRoleId()
    {
        return dgvUserRoles.CurrentRow?.DataBoundItem is UserRoleResponse userRole
            ? userRole.Id
            : null;
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
        var lookupItems = new List<LookupItem> { new LookupItem(0, "All") };
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

    private async void OpenDetailsForm(UserRoleDetailsForm form)
    {
        if (form.ShowDialog() == DialogResult.OK)
        {
            await LoadLookups();
            await LoadUserRoles();
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
