using ZenCare.Model.Enums;
using ZenCare.Model.Requests;
using ZenCare.Model.Responses;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI.Forms;

public partial class ReviewDetailsForm : Form
{
    private const int LookupPageSize = 100;
    private readonly APIService _apiService = new APIService();
    private readonly int? _reviewId;
    private ReviewResponse? _currentReview;
    private bool _isLoadingLookups;

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
            await PopulateAppointments();
            SelectStatus(ReviewStatus.PendingApproval);
            nudRating.Value = 1;
        }
    }

    private async Task LoadLookups()
    {
        _isLoadingLookups = true;

        try
        {
            var users = await _apiService.Get<PagedResult<UserResponse>>("User");
            cmbUser.DataSource = ReviewForm.CreateLookupItems(users?.Items.Select(x => new ReviewForm.LookupItem(x.Id, ReviewForm.GetUserDisplayName(x))), "Select");

            if (_reviewId.HasValue)
            {
                _currentReview = await _apiService.Get<ReviewResponse>($"Review/{_reviewId.Value}");
            }

            var products = await _apiService.Get<PagedResult<ProductResponse>>("Product");
            cmbProduct.DataSource = ReviewForm.CreateLookupItems(products?.Items.Select(x => new ReviewForm.LookupItem(x.Id, x.Name)), "None");
        }
        finally
        {
            _isLoadingLookups = false;
        }
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
        await PopulateAppointments();
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

        MessageBox.Show("Review was added successfully.");
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

        MessageBox.Show("Review was updated successfully.");
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

        if (GetSelectedNullableLookupId(cmbAppointment).HasValue == GetSelectedNullableLookupId(cmbProduct).HasValue)
        {
            MessageBox.Show("Select exactly one review target.");
            return false;
        }

        return true;
    }

    private async void cmbUser_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (_isLoadingLookups)
        {
            return;
        }

        await PopulateAppointments();
    }

    private async Task PopulateAppointments()
    {
        var selectedAppointmentId = GetSelectedLookupId(cmbAppointment);
        var selectedUserId = GetSelectedLookupId(cmbUser);
        var currentAppointmentId = _currentReview?.AppointmentId;

        if (selectedUserId <= 0)
        {
            cmbAppointment.DataSource = ReviewForm.CreateLookupItems(null, "None");
            return;
        }

        var reviews = await LoadReviewsForUser(selectedUserId);
        var reviewedAppointmentIds = reviews
            .Where(x => x.AppointmentId.HasValue
                && (!_reviewId.HasValue || x.Id != _reviewId.Value))
            .Select(x => x.AppointmentId!.Value)
            .ToHashSet();

        var appointments = (await LoadCompletedAppointmentsForUser(selectedUserId))
            .Where(x => !reviewedAppointmentIds.Contains(x.Id) || x.Id == currentAppointmentId)
            .Select(x => new ReviewForm.LookupItem(x.Id, ReviewForm.GetAppointmentDisplayName(x)));

        cmbAppointment.DataSource = ReviewForm.CreateLookupItems(appointments, "None");

        if (selectedAppointmentId > 0)
        {
            SelectLookupItem(cmbAppointment, selectedAppointmentId);
        }
    }

    private async Task<List<AppointmentResponse>> LoadCompletedAppointmentsForUser(int userId)
    {
        var appointments = new List<AppointmentResponse>();

        for (var page = 1; ; page++)
        {
            var endpoint = $"Appointment?UserId={userId}&Status={AppointmentStatus.Completed}&Page={page}&PageSize={LookupPageSize}";
            var result = await _apiService.Get<PagedResult<AppointmentResponse>>(endpoint);

            if (result?.Items == null || result.Items.Count == 0)
            {
                break;
            }

            appointments.AddRange(result.Items);

            if (result.Items.Count < LookupPageSize)
            {
                break;
            }
        }

        return appointments;
    }

    private async Task<List<ReviewResponse>> LoadReviewsForUser(int userId)
    {
        var reviews = new List<ReviewResponse>();

        for (var page = 1; ; page++)
        {
            var endpoint = $"Review?UserId={userId}&Page={page}&PageSize={LookupPageSize}";
            var result = await _apiService.Get<PagedResult<ReviewResponse>>(endpoint);

            if (result?.Items == null || result.Items.Count == 0)
            {
                break;
            }

            reviews.AddRange(result.Items);

            if (result.Items.Count < LookupPageSize)
            {
                break;
            }
        }

        return reviews;
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
