using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI.Forms;

public partial class ServiceCategoryDetailsForm : Form
{
    private readonly APIService _apiService = new APIService();
    private readonly int? _serviceCategoryId;

    public ServiceCategoryDetailsForm()
    {
        InitializeComponent();
        chkIsActive.Checked = true;
    }

    public ServiceCategoryDetailsForm(int id)
        : this()
    {
        _serviceCategoryId = id;
    }

    private async void ServiceCategoryDetailsForm_Load(object sender, EventArgs e)
    {
        if (_serviceCategoryId.HasValue)
        {
            await LoadServiceCategory();
        }
    }

    private async Task LoadServiceCategory()
    {
        var serviceCategory = await _apiService.Get<ServiceCategoryResponse>($"ServiceCategory/{_serviceCategoryId}");

        if (serviceCategory == null)
        {
            MessageBox.Show("Service category was not found.");
            DialogResult = DialogResult.Cancel;
            Close();
            return;
        }

        txtName.Text = serviceCategory.Name;
        txtDescription.Text = serviceCategory.Description;
        chkIsActive.Checked = serviceCategory.IsActive;
    }

    private async void btnSave_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtName.Text))
        {
            MessageBox.Show("Name is required.");
            return;
        }

        if (_serviceCategoryId.HasValue)
        {
            var request = new ServiceCategoryUpdateRequest
            {
                Id = _serviceCategoryId.Value,
                Name = txtName.Text,
                Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text,
                IsActive = chkIsActive.Checked
            };

            var response = await _apiService.Put<ServiceCategoryResponse>($"ServiceCategory/{_serviceCategoryId.Value}", request);

            if (response == null)
            {
                MessageBox.Show("Unable to save service category.");
                return;
            }
        }
        else
        {
            var request = new ServiceCategoryInsertRequest
            {
                Name = txtName.Text,
                Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text,
                IsActive = chkIsActive.Checked
            };

            var response = await _apiService.Post<ServiceCategoryResponse>("ServiceCategory", request);

            if (response == null)
            {
                MessageBox.Show("Unable to save service category.");
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
