namespace ZenCare.WinUI.Forms;

partial class ProductForm
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
        cmbProductType = new ComboBox();
        lblProductType = new Label();
        cmbProductCategory = new ComboBox();
        lblProductCategory = new Label();
        btnRefresh = new Button();
        btnDelete = new Button();
        btnEdit = new Button();
        btnAdd = new Button();
        btnSearch = new Button();
        txtName = new TextBox();
        lblName = new Label();
        dgvProducts = new DataGridView();
        colId = new DataGridViewTextBoxColumn();
        colName = new DataGridViewTextBoxColumn();
        colDescription = new DataGridViewTextBoxColumn();
        colPrice = new DataGridViewTextBoxColumn();
        colStockQuantity = new DataGridViewTextBoxColumn();
        colProductCategoryName = new DataGridViewTextBoxColumn();
        colProductTypeName = new DataGridViewTextBoxColumn();
        colUnitOfMeasureName = new DataGridViewTextBoxColumn();
        colIsActive = new DataGridViewCheckBoxColumn();
        colCreatedAt = new DataGridViewTextBoxColumn();
        pnlTop.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvProducts).BeginInit();
        SuspendLayout();
        // 
        // pnlTop
        // 
        pnlTop.Controls.Add(chkIsActive);
        pnlTop.Controls.Add(cmbProductType);
        pnlTop.Controls.Add(lblProductType);
        pnlTop.Controls.Add(cmbProductCategory);
        pnlTop.Controls.Add(lblProductCategory);
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
        chkIsActive.Location = new Point(760, 60);
        chkIsActive.Name = "chkIsActive";
        chkIsActive.Size = new Size(92, 24);
        chkIsActive.TabIndex = 11;
        chkIsActive.Text = "IsActive";
        chkIsActive.ThreeState = true;
        chkIsActive.UseVisualStyleBackColor = true;
        // 
        // cmbProductType
        // 
        cmbProductType.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbProductType.FormattingEnabled = true;
        cmbProductType.Location = new Point(520, 62);
        cmbProductType.Name = "cmbProductType";
        cmbProductType.Size = new Size(200, 23);
        cmbProductType.TabIndex = 10;
        // 
        // lblProductType
        // 
        lblProductType.AutoSize = true;
        lblProductType.Location = new Point(432, 66);
        lblProductType.Name = "lblProductType";
        lblProductType.Size = new Size(75, 15);
        lblProductType.TabIndex = 9;
        lblProductType.Text = "Product type";
        // 
        // cmbProductCategory
        // 
        cmbProductCategory.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbProductCategory.FormattingEnabled = true;
        cmbProductCategory.Location = new Point(168, 62);
        cmbProductCategory.Name = "cmbProductCategory";
        cmbProductCategory.Size = new Size(200, 23);
        cmbProductCategory.TabIndex = 8;
        // 
        // lblProductCategory
        // 
        lblProductCategory.AutoSize = true;
        lblProductCategory.Location = new Point(24, 66);
        lblProductCategory.Name = "lblProductCategory";
        lblProductCategory.Size = new Size(99, 15);
        lblProductCategory.TabIndex = 7;
        lblProductCategory.Text = "Product category";
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
        // dgvProducts
        // 
        dgvProducts.AllowUserToAddRows = false;
        dgvProducts.AllowUserToDeleteRows = false;
        dgvProducts.AutoGenerateColumns = false;
        dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvProducts.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvProducts.Columns.AddRange(new DataGridViewColumn[] { colId, colName, colDescription, colPrice, colStockQuantity, colProductCategoryName, colProductTypeName, colUnitOfMeasureName, colIsActive, colCreatedAt });
        dgvProducts.Dock = DockStyle.Fill;
        dgvProducts.Location = new Point(0, 116);
        dgvProducts.Name = "dgvProducts";
        dgvProducts.ReadOnly = true;
        dgvProducts.RowHeadersVisible = false;
        dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvProducts.Size = new Size(1000, 684);
        dgvProducts.TabIndex = 1;
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
        colPrice.DataPropertyName = "Price";
        colPrice.HeaderText = "Price";
        colPrice.Name = "colPrice";
        colPrice.ReadOnly = true;
        colStockQuantity.DataPropertyName = "StockQuantity";
        colStockQuantity.HeaderText = "Stock";
        colStockQuantity.Name = "colStockQuantity";
        colStockQuantity.ReadOnly = true;
        colProductCategoryName.DataPropertyName = "ProductCategoryName";
        colProductCategoryName.HeaderText = "Category";
        colProductCategoryName.Name = "colProductCategoryName";
        colProductCategoryName.ReadOnly = true;
        colProductTypeName.DataPropertyName = "ProductTypeName";
        colProductTypeName.HeaderText = "Type";
        colProductTypeName.Name = "colProductTypeName";
        colProductTypeName.ReadOnly = true;
        colUnitOfMeasureName.DataPropertyName = "UnitOfMeasureName";
        colUnitOfMeasureName.HeaderText = "Unit";
        colUnitOfMeasureName.Name = "colUnitOfMeasureName";
        colUnitOfMeasureName.ReadOnly = true;
        colIsActive.DataPropertyName = "IsActive";
        colIsActive.HeaderText = "IsActive";
        colIsActive.Name = "colIsActive";
        colIsActive.ReadOnly = true;
        colCreatedAt.DataPropertyName = "CreatedAt";
        colCreatedAt.HeaderText = "CreatedAt";
        colCreatedAt.Name = "colCreatedAt";
        colCreatedAt.ReadOnly = true;
        // 
        // ProductForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 800);
        Controls.Add(dgvProducts);
        Controls.Add(pnlTop);
        FormBorderStyle = FormBorderStyle.None;
        Name = "ProductForm";
        Text = "Products Management";
        Load += ProductForm_Load;
        pnlTop.ResumeLayout(false);
        pnlTop.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvProducts).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private Panel pnlTop;
    private CheckBox chkIsActive;
    private ComboBox cmbProductType;
    private Label lblProductType;
    private ComboBox cmbProductCategory;
    private Label lblProductCategory;
    private Button btnRefresh;
    private Button btnDelete;
    private Button btnEdit;
    private Button btnAdd;
    private Button btnSearch;
    private TextBox txtName;
    private Label lblName;
    private DataGridView dgvProducts;
    private DataGridViewTextBoxColumn colId;
    private DataGridViewTextBoxColumn colName;
    private DataGridViewTextBoxColumn colDescription;
    private DataGridViewTextBoxColumn colPrice;
    private DataGridViewTextBoxColumn colStockQuantity;
    private DataGridViewTextBoxColumn colProductCategoryName;
    private DataGridViewTextBoxColumn colProductTypeName;
    private DataGridViewTextBoxColumn colUnitOfMeasureName;
    private DataGridViewCheckBoxColumn colIsActive;
    private DataGridViewTextBoxColumn colCreatedAt;
}
