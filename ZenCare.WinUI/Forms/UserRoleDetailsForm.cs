using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI.Forms;

public partial class UserRoleDetailsForm : Form
{
    private readonly APIService _apiService = new APIService();
    private readonly int? _userRoleId;

    public UserRoleDetailsForm()
    {
        InitializeComponent();
    }

    public UserRoleDetailsForm(int userRoleId)
        : this()
    {
        _userRoleId = userRoleId;
    }

    private async void UserRoleDetailsForm_Load(object sender, EventArgs e)
    {
        await LoadLookups();

        if (_userRoleId.HasValue)
        {
            Text = "Edit user role";
            await LoadUserRole();
        }
        else
        {
            Text = "New user role";
        }
    }

    private async Task LoadLookups()
    {
        var users = await _apiService.Get<PagedResult<UserResponse>>("User");
        cmbUser.DataSource = CreateLookupItems(users?.Items.Select(x => new LookupItem(x.Id, GetUserDisplayName(x))));

        var roles = await _apiService.Get<PagedResult<RoleResponse>>("Role");
        cmbRole.DataSource = CreateLookupItems(roles?.Items.Select(x => new LookupItem(x.Id, x.Name)));
    }

    private async Task LoadUserRole()
    {
        var userRole = await _apiService.Get<UserRoleResponse>($"UserRole/{_userRoleId}");

        if (userRole == null)
        {
            MessageBox.Show(GetApiErrorMessage("Unable to load assigned role."));
            DialogResult = DialogResult.Cancel;
            Close();
            return;
        }

        SelectLookupItem(cmbUser, userRole.UserId);
        SelectLookupItem(cmbRole, userRole.RoleId);
    }

    private async void btnSave_Click(object sender, EventArgs e)
    {
        if (!ValidateInput())
        {
            return;
        }

        if (_userRoleId.HasValue)
        {
            await UpdateUserRole();
        }
        else
        {
            await InsertUserRole();
        }
    }

    private async Task InsertUserRole()
    {
        var request = new UserRoleInsertRequest
        {
            UserId = GetSelectedLookupId(cmbUser),
            RoleId = GetSelectedLookupId(cmbRole)
        };

        var response = await _apiService.Post<UserRoleResponse>("UserRole", request);

        if (response == null)
        {
            MessageBox.Show(GetSaveErrorMessage());
            return;
        }

        MessageBox.Show("Assigned role was added successfully.");
        DialogResult = DialogResult.OK;
        Close();
    }

    private async Task UpdateUserRole()
    {
        var request = new UserRoleUpdateRequest
        {
            Id = _userRoleId!.Value,
            UserId = GetSelectedLookupId(cmbUser),
            RoleId = GetSelectedLookupId(cmbRole)
        };

        var response = await _apiService.Put<UserRoleResponse>($"UserRole/{_userRoleId.Value}", request);

        if (response == null)
        {
            MessageBox.Show(GetSaveErrorMessage());
            return;
        }

        MessageBox.Show("Assigned role was updated successfully.");
        DialogResult = DialogResult.OK;
        Close();
    }

    private bool ValidateInput()
    {
        if (GetSelectedLookupId(cmbUser) <= 0)
        {
            MessageBox.Show("Please select a user.");
            cmbUser.Focus();
            return false;
        }

        if (GetSelectedLookupId(cmbRole) <= 0)
        {
            MessageBox.Show("Please select a role.");
            cmbRole.Focus();
            return false;
        }

        return true;
    }

    private string GetSaveErrorMessage()
    {
        if (IsDuplicateError(_apiService.LastErrorMessage))
        {
            return "The selected user already has this role.";
        }

        return GetApiErrorMessage("Unable to save assigned role.");
    }

    private static bool IsDuplicateError(string? errorMessage)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
        {
            return false;
        }

        return errorMessage.Contains("duplicate", StringComparison.OrdinalIgnoreCase)
            || errorMessage.Contains("unique", StringComparison.OrdinalIgnoreCase)
            || errorMessage.Contains("IX_UserRoles", StringComparison.OrdinalIgnoreCase);
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

    private sealed record LookupItem(int Id, string Name)
    {
        public override string ToString()
        {
            return Name;
        }
    }
}
