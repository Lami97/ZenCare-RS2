using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI.Forms;

public partial class ProductDetailsForm : Form
{
    private readonly APIService _apiService = new APIService();
    private readonly int? _productId;

    public ProductDetailsForm()
    {
        InitializeComponent();
        chkIsActive.Checked = true;
    }

    public ProductDetailsForm(int id)
        : this()
    {
        _productId = id;
    }

    private async void ProductDetailsForm_Load(object sender, EventArgs e)
    {
        await LoadLookups();

        if (_productId.HasValue)
        {
            await LoadProduct();
        }
    }

    private async Task LoadLookups()
    {
        var categories = await _apiService.Get<PagedResult<ProductCategoryResponse>>("ProductCategory");
        cmbProductCategory.DataSource = CreateLookupItems(categories?.Items.Select(x => new LookupItem(x.Id, x.Name)));

        var types = await _apiService.Get<PagedResult<ProductTypeResponse>>("ProductType");
        cmbProductType.DataSource = CreateLookupItems(types?.Items.Select(x => new LookupItem(x.Id, x.Name)));

        var units = await _apiService.Get<PagedResult<UnitOfMeasureResponse>>("UnitOfMeasure");
        cmbUnitOfMeasure.DataSource = CreateLookupItems(units?.Items.Select(x => new LookupItem(x.Id, x.Name)));
    }

    private async Task LoadProduct()
    {
        var product = await _apiService.Get<ProductResponse>($"Product/{_productId}");

        if (product == null)
        {
            MessageBox.Show("Product was not found.");
            DialogResult = DialogResult.Cancel;
            Close();
            return;
        }

        txtName.Text = product.Name;
        txtDescription.Text = product.Description;
        nudPrice.Value = product.Price > nudPrice.Maximum ? nudPrice.Maximum : product.Price;
        nudStockQuantity.Value = product.StockQuantity > nudStockQuantity.Maximum ? nudStockQuantity.Maximum : product.StockQuantity;
        SelectLookupItem(cmbProductCategory, product.ProductCategoryId);
        SelectLookupItem(cmbProductType, product.ProductTypeId);
        SelectLookupItem(cmbUnitOfMeasure, product.UnitOfMeasureId);
        chkIsActive.Checked = product.IsActive;
    }

    private async void btnSave_Click(object sender, EventArgs e)
    {
        if (!ValidateInput())
        {
            return;
        }

        if (_productId.HasValue)
        {
            var request = new ProductUpdateRequest
            {
                Id = _productId.Value,
                Name = txtName.Text,
                Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text,
                Price = nudPrice.Value,
                StockQuantity = (int)nudStockQuantity.Value,
                ProductCategoryId = GetSelectedLookupId(cmbProductCategory),
                ProductTypeId = GetSelectedLookupId(cmbProductType),
                UnitOfMeasureId = GetSelectedLookupId(cmbUnitOfMeasure),
                IsActive = chkIsActive.Checked
            };

            var response = await _apiService.Put<ProductResponse>($"Product/{_productId.Value}", request);

            if (response == null)
            {
                MessageBox.Show("Unable to save product.");
                return;
            }
        }
        else
        {
            var request = new ProductInsertRequest
            {
                Name = txtName.Text,
                Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text,
                Price = nudPrice.Value,
                StockQuantity = (int)nudStockQuantity.Value,
                ProductCategoryId = GetSelectedLookupId(cmbProductCategory),
                ProductTypeId = GetSelectedLookupId(cmbProductType),
                UnitOfMeasureId = GetSelectedLookupId(cmbUnitOfMeasure),
                IsActive = chkIsActive.Checked
            };

            var response = await _apiService.Post<ProductResponse>("Product", request);

            if (response == null)
            {
                MessageBox.Show("Unable to save product.");
                return;
            }
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(txtName.Text))
        {
            MessageBox.Show("Name is required.");
            return false;
        }

        if (nudPrice.Value < 0.01m)
        {
            MessageBox.Show("Price must be greater than 0.");
            return false;
        }

        if (GetSelectedLookupId(cmbProductCategory) <= 0)
        {
            MessageBox.Show("Product category is required.");
            return false;
        }

        if (GetSelectedLookupId(cmbProductType) <= 0)
        {
            MessageBox.Show("Product type is required.");
            return false;
        }

        if (GetSelectedLookupId(cmbUnitOfMeasure) <= 0)
        {
            MessageBox.Show("Unit of measure is required.");
            return false;
        }

        return true;
    }

    private static int GetSelectedLookupId(ComboBox comboBox)
    {
        return comboBox.SelectedItem is LookupItem item ? item.Id : 0;
    }

    private static void SelectLookupItem(ComboBox comboBox, int id)
    {
        for (var i = 0; i < comboBox.Items.Count; i++)
        {
            if (comboBox.Items[i] is LookupItem item && item.Id == id)
            {
                comboBox.SelectedIndex = i;
                return;
            }
        }
    }

    private static List<LookupItem> CreateLookupItems(IEnumerable<LookupItem>? items)
    {
        var lookupItems = new List<LookupItem> { new LookupItem(0, "Select") };
        lookupItems.AddRange(items ?? Enumerable.Empty<LookupItem>());
        return lookupItems;
    }

    private sealed record LookupItem(int Id, string Name)
    {
        public override string ToString()
        {
            return Name;
        }
    }
}
