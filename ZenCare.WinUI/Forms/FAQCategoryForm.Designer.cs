namespace ZenCare.WinUI.Forms;

partial class FAQCategoryForm
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
        btnRefresh = new Button();
        btnDelete = new Button();
        btnEdit = new Button();
        btnAdd = new Button();
        btnSearch = new Button();
        chkIsActive = new CheckBox();
        txtName = new TextBox();
        lblName = new Label();
        dgvFAQCategories = new DataGridView();
        colName = new DataGridViewTextBoxColumn();
        colDescription = new DataGridViewTextBoxColumn();
        colIsActive = new DataGridViewCheckBoxColumn();
        colCreatedAt = new DataGridViewTextBoxColumn();
        pnlTop.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvFAQCategories).BeginInit();
        SuspendLayout();
        // 
        // pnlTop
        // 
        pnlTop.Controls.Add(btnRefresh);
        pnlTop.Controls.Add(btnDelete);
        pnlTop.Controls.Add(btnEdit);
        pnlTop.Controls.Add(btnAdd);
        pnlTop.Controls.Add(btnSearch);
        pnlTop.Controls.Add(chkIsActive);
        pnlTop.Controls.Add(txtName);
        pnlTop.Controls.Add(lblName);
        pnlTop.Dock = DockStyle.Top;
        pnlTop.Location = new Point(0, 0);
        pnlTop.Name = "pnlTop";
        pnlTop.Padding = new Padding(24, 20, 24, 12);
        pnlTop.Size = new Size(1000, 84);
        pnlTop.TabIndex = 0;
        // 
        // btnRefresh
        // 
        btnRefresh.Location = new Point(744, 28);
        btnRefresh.Name = "btnRefresh";
        btnRefresh.Size = new Size(96, 28);
        btnRefresh.TabIndex = 7;
        btnRefresh.Text = "Refresh";
        btnRefresh.UseVisualStyleBackColor = true;
        btnRefresh.Click += btnRefresh_Click;
        // 
        // btnDelete
        // 
        btnDelete.Location = new Point(632, 28);
        btnDelete.Name = "btnDelete";
        btnDelete.Size = new Size(96, 28);
        btnDelete.TabIndex = 6;
        btnDelete.Text = "Delete";
        btnDelete.UseVisualStyleBackColor = true;
        btnDelete.Click += btnDelete_Click;
        // 
        // btnEdit
        // 
        btnEdit.Location = new Point(520, 28);
        btnEdit.Name = "btnEdit";
        btnEdit.Size = new Size(96, 28);
        btnEdit.TabIndex = 5;
        btnEdit.Text = "Edit";
        btnEdit.UseVisualStyleBackColor = true;
        btnEdit.Click += btnEdit_Click;
        // 
        // btnAdd
        // 
        btnAdd.Location = new Point(408, 28);
        btnAdd.Name = "btnAdd";
        btnAdd.Size = new Size(96, 28);
        btnAdd.TabIndex = 4;
        btnAdd.Text = "Add";
        btnAdd.UseVisualStyleBackColor = true;
        btnAdd.Click += btnAdd_Click;
        // 
        // btnSearch
        // 
        btnSearch.Location = new Point(296, 28);
        btnSearch.Name = "btnSearch";
        btnSearch.Size = new Size(96, 28);
        btnSearch.TabIndex = 3;
        btnSearch.Text = "Search";
        btnSearch.UseVisualStyleBackColor = true;
        btnSearch.Click += btnSearch_Click;
        // 
        // chkIsActive
        // 
        chkIsActive.AutoSize = true;
        chkIsActive.Location = new Point(860, 33);
        chkIsActive.Name = "chkIsActive";
        chkIsActive.Size = new Size(68, 19);
        chkIsActive.TabIndex = 2;
        chkIsActive.Text = "IsActive";
        chkIsActive.ThreeState = true;
        chkIsActive.UseVisualStyleBackColor = true;
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
        // dgvFAQCategories
        // 
        dgvFAQCategories.AllowUserToAddRows = false;
        dgvFAQCategories.AllowUserToDeleteRows = false;
        dgvFAQCategories.AutoGenerateColumns = false;
        dgvFAQCategories.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvFAQCategories.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvFAQCategories.Columns.AddRange(new DataGridViewColumn[] { colName, colDescription, colIsActive, colCreatedAt });
        dgvFAQCategories.Dock = DockStyle.Fill;
        dgvFAQCategories.Location = new Point(0, 84);
        dgvFAQCategories.MultiSelect = false;
        dgvFAQCategories.Name = "dgvFAQCategories";
        dgvFAQCategories.ReadOnly = true;
        dgvFAQCategories.RowHeadersVisible = false;
        dgvFAQCategories.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvFAQCategories.Size = new Size(1000, 716);
        dgvFAQCategories.TabIndex = 1;
        // 
        // colName
        // 
        colName.DataPropertyName = "Name";
        colName.HeaderText = "Name";
        colName.Name = "colName";
        colName.ReadOnly = true;
        // 
        // colDescription
        // 
        colDescription.DataPropertyName = "Description";
        colDescription.HeaderText = "Description";
        colDescription.Name = "colDescription";
        colDescription.ReadOnly = true;
        // 
        // colIsActive
        // 
        colIsActive.DataPropertyName = "IsActive";
        colIsActive.HeaderText = "Active";
        colIsActive.Name = "colIsActive";
        colIsActive.ReadOnly = true;
        // 
        // colCreatedAt
        // 
        colCreatedAt.DataPropertyName = "CreatedAt";
        colCreatedAt.HeaderText = "Created";
        colCreatedAt.Name = "colCreatedAt";
        colCreatedAt.ReadOnly = true;
        // 
        // FAQCategoryForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 800);
        Controls.Add(dgvFAQCategories);
        Controls.Add(pnlTop);
        FormBorderStyle = FormBorderStyle.None;
        Name = "FAQCategoryForm";
        Text = "FAQ Categories";
        Load += FAQCategoryForm_Load;
        pnlTop.ResumeLayout(false);
        pnlTop.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvFAQCategories).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private Panel pnlTop;
    private Button btnRefresh;
    private Button btnDelete;
    private Button btnEdit;
    private Button btnAdd;
    private Button btnSearch;
    private CheckBox chkIsActive;
    private TextBox txtName;
    private Label lblName;
    private DataGridView dgvFAQCategories;
    private DataGridViewTextBoxColumn colName;
    private DataGridViewTextBoxColumn colDescription;
    private DataGridViewCheckBoxColumn colIsActive;
    private DataGridViewTextBoxColumn colCreatedAt;
}
