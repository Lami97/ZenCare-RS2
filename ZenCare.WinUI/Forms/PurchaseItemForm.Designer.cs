namespace ZenCare.WinUI.Forms;

partial class PurchaseItemForm
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
        btnRefresh = new Button();
        btnDelete = new Button();
        btnEdit = new Button();
        btnAdd = new Button();
        btnSearch = new Button();
        cmbProduct = new ComboBox();
        lblProduct = new Label();
        cmbPurchase = new ComboBox();
        lblPurchase = new Label();
        dgvPurchaseItems = new DataGridView();
        colPurchaseNumber = new DataGridViewTextBoxColumn();
        colProductName = new DataGridViewTextBoxColumn();
        colQuantity = new DataGridViewTextBoxColumn();
        colUnitPrice = new DataGridViewTextBoxColumn();
        colTotalPrice = new DataGridViewTextBoxColumn();
        pnlTop.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvPurchaseItems).BeginInit();
        SuspendLayout();
        // 
        // pnlTop
        // 
        pnlTop.Controls.Add(btnRefresh);
        pnlTop.Controls.Add(btnDelete);
        pnlTop.Controls.Add(btnEdit);
        pnlTop.Controls.Add(btnAdd);
        pnlTop.Controls.Add(btnSearch);
        pnlTop.Controls.Add(cmbProduct);
        pnlTop.Controls.Add(lblProduct);
        pnlTop.Controls.Add(cmbPurchase);
        pnlTop.Controls.Add(lblPurchase);
        pnlTop.Dock = DockStyle.Top;
        pnlTop.Location = new Point(0, 0);
        pnlTop.Name = "pnlTop";
        pnlTop.Padding = new Padding(24, 20, 24, 12);
        pnlTop.Size = new Size(1000, 116);
        pnlTop.TabIndex = 0;
        btnSearch.Location = new Point(296, 28);
        btnSearch.Name = "btnSearch";
        btnSearch.Size = new Size(96, 28);
        btnSearch.TabIndex = 0;
        btnSearch.Text = "Search";
        btnSearch.UseVisualStyleBackColor = true;
        btnSearch.Click += btnSearch_Click;
        btnAdd.Location = new Point(408, 28);
        btnAdd.Name = "btnAdd";
        btnAdd.Size = new Size(96, 28);
        btnAdd.TabIndex = 1;
        btnAdd.Text = "Add";
        btnAdd.UseVisualStyleBackColor = true;
        btnAdd.Click += btnAdd_Click;
        btnEdit.Location = new Point(520, 28);
        btnEdit.Name = "btnEdit";
        btnEdit.Size = new Size(96, 28);
        btnEdit.TabIndex = 2;
        btnEdit.Text = "Edit";
        btnEdit.UseVisualStyleBackColor = true;
        btnEdit.Click += btnEdit_Click;
        btnDelete.Location = new Point(632, 28);
        btnDelete.Name = "btnDelete";
        btnDelete.Size = new Size(96, 28);
        btnDelete.TabIndex = 3;
        btnDelete.Text = "Delete";
        btnDelete.UseVisualStyleBackColor = true;
        btnDelete.Click += btnDelete_Click;
        btnRefresh.Location = new Point(744, 28);
        btnRefresh.Name = "btnRefresh";
        btnRefresh.Size = new Size(96, 28);
        btnRefresh.TabIndex = 4;
        btnRefresh.Text = "Refresh";
        btnRefresh.UseVisualStyleBackColor = true;
        btnRefresh.Click += btnRefresh_Click;
        lblPurchase.AutoSize = true;
        lblPurchase.Location = new Point(24, 34);
        lblPurchase.Name = "lblPurchase";
        lblPurchase.Size = new Size(54, 15);
        lblPurchase.Text = "Purchase";
        cmbPurchase.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbPurchase.FormattingEnabled = true;
        cmbPurchase.Location = new Point(88, 30);
        cmbPurchase.Name = "cmbPurchase";
        cmbPurchase.Size = new Size(188, 23);
        cmbPurchase.TabIndex = 6;
        lblProduct.AutoSize = true;
        lblProduct.Location = new Point(312, 74);
        lblProduct.Name = "lblProduct";
        lblProduct.Size = new Size(49, 15);
        lblProduct.Text = "Product";
        cmbProduct.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbProduct.FormattingEnabled = true;
        cmbProduct.Location = new Point(368, 70);
        cmbProduct.Name = "cmbProduct";
        cmbProduct.Size = new Size(220, 23);
        cmbProduct.TabIndex = 8;
        // 
        // dgvPurchaseItems
        // 
        dgvPurchaseItems.AllowUserToAddRows = false;
        dgvPurchaseItems.AllowUserToDeleteRows = false;
        dgvPurchaseItems.AutoGenerateColumns = false;
        dgvPurchaseItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvPurchaseItems.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvPurchaseItems.Columns.AddRange(new DataGridViewColumn[] { colPurchaseNumber, colProductName, colQuantity, colUnitPrice, colTotalPrice });
        dgvPurchaseItems.Dock = DockStyle.Fill;
        dgvPurchaseItems.Location = new Point(0, 116);
        dgvPurchaseItems.MultiSelect = false;
        dgvPurchaseItems.Name = "dgvPurchaseItems";
        dgvPurchaseItems.ReadOnly = true;
        dgvPurchaseItems.RowHeadersVisible = false;
        dgvPurchaseItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvPurchaseItems.Size = new Size(1000, 684);
        dgvPurchaseItems.TabIndex = 1;
        colPurchaseNumber.DataPropertyName = "PurchaseNumber";
        colPurchaseNumber.HeaderText = "Purchase";
        colPurchaseNumber.Name = "colPurchaseNumber";
        colPurchaseNumber.ReadOnly = true;
        colProductName.DataPropertyName = "ProductName";
        colProductName.HeaderText = "Product";
        colProductName.Name = "colProductName";
        colProductName.ReadOnly = true;
        colQuantity.DataPropertyName = "Quantity";
        colQuantity.HeaderText = "Quantity";
        colQuantity.Name = "colQuantity";
        colQuantity.ReadOnly = true;
        colUnitPrice.DataPropertyName = "UnitPrice";
        colUnitPrice.HeaderText = "Unit Price";
        colUnitPrice.Name = "colUnitPrice";
        colUnitPrice.ReadOnly = true;
        colTotalPrice.DataPropertyName = "TotalPrice";
        colTotalPrice.HeaderText = "Total";
        colTotalPrice.Name = "colTotalPrice";
        colTotalPrice.ReadOnly = true;
        // 
        // PurchaseItemForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 800);
        Controls.Add(dgvPurchaseItems);
        Controls.Add(pnlTop);
        FormBorderStyle = FormBorderStyle.None;
        Name = "PurchaseItemForm";
        Text = "Purchase Items";
        Load += PurchaseItemForm_Load;
        pnlTop.ResumeLayout(false);
        pnlTop.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvPurchaseItems).EndInit();
        ResumeLayout(false);
    }

    private Panel pnlTop;
    private Button btnRefresh;
    private Button btnDelete;
    private Button btnEdit;
    private Button btnAdd;
    private Button btnSearch;
    private ComboBox cmbProduct;
    private Label lblProduct;
    private ComboBox cmbPurchase;
    private Label lblPurchase;
    private DataGridView dgvPurchaseItems;
    private DataGridViewTextBoxColumn colPurchaseNumber;
    private DataGridViewTextBoxColumn colProductName;
    private DataGridViewTextBoxColumn colQuantity;
    private DataGridViewTextBoxColumn colUnitPrice;
    private DataGridViewTextBoxColumn colTotalPrice;
}
