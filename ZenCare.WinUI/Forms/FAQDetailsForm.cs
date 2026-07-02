using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI.Forms;

public partial class FAQDetailsForm : Form
{
    private readonly APIService _apiService = new APIService();
    private readonly int? _faqId;

    public FAQDetailsForm()
    {
        InitializeComponent();
        chkIsActive.Checked = true;
    }

    public FAQDetailsForm(int id)
        : this()
    {
        _faqId = id;
    }

    private async void FAQDetailsForm_Load(object sender, EventArgs e)
    {
        await LoadLookups();

        if (_faqId.HasValue)
        {
            Text = "Edit FAQ";
            await LoadFAQ();
        }
        else
        {
            Text = "New FAQ";
        }
    }

    private async Task LoadLookups()
    {
        var categories = await _apiService.Get<PagedResult<FAQCategoryResponse>>("FAQCategory");
        cmbFAQCategory.DataSource = FAQForm.CreateLookupItems(categories?.Items.Select(x => new FAQForm.LookupItem(x.Id, x.Name)), "Select");
    }

    private async Task LoadFAQ()
    {
        var faq = await _apiService.Get<FAQResponse>($"FAQ/{_faqId}");

        if (faq == null)
        {
            MessageBox.Show(GetApiErrorMessage("FAQ was not found."));
            DialogResult = DialogResult.Cancel;
            Close();
            return;
        }

        txtQuestion.Text = faq.Question;
        txtAnswer.Text = faq.Answer;
        SelectLookupItem(cmbFAQCategory, faq.FAQCategoryId);
        nudDisplayOrder.Value = faq.DisplayOrder < nudDisplayOrder.Minimum
            ? nudDisplayOrder.Minimum
            : faq.DisplayOrder > nudDisplayOrder.Maximum
                ? nudDisplayOrder.Maximum
                : faq.DisplayOrder;
        chkIsActive.Checked = faq.IsActive;
    }

    private async void btnSave_Click(object sender, EventArgs e)
    {
        if (!ValidateInput())
        {
            return;
        }

        if (_faqId.HasValue)
        {
            await UpdateFAQ();
        }
        else
        {
            await InsertFAQ();
        }
    }

    private async Task InsertFAQ()
    {
        var request = new FAQInsertRequest
        {
            Question = txtQuestion.Text.Trim(),
            Answer = txtAnswer.Text.Trim(),
            FAQCategoryId = GetSelectedLookupId(cmbFAQCategory),
            DisplayOrder = (int)nudDisplayOrder.Value,
            IsActive = chkIsActive.Checked
        };

        var response = await _apiService.Post<FAQResponse>("FAQ", request);

        if (response == null)
        {
            MessageBox.Show(GetApiErrorMessage("Unable to save FAQ."));
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    private async Task UpdateFAQ()
    {
        var request = new FAQUpdateRequest
        {
            Id = _faqId!.Value,
            Question = txtQuestion.Text.Trim(),
            Answer = txtAnswer.Text.Trim(),
            FAQCategoryId = GetSelectedLookupId(cmbFAQCategory),
            DisplayOrder = (int)nudDisplayOrder.Value,
            IsActive = chkIsActive.Checked
        };

        var response = await _apiService.Put<FAQResponse>($"FAQ/{_faqId.Value}", request);

        if (response == null)
        {
            MessageBox.Show(GetApiErrorMessage("Unable to save FAQ."));
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    private bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(txtQuestion.Text))
        {
            MessageBox.Show("Question is required.");
            txtQuestion.Focus();
            return false;
        }

        if (txtQuestion.Text.Length > 250)
        {
            MessageBox.Show("Question can contain up to 250 characters.");
            txtQuestion.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtAnswer.Text))
        {
            MessageBox.Show("Answer is required.");
            txtAnswer.Focus();
            return false;
        }

        if (txtAnswer.Text.Length > 2000)
        {
            MessageBox.Show("Answer can contain up to 2000 characters.");
            txtAnswer.Focus();
            return false;
        }

        if (GetSelectedLookupId(cmbFAQCategory) <= 0)
        {
            MessageBox.Show("Category is required.");
            cmbFAQCategory.Focus();
            return false;
        }

        return true;
    }

    private static int GetSelectedLookupId(ComboBox comboBox)
    {
        return comboBox.SelectedItem is FAQForm.LookupItem item ? item.Id : 0;
    }

    private static void SelectLookupItem(ComboBox comboBox, int id)
    {
        for (var i = 0; i < comboBox.Items.Count; i++)
        {
            if (comboBox.Items[i] is FAQForm.LookupItem item && item.Id == id)
            {
                comboBox.SelectedIndex = i;
                return;
            }
        }
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
