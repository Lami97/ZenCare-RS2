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

    private void btnServices_Click(object sender, EventArgs e)
    {
        LoadChildForm(new ServiceForm());
    }

    private void btnServiceCategories_Click(object sender, EventArgs e)
    {
        LoadChildForm(new ServiceCategoryForm());
    }

    private void btnFAQCategories_Click(object sender, EventArgs e)
    {
        LoadChildForm(new FAQCategoryForm());
    }

    private void btnFAQs_Click(object sender, EventArgs e)
    {
        LoadChildForm(new FAQForm());
    }

    private void btnEmployees_Click(object sender, EventArgs e)
    {
        LoadChildForm(new EmployeeForm());
    }

    private void btnUsers_Click(object sender, EventArgs e)
    {
        LoadChildForm(new UserForm());
    }

    private void btnUserRoles_Click(object sender, EventArgs e)
    {
        LoadChildForm(new UserRoleForm());
    }

    private void btnAppointments_Click(object sender, EventArgs e)
    {
        LoadChildForm(new AppointmentForm());
    }

    private void btnReports_Click(object sender, EventArgs e)
    {
        LoadChildForm(new ReportsForm());
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
