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
        MessageBox.Show("Coming soon");
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
}
