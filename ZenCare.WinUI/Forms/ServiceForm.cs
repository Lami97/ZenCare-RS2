using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI.Forms;

public partial class ServiceForm : Form
{
    private readonly APIService _apiService = new APIService();

    public ServiceForm()
    {
        InitializeComponent();
    }

    private async void ServiceForm_Load(object sender, EventArgs e)
    {
        await LoadLookups();
        await LoadServices();
    }

    private async void btnSearch_Click(object sender, EventArgs e)
    {
        await LoadServices();
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
        OpenDetailsForm(new ServiceDetailsForm());
    }

    private void btnEdit_Click(object sender, EventArgs e)
    {
        var selectedId = GetSelectedServiceId();

        if (selectedId == null)
        {
            MessageBox.Show("Please select a service.");
            return;
        }

        OpenDetailsForm(new ServiceDetailsForm(selectedId.Value));
    }

    private async void btnDelete_Click(object sender, EventArgs e)
    {
        var selectedId = GetSelectedServiceId();

        if (selectedId == null)
        {
            MessageBox.Show("Please select a service.");
            return;
        }

        var result = MessageBox.Show(
            "Are you sure you want to delete this service?",
            "Confirm delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result != DialogResult.Yes)
        {
            return;
        }

        await _apiService.Delete($"Service/{selectedId.Value}");
        await LoadServices();
    }

    private async void btnRefresh_Click(object sender, EventArgs e)
    {
        txtName.Clear();
        cmbServiceCategory.SelectedIndex = 0;
        chkIsActive.CheckState = CheckState.Indeterminate;
        await LoadServices();
    }

    private async Task LoadLookups()
    {
        var categories = await _apiService.Get<PagedResult<ServiceCategoryResponse>>("ServiceCategory");
        cmbServiceCategory.DataSource = CreateLookupItems(categories?.Items.Select(x => new LookupItem(x.Id, x.Name)), "All");
    }

    private async Task LoadServices()
    {
        var search = new ServiceSearchObject
        {
            Name = string.IsNullOrWhiteSpace(txtName.Text) ? null : txtName.Text,
            ServiceCategoryId = GetSelectedLookupId(cmbServiceCategory),
            IsActive = chkIsActive.CheckState == CheckState.Indeterminate ? null : chkIsActive.Checked
        };

        var query = new List<string>();

        if (!string.IsNullOrWhiteSpace(search.Name))
        {
            query.Add($"Name={Uri.EscapeDataString(search.Name)}");
        }

        if (search.ServiceCategoryId.HasValue)
        {
            query.Add($"ServiceCategoryId={search.ServiceCategoryId.Value}");
        }

        if (search.IsActive.HasValue)
        {
            query.Add($"IsActive={search.IsActive.Value}");
        }

        var endpoint = query.Count == 0 ? "Service" : $"Service?{string.Join("&", query)}";
        var result = await _apiService.Get<PagedResult<ServiceResponse>>(endpoint);
        dgvServices.DataSource = result?.Items ?? new List<ServiceResponse>();
    }

    private int? GetSelectedServiceId()
    {
        return dgvServices.CurrentRow?.DataBoundItem is ServiceResponse service
            ? service.Id
            : null;
    }

    private static int? GetSelectedLookupId(ComboBox comboBox)
    {
        return comboBox.SelectedItem is LookupItem { Id: > 0 } item ? item.Id : null;
    }

    internal static List<LookupItem> CreateLookupItems(IEnumerable<LookupItem>? items, string emptyText)
    {
        var lookupItems = new List<LookupItem> { new LookupItem(0, emptyText) };
        lookupItems.AddRange(items ?? Enumerable.Empty<LookupItem>());
        return lookupItems;
    }

    private async void OpenDetailsForm(ServiceDetailsForm form)
    {
        if (form.ShowDialog() == DialogResult.OK)
        {
            await LoadServices();
        }
    }

    internal sealed record LookupItem(int Id, string Name)
    {
        public override string ToString()
        {
            return Name;
        }
    }
}
