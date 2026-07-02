using ZenCare.Model.Responses;
using ZenCare.Model.SearchObjects;
using ZenCare.WinUI.Helpers;

namespace ZenCare.WinUI.Forms;

public partial class UnitOfMeasureForm : Form
{
    private readonly APIService _apiService = new APIService();

    public UnitOfMeasureForm()
    {
        InitializeComponent();
    }

    private async void UnitOfMeasureForm_Load(object sender, EventArgs e)
    {
        await LoadUnits();
    }

    private async void btnSearch_Click(object sender, EventArgs e)
    {
        await LoadUnits();
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
        OpenDetailsForm(new UnitOfMeasureDetailsForm());
    }

    private void btnEdit_Click(object sender, EventArgs e)
    {
        var selectedId = GetSelectedUnitId();

        if (selectedId == null)
        {
            MessageBox.Show("Please select a unit.");
            return;
        }

        OpenDetailsForm(new UnitOfMeasureDetailsForm(selectedId.Value));
    }

    private async void btnDelete_Click(object sender, EventArgs e)
    {
        var selectedId = GetSelectedUnitId();

        if (selectedId == null)
        {
            MessageBox.Show("Please select a unit.");
            return;
        }

        var result = MessageBox.Show(
            "Are you sure you want to delete this unit?",
            "Confirm delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result != DialogResult.Yes)
        {
            return;
        }

        await _apiService.Delete($"UnitOfMeasure/{selectedId.Value}");
        await LoadUnits();
    }

    private async void btnRefresh_Click(object sender, EventArgs e)
    {
        txtName.Clear();
        await LoadUnits();
    }

    private async Task LoadUnits()
    {
        var search = new UnitOfMeasureSearchObject
        {
            Name = string.IsNullOrWhiteSpace(txtName.Text) ? null : txtName.Text
        };

        var endpoint = "UnitOfMeasure";

        if (!string.IsNullOrWhiteSpace(search.Name))
        {
            endpoint += $"?Name={Uri.EscapeDataString(search.Name)}";
        }

        var result = await _apiService.Get<PagedResult<UnitOfMeasureResponse>>(endpoint);
        dgvUnits.DataSource = result?.Items ?? new List<UnitOfMeasureResponse>();
    }

    private int? GetSelectedUnitId()
    {
        return dgvUnits.CurrentRow?.DataBoundItem is UnitOfMeasureResponse unit
            ? unit.Id
            : null;
    }

    private async void OpenDetailsForm(UnitOfMeasureDetailsForm form)
    {
        if (form.ShowDialog() == DialogResult.OK)
        {
            await LoadUnits();
        }
    }
}
