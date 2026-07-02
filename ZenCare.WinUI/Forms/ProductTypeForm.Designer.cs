namespace ZenCare.WinUI.Forms;

partial class ProductTypeForm
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
        txtName = new TextBox();
        lblName = new Label();
        dgvProductTypes = new DataGridView();
        colId = new DataGridViewTextBoxColumn();
        colName = new DataGridViewTextBoxColumn();
        colIsActive = new DataGridViewCheckBoxColumn();
        colCreatedAt = new DataGridViewTextBoxColumn();
        pnlTop.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvProductTypes).BeginInit();
        SuspendLayout();
        // 
        // pnlTop
        // 
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
        // dgvProductTypes
        // 
        dgvProductTypes.AllowUserToAddRows = false;
        dgvProductTypes.AllowUserToDeleteRows = false;
        dgvProductTypes.AutoGenerateColumns = false;
        dgvProductTypes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvProductTypes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvProductTypes.Columns.AddRange(new DataGridViewColumn[] { colId, colName, colIsActive, colCreatedAt });
        dgvProductTypes.Dock = DockStyle.Fill;
        dgvProductTypes.Location = new Point(0, 84);
        dgvProductTypes.Name = "dgvProductTypes";
        dgvProductTypes.ReadOnly = true;
        dgvProductTypes.RowHeadersVisible = false;
        dgvProductTypes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvProductTypes.Size = new Size(1000, 716);
        dgvProductTypes.TabIndex = 1;
        // 
        // colId
        // 
        colId.DataPropertyName = "Id";
        colId.HeaderText = "Id";
        colId.Name = "colId";
        colId.ReadOnly = true;
        // 
        // colName
        // 
        colName.DataPropertyName = "Name";
        colName.HeaderText = "Name";
        colName.Name = "colName";
        colName.ReadOnly = true;
        // 
        // colIsActive
        // 
        colIsActive.DataPropertyName = "IsActive";
        colIsActive.HeaderText = "IsActive";
        colIsActive.Name = "colIsActive";
        colIsActive.ReadOnly = true;
        // 
        // colCreatedAt
        // 
        colCreatedAt.DataPropertyName = "CreatedAt";
        colCreatedAt.HeaderText = "CreatedAt";
        colCreatedAt.Name = "colCreatedAt";
        colCreatedAt.ReadOnly = true;
        // 
        // ProductTypeForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 800);
        Controls.Add(dgvProductTypes);
        Controls.Add(pnlTop);
        FormBorderStyle = FormBorderStyle.None;
        Name = "ProductTypeForm";
        Text = "Product Types";
        Load += ProductTypeForm_Load;
        pnlTop.ResumeLayout(false);
        pnlTop.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvProductTypes).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private Panel pnlTop;
    private Button btnRefresh;
    private Button btnDelete;
    private Button btnEdit;
    private Button btnAdd;
    private Button btnSearch;
    private TextBox txtName;
    private Label lblName;
    private DataGridView dgvProductTypes;
    private DataGridViewTextBoxColumn colId;
    private DataGridViewTextBoxColumn colName;
    private DataGridViewCheckBoxColumn colIsActive;
    private DataGridViewTextBoxColumn colCreatedAt;
}
