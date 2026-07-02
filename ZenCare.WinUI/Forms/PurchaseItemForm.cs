using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI.Forms;

public partial class PurchaseItemForm : Form
{
    private readonly APIService _apiService = new APIService();

    public PurchaseItemForm()
    {
        InitializeComponent();
    }

    private async void PurchaseItemForm_Load(object sender, EventArgs e)
    {
        await LoadLookups();
        await LoadPurchaseItems();
    }

    private async void btnSearch_Click(object sender, EventArgs e)
    {
        await LoadPurchaseItems();
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
        OpenDetailsForm(new PurchaseItemDetailsForm());
    }

    private void btnEdit_Click(object sender, EventArgs e)
    {
        var selectedId = GetSelectedPurchaseItemId();

        if (selectedId == null)
        {
            MessageBox.Show("Please select a purchase item.");
            return;
        }

        OpenDetailsForm(new PurchaseItemDetailsForm(selectedId.Value));
    }

    private async void btnDelete_Click(object sender, EventArgs e)
    {
        var selectedId = GetSelectedPurchaseItemId();

        if (selectedId == null)
        {
            MessageBox.Show("Please select a purchase item.");
            return;
        }

        var result = MessageBox.Show("Are you sure you want to delete this purchase item?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (result != DialogResult.Yes)
        {
            return;
        }

        var deleted = await _apiService.Delete($"PurchaseItem/{selectedId.Value}");

        if (!deleted)
        {
            MessageBox.Show(GetApiErrorMessage("Unable to delete purchase item."));
            return;
        }

        await LoadPurchaseItems();
    }

    private async void btnRefresh_Click(object sender, EventArgs e)
    {
        SelectLookupItem(cmbPurchase, 0);
        SelectLookupItem(cmbProduct, 0);
        await LoadPurchaseItems();
    }

    private async Task LoadLookups()
    {
        var purchases = await _apiService.Get<PagedResult<PurchaseResponse>>("Purchase");
        cmbPurchase.DataSource = CreateLookupItems(purchases?.Items.Select(x => new LookupItem(x.Id, x.PurchaseNumber)), "All");

        var products = await _apiService.Get<PagedResult<ProductResponse>>("Product");
        cmbProduct.DataSource = CreateLookupItems(products?.Items.Select(x => new LookupItem(x.Id, x.Name)), "All");
    }

    private async Task LoadPurchaseItems()
    {
        var search = new PurchaseItemSearchObject
        {
            PurchaseId = GetSelectedLookupId(cmbPurchase),
            ProductId = GetSelectedLookupId(cmbProduct)
        };

        var query = new List<string>();

        if (search.PurchaseId.HasValue)
        {
            query.Add($"PurchaseId={search.PurchaseId.Value}");
        }

        if (search.ProductId.HasValue)
        {
            query.Add($"ProductId={search.ProductId.Value}");
        }

        var endpoint = query.Count == 0 ? "PurchaseItem" : $"PurchaseItem?{string.Join("&", query)}";
        var result = await _apiService.Get<PagedResult<PurchaseItemResponse>>(endpoint);

        if (result == null)
        {
            MessageBox.Show(GetApiErrorMessage("Unable to load purchase items."));
            dgvPurchaseItems.DataSource = new List<PurchaseItemResponse>();
            return;
        }

        dgvPurchaseItems.DataSource = result.Items ?? new List<PurchaseItemResponse>();
    }

    private int? GetSelectedPurchaseItemId()
    {
        return dgvPurchaseItems.CurrentRow?.DataBoundItem is PurchaseItemResponse item ? item.Id : null;
    }

    private static int? GetSelectedLookupId(ComboBox comboBox)
    {
        return comboBox.SelectedItem is LookupItem { Id: > 0 } item ? item.Id : null;
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

    private string GetApiErrorMessage(string fallback)
    {
        return string.IsNullOrWhiteSpace(_apiService.LastErrorMessage) ? fallback : _apiService.LastErrorMessage;
    }

    private async void OpenDetailsForm(PurchaseItemDetailsForm form)
    {
        if (form.ShowDialog() == DialogResult.OK)
        {
            await LoadPurchaseItems();
        }
    }

    internal sealed record LookupItem(int Id, string Name)
    {
        public override string ToString() => Name;
    }
}
