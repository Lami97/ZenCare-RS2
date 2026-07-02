using ZenCare.WinUI.Forms;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
    }

    private void btnLogout_Click(object sender, EventArgs e)
    {
        AuthStorage.Token = null;
        AuthStorage.Username = null;
        AuthStorage.Roles.Clear();

        Hide();

        using var loginForm = new LoginForm();
        loginForm.ShowDialog();

        Close();
    }
}
