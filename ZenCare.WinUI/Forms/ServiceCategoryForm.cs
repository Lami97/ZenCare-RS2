using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI.Forms;

public partial class ServiceCategoryForm : Form
{
    private readonly APIService _apiService = new APIService();

    public ServiceCategoryForm()
    {
        InitializeComponent();
    }

    private async void ServiceCategoryForm_Load(object sender, EventArgs e)
    {
        await LoadServiceCategories();
    }

    private async void btnSearch_Click(object sender, EventArgs e)
    {
        await LoadServiceCategories();
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
        OpenDetailsForm(new ServiceCategoryDetailsForm());
    }

    private void btnEdit_Click(object sender, EventArgs e)
    {
        var selectedId = GetSelectedServiceCategoryId();

        if (selectedId == null)
        {
            MessageBox.Show("Please select a service category.");
            return;
        }

        OpenDetailsForm(new ServiceCategoryDetailsForm(selectedId.Value));
    }

    private async void btnDelete_Click(object sender, EventArgs e)
    {
        var selectedId = GetSelectedServiceCategoryId();

        if (selectedId == null)
        {
            MessageBox.Show("Please select a service category.");
            return;
        }

        var result = MessageBox.Show(
            "Are you sure you want to delete this service category?",
            "Confirm delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result != DialogResult.Yes)
        {
            return;
        }

        await _apiService.Delete($"ServiceCategory/{selectedId.Value}");
        await LoadServiceCategories();
    }

    private async void btnRefresh_Click(object sender, EventArgs e)
    {
        txtName.Clear();
        chkIsActive.CheckState = CheckState.Indeterminate;
        await LoadServiceCategories();
    }

    private async Task LoadServiceCategories()
    {
        var search = new ServiceCategorySearchObject
        {
            Name = string.IsNullOrWhiteSpace(txtName.Text) ? null : txtName.Text,
            IsActive = chkIsActive.CheckState == CheckState.Indeterminate ? null : chkIsActive.Checked
        };

        var query = new List<string>();

        if (!string.IsNullOrWhiteSpace(search.Name))
        {
            query.Add($"Name={Uri.EscapeDataString(search.Name)}");
        }

        if (search.IsActive.HasValue)
        {
            query.Add($"IsActive={search.IsActive.Value}");
        }

        var endpoint = query.Count == 0 ? "ServiceCategory" : $"ServiceCategory?{string.Join("&", query)}";
        var result = await _apiService.Get<PagedResult<ServiceCategoryResponse>>(endpoint);
        dgvServiceCategories.DataSource = result?.Items ?? new List<ServiceCategoryResponse>();
    }

    private int? GetSelectedServiceCategoryId()
    {
        return dgvServiceCategories.CurrentRow?.DataBoundItem is ServiceCategoryResponse serviceCategory
            ? serviceCategory.Id
            : null;
    }

    private async void OpenDetailsForm(ServiceCategoryDetailsForm form)
    {
        if (form.ShowDialog() == DialogResult.OK)
        {
            await LoadServiceCategories();
        }
    }
}
