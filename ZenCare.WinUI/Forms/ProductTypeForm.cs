using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI.Forms;

public partial class ProductTypeForm : Form
{
    private readonly APIService _apiService = new APIService();

    public ProductTypeForm()
    {
        InitializeComponent();
    }

    private async void ProductTypeForm_Load(object sender, EventArgs e)
    {
        await LoadProductTypes();
    }

    private async void btnSearch_Click(object sender, EventArgs e)
    {
        await LoadProductTypes();
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
        OpenDetailsForm(new ProductTypeDetailsForm());
    }

    private void btnEdit_Click(object sender, EventArgs e)
    {
        var selectedId = GetSelectedProductTypeId();

        if (selectedId == null)
        {
            MessageBox.Show("Please select a product type.");
            return;
        }

        OpenDetailsForm(new ProductTypeDetailsForm(selectedId.Value));
    }

    private async void btnDelete_Click(object sender, EventArgs e)
    {
        var selectedId = GetSelectedProductTypeId();

        if (selectedId == null)
        {
            MessageBox.Show("Please select a product type.");
            return;
        }

        var result = MessageBox.Show(
            "Are you sure you want to delete this product type?",
            "Confirm delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result != DialogResult.Yes)
        {
            return;
        }

        await _apiService.Delete($"ProductType/{selectedId.Value}");
        await LoadProductTypes();
    }

    private async void btnRefresh_Click(object sender, EventArgs e)
    {
        await LoadProductTypes();
    }

    private async Task LoadProductTypes()
    {
        var search = new ProductTypeSearchObject
        {
            Name = string.IsNullOrWhiteSpace(txtName.Text) ? null : txtName.Text
        };

        var endpoint = "ProductType";

        if (!string.IsNullOrWhiteSpace(search.Name))
        {
            endpoint += $"?Name={Uri.EscapeDataString(search.Name)}";
        }

        var result = await _apiService.Get<PagedResult<ProductTypeResponse>>(endpoint);
        dgvProductTypes.DataSource = result?.Items ?? new List<ProductTypeResponse>();
    }

    private int? GetSelectedProductTypeId()
    {
        return dgvProductTypes.CurrentRow?.DataBoundItem is ProductTypeResponse productType
            ? productType.Id
            : null;
    }

    private async void OpenDetailsForm(ProductTypeDetailsForm form)
    {
        if (form.ShowDialog() == DialogResult.OK)
        {
            await LoadProductTypes();
        }
    }
}
