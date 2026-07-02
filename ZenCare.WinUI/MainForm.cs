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

    private void btnProducts_Click(object sender, EventArgs e)
    {
        LoadChildForm(new ProductCategoryForm());
    }

    private void btnProductsManagement_Click(object sender, EventArgs e)
    {
        LoadChildForm(new ProductForm());
    }

    private void btnProductTypes_Click(object sender, EventArgs e)
    {
        LoadChildForm(new ProductTypeForm());
    }

    private void btnUnits_Click(object sender, EventArgs e)
    {
        LoadChildForm(new UnitOfMeasureForm());
    }

    private void LoadChildForm(Form form)
    {
        pnlContent.Controls.Clear();

        form.TopLevel = false;
        form.FormBorderStyle = FormBorderStyle.None;
        form.Dock = DockStyle.Fill;

        pnlContent.Controls.Add(form);
        form.Show();
    }
}
