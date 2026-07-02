namespace ZenCare.WinUI.Forms;

partial class ProductDetailsForm
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
        lblName = new Label();
        txtName = new TextBox();
        lblDescription = new Label();
        txtDescription = new TextBox();
        lblPrice = new Label();
        nudPrice = new NumericUpDown();
        lblStockQuantity = new Label();
        nudStockQuantity = new NumericUpDown();
        lblProductCategory = new Label();
        cmbProductCategory = new ComboBox();
        lblProductType = new Label();
        cmbProductType = new ComboBox();
        lblUnitOfMeasure = new Label();
        cmbUnitOfMeasure = new ComboBox();
        chkIsActive = new CheckBox();
        btnSave = new Button();
        btnCancel = new Button();
        ((System.ComponentModel.ISupportInitialize)nudPrice).BeginInit();
        ((System.ComponentModel.ISupportInitialize)nudStockQuantity).BeginInit();
        SuspendLayout();
        // 
        // labels and inputs
        // 
        lblName.AutoSize = true;
        lblName.Location = new Point(24, 24);
        lblName.Name = "lblName";
        lblName.Size = new Size(39, 15);
        lblName.Text = "Name";
        txtName.Location = new Point(24, 46);
        txtName.Name = "txtName";
        txtName.Size = new Size(416, 23);
        txtName.TabIndex = 1;
        lblDescription.AutoSize = true;
        lblDescription.Location = new Point(24, 84);
        lblDescription.Name = "lblDescription";
        lblDescription.Size = new Size(67, 15);
        lblDescription.Text = "Description";
        txtDescription.Location = new Point(24, 106);
        txtDescription.Multiline = true;
        txtDescription.Name = "txtDescription";
        txtDescription.Size = new Size(416, 60);
        txtDescription.TabIndex = 3;
        lblPrice.AutoSize = true;
        lblPrice.Location = new Point(24, 184);
        lblPrice.Name = "lblPrice";
        lblPrice.Size = new Size(33, 15);
        lblPrice.Text = "Price";
        nudPrice.DecimalPlaces = 2;
        nudPrice.Location = new Point(24, 206);
        nudPrice.Maximum = 1000000;
        nudPrice.Minimum = 0.01m;
        nudPrice.Name = "nudPrice";
        nudPrice.Size = new Size(196, 23);
        nudPrice.TabIndex = 5;
        nudPrice.Value = 0.01m;
        lblStockQuantity.AutoSize = true;
        lblStockQuantity.Location = new Point(244, 184);
        lblStockQuantity.Name = "lblStockQuantity";
        lblStockQuantity.Size = new Size(85, 15);
        lblStockQuantity.Text = "Stock quantity";
        nudStockQuantity.Location = new Point(244, 206);
        nudStockQuantity.Maximum = 1000000;
        nudStockQuantity.Name = "nudStockQuantity";
        nudStockQuantity.Size = new Size(196, 23);
        nudStockQuantity.TabIndex = 7;
        lblProductCategory.AutoSize = true;
        lblProductCategory.Location = new Point(24, 248);
        lblProductCategory.Name = "lblProductCategory";
        lblProductCategory.Size = new Size(99, 15);
        lblProductCategory.Text = "Product category";
        cmbProductCategory.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbProductCategory.FormattingEnabled = true;
        cmbProductCategory.Location = new Point(24, 270);
        cmbProductCategory.Name = "cmbProductCategory";
        cmbProductCategory.Size = new Size(416, 23);
        cmbProductCategory.TabIndex = 9;
        lblProductType.AutoSize = true;
        lblProductType.Location = new Point(24, 312);
        lblProductType.Name = "lblProductType";
        lblProductType.Size = new Size(75, 15);
        lblProductType.Text = "Product type";
        cmbProductType.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbProductType.FormattingEnabled = true;
        cmbProductType.Location = new Point(24, 334);
        cmbProductType.Name = "cmbProductType";
        cmbProductType.Size = new Size(416, 23);
        cmbProductType.TabIndex = 11;
        lblUnitOfMeasure.AutoSize = true;
        lblUnitOfMeasure.Location = new Point(24, 376);
        lblUnitOfMeasure.Name = "lblUnitOfMeasure";
        lblUnitOfMeasure.Size = new Size(91, 15);
        lblUnitOfMeasure.Text = "Unit of measure";
        cmbUnitOfMeasure.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbUnitOfMeasure.FormattingEnabled = true;
        cmbUnitOfMeasure.Location = new Point(24, 398);
        cmbUnitOfMeasure.Name = "cmbUnitOfMeasure";
        cmbUnitOfMeasure.Size = new Size(416, 23);
        cmbUnitOfMeasure.TabIndex = 13;
        chkIsActive.AutoSize = true;
        chkIsActive.Location = new Point(24, 440);
        chkIsActive.Name = "chkIsActive";
        chkIsActive.Size = new Size(68, 19);
        chkIsActive.TabIndex = 14;
        chkIsActive.Text = "IsActive";
        chkIsActive.UseVisualStyleBackColor = true;
        btnSave.Location = new Point(264, 480);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(84, 32);
        btnSave.TabIndex = 15;
        btnSave.Text = "Save";
        btnSave.UseVisualStyleBackColor = true;
        btnSave.Click += btnSave_Click;
        btnCancel.Location = new Point(356, 480);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(84, 32);
        btnCancel.TabIndex = 16;
        btnCancel.Text = "Cancel";
        btnCancel.UseVisualStyleBackColor = true;
        btnCancel.Click += btnCancel_Click;
        // 
        // ProductDetailsForm
        // 
        AcceptButton = btnSave;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(464, 537);
        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Controls.Add(chkIsActive);
        Controls.Add(cmbUnitOfMeasure);
        Controls.Add(lblUnitOfMeasure);
        Controls.Add(cmbProductType);
        Controls.Add(lblProductType);
        Controls.Add(cmbProductCategory);
        Controls.Add(lblProductCategory);
        Controls.Add(nudStockQuantity);
        Controls.Add(lblStockQuantity);
        Controls.Add(nudPrice);
        Controls.Add(lblPrice);
        Controls.Add(txtDescription);
        Controls.Add(lblDescription);
        Controls.Add(txtName);
        Controls.Add(lblName);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "ProductDetailsForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Product";
        Load += ProductDetailsForm_Load;
        ((System.ComponentModel.ISupportInitialize)nudPrice).EndInit();
        ((System.ComponentModel.ISupportInitialize)nudStockQuantity).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblName;
    private TextBox txtName;
    private Label lblDescription;
    private TextBox txtDescription;
    private Label lblPrice;
    private NumericUpDown nudPrice;
    private Label lblStockQuantity;
    private NumericUpDown nudStockQuantity;
    private Label lblProductCategory;
    private ComboBox cmbProductCategory;
    private Label lblProductType;
    private ComboBox cmbProductType;
    private Label lblUnitOfMeasure;
    private ComboBox cmbUnitOfMeasure;
    private CheckBox chkIsActive;
    private Button btnSave;
    private Button btnCancel;
}
