using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI.Forms;

public partial class ProductCategoryForm : Form
{
    private readonly APIService _apiService = new APIService();

    public ProductCategoryForm()
    {
        InitializeComponent();
    }

    private async void ProductCategoryForm_Load(object sender, EventArgs e)
    {
        await LoadProductCategories();
    }

    private async void btnSearch_Click(object sender, EventArgs e)
    {
        await LoadProductCategories();
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
        OpenDetailsForm(new ProductCategoryDetailsForm());
    }

    private void btnEdit_Click(object sender, EventArgs e)
    {
        var selectedId = GetSelectedProductCategoryId();

        if (selectedId == null)
        {
            MessageBox.Show("Please select a product category.");
            return;
        }

        OpenDetailsForm(new ProductCategoryDetailsForm(selectedId.Value));
    }

    private async void btnDelete_Click(object sender, EventArgs e)
    {
        var selectedId = GetSelectedProductCategoryId();

        if (selectedId == null)
        {
            MessageBox.Show("Please select a product category.");
            return;
        }

        var result = MessageBox.Show(
            "Are you sure you want to delete this product category?",
            "Confirm delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result != DialogResult.Yes)
        {
            return;
        }

        await _apiService.Delete($"ProductCategory/{selectedId.Value}");
        await LoadProductCategories();
    }

    private async void btnRefresh_Click(object sender, EventArgs e)
    {
        txtName.Clear();
        chkIsActive.Checked = false;
        await LoadProductCategories();
    }

    private async Task LoadProductCategories()
    {
        var search = new ProductCategorySearchObject
        {
            Name = string.IsNullOrWhiteSpace(txtName.Text) ? null : txtName.Text
        };

        var endpoint = "ProductCategory";

        if (!string.IsNullOrWhiteSpace(search.Name))
        {
            endpoint += $"?Name={Uri.EscapeDataString(search.Name)}";
        }

        var result = await _apiService.Get<PagedResult<ProductCategoryResponse>>(endpoint);
        dgvProductCategories.DataSource = result?.Items ?? new List<ProductCategoryResponse>();
    }

    private int? GetSelectedProductCategoryId()
    {
        return dgvProductCategories.CurrentRow?.DataBoundItem is ProductCategoryResponse productCategory
            ? productCategory.Id
            : null;
    }

    private async void OpenDetailsForm(ProductCategoryDetailsForm form)
    {
        if (form.ShowDialog() == DialogResult.OK)
        {
            await LoadProductCategories();
        }
    }
}
