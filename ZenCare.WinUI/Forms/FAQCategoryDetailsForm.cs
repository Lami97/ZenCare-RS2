using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI.Forms;

public partial class FAQCategoryDetailsForm : Form
{
    private readonly APIService _apiService = new APIService();
    private readonly int? _faqCategoryId;

    public FAQCategoryDetailsForm()
    {
        InitializeComponent();
        chkIsActive.Checked = true;
    }

    public FAQCategoryDetailsForm(int id)
        : this()
    {
        _faqCategoryId = id;
    }

    private async void FAQCategoryDetailsForm_Load(object sender, EventArgs e)
    {
        if (_faqCategoryId.HasValue)
        {
            Text = "Edit FAQ category";
            await LoadFAQCategory();
        }
        else
        {
            Text = "New FAQ category";
        }
    }

    private async Task LoadFAQCategory()
    {
        var category = await _apiService.Get<FAQCategoryResponse>($"FAQCategory/{_faqCategoryId}");

        if (category == null)
        {
            MessageBox.Show(GetApiErrorMessage("FAQ category was not found."));
            DialogResult = DialogResult.Cancel;
            Close();
            return;
        }

        txtName.Text = category.Name;
        txtDescription.Text = category.Description;
        chkIsActive.Checked = category.IsActive;
    }

    private async void btnSave_Click(object sender, EventArgs e)
    {
        if (!ValidateInput())
        {
            return;
        }

        if (_faqCategoryId.HasValue)
        {
            await UpdateFAQCategory();
        }
        else
        {
            await InsertFAQCategory();
        }
    }

    private async Task InsertFAQCategory()
    {
        var request = new FAQCategoryInsertRequest
        {
            Name = txtName.Text.Trim(),
            Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text.Trim(),
            IsActive = chkIsActive.Checked
        };

        var response = await _apiService.Post<FAQCategoryResponse>("FAQCategory", request);

        if (response == null)
        {
            MessageBox.Show(GetApiErrorMessage("Unable to save FAQ category."));
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    private async Task UpdateFAQCategory()
    {
        var request = new FAQCategoryUpdateRequest
        {
            Id = _faqCategoryId!.Value,
            Name = txtName.Text.Trim(),
            Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text.Trim(),
            IsActive = chkIsActive.Checked
        };

        var response = await _apiService.Put<FAQCategoryResponse>($"FAQCategory/{_faqCategoryId.Value}", request);

        if (response == null)
        {
            MessageBox.Show(GetApiErrorMessage("Unable to save FAQ category."));
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    private bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(txtName.Text))
        {
            MessageBox.Show("Name is required.");
            txtName.Focus();
            return false;
        }

        if (txtName.Text.Length > 100)
        {
            MessageBox.Show("Name can contain up to 100 characters.");
            txtName.Focus();
            return false;
        }

        if (txtDescription.Text.Length > 500)
        {
            MessageBox.Show("Description can contain up to 500 characters.");
            txtDescription.Focus();
            return false;
        }

        return true;
    }

    private string GetApiErrorMessage(string fallback)
    {
        return string.IsNullOrWhiteSpace(_apiService.LastErrorMessage)
            ? fallback
            : _apiService.LastErrorMessage;
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}
