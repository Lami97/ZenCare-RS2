using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI.Forms;

public partial class FAQCategoryForm : Form
{
    private readonly APIService _apiService = new APIService();

    public FAQCategoryForm()
    {
        InitializeComponent();
        chkIsActive.CheckState = CheckState.Indeterminate;
    }

    private async void FAQCategoryForm_Load(object sender, EventArgs e)
    {
        await LoadFAQCategories();
    }

    private async void btnSearch_Click(object sender, EventArgs e)
    {
        await LoadFAQCategories();
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
        OpenDetailsForm(new FAQCategoryDetailsForm());
    }

    private void btnEdit_Click(object sender, EventArgs e)
    {
        var selectedId = GetSelectedFAQCategoryId();

        if (selectedId == null)
        {
            MessageBox.Show("Please select an FAQ category.");
            return;
        }

        OpenDetailsForm(new FAQCategoryDetailsForm(selectedId.Value));
    }

    private async void btnDelete_Click(object sender, EventArgs e)
    {
        var selectedId = GetSelectedFAQCategoryId();

        if (selectedId == null)
        {
            MessageBox.Show("Please select an FAQ category.");
            return;
        }

        var result = MessageBox.Show(
            "Are you sure you want to delete this FAQ category?",
            "Confirm delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result != DialogResult.Yes)
        {
            return;
        }

        var deleted = await _apiService.Delete($"FAQCategory/{selectedId.Value}");

        if (!deleted)
        {
            MessageBox.Show(GetDeleteErrorMessage());
            return;
        }

        await LoadFAQCategories();
    }

    private async void btnRefresh_Click(object sender, EventArgs e)
    {
        txtName.Clear();
        chkIsActive.CheckState = CheckState.Indeterminate;
        await LoadFAQCategories();
    }

    private async Task LoadFAQCategories()
    {
        var search = new FAQCategorySearchObject
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

        var endpoint = query.Count == 0 ? "FAQCategory" : $"FAQCategory?{string.Join("&", query)}";
        var result = await _apiService.Get<PagedResult<FAQCategoryResponse>>(endpoint);

        if (result == null)
        {
            MessageBox.Show(GetApiErrorMessage("Unable to load FAQ categories."));
            dgvFAQCategories.DataSource = new List<FAQCategoryResponse>();
            return;
        }

        dgvFAQCategories.DataSource = result.Items ?? new List<FAQCategoryResponse>();
    }

    private int? GetSelectedFAQCategoryId()
    {
        return dgvFAQCategories.CurrentRow?.DataBoundItem is FAQCategoryResponse category
            ? category.Id
            : null;
    }

    private string GetDeleteErrorMessage()
    {
        const string linkedRecordsMessage = "FAQ category cannot be deleted because it is linked to FAQ records.";

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
            || errorMessage.Contains("<html", StringComparison.OrdinalIgnoreCase))
        {
            return linkedRecordsMessage;
        }

        return errorMessage;
    }

    private string GetApiErrorMessage(string fallback)
    {
        return string.IsNullOrWhiteSpace(_apiService.LastErrorMessage)
            ? fallback
            : _apiService.LastErrorMessage;
    }

    private async void OpenDetailsForm(FAQCategoryDetailsForm form)
    {
        if (form.ShowDialog() == DialogResult.OK)
        {
            await LoadFAQCategories();
        }
    }
}
