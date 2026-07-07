using System.Text.RegularExpressions;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI.Forms;

public partial class SupplierDetailsForm : Form
{
    private readonly APIService _apiService = new APIService();
    private readonly int? _supplierId;

    public SupplierDetailsForm()
    {
        InitializeComponent();
        chkIsActive.Checked = true;
    }

    public SupplierDetailsForm(int supplierId)
        : this()
    {
        _supplierId = supplierId;
    }

    private async void SupplierDetailsForm_Load(object sender, EventArgs e)
    {
        if (_supplierId.HasValue)
        {
            Text = "Edit Supplier";
            await LoadSupplier();
        }
    }

    private async Task LoadSupplier()
    {
        var supplier = await _apiService.Get<SupplierResponse>($"Supplier/{_supplierId}");

        if (supplier == null)
        {
            MessageBox.Show(GetApiErrorMessage("Supplier was not found."));
            DialogResult = DialogResult.Cancel;
            Close();
            return;
        }

        txtName.Text = supplier.Name;
        txtContactEmail.Text = supplier.ContactEmail;
        txtPhoneNumber.Text = supplier.PhoneNumber;
        txtAddress.Text = supplier.Address;
        chkIsActive.Checked = supplier.IsActive;
    }

    private async void btnSave_Click(object sender, EventArgs e)
    {
        if (!ValidateInput())
        {
            return;
        }

        if (_supplierId.HasValue)
        {
            var request = new SupplierUpdateRequest
            {
                Id = _supplierId.Value,
                Name = txtName.Text.Trim(),
                ContactEmail = GetNullableText(txtContactEmail),
                PhoneNumber = GetNullableText(txtPhoneNumber),
                Address = GetNullableText(txtAddress),
                IsActive = chkIsActive.Checked
            };

            var response = await _apiService.Put<SupplierResponse>($"Supplier/{_supplierId.Value}", request);

            if (response == null)
            {
                MessageBox.Show(GetApiErrorMessage("Unable to save supplier."));
                return;
            }
        }
        else
        {
            var request = new SupplierInsertRequest
            {
                Name = txtName.Text.Trim(),
                ContactEmail = GetNullableText(txtContactEmail),
                PhoneNumber = GetNullableText(txtPhoneNumber),
                Address = GetNullableText(txtAddress),
                IsActive = chkIsActive.Checked
            };

            var response = await _apiService.Post<SupplierResponse>("Supplier", request);

            if (response == null)
            {
                MessageBox.Show(GetApiErrorMessage("Unable to save supplier."));
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
            txtName.Focus();
            return false;
        }

        if (txtName.Text.Length > 100)
        {
            MessageBox.Show("Name can contain up to 100 characters.");
            txtName.Focus();
            return false;
        }

        if (!string.IsNullOrWhiteSpace(txtContactEmail.Text))
        {
            if (txtContactEmail.Text.Length > 150)
            {
                MessageBox.Show("Email can contain up to 150 characters.");
                txtContactEmail.Focus();
                return false;
            }

            if (!Regex.IsMatch(txtContactEmail.Text.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Please enter a valid email address.");
                txtContactEmail.Focus();
                return false;
            }
        }

        if (txtPhoneNumber.Text.Length > 30)
        {
            MessageBox.Show("Phone number can contain up to 30 characters.");
            txtPhoneNumber.Focus();
            return false;
        }

        if (txtAddress.Text.Length > 500)
        {
            MessageBox.Show("Address can contain up to 500 characters.");
            txtAddress.Focus();
            return false;
        }

        return true;
    }

    private static string? GetNullableText(TextBox textBox)
    {
        return string.IsNullOrWhiteSpace(textBox.Text) ? null : textBox.Text.Trim();
    }

    private string GetApiErrorMessage(string fallback)
    {
        return string.IsNullOrWhiteSpace(_apiService.LastErrorMessage) ? fallback : _apiService.LastErrorMessage;
    }
}
