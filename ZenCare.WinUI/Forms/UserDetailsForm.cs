using System.Net.Mail;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI.Forms;

public partial class UserDetailsForm : Form
{
    private readonly APIService _apiService = new APIService();
    private readonly int? _userId;

    public UserDetailsForm()
    {
        InitializeComponent();
        chkIsActive.Checked = true;
    }

    public UserDetailsForm(int userId)
        : this()
    {
        _userId = userId;
    }

    private async void UserDetailsForm_Load(object sender, EventArgs e)
    {
        if (_userId.HasValue)
        {
            Text = "Edit user";
            HidePasswordFields();
            await LoadUser();
        }
        else
        {
            Text = "New user";
        }
    }

    private async Task LoadUser()
    {
        var user = await _apiService.Get<UserResponse>($"User/{_userId}");

        if (user == null)
        {
            MessageBox.Show(GetApiErrorMessage("Unable to load user."));
            DialogResult = DialogResult.Cancel;
            Close();
            return;
        }

        txtFirstName.Text = user.FirstName;
        txtLastName.Text = user.LastName;
        txtEmail.Text = user.Email;
        txtUsername.Text = user.Username;
        txtPhoneNumber.Text = user.PhoneNumber;
        chkIsActive.Checked = user.IsActive;
    }

    private async void btnSave_Click(object sender, EventArgs e)
    {
        if (!ValidateInput())
        {
            return;
        }

        if (_userId.HasValue)
        {
            await UpdateUser();
        }
        else
        {
            await InsertUser();
        }
    }

    private async Task InsertUser()
    {
        var request = new UserInsertRequest
        {
            FirstName = txtFirstName.Text.Trim(),
            LastName = txtLastName.Text.Trim(),
            Email = txtEmail.Text.Trim(),
            Username = txtUsername.Text.Trim(),
            Password = txtPassword.Text,
            PasswordConfirm = txtPasswordConfirm.Text,
            PhoneNumber = string.IsNullOrWhiteSpace(txtPhoneNumber.Text) ? null : txtPhoneNumber.Text.Trim(),
            IsActive = chkIsActive.Checked
        };

        var response = await _apiService.Post<UserResponse>("User", request);

        if (response == null)
        {
            MessageBox.Show(GetApiErrorMessage("Unable to add user."));
            return;
        }

        MessageBox.Show("User was added successfully.");
        DialogResult = DialogResult.OK;
        Close();
    }

    private async Task UpdateUser()
    {
        var request = new UserUpdateRequest
        {
            Id = _userId!.Value,
            FirstName = txtFirstName.Text.Trim(),
            LastName = txtLastName.Text.Trim(),
            Email = txtEmail.Text.Trim(),
            Username = txtUsername.Text.Trim(),
            PhoneNumber = string.IsNullOrWhiteSpace(txtPhoneNumber.Text) ? null : txtPhoneNumber.Text.Trim(),
            IsActive = chkIsActive.Checked
        };

        var response = await _apiService.Put<UserResponse>($"User/{_userId.Value}", request);

        if (response == null)
        {
            MessageBox.Show(GetApiErrorMessage("Unable to update user."));
            return;
        }

        MessageBox.Show("User was updated successfully.");
        DialogResult = DialogResult.OK;
        Close();
    }

    private bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(txtFirstName.Text))
        {
            MessageBox.Show("First name is required.");
            txtFirstName.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtLastName.Text))
        {
            MessageBox.Show("Last name is required.");
            txtLastName.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtEmail.Text) || !IsValidEmail(txtEmail.Text))
        {
            MessageBox.Show("Enter a valid email address.");
            txtEmail.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtUsername.Text))
        {
            MessageBox.Show("Username is required.");
            txtUsername.Focus();
            return false;
        }

        if (!string.IsNullOrWhiteSpace(txtPhoneNumber.Text) && txtPhoneNumber.Text.Length > 20)
        {
            MessageBox.Show("Phone can contain at most 20 characters.");
            txtPhoneNumber.Focus();
            return false;
        }

        if (!_userId.HasValue)
        {
            if (txtPassword.Text.Length < 6)
            {
                MessageBox.Show("Password must contain at least 6 characters.");
                txtPassword.Focus();
                return false;
            }

            if (txtPassword.Text != txtPasswordConfirm.Text)
            {
                MessageBox.Show("Password and confirmation password do not match.");
                txtPasswordConfirm.Focus();
                return false;
            }
        }

        return true;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            _ = new MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private void HidePasswordFields()
    {
        lblPassword.Visible = false;
        txtPassword.Visible = false;
        lblPasswordConfirm.Visible = false;
        txtPasswordConfirm.Visible = false;
        chkIsActive.Location = new Point(24, 264);
        btnSave.Location = new Point(264, 316);
        btnCancel.Location = new Point(356, 316);
        ClientSize = new Size(464, 373);
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
}
