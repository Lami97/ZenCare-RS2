using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI.Forms;

public partial class SupplierForm : Form
{
    private readonly APIService _apiService = new APIService();

    public SupplierForm()
    {
        InitializeComponent();
    }

    private async void SupplierForm_Load(object sender, EventArgs e)
    {
        await LoadSuppliers();
    }

    private async void btnSearch_Click(object sender, EventArgs e)
    {
        await LoadSuppliers();
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
        OpenDetailsForm(new SupplierDetailsForm());
    }

    private void btnEdit_Click(object sender, EventArgs e)
    {
        var selectedId = GetSelectedSupplierId();

        if (selectedId == null)
        {
            MessageBox.Show("Please select a supplier.");
            return;
        }

        OpenDetailsForm(new SupplierDetailsForm(selectedId.Value));
    }

    private async void btnDelete_Click(object sender, EventArgs e)
    {
        var selectedId = GetSelectedSupplierId();

        if (selectedId == null)
        {
            MessageBox.Show("Please select a supplier.");
            return;
        }

        var result = MessageBox.Show(
            "Are you sure you want to delete this supplier?",
            "Confirm delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result != DialogResult.Yes)
        {
            return;
        }

        var deleted = await _apiService.Delete($"Supplier/{selectedId.Value}");

        if (!deleted)
        {
            MessageBox.Show(GetApiErrorMessage("Supplier could not be deleted."));
            return;
        }

        await LoadSuppliers();
    }

    private async void btnRefresh_Click(object sender, EventArgs e)
    {
        txtName.Clear();
        chkIsActive.CheckState = CheckState.Indeterminate;
        await LoadSuppliers();
    }

    private async Task LoadSuppliers()
    {
        var search = new SupplierSearchObject
        {
            Name = string.IsNullOrWhiteSpace(txtName.Text) ? null : txtName.Text.Trim(),
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

        var endpoint = query.Count == 0 ? "Supplier" : $"Supplier?{string.Join("&", query)}";
        var result = await _apiService.Get<PagedResult<SupplierResponse>>(endpoint);

        if (result == null)
        {
            MessageBox.Show(GetApiErrorMessage("Unable to load suppliers."));
            dgvSuppliers.DataSource = new List<SupplierResponse>();
            return;
        }

        dgvSuppliers.DataSource = result.Items ?? new List<SupplierResponse>();
    }

    private int? GetSelectedSupplierId()
    {
        return dgvSuppliers.CurrentRow?.DataBoundItem is SupplierResponse supplier ? supplier.Id : null;
    }

    private string GetApiErrorMessage(string fallback)
    {
        return string.IsNullOrWhiteSpace(_apiService.LastErrorMessage) ? fallback : _apiService.LastErrorMessage;
    }

    private async void OpenDetailsForm(SupplierDetailsForm form)
    {
        if (form.ShowDialog() == DialogResult.OK)
        {
            await LoadSuppliers();
        }
    }
}
