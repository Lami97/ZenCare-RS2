namespace ZenCare.WinUI.Forms;

partial class SupplierForm
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
        lblSearch = new Label();
        dgvSuppliers = new DataGridView();
        colName = new DataGridViewTextBoxColumn();
        colContactEmail = new DataGridViewTextBoxColumn();
        colPhoneNumber = new DataGridViewTextBoxColumn();
        colAddress = new DataGridViewTextBoxColumn();
        colIsActive = new DataGridViewCheckBoxColumn();
        pnlTop.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvSuppliers).BeginInit();
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
        pnlTop.Controls.Add(lblSearch);
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
        chkIsActive.Text = "Active";
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
        // lblSearch
        // 
        lblSearch.AutoSize = true;
        lblSearch.Location = new Point(24, 34);
        lblSearch.Name = "lblSearch";
        lblSearch.Size = new Size(42, 15);
        lblSearch.TabIndex = 0;
        lblSearch.Text = "Search";
        // 
        // dgvSuppliers
        // 
        dgvSuppliers.AllowUserToAddRows = false;
        dgvSuppliers.AllowUserToDeleteRows = false;
        dgvSuppliers.AutoGenerateColumns = false;
        dgvSuppliers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvSuppliers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvSuppliers.Columns.AddRange(new DataGridViewColumn[] { colName, colContactEmail, colPhoneNumber, colAddress, colIsActive });
        dgvSuppliers.Dock = DockStyle.Fill;
        dgvSuppliers.Location = new Point(0, 84);
        dgvSuppliers.MultiSelect = false;
        dgvSuppliers.Name = "dgvSuppliers";
        dgvSuppliers.ReadOnly = true;
        dgvSuppliers.RowHeadersVisible = false;
        dgvSuppliers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvSuppliers.Size = new Size(1000, 716);
        dgvSuppliers.TabIndex = 1;
        // 
        // columns
        // 
        colName.DataPropertyName = "Name";
        colName.HeaderText = "Name";
        colName.Name = "colName";
        colName.ReadOnly = true;
        colContactEmail.DataPropertyName = "ContactEmail";
        colContactEmail.HeaderText = "Email";
        colContactEmail.Name = "colContactEmail";
        colContactEmail.ReadOnly = true;
        colPhoneNumber.DataPropertyName = "PhoneNumber";
        colPhoneNumber.HeaderText = "Phone";
        colPhoneNumber.Name = "colPhoneNumber";
        colPhoneNumber.ReadOnly = true;
        colAddress.DataPropertyName = "Address";
        colAddress.HeaderText = "Address";
        colAddress.Name = "colAddress";
        colAddress.ReadOnly = true;
        colIsActive.DataPropertyName = "IsActive";
        colIsActive.HeaderText = "Active";
        colIsActive.Name = "colIsActive";
        colIsActive.ReadOnly = true;
        // 
        // SupplierForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 800);
        Controls.Add(dgvSuppliers);
        Controls.Add(pnlTop);
        FormBorderStyle = FormBorderStyle.None;
        Name = "SupplierForm";
        Text = "Suppliers";
        Load += SupplierForm_Load;
        pnlTop.ResumeLayout(false);
        pnlTop.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvSuppliers).EndInit();
        ResumeLayout(false);
    }

    private Panel pnlTop;
    private CheckBox chkIsActive;
    private Button btnRefresh;
    private Button btnDelete;
    private Button btnEdit;
    private Button btnAdd;
    private Button btnSearch;
    private TextBox txtName;
    private Label lblSearch;
    private DataGridView dgvSuppliers;
    private DataGridViewTextBoxColumn colName;
    private DataGridViewTextBoxColumn colContactEmail;
    private DataGridViewTextBoxColumn colPhoneNumber;
    private DataGridViewTextBoxColumn colAddress;
    private DataGridViewCheckBoxColumn colIsActive;
}
