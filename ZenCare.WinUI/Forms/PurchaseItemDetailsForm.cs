using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI.Forms;

public partial class PurchaseItemDetailsForm : Form
{
    private readonly APIService _apiService = new APIService();
    private readonly int? _purchaseItemId;
    private bool _isLoading;

    public PurchaseItemDetailsForm()
    {
        InitializeComponent();
    }

    public PurchaseItemDetailsForm(int purchaseItemId) : this()
    {
        _purchaseItemId = purchaseItemId;
    }

    private async void PurchaseItemDetailsForm_Load(object sender, EventArgs e)
    {
        _isLoading = true;
        await LoadLookups();

        if (_purchaseItemId.HasValue)
        {
            Text = "Edit Purchase Item";
            await LoadPurchaseItem();
        }

        _isLoading = false;
        UpdateTotalPrice();
    }

    private async Task LoadLookups()
    {
        var purchases = await _apiService.Get<PagedResult<PurchaseResponse>>("Purchase");
        cmbPurchase.DataSource = PurchaseItemForm.CreateLookupItems(
            purchases?.Items.Select(x => new PurchaseItemForm.LookupItem(x.Id, x.PurchaseNumber)),
            "Select purchase");
        cmbPurchase.DisplayMember = "Name";
        cmbPurchase.ValueMember = "Id";

        var products = await _apiService.Get<PagedResult<ProductResponse>>("Product");
        cmbProduct.DataSource = PurchaseItemForm.CreateLookupItems(
            products?.Items.Select(x => new PurchaseItemForm.LookupItem(x.Id, x.Name)),
            "Select product");
        cmbProduct.DisplayMember = "Name";
        cmbProduct.ValueMember = "Id";
    }

    private async Task LoadPurchaseItem()
    {
        var purchaseItem = await _apiService.Get<PurchaseItemResponse>($"PurchaseItem/{_purchaseItemId}");

        if (purchaseItem == null)
        {
            MessageBox.Show("Purchase item could not be loaded.");
            DialogResult = DialogResult.Cancel;
            Close();
            return;
        }

        cmbPurchase.SelectedValue = purchaseItem.PurchaseId;
        cmbProduct.SelectedValue = purchaseItem.ProductId;
        nudQuantity.Value = purchaseItem.Quantity;
        nudUnitPrice.Value = purchaseItem.UnitPrice;
        nudTotalPrice.Value = purchaseItem.TotalPrice;
    }

    private async void btnSave_Click(object sender, EventArgs e)
    {
        if (!ValidateInput())
        {
            return;
        }

        var request = new PurchaseItemInsertRequest
        {
            PurchaseId = Convert.ToInt32(cmbPurchase.SelectedValue),
            ProductId = Convert.ToInt32(cmbProduct.SelectedValue),
            Quantity = (int)nudQuantity.Value,
            UnitPrice = nudUnitPrice.Value,
            TotalPrice = nudTotalPrice.Value
        };

        if (_purchaseItemId.HasValue)
        {
            var updateRequest = new PurchaseItemUpdateRequest
            {
                Id = _purchaseItemId.Value,
                PurchaseId = request.PurchaseId,
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                UnitPrice = request.UnitPrice,
                TotalPrice = request.TotalPrice
            };

            var response = await _apiService.Put<PurchaseItemResponse>($"PurchaseItem/{_purchaseItemId.Value}", updateRequest);

            if (response == null)
            {
                MessageBox.Show(GetApiErrorMessage("Unable to save purchase item."));
                return;
            }
        }
        else
        {
            var response = await _apiService.Post<PurchaseItemResponse>("PurchaseItem", request);

            if (response == null)
            {
                MessageBox.Show(GetApiErrorMessage("Unable to save purchase item."));
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

    private void nudQuantity_ValueChanged(object sender, EventArgs e)
    {
        if (!_isLoading)
        {
            UpdateTotalPrice();
        }
    }

    private void nudUnitPrice_ValueChanged(object sender, EventArgs e)
    {
        if (!_isLoading)
        {
            UpdateTotalPrice();
        }
    }

    private void UpdateTotalPrice()
    {
        nudTotalPrice.Value = nudQuantity.Value * nudUnitPrice.Value;
    }

    private bool ValidateInput()
    {
        if (cmbPurchase.SelectedValue == null || Convert.ToInt32(cmbPurchase.SelectedValue) <= 0)
        {
            MessageBox.Show("Purchase is required.");
            return false;
        }

        if (cmbProduct.SelectedValue == null || Convert.ToInt32(cmbProduct.SelectedValue) <= 0)
        {
            MessageBox.Show("Product is required.");
            return false;
        }

        if (nudQuantity.Value <= 0)
        {
            MessageBox.Show("Quantity must be greater than 0.");
            return false;
        }

        if (nudUnitPrice.Value < 0)
        {
            MessageBox.Show("Unit price cannot be negative.");
            return false;
        }

        if (nudTotalPrice.Value < 0)
        {
            MessageBox.Show("Total price cannot be negative.");
            return false;
        }

        return true;
    }

    private string GetApiErrorMessage(string fallback)
    {
        return string.IsNullOrWhiteSpace(_apiService.LastErrorMessage) ? fallback : _apiService.LastErrorMessage;
    }
}
