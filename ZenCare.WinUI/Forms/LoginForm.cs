using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI.Forms;

public partial class LoginForm : Form
{
    private readonly APIService _apiService = new APIService();

    public LoginForm()
    {
        InitializeComponent();
    }

    private async void btnLogin_Click(object sender, EventArgs e)
    {
        lblError.Visible = false;

        var request = new LoginRequest
        {
            Username = txtUsername.Text,
            Password = txtPassword.Text
        };

        var response = await _apiService.Post<LoginResponse>("Auth/Login", request);

        if (response == null)
        {
            lblError.Text = "Invalid username or password";
            lblError.Visible = true;
            return;
        }

        AuthStorage.Token = response.Token;
        AuthStorage.Username = response.Username;
        AuthStorage.Roles = response.Roles;

        Hide();

        using var mainForm = new MainForm();
        mainForm.ShowDialog();

        Close();
    }
}
