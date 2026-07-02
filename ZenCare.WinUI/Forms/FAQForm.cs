using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI.Forms;

public partial class FAQForm : Form
{
    private readonly APIService _apiService = new APIService();

    public FAQForm()
    {
        InitializeComponent();
        chkIsActive.CheckState = CheckState.Indeterminate;
    }

    private async void FAQForm_Load(object sender, EventArgs e)
    {
        await LoadLookups();
        await LoadFAQs();
    }

    private async void btnSearch_Click(object sender, EventArgs e)
    {
        await LoadFAQs();
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
        OpenDetailsForm(new FAQDetailsForm());
    }

    private void btnEdit_Click(object sender, EventArgs e)
    {
        var selectedId = GetSelectedFAQId();

        if (selectedId == null)
        {
            MessageBox.Show("Please select an FAQ.");
            return;
        }

        OpenDetailsForm(new FAQDetailsForm(selectedId.Value));
    }

    private async void btnDelete_Click(object sender, EventArgs e)
    {
        var selectedId = GetSelectedFAQId();

        if (selectedId == null)
        {
            MessageBox.Show("Please select an FAQ.");
            return;
        }

        var result = MessageBox.Show(
            "Are you sure you want to delete this FAQ?",
            "Confirm delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result != DialogResult.Yes)
        {
            return;
        }

        var deleted = await _apiService.Delete($"FAQ/{selectedId.Value}");

        if (!deleted)
        {
            MessageBox.Show(GetApiErrorMessage("Unable to delete FAQ."));
            return;
        }

        await LoadFAQs();
    }

    private async void btnRefresh_Click(object sender, EventArgs e)
    {
        txtQuestion.Clear();
        cmbFAQCategory.SelectedIndex = 0;
        chkIsActive.CheckState = CheckState.Indeterminate;
        await LoadFAQs();
    }

    private async Task LoadLookups()
    {
        var categories = await _apiService.Get<PagedResult<FAQCategoryResponse>>("FAQCategory");
        cmbFAQCategory.DataSource = CreateLookupItems(categories?.Items.Select(x => new LookupItem(x.Id, x.Name)), "All");
    }

    private async Task LoadFAQs()
    {
        var search = new FAQSearchObject
        {
            Question = string.IsNullOrWhiteSpace(txtQuestion.Text) ? null : txtQuestion.Text,
            FAQCategoryId = GetSelectedLookupId(cmbFAQCategory),
            IsActive = chkIsActive.CheckState == CheckState.Indeterminate ? null : chkIsActive.Checked
        };

        var query = new List<string>();

        if (!string.IsNullOrWhiteSpace(search.Question))
        {
            query.Add($"Question={Uri.EscapeDataString(search.Question)}");
        }

        if (search.FAQCategoryId.HasValue)
        {
            query.Add($"FAQCategoryId={search.FAQCategoryId.Value}");
        }

        if (search.IsActive.HasValue)
        {
            query.Add($"IsActive={search.IsActive.Value}");
        }

        var endpoint = query.Count == 0 ? "FAQ" : $"FAQ?{string.Join("&", query)}";
        var result = await _apiService.Get<PagedResult<FAQResponse>>(endpoint);

        if (result == null)
        {
            MessageBox.Show(GetApiErrorMessage("Unable to load FAQs."));
            dgvFAQs.DataSource = new List<FAQResponse>();
            return;
        }

        dgvFAQs.DataSource = result.Items ?? new List<FAQResponse>();
    }

    private int? GetSelectedFAQId()
    {
        return dgvFAQs.CurrentRow?.DataBoundItem is FAQResponse faq
            ? faq.Id
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

    private string GetApiErrorMessage(string fallback)
    {
        return string.IsNullOrWhiteSpace(_apiService.LastErrorMessage)
            ? fallback
            : _apiService.LastErrorMessage;
    }

    private async void OpenDetailsForm(FAQDetailsForm form)
    {
        if (form.ShowDialog() == DialogResult.OK)
        {
            await LoadFAQs();
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
