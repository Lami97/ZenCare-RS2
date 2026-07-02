using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI.Forms;

public partial class UnitOfMeasureDetailsForm : Form
{
    private readonly APIService _apiService = new APIService();
    private readonly int? _unitOfMeasureId;

    public UnitOfMeasureDetailsForm()
    {
        InitializeComponent();
        chkIsActive.Checked = true;
    }

    public UnitOfMeasureDetailsForm(int id)
        : this()
    {
        _unitOfMeasureId = id;
    }

    private async void UnitOfMeasureDetailsForm_Load(object sender, EventArgs e)
    {
        if (_unitOfMeasureId.HasValue)
        {
            await LoadUnit();
        }
    }

    private async Task LoadUnit()
    {
        var unit = await _apiService.Get<UnitOfMeasureResponse>($"UnitOfMeasure/{_unitOfMeasureId}");

        if (unit == null)
        {
            MessageBox.Show("Unit was not found.");
            DialogResult = DialogResult.Cancel;
            Close();
            return;
        }

        txtName.Text = unit.Name;
        txtDescription.Text = unit.Description;
        chkIsActive.Checked = unit.IsActive;
    }

    private async void btnSave_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtName.Text))
        {
            MessageBox.Show("Name is required.");
            return;
        }

        if (_unitOfMeasureId.HasValue)
        {
            var request = new UnitOfMeasureUpdateRequest
            {
                Id = _unitOfMeasureId.Value,
                Name = txtName.Text,
                Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text,
                IsActive = chkIsActive.Checked
            };

            var response = await _apiService.Put<UnitOfMeasureResponse>($"UnitOfMeasure/{_unitOfMeasureId.Value}", request);

            if (response == null)
            {
                MessageBox.Show("Unable to save unit.");
                return;
            }
        }
        else
        {
            var request = new UnitOfMeasureInsertRequest
            {
                Name = txtName.Text,
                Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text,
                IsActive = chkIsActive.Checked
            };

            var response = await _apiService.Post<UnitOfMeasureResponse>("UnitOfMeasure", request);

            if (response == null)
            {
                MessageBox.Show("Unable to save unit.");
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
