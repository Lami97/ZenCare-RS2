using ZenCare.Model.Enums;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI.Forms;

public partial class PurchaseDetailsForm : Form
{
    private readonly APIService _apiService = new APIService();
    private readonly int? _purchaseId;

    public PurchaseDetailsForm()
    {
        InitializeComponent();
    }

    public PurchaseDetailsForm(int purchaseId)
        : this()
    {
        _purchaseId = purchaseId;
    }

    private async void PurchaseDetailsForm_Load(object sender, EventArgs e)
    {
        LoadStatusLookups();
        await LoadLookups();

        if (_purchaseId.HasValue)
        {
            Text = "Edit purchase";
            await LoadPurchase();
        }
        else
        {
            Text = "New purchase";
            SelectPurchaseStatus(PurchaseStatus.Draft);
            SelectPaymentStatus(PaymentStatus.Pending);
            chkPaidAt.Checked = false;
        }
    }

    private async Task LoadLookups()
    {
        var users = await _apiService.Get<PagedResult<UserResponse>>("User");
        cmbUser.DataSource = PurchaseForm.CreateLookupItems(users?.Items.Select(x => new PurchaseForm.LookupItem(x.Id, PurchaseForm.GetUserDisplayName(x))), "Select");
    }

    private void LoadStatusLookups()
    {
        cmbStatus.DataSource = Enum.GetValues<PurchaseStatus>().Select(x => new PurchaseStatusLookupItem(x, x.ToString())).ToList();
        cmbPaymentStatus.DataSource = Enum.GetValues<PaymentStatus>().Select(x => new PaymentStatusLookupItem(x, x.ToString())).ToList();
    }

    private async Task LoadPurchase()
    {
        var purchase = await _apiService.Get<PurchaseResponse>($"Purchase/{_purchaseId}");

        if (purchase == null)
        {
            MessageBox.Show(GetApiErrorMessage("Purchase was not found."));
            DialogResult = DialogResult.Cancel;
            Close();
            return;
        }

        SelectLookupItem(cmbUser, purchase.UserId);
        txtPurchaseNumber.Text = purchase.PurchaseNumber;
        SelectPurchaseStatus(purchase.Status);
        SelectPaymentStatus(purchase.PaymentStatus);
        nudTotalAmount.Value = purchase.TotalAmount > nudTotalAmount.Maximum ? nudTotalAmount.Maximum : purchase.TotalAmount;
        txtStripePaymentIntentId.Text = purchase.StripePaymentIntentId;
        chkPaidAt.Checked = purchase.PaidAt.HasValue;
        dtpPaidAt.Value = purchase.PaidAt ?? DateTime.Today;
    }

    private async void btnSave_Click(object sender, EventArgs e)
    {
        if (!ValidateInput())
        {
            return;
        }

        if (_purchaseId.HasValue)
        {
            await UpdatePurchase();
        }
        else
        {
            await InsertPurchase();
        }
    }

    private async Task InsertPurchase()
    {
        var request = new PurchaseInsertRequest
        {
            UserId = GetSelectedLookupId(cmbUser),
            PurchaseNumber = txtPurchaseNumber.Text.Trim(),
            Status = GetSelectedPurchaseStatus(),
            PaymentStatus = GetSelectedPaymentStatus(),
            TotalAmount = nudTotalAmount.Value,
            StripePaymentIntentId = string.IsNullOrWhiteSpace(txtStripePaymentIntentId.Text) ? null : txtStripePaymentIntentId.Text.Trim(),
            PaidAt = chkPaidAt.Checked ? dtpPaidAt.Value : null
        };

        var response = await _apiService.Post<PurchaseResponse>("Purchase", request);

        if (response == null)
        {
            MessageBox.Show(GetApiErrorMessage("Unable to save purchase."));
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    private async Task UpdatePurchase()
    {
        var request = new PurchaseUpdateRequest
        {
            Id = _purchaseId!.Value,
            UserId = GetSelectedLookupId(cmbUser),
            PurchaseNumber = txtPurchaseNumber.Text.Trim(),
            Status = GetSelectedPurchaseStatus(),
            PaymentStatus = GetSelectedPaymentStatus(),
            TotalAmount = nudTotalAmount.Value,
            StripePaymentIntentId = string.IsNullOrWhiteSpace(txtStripePaymentIntentId.Text) ? null : txtStripePaymentIntentId.Text.Trim(),
            PaidAt = chkPaidAt.Checked ? dtpPaidAt.Value : null
        };

        var response = await _apiService.Put<PurchaseResponse>($"Purchase/{_purchaseId.Value}", request);

        if (response == null)
        {
            MessageBox.Show(GetApiErrorMessage("Unable to save purchase."));
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    private bool ValidateInput()
    {
        if (GetSelectedLookupId(cmbUser) <= 0)
        {
            MessageBox.Show("User is required.");
            cmbUser.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtPurchaseNumber.Text))
        {
            MessageBox.Show("Purchase number is required.");
            txtPurchaseNumber.Focus();
            return false;
        }

        if (txtPurchaseNumber.Text.Length > 30)
        {
            MessageBox.Show("Purchase number can contain up to 30 characters.");
            txtPurchaseNumber.Focus();
            return false;
        }

        if (txtStripePaymentIntentId.Text.Length > 150)
        {
            MessageBox.Show("Stripe payment intent can contain up to 150 characters.");
            txtStripePaymentIntentId.Focus();
            return false;
        }

        return true;
    }

    private static int GetSelectedLookupId(ComboBox comboBox)
    {
        return comboBox.SelectedItem is PurchaseForm.LookupItem item ? item.Id : 0;
    }

    private PurchaseStatus GetSelectedPurchaseStatus()
    {
        return cmbStatus.SelectedItem is PurchaseStatusLookupItem item ? item.Status : PurchaseStatus.Draft;
    }

    private PaymentStatus GetSelectedPaymentStatus()
    {
        return cmbPaymentStatus.SelectedItem is PaymentStatusLookupItem item ? item.Status : PaymentStatus.Pending;
    }

    private static void SelectLookupItem(ComboBox comboBox, int id)
    {
        for (var i = 0; i < comboBox.Items.Count; i++)
        {
            if (comboBox.Items[i] is PurchaseForm.LookupItem item && item.Id == id)
            {
                comboBox.SelectedIndex = i;
                return;
            }
        }
    }

    private void SelectPurchaseStatus(PurchaseStatus status)
    {
        for (var i = 0; i < cmbStatus.Items.Count; i++)
        {
            if (cmbStatus.Items[i] is PurchaseStatusLookupItem item && item.Status == status)
            {
                cmbStatus.SelectedIndex = i;
                return;
            }
        }
    }

    private void SelectPaymentStatus(PaymentStatus status)
    {
        for (var i = 0; i < cmbPaymentStatus.Items.Count; i++)
        {
            if (cmbPaymentStatus.Items[i] is PaymentStatusLookupItem item && item.Status == status)
            {
                cmbPaymentStatus.SelectedIndex = i;
                return;
            }
        }
    }

    private string GetApiErrorMessage(string fallback)
    {
        return string.IsNullOrWhiteSpace(_apiService.LastErrorMessage) ? fallback : _apiService.LastErrorMessage;
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private sealed record PurchaseStatusLookupItem(PurchaseStatus Status, string Name)
    {
        public override string ToString() => Name;
    }

    private sealed record PaymentStatusLookupItem(PaymentStatus Status, string Name)
    {
        public override string ToString() => Name;
    }
}
