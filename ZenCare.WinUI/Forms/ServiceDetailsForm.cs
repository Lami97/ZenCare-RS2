using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI.Forms;

public partial class ServiceDetailsForm : Form
{
    private readonly APIService _apiService = new APIService();
    private readonly int? _serviceId;

    public ServiceDetailsForm()
    {
        InitializeComponent();
        chkIsActive.Checked = true;
    }

    public ServiceDetailsForm(int id)
        : this()
    {
        _serviceId = id;
    }

    private async void ServiceDetailsForm_Load(object sender, EventArgs e)
    {
        await LoadLookups();

        if (_serviceId.HasValue)
        {
            await LoadService();
        }
    }

    private async Task LoadLookups()
    {
        var categories = await _apiService.Get<PagedResult<ServiceCategoryResponse>>("ServiceCategory");
        cmbServiceCategory.DataSource = CreateLookupItems(categories?.Items.Select(x => new LookupItem(x.Id, x.Name)), "Select");
    }

    private async Task LoadService()
    {
        var service = await _apiService.Get<ServiceResponse>($"Service/{_serviceId}");

        if (service == null)
        {
            MessageBox.Show("Service was not found.");
            DialogResult = DialogResult.Cancel;
            Close();
            return;
        }

        txtName.Text = service.Name;
        txtDescription.Text = service.Description;
        nudDurationMinutes.Value = service.DurationMinutes > nudDurationMinutes.Maximum ? nudDurationMinutes.Maximum : service.DurationMinutes;
        nudPrice.Value = service.Price > nudPrice.Maximum ? nudPrice.Maximum : service.Price;
        SelectLookupItem(cmbServiceCategory, service.ServiceCategoryId);
        chkIsActive.Checked = service.IsActive;
    }

    private async void btnSave_Click(object sender, EventArgs e)
    {
        if (!ValidateInput())
        {
            return;
        }

        if (_serviceId.HasValue)
        {
            var request = new ServiceUpdateRequest
            {
                Id = _serviceId.Value,
                Name = txtName.Text,
                Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text,
                DurationMinutes = (int)nudDurationMinutes.Value,
                Price = nudPrice.Value,
                ServiceCategoryId = GetSelectedLookupId(cmbServiceCategory),
                IsActive = chkIsActive.Checked
            };

            var response = await _apiService.Put<ServiceResponse>($"Service/{_serviceId.Value}", request);

            if (response == null)
            {
                MessageBox.Show("Unable to save service.");
                return;
            }
        }
        else
        {
            var request = new ServiceInsertRequest
            {
                Name = txtName.Text,
                Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text,
                DurationMinutes = (int)nudDurationMinutes.Value,
                Price = nudPrice.Value,
                ServiceCategoryId = GetSelectedLookupId(cmbServiceCategory),
                IsActive = chkIsActive.Checked
            };

            var response = await _apiService.Post<ServiceResponse>("Service", request);

            if (response == null)
            {
                MessageBox.Show("Unable to save service.");
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

        if (nudDurationMinutes.Value < 1)
        {
            MessageBox.Show("Duration must be at least 1 minute.");
            return false;
        }

        if (nudPrice.Value < 0.01m)
        {
            MessageBox.Show("Price must be greater than 0.");
            return false;
        }

        if (GetSelectedLookupId(cmbServiceCategory) <= 0)
        {
            MessageBox.Show("Service category is required.");
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

    private static List<LookupItem> CreateLookupItems(IEnumerable<LookupItem>? items, string emptyText)
    {
        var lookupItems = new List<LookupItem> { new LookupItem(0, emptyText) };
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
