using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI.Forms;

public partial class ProductTypeDetailsForm : Form
{
    private readonly APIService _apiService = new APIService();
    private readonly int? _productTypeId;

    public ProductTypeDetailsForm()
    {
        InitializeComponent();
        chkIsActive.Checked = true;
    }

    public ProductTypeDetailsForm(int id)
        : this()
    {
        _productTypeId = id;
    }

    private async void ProductTypeDetailsForm_Load(object sender, EventArgs e)
    {
        if (_productTypeId.HasValue)
        {
            await LoadProductType();
        }
    }

    private async Task LoadProductType()
    {
        var productType = await _apiService.Get<ProductTypeResponse>($"ProductType/{_productTypeId}");

        if (productType == null)
        {
            MessageBox.Show("Product type was not found.");
            DialogResult = DialogResult.Cancel;
            Close();
            return;
        }

        txtName.Text = productType.Name;
        chkIsActive.Checked = productType.IsActive;
    }

    private async void btnSave_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtName.Text))
        {
            MessageBox.Show("Name is required.");
            return;
        }

        if (_productTypeId.HasValue)
        {
            var request = new ProductTypeUpdateRequest
            {
                Id = _productTypeId.Value,
                Name = txtName.Text,
                IsActive = chkIsActive.Checked
            };

            var response = await _apiService.Put<ProductTypeResponse>($"ProductType/{_productTypeId.Value}", request);

            if (response == null)
            {
                MessageBox.Show("Unable to save product type.");
                return;
            }
        }
        else
        {
            var request = new ProductTypeInsertRequest
            {
                Name = txtName.Text,
                IsActive = chkIsActive.Checked
            };

            var response = await _apiService.Post<ProductTypeResponse>("ProductType", request);

            if (response == null)
            {
                MessageBox.Show("Unable to save product type.");
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
