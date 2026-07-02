namespace ZenCare.WinUI.Forms;

partial class ServiceCategoryForm
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
        btnRefresh = new Button();
        btnDelete = new Button();
        btnEdit = new Button();
        btnAdd = new Button();
        btnSearch = new Button();
        txtName = new TextBox();
        lblName = new Label();
        dgvServiceCategories = new DataGridView();
        colId = new DataGridViewTextBoxColumn();
        colName = new DataGridViewTextBoxColumn();
        colDescription = new DataGridViewTextBoxColumn();
        colIsActive = new DataGridViewCheckBoxColumn();
        colCreatedAt = new DataGridViewTextBoxColumn();
        pnlTop.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvServiceCategories).BeginInit();
        SuspendLayout();
        // 
        // pnlTop
        // 
        pnlTop.Controls.Add(chkIsActive);
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
        pnlTop.Padding = new Padding(24, 20, 24, 12);
        pnlTop.Size = new Size(1000, 84);
        pnlTop.TabIndex = 0;
        // 
        // chkIsActive
        // 
        chkIsActive.CheckState = CheckState.Indeterminate;
        chkIsActive.Location = new Point(860, 30);
        chkIsActive.Name = "chkIsActive";
        chkIsActive.Size = new Size(92, 24);
        chkIsActive.TabIndex = 7;
        chkIsActive.Text = "IsActive";
        chkIsActive.ThreeState = true;
        chkIsActive.UseVisualStyleBackColor = true;
        // 
        // btnRefresh
        // 
        btnRefresh.Location = new Point(744, 28);
        btnRefresh.Name = "btnRefresh";
        btnRefresh.Size = new Size(96, 28);
        btnRefresh.TabIndex = 6;
        btnRefresh.Text = "Refresh";
        btnRefresh.UseVisualStyleBackColor = true;
        btnRefresh.Click += btnRefresh_Click;
        // 
        // btnDelete
        // 
        btnDelete.Location = new Point(632, 28);
        btnDelete.Name = "btnDelete";
        btnDelete.Size = new Size(96, 28);
        btnDelete.TabIndex = 5;
        btnDelete.Text = "Delete";
        btnDelete.UseVisualStyleBackColor = true;
        btnDelete.Click += btnDelete_Click;
        // 
        // btnEdit
        // 
        btnEdit.Location = new Point(520, 28);
        btnEdit.Name = "btnEdit";
        btnEdit.Size = new Size(96, 28);
        btnEdit.TabIndex = 4;
        btnEdit.Text = "Edit";
        btnEdit.UseVisualStyleBackColor = true;
        btnEdit.Click += btnEdit_Click;
        // 
        // btnAdd
        // 
        btnAdd.Location = new Point(408, 28);
        btnAdd.Name = "btnAdd";
        btnAdd.Size = new Size(96, 28);
        btnAdd.TabIndex = 3;
        btnAdd.Text = "Add";
        btnAdd.UseVisualStyleBackColor = true;
        btnAdd.Click += btnAdd_Click;
        // 
        // btnSearch
        // 
        btnSearch.Location = new Point(296, 28);
        btnSearch.Name = "btnSearch";
        btnSearch.Size = new Size(96, 28);
        btnSearch.TabIndex = 2;
        btnSearch.Text = "Search";
        btnSearch.UseVisualStyleBackColor = true;
        btnSearch.Click += btnSearch_Click;
        // 
        // txtName
        // 
        txtName.Location = new Point(76, 30);
        txtName.Name = "txtName";
        txtName.Size = new Size(200, 23);
        txtName.TabIndex = 1;
        // 
        // lblName
        // 
        lblName.AutoSize = true;
        lblName.Location = new Point(24, 34);
        lblName.Name = "lblName";
        lblName.Size = new Size(39, 15);
        lblName.TabIndex = 0;
        lblName.Text = "Name";
        // 
        // dgvServiceCategories
        // 
        dgvServiceCategories.AllowUserToAddRows = false;
        dgvServiceCategories.AllowUserToDeleteRows = false;
        dgvServiceCategories.AutoGenerateColumns = false;
        dgvServiceCategories.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvServiceCategories.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvServiceCategories.Columns.AddRange(new DataGridViewColumn[] { colId, colName, colDescription, colIsActive, colCreatedAt });
        dgvServiceCategories.Dock = DockStyle.Fill;
        dgvServiceCategories.Location = new Point(0, 84);
        dgvServiceCategories.Name = "dgvServiceCategories";
        dgvServiceCategories.ReadOnly = true;
        dgvServiceCategories.RowHeadersVisible = false;
        dgvServiceCategories.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvServiceCategories.Size = new Size(1000, 716);
        dgvServiceCategories.TabIndex = 1;
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
        colIsActive.DataPropertyName = "IsActive";
        colIsActive.HeaderText = "IsActive";
        colIsActive.Name = "colIsActive";
        colIsActive.ReadOnly = true;
        colCreatedAt.DataPropertyName = "CreatedAt";
        colCreatedAt.HeaderText = "CreatedAt";
        colCreatedAt.Name = "colCreatedAt";
        colCreatedAt.ReadOnly = true;
        // 
        // ServiceCategoryForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 800);
        Controls.Add(dgvServiceCategories);
        Controls.Add(pnlTop);
        FormBorderStyle = FormBorderStyle.None;
        Name = "ServiceCategoryForm";
        Text = "Service Categories";
        Load += ServiceCategoryForm_Load;
        pnlTop.ResumeLayout(false);
        pnlTop.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvServiceCategories).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private Panel pnlTop;
    private CheckBox chkIsActive;
    private Button btnRefresh;
    private Button btnDelete;
    private Button btnEdit;
    private Button btnAdd;
    private Button btnSearch;
    private TextBox txtName;
    private Label lblName;
    private DataGridView dgvServiceCategories;
    private DataGridViewTextBoxColumn colId;
    private DataGridViewTextBoxColumn colName;
    private DataGridViewTextBoxColumn colDescription;
    private DataGridViewCheckBoxColumn colIsActive;
    private DataGridViewTextBoxColumn colCreatedAt;
}
