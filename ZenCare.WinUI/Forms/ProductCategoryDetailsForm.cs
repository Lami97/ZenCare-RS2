using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI.Forms;

public partial class ProductCategoryDetailsForm : Form
{
    private readonly APIService _apiService = new APIService();
    private readonly int? _productCategoryId;

    public ProductCategoryDetailsForm()
    {
        InitializeComponent();
        chkIsActive.Checked = true;
    }

    public ProductCategoryDetailsForm(int id)
        : this()
    {
        _productCategoryId = id;
    }

    private async void ProductCategoryDetailsForm_Load(object sender, EventArgs e)
    {
        if (_productCategoryId.HasValue)
        {
            await LoadProductCategory();
        }
    }

    private async Task LoadProductCategory()
    {
        var productCategory = await _apiService.Get<ProductCategoryResponse>($"ProductCategory/{_productCategoryId}");

        if (productCategory == null)
        {
            MessageBox.Show("Product category was not found.");
            DialogResult = DialogResult.Cancel;
            Close();
            return;
        }

        txtName.Text = productCategory.Name;
        chkIsActive.Checked = productCategory.IsActive;
    }

    private async void btnSave_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtName.Text))
        {
            MessageBox.Show("Name is required.");
            return;
        }

        if (_productCategoryId.HasValue)
        {
            var request = new ProductCategoryUpdateRequest
            {
                Id = _productCategoryId.Value,
                Name = txtName.Text,
                IsActive = chkIsActive.Checked
            };

            var response = await _apiService.Put<ProductCategoryResponse>($"ProductCategory/{_productCategoryId.Value}", request);

            if (response == null)
            {
                MessageBox.Show("Unable to save product category.");
                return;
            }
        }
        else
        {
            var request = new ProductCategoryInsertRequest
            {
                Name = txtName.Text,
                IsActive = chkIsActive.Checked
            };

            var response = await _apiService.Post<ProductCategoryResponse>("ProductCategory", request);

            if (response == null)
            {
                MessageBox.Show("Unable to save product category.");
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
}
