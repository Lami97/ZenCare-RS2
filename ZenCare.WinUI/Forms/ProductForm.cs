using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI.Forms;

public partial class ProductForm : Form
{
    private readonly APIService _apiService = new APIService();

    public ProductForm()
    {
        InitializeComponent();
    }

    private async void ProductForm_Load(object sender, EventArgs e)
    {
        await LoadLookups();
        await LoadProducts();
    }

    private async void btnSearch_Click(object sender, EventArgs e)
    {
        await LoadProducts();
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
        OpenDetailsForm(new ProductDetailsForm());
    }

    private void btnEdit_Click(object sender, EventArgs e)
    {
        var selectedId = GetSelectedProductId();

        if (selectedId == null)
        {
            MessageBox.Show("Please select a product.");
            return;
        }

        OpenDetailsForm(new ProductDetailsForm(selectedId.Value));
    }

    private async void btnDelete_Click(object sender, EventArgs e)
    {
        var selectedId = GetSelectedProductId();

        if (selectedId == null)
        {
            MessageBox.Show("Please select a product.");
            return;
        }

        var result = MessageBox.Show(
            "Are you sure you want to delete this product?",
            "Confirm delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result != DialogResult.Yes)
        {
            return;
        }

        await _apiService.Delete($"Product/{selectedId.Value}");
        await LoadProducts();
    }

    private async void btnRefresh_Click(object sender, EventArgs e)
    {
        txtName.Clear();
        cmbProductCategory.SelectedIndex = 0;
        cmbProductType.SelectedIndex = 0;
        chkIsActive.CheckState = CheckState.Indeterminate;
        await LoadProducts();
    }

    private async Task LoadLookups()
    {
        var categories = await _apiService.Get<PagedResult<ProductCategoryResponse>>("ProductCategory");
        cmbProductCategory.DataSource = CreateLookupItems(categories?.Items.Select(x => new LookupItem(x.Id, x.Name)));

        var types = await _apiService.Get<PagedResult<ProductTypeResponse>>("ProductType");
        cmbProductType.DataSource = CreateLookupItems(types?.Items.Select(x => new LookupItem(x.Id, x.Name)));
    }

    private async Task LoadProducts()
    {
        var search = new ProductSearchObject
        {
            Name = string.IsNullOrWhiteSpace(txtName.Text) ? null : txtName.Text,
            ProductCategoryId = GetSelectedLookupId(cmbProductCategory),
            ProductTypeId = GetSelectedLookupId(cmbProductType),
            IsActive = chkIsActive.CheckState == CheckState.Indeterminate ? null : chkIsActive.Checked
        };

        var query = new List<string>();

        if (!string.IsNullOrWhiteSpace(search.Name))
        {
            query.Add($"Name={Uri.EscapeDataString(search.Name)}");
        }

        if (search.ProductCategoryId.HasValue)
        {
            query.Add($"ProductCategoryId={search.ProductCategoryId.Value}");
        }

        if (search.ProductTypeId.HasValue)
        {
            query.Add($"ProductTypeId={search.ProductTypeId.Value}");
        }

        if (search.IsActive.HasValue)
        {
            query.Add($"IsActive={search.IsActive.Value}");
        }

        var endpoint = query.Count == 0 ? "Product" : $"Product?{string.Join("&", query)}";
        var result = await _apiService.Get<PagedResult<ProductResponse>>(endpoint);
        dgvProducts.DataSource = result?.Items ?? new List<ProductResponse>();
    }

    private int? GetSelectedProductId()
    {
        return dgvProducts.CurrentRow?.DataBoundItem is ProductResponse product
            ? product.Id
            : null;
    }

    private static int? GetSelectedLookupId(ComboBox comboBox)
    {
        return comboBox.SelectedItem is LookupItem { Id: > 0 } item ? item.Id : null;
    }

    private static List<LookupItem> CreateLookupItems(IEnumerable<LookupItem>? items)
    {
        var lookupItems = new List<LookupItem> { new LookupItem(0, "All") };
        lookupItems.AddRange(items ?? Enumerable.Empty<LookupItem>());
        return lookupItems;
    }

    private async void OpenDetailsForm(ProductDetailsForm form)
    {
        if (form.ShowDialog() == DialogResult.OK)
        {
            await LoadProducts();
        }
    }

    private sealed record LookupItem(int Id, string Name)
    {
        public override string ToString()
        {
            return Name;
        }
    }
}
