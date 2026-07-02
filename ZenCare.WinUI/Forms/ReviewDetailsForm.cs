using ZenCare.Model.Enums;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI.Forms;

public partial class ReviewDetailsForm : Form
{
    private readonly APIService _apiService = new APIService();
    private readonly int? _reviewId;

    public ReviewDetailsForm()
    {
        InitializeComponent();
    }

    public ReviewDetailsForm(int reviewId)
        : this()
    {
        _reviewId = reviewId;
    }

    private async void ReviewDetailsForm_Load(object sender, EventArgs e)
    {
        LoadStatusLookup();
        await LoadLookups();

        if (_reviewId.HasValue)
        {
            Text = "Edit review";
            await LoadReview();
        }
        else
        {
            Text = "New review";
            SelectStatus(ReviewStatus.PendingApproval);
            nudRating.Value = 1;
        }
    }

    private async Task LoadLookups()
    {
        var users = await _apiService.Get<PagedResult<UserResponse>>("User");
        cmbUser.DataSource = ReviewForm.CreateLookupItems(users?.Items.Select(x => new ReviewForm.LookupItem(x.Id, ReviewForm.GetUserDisplayName(x))), "Select");

        var appointments = await _apiService.Get<PagedResult<AppointmentResponse>>("Appointment");
        cmbAppointment.DataSource = ReviewForm.CreateLookupItems(appointments?.Items.Select(x => new ReviewForm.LookupItem(x.Id, ReviewForm.GetAppointmentDisplayName(x))), "None");

        var products = await _apiService.Get<PagedResult<ProductResponse>>("Product");
        cmbProduct.DataSource = ReviewForm.CreateLookupItems(products?.Items.Select(x => new ReviewForm.LookupItem(x.Id, x.Name)), "None");
    }

    private void LoadStatusLookup()
    {
        cmbStatus.DataSource = Enum.GetValues<ReviewStatus>()
            .Select(x => new StatusLookupItem(x, x.ToString()))
            .ToList();
    }

    private async Task LoadReview()
    {
        var review = await _apiService.Get<ReviewResponse>($"Review/{_reviewId}");

        if (review == null)
        {
            MessageBox.Show(GetApiErrorMessage("Review was not found."));
            DialogResult = DialogResult.Cancel;
            Close();
            return;
        }

        SelectLookupItem(cmbUser, review.UserId);
        SelectLookupItem(cmbAppointment, review.AppointmentId ?? 0);
        SelectLookupItem(cmbProduct, review.ProductId ?? 0);
        nudRating.Value = review.Rating;
        txtComment.Text = review.Comment;
        SelectStatus(review.Status);
    }

    private async void btnSave_Click(object sender, EventArgs e)
    {
        if (!ValidateInput())
        {
            return;
        }

        if (_reviewId.HasValue)
        {
            await UpdateReview();
        }
        else
        {
            await InsertReview();
        }
    }

    private async Task InsertReview()
    {
        var request = new ReviewInsertRequest
        {
            UserId = GetSelectedLookupId(cmbUser),
            AppointmentId = GetSelectedNullableLookupId(cmbAppointment),
            ProductId = GetSelectedNullableLookupId(cmbProduct),
            Rating = (int)nudRating.Value,
            Comment = string.IsNullOrWhiteSpace(txtComment.Text) ? null : txtComment.Text.Trim(),
            Status = GetSelectedStatus()
        };

        var response = await _apiService.Post<ReviewResponse>("Review", request);

        if (response == null)
        {
            MessageBox.Show(GetApiErrorMessage("Unable to save review."));
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    private async Task UpdateReview()
    {
        var request = new ReviewUpdateRequest
        {
            Id = _reviewId!.Value,
            UserId = GetSelectedLookupId(cmbUser),
            AppointmentId = GetSelectedNullableLookupId(cmbAppointment),
            ProductId = GetSelectedNullableLookupId(cmbProduct),
            Rating = (int)nudRating.Value,
            Comment = string.IsNullOrWhiteSpace(txtComment.Text) ? null : txtComment.Text.Trim(),
            Status = GetSelectedStatus()
        };

        var response = await _apiService.Put<ReviewResponse>($"Review/{_reviewId.Value}", request);

        if (response == null)
        {
            MessageBox.Show(GetApiErrorMessage("Unable to save review."));
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

        if (nudRating.Value < 1 || nudRating.Value > 5)
        {
            MessageBox.Show("Rating must be between 1 and 5.");
            nudRating.Focus();
            return false;
        }

        if (txtComment.Text.Length > 1000)
        {
            MessageBox.Show("Comment can contain up to 1000 characters.");
            txtComment.Focus();
            return false;
        }

        return true;
    }

    private static int GetSelectedLookupId(ComboBox comboBox)
    {
        return comboBox.SelectedItem is ReviewForm.LookupItem item ? item.Id : 0;
    }

    private static int? GetSelectedNullableLookupId(ComboBox comboBox)
    {
        var id = GetSelectedLookupId(comboBox);
        return id > 0 ? id : null;
    }

    private ReviewStatus GetSelectedStatus()
    {
        return cmbStatus.SelectedItem is StatusLookupItem item
            ? item.Status
            : ReviewStatus.PendingApproval;
    }

    private static void SelectLookupItem(ComboBox comboBox, int id)
    {
        for (var i = 0; i < comboBox.Items.Count; i++)
        {
            if (comboBox.Items[i] is ReviewForm.LookupItem item && item.Id == id)
            {
                comboBox.SelectedIndex = i;
                return;
            }
        }
    }

    private void SelectStatus(ReviewStatus status)
    {
        for (var i = 0; i < cmbStatus.Items.Count; i++)
        {
            if (cmbStatus.Items[i] is StatusLookupItem item && item.Status == status)
            {
                cmbStatus.SelectedIndex = i;
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

    private sealed record StatusLookupItem(ReviewStatus Status, string Name)
    {
        public override string ToString()
        {
            return Name;
        }
    }
}
