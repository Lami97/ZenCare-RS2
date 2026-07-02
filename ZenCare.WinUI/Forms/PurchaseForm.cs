using ZenCare.Model.Enums;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI.Forms;

public partial class PurchaseForm : Form
{
    private readonly APIService _apiService = new APIService();

    public PurchaseForm()
    {
        InitializeComponent();
    }

    private async void PurchaseForm_Load(object sender, EventArgs e)
    {
        LoadStatusLookups();
        await LoadLookups();
        await LoadPurchases();
    }

    private async void btnSearch_Click(object sender, EventArgs e)
    {
        await LoadPurchases();
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
        OpenDetailsForm(new PurchaseDetailsForm());
    }

    private void btnEdit_Click(object sender, EventArgs e)
    {
        var selectedId = GetSelectedPurchaseId();

        if (selectedId == null)
        {
            MessageBox.Show("Please select a purchase.");
            return;
        }

        OpenDetailsForm(new PurchaseDetailsForm(selectedId.Value));
    }

    private async void btnDelete_Click(object sender, EventArgs e)
    {
        var selectedId = GetSelectedPurchaseId();

        if (selectedId == null)
        {
            MessageBox.Show("Please select a purchase.");
            return;
        }

        var result = MessageBox.Show("Are you sure you want to delete this purchase?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (result != DialogResult.Yes)
        {
            return;
        }

        var deleted = await _apiService.Delete($"Purchase/{selectedId.Value}");

        if (!deleted)
        {
            MessageBox.Show(GetApiErrorMessage("Unable to delete purchase."));
            return;
        }

        await LoadPurchases();
    }

    private async void btnRefresh_Click(object sender, EventArgs e)
    {
        SelectLookupItem(cmbUser, 0);
        txtPurchaseNumber.Clear();
        cmbStatus.SelectedIndex = 0;
        cmbPaymentStatus.SelectedIndex = 0;
        await LoadPurchases();
    }

    private async Task LoadLookups()
    {
        var users = await _apiService.Get<PagedResult<UserResponse>>("User");
        cmbUser.DataSource = CreateLookupItems(users?.Items.Select(x => new LookupItem(x.Id, GetUserDisplayName(x))), "All");
    }

    private void LoadStatusLookups()
    {
        var purchaseStatuses = new List<PurchaseStatusLookupItem> { new PurchaseStatusLookupItem(null, "All") };
        purchaseStatuses.AddRange(Enum.GetValues<PurchaseStatus>().Select(x => new PurchaseStatusLookupItem(x, x.ToString())));
        cmbStatus.DataSource = purchaseStatuses;

        var paymentStatuses = new List<PaymentStatusLookupItem> { new PaymentStatusLookupItem(null, "All") };
        paymentStatuses.AddRange(Enum.GetValues<PaymentStatus>().Select(x => new PaymentStatusLookupItem(x, x.ToString())));
        cmbPaymentStatus.DataSource = paymentStatuses;
    }

    private async Task LoadPurchases()
    {
        var search = new PurchaseSearchObject
        {
            UserId = GetSelectedLookupId(cmbUser),
            PurchaseNumber = string.IsNullOrWhiteSpace(txtPurchaseNumber.Text) ? null : txtPurchaseNumber.Text,
            Status = GetSelectedPurchaseStatus(),
            PaymentStatus = GetSelectedPaymentStatus()
        };

        var query = new List<string>();

        if (search.UserId.HasValue)
        {
            query.Add($"UserId={search.UserId.Value}");
        }

        if (!string.IsNullOrWhiteSpace(search.PurchaseNumber))
        {
            query.Add($"PurchaseNumber={Uri.EscapeDataString(search.PurchaseNumber)}");
        }

        if (search.Status.HasValue)
        {
            query.Add($"Status={(int)search.Status.Value}");
        }

        if (search.PaymentStatus.HasValue)
        {
            query.Add($"PaymentStatus={(int)search.PaymentStatus.Value}");
        }

        var endpoint = query.Count == 0 ? "Purchase" : $"Purchase?{string.Join("&", query)}";
        var result = await _apiService.Get<PagedResult<PurchaseResponse>>(endpoint);

        if (result == null)
        {
            MessageBox.Show(GetApiErrorMessage("Unable to load purchases."));
            dgvPurchases.DataSource = new List<PurchaseResponse>();
            return;
        }

        dgvPurchases.DataSource = result.Items ?? new List<PurchaseResponse>();
    }

    private int? GetSelectedPurchaseId()
    {
        return dgvPurchases.CurrentRow?.DataBoundItem is PurchaseResponse purchase ? purchase.Id : null;
    }

    private static int? GetSelectedLookupId(ComboBox comboBox)
    {
        return comboBox.SelectedItem is LookupItem { Id: > 0 } item ? item.Id : null;
    }

    private PurchaseStatus? GetSelectedPurchaseStatus()
    {
        return cmbStatus.SelectedItem is PurchaseStatusLookupItem item ? item.Status : null;
    }

    private PaymentStatus? GetSelectedPaymentStatus()
    {
        return cmbPaymentStatus.SelectedItem is PaymentStatusLookupItem item ? item.Status : null;
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

    internal static List<LookupItem> CreateLookupItems(IEnumerable<LookupItem>? items, string emptyText)
    {
        var lookupItems = new List<LookupItem> { new LookupItem(0, emptyText) };
        lookupItems.AddRange(items ?? Enumerable.Empty<LookupItem>());
        return lookupItems;
    }

    internal static string GetUserDisplayName(UserResponse user)
    {
        var fullName = $"{user.FirstName} {user.LastName}".Trim();
        return string.IsNullOrWhiteSpace(fullName) ? user.Username : $"{user.Username} - {fullName}";
    }

    private string GetApiErrorMessage(string fallback)
    {
        return string.IsNullOrWhiteSpace(_apiService.LastErrorMessage) ? fallback : _apiService.LastErrorMessage;
    }

    private async void OpenDetailsForm(PurchaseDetailsForm form)
    {
        if (form.ShowDialog() == DialogResult.OK)
        {
            await LoadPurchases();
        }
    }

    internal sealed record LookupItem(int Id, string Name)
    {
        public override string ToString() => Name;
    }

    private sealed record PurchaseStatusLookupItem(PurchaseStatus? Status, string Name)
    {
        public override string ToString() => Name;
    }

    private sealed record PaymentStatusLookupItem(PaymentStatus? Status, string Name)
    {
        public override string ToString() => Name;
    }
}
