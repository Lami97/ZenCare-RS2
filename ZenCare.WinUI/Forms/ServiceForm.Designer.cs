namespace ZenCare.WinUI.Forms;

partial class ServiceForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        pnlTop = new Panel();
        chkIsActive = new CheckBox();
        cmbServiceCategory = new ComboBox();
        lblServiceCategory = new Label();
        btnRefresh = new Button();
        btnDelete = new Button();
        btnEdit = new Button();
        btnAdd = new Button();
        btnSearch = new Button();
        txtName = new TextBox();
        lblName = new Label();
        dgvServices = new DataGridView();
        colId = new DataGridViewTextBoxColumn();
        colName = new DataGridViewTextBoxColumn();
        colDescription = new DataGridViewTextBoxColumn();
        colDurationMinutes = new DataGridViewTextBoxColumn();
        colPrice = new DataGridViewTextBoxColumn();
        colServiceCategoryName = new DataGridViewTextBoxColumn();
        colIsActive = new DataGridViewCheckBoxColumn();
        colCreatedAt = new DataGridViewTextBoxColumn();
        pnlTop.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvServices).BeginInit();
        SuspendLayout();
        // 
        // pnlTop
        // 
        pnlTop.Controls.Add(chkIsActive);
        pnlTop.Controls.Add(cmbServiceCategory);
        pnlTop.Controls.Add(lblServiceCategory);
        pnlTop.Controls.Add(btnRefresh);
        pnlTop.Controls.Add(btnDelete);
        pnlTop.Controls.Add(btnEdit);
        pnlTop.Controls.Add(btnAdd);
        pnlTop.Controls.Add(btnSearch);
        pnlTop.Controls.Add(txtName);
        pnlTop.Controls.Add(lblName);
        pnlTop.Dock = DockStyle.Top;
        pnlTop.Location = new Point(0, 0);
        pnlTop.Name = "pnlTop";
        pnlTop.Padding = new Padding(24, 16, 24, 12);
        pnlTop.Size = new Size(1000, 116);
        pnlTop.TabIndex = 0;
        // 
        // chkIsActive
        // 
        chkIsActive.CheckState = CheckState.Indeterminate;
        chkIsActive.Location = new Point(432, 62);
        chkIsActive.Name = "chkIsActive";
        chkIsActive.Size = new Size(92, 24);
        chkIsActive.TabIndex = 9;
        chkIsActive.Text = "IsActive";
        chkIsActive.ThreeState = true;
        chkIsActive.UseVisualStyleBackColor = true;
        // 
        // cmbServiceCategory
        // 
        cmbServiceCategory.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbServiceCategory.FormattingEnabled = true;
        cmbServiceCategory.Location = new Point(136, 62);
        cmbServiceCategory.Name = "cmbServiceCategory";
        cmbServiceCategory.Size = new Size(240, 23);
        cmbServiceCategory.TabIndex = 8;
        // 
        // lblServiceCategory
        // 
        lblServiceCategory.AutoSize = true;
        lblServiceCategory.Location = new Point(24, 66);
        lblServiceCategory.Name = "lblServiceCategory";
        lblServiceCategory.Size = new Size(94, 15);
        lblServiceCategory.TabIndex = 7;
        lblServiceCategory.Text = "Service category";
        // 
        // btnRefresh
        // 
        btnRefresh.Location = new Point(744, 20);
        btnRefresh.Name = "btnRefresh";
        btnRefresh.Size = new Size(96, 28);
        btnRefresh.TabIndex = 6;
        btnRefresh.Text = "Refresh";
        btnRefresh.UseVisualStyleBackColor = true;
        btnRefresh.Click += btnRefresh_Click;
        // 
        // btnDelete
        // 
        btnDelete.Location = new Point(632, 20);
        btnDelete.Name = "btnDelete";
        btnDelete.Size = new Size(96, 28);
        btnDelete.TabIndex = 5;
        btnDelete.Text = "Delete";
        btnDelete.UseVisualStyleBackColor = true;
        btnDelete.Click += btnDelete_Click;
        // 
        // btnEdit
        // 
        btnEdit.Location = new Point(520, 20);
        btnEdit.Name = "btnEdit";
        btnEdit.Size = new Size(96, 28);
        btnEdit.TabIndex = 4;
        btnEdit.Text = "Edit";
        btnEdit.UseVisualStyleBackColor = true;
        btnEdit.Click += btnEdit_Click;
        // 
        // btnAdd
        // 
        btnAdd.Location = new Point(408, 20);
        btnAdd.Name = "btnAdd";
        btnAdd.Size = new Size(96, 28);
        btnAdd.TabIndex = 3;
        btnAdd.Text = "Add";
        btnAdd.UseVisualStyleBackColor = true;
        btnAdd.Click += btnAdd_Click;
        // 
        // btnSearch
        // 
        btnSearch.Location = new Point(296, 20);
        btnSearch.Name = "btnSearch";
        btnSearch.Size = new Size(96, 28);
        btnSearch.TabIndex = 2;
        btnSearch.Text = "Search";
        btnSearch.UseVisualStyleBackColor = true;
        btnSearch.Click += btnSearch_Click;
        // 
        // txtName
        // 
        txtName.Location = new Point(76, 22);
        txtName.Name = "txtName";
        txtName.Size = new Size(200, 23);
        txtName.TabIndex = 1;
        // 
        // lblName
        // 
        lblName.AutoSize = true;
        lblName.Location = new Point(24, 26);
        lblName.Name = "lblName";
        lblName.Size = new Size(39, 15);
        lblName.TabIndex = 0;
        lblName.Text = "Name";
        // 
        // dgvServices
        // 
        dgvServices.AllowUserToAddRows = false;
        dgvServices.AllowUserToDeleteRows = false;
        dgvServices.AutoGenerateColumns = false;
        dgvServices.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvServices.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvServices.Columns.AddRange(new DataGridViewColumn[] { colId, colName, colDescription, colDurationMinutes, colPrice, colServiceCategoryName, colIsActive, colCreatedAt });
        dgvServices.Dock = DockStyle.Fill;
        dgvServices.Location = new Point(0, 116);
        dgvServices.Name = "dgvServices";
        dgvServices.ReadOnly = true;
        dgvServices.RowHeadersVisible = false;
        dgvServices.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvServices.Size = new Size(1000, 684);
        dgvServices.TabIndex = 1;
        // 
        // columns
        // 
        colId.DataPropertyName = "Id";
        colId.HeaderText = "Id";
        colId.Name = "colId";
        colId.ReadOnly = true;
        colName.DataPropertyName = "Name";
        colName.HeaderText = "Name";
        colName.Name = "colName";
        colName.ReadOnly = true;
        colDescription.DataPropertyName = "Description";
        colDescription.HeaderText = "Description";
        colDescription.Name = "colDescription";
        colDescription.ReadOnly = true;
        colDurationMinutes.DataPropertyName = "DurationMinutes";
        colDurationMinutes.HeaderText = "Duration";
        colDurationMinutes.Name = "colDurationMinutes";
        colDurationMinutes.ReadOnly = true;
        colPrice.DataPropertyName = "Price";
        colPrice.HeaderText = "Price";
        colPrice.Name = "colPrice";
        colPrice.ReadOnly = true;
        colServiceCategoryName.DataPropertyName = "ServiceCategoryName";
        colServiceCategoryName.HeaderText = "Category";
        colServiceCategoryName.Name = "colServiceCategoryName";
        colServiceCategoryName.ReadOnly = true;
        colIsActive.DataPropertyName = "IsActive";
        colIsActive.HeaderText = "IsActive";
        colIsActive.Name = "colIsActive";
        colIsActive.ReadOnly = true;
        colCreatedAt.DataPropertyName = "CreatedAt";
        colCreatedAt.HeaderText = "CreatedAt";
        colCreatedAt.Name = "colCreatedAt";
        colCreatedAt.ReadOnly = true;
        // 
        // ServiceForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 800);
        Controls.Add(dgvServices);
        Controls.Add(pnlTop);
        FormBorderStyle = FormBorderStyle.None;
        Name = "ServiceForm";
        Text = "Services Management";
        Load += ServiceForm_Load;
        pnlTop.ResumeLayout(false);
        pnlTop.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvServices).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private Panel pnlTop;
    private CheckBox chkIsActive;
    private ComboBox cmbServiceCategory;
    private Label lblServiceCategory;
    private Button btnRefresh;
    private Button btnDelete;
    private Button btnEdit;
    private Button btnAdd;
    private Button btnSearch;
    private TextBox txtName;
    private Label lblName;
    private DataGridView dgvServices;
    private DataGridViewTextBoxColumn colId;
    private DataGridViewTextBoxColumn colName;
    private DataGridViewTextBoxColumn colDescription;
    private DataGridViewTextBoxColumn colDurationMinutes;
    private DataGridViewTextBoxColumn colPrice;
    private DataGridViewTextBoxColumn colServiceCategoryName;
    private DataGridViewCheckBoxColumn colIsActive;
    private DataGridViewTextBoxColumn colCreatedAt;
}
