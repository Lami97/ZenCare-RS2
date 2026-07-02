using ZenCare.Model.Enums;
using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI.Forms;

public partial class ReviewForm : Form
{
    private readonly APIService _apiService = new APIService();

    public ReviewForm()
    {
        InitializeComponent();
    }

    private async void ReviewForm_Load(object sender, EventArgs e)
    {
        LoadStatusLookup();
        await LoadLookups();
        await LoadReviews();
    }

    private async void btnSearch_Click(object sender, EventArgs e)
    {
        await LoadReviews();
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
        OpenDetailsForm(new ReviewDetailsForm());
    }

    private void btnEdit_Click(object sender, EventArgs e)
    {
        var selectedId = GetSelectedReviewId();

        if (selectedId == null)
        {
            MessageBox.Show("Please select a review.");
            return;
        }

        OpenDetailsForm(new ReviewDetailsForm(selectedId.Value));
    }

    private async void btnDelete_Click(object sender, EventArgs e)
    {
        var selectedId = GetSelectedReviewId();

        if (selectedId == null)
        {
            MessageBox.Show("Please select a review.");
            return;
        }

        var result = MessageBox.Show(
            "Are you sure you want to delete this review?",
            "Confirm delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result != DialogResult.Yes)
        {
            return;
        }

        var deleted = await _apiService.Delete($"Review/{selectedId.Value}");

        if (!deleted)
        {
            MessageBox.Show(GetApiErrorMessage("Unable to delete review."));
            return;
        }

        await LoadReviews();
    }

    private async void btnRefresh_Click(object sender, EventArgs e)
    {
        SelectLookupItem(cmbUser, 0);
        SelectLookupItem(cmbAppointment, 0);
        SelectLookupItem(cmbProduct, 0);
        nudRating.Value = 0;
        cmbStatus.SelectedIndex = 0;
        await LoadReviews();
    }

    private async Task LoadLookups()
    {
        var users = await _apiService.Get<PagedResult<UserResponse>>("User");
        cmbUser.DataSource = CreateLookupItems(users?.Items.Select(x => new LookupItem(x.Id, GetUserDisplayName(x))), "All");

        var appointments = await _apiService.Get<PagedResult<AppointmentResponse>>("Appointment");
        cmbAppointment.DataSource = CreateLookupItems(appointments?.Items.Select(x => new LookupItem(x.Id, GetAppointmentDisplayName(x))), "All");

        var products = await _apiService.Get<PagedResult<ProductResponse>>("Product");
        cmbProduct.DataSource = CreateLookupItems(products?.Items.Select(x => new LookupItem(x.Id, x.Name)), "All");
    }

    private void LoadStatusLookup()
    {
        var statuses = new List<StatusLookupItem> { new StatusLookupItem(null, "All") };
        statuses.AddRange(Enum.GetValues<ReviewStatus>().Select(x => new StatusLookupItem(x, x.ToString())));
        cmbStatus.DataSource = statuses;
    }

    private async Task LoadReviews()
    {
        var search = new ReviewSearchObject
        {
            UserId = GetSelectedLookupId(cmbUser),
            AppointmentId = GetSelectedLookupId(cmbAppointment),
            ProductId = GetSelectedLookupId(cmbProduct),
            Rating = nudRating.Value > 0 ? (int)nudRating.Value : null,
            Status = GetSelectedStatus()
        };

        var query = new List<string>();

        if (search.UserId.HasValue)
        {
            query.Add($"UserId={search.UserId.Value}");
        }

        if (search.AppointmentId.HasValue)
        {
            query.Add($"AppointmentId={search.AppointmentId.Value}");
        }

        if (search.ProductId.HasValue)
        {
            query.Add($"ProductId={search.ProductId.Value}");
        }

        if (search.Rating.HasValue)
        {
            query.Add($"Rating={search.Rating.Value}");
        }

        if (search.Status.HasValue)
        {
            query.Add($"Status={search.Status.Value}");
        }

        var endpoint = query.Count == 0 ? "Review" : $"Review?{string.Join("&", query)}";
        var result = await _apiService.Get<PagedResult<ReviewResponse>>(endpoint);

        if (result == null)
        {
            MessageBox.Show(GetApiErrorMessage("Unable to load reviews."));
            dgvReviews.DataSource = new List<ReviewResponse>();
            return;
        }

        dgvReviews.DataSource = result.Items ?? new List<ReviewResponse>();
    }

    private int? GetSelectedReviewId()
    {
        return dgvReviews.CurrentRow?.DataBoundItem is ReviewResponse review
            ? review.Id
            : null;
    }

    private static int? GetSelectedLookupId(ComboBox comboBox)
    {
        return comboBox.SelectedItem is LookupItem { Id: > 0 } item ? item.Id : null;
    }

    private ReviewStatus? GetSelectedStatus()
    {
        return cmbStatus.SelectedItem is StatusLookupItem item ? item.Status : null;
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

    internal static string GetUserDisplayName(UserResponse user)
    {
        var fullName = $"{user.FirstName} {user.LastName}".Trim();
        return string.IsNullOrWhiteSpace(fullName)
            ? user.Username
            : $"{user.Username} - {fullName}";
    }

    internal static string GetAppointmentDisplayName(AppointmentResponse appointment)
    {
        return $"{appointment.AppointmentDate:yyyy-MM-dd} {appointment.StartTime:hh\\:mm} - {appointment.UserName} / {appointment.ServiceName}";
    }

    private string GetApiErrorMessage(string fallback)
    {
        return string.IsNullOrWhiteSpace(_apiService.LastErrorMessage)
            ? fallback
            : _apiService.LastErrorMessage;
    }

    private async void OpenDetailsForm(ReviewDetailsForm form)
    {
        if (form.ShowDialog() == DialogResult.OK)
        {
            await LoadReviews();
        }
    }

    internal sealed record LookupItem(int Id, string Name)
    {
        public override string ToString()
        {
            return Name;
        }
    }

    private sealed record StatusLookupItem(ReviewStatus? Status, string Name)
    {
        public override string ToString()
        {
            return Name;
        }
    }
}
