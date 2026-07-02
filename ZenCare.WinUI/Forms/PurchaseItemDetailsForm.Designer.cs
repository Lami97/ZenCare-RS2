namespace ZenCare.WinUI.Forms;

partial class PurchaseItemDetailsForm
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
        lblPurchase = new Label();
        cmbPurchase = new ComboBox();
        lblProduct = new Label();
        cmbProduct = new ComboBox();
        lblQuantity = new Label();
        nudQuantity = new NumericUpDown();
        lblUnitPrice = new Label();
        nudUnitPrice = new NumericUpDown();
        lblTotalPrice = new Label();
        nudTotalPrice = new NumericUpDown();
        btnSave = new Button();
        btnCancel = new Button();
        ((System.ComponentModel.ISupportInitialize)nudQuantity).BeginInit();
        ((System.ComponentModel.ISupportInitialize)nudUnitPrice).BeginInit();
        ((System.ComponentModel.ISupportInitialize)nudTotalPrice).BeginInit();
        SuspendLayout();
        // 
        // lblPurchase
        // 
        lblPurchase.AutoSize = true;
        lblPurchase.Location = new Point(24, 24);
        lblPurchase.Name = "lblPurchase";
        lblPurchase.Size = new Size(54, 15);
        lblPurchase.TabIndex = 0;
        lblPurchase.Text = "Purchase";
        // 
        // cmbPurchase
        // 
        cmbPurchase.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbPurchase.FormattingEnabled = true;
        cmbPurchase.Location = new Point(24, 46);
        cmbPurchase.Name = "cmbPurchase";
        cmbPurchase.Size = new Size(416, 23);
        cmbPurchase.TabIndex = 1;
        // 
        // lblProduct
        // 
        lblProduct.AutoSize = true;
        lblProduct.Location = new Point(24, 86);
        lblProduct.Name = "lblProduct";
        lblProduct.Size = new Size(49, 15);
        lblProduct.TabIndex = 2;
        lblProduct.Text = "Product";
        // 
        // cmbProduct
        // 
        cmbProduct.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbProduct.FormattingEnabled = true;
        cmbProduct.Location = new Point(24, 108);
        cmbProduct.Name = "cmbProduct";
        cmbProduct.Size = new Size(416, 23);
        cmbProduct.TabIndex = 3;
        // 
        // lblQuantity
        // 
        lblQuantity.AutoSize = true;
        lblQuantity.Location = new Point(24, 148);
        lblQuantity.Name = "lblQuantity";
        lblQuantity.Size = new Size(53, 15);
        lblQuantity.TabIndex = 4;
        lblQuantity.Text = "Quantity";
        // 
        // nudQuantity
        // 
        nudQuantity.Location = new Point(24, 170);
        nudQuantity.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
        nudQuantity.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        nudQuantity.Name = "nudQuantity";
        nudQuantity.Size = new Size(196, 23);
        nudQuantity.TabIndex = 5;
        nudQuantity.Value = new decimal(new int[] { 1, 0, 0, 0 });
        nudQuantity.ValueChanged += nudQuantity_ValueChanged;
        // 
        // lblUnitPrice
        // 
        lblUnitPrice.AutoSize = true;
        lblUnitPrice.Location = new Point(244, 148);
        lblUnitPrice.Name = "lblUnitPrice";
        lblUnitPrice.Size = new Size(57, 15);
        lblUnitPrice.TabIndex = 6;
        lblUnitPrice.Text = "Unit Price";
        // 
        // nudUnitPrice
        // 
        nudUnitPrice.DecimalPlaces = 2;
        nudUnitPrice.Location = new Point(244, 170);
        nudUnitPrice.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
        nudUnitPrice.Name = "nudUnitPrice";
        nudUnitPrice.Size = new Size(196, 23);
        nudUnitPrice.TabIndex = 7;
        nudUnitPrice.ValueChanged += nudUnitPrice_ValueChanged;
        // 
        // lblTotalPrice
        // 
        lblTotalPrice.AutoSize = true;
        lblTotalPrice.Location = new Point(24, 210);
        lblTotalPrice.Name = "lblTotalPrice";
        lblTotalPrice.Size = new Size(60, 15);
        lblTotalPrice.TabIndex = 8;
        lblTotalPrice.Text = "Total Price";
        // 
        // nudTotalPrice
        // 
        nudTotalPrice.DecimalPlaces = 2;
        nudTotalPrice.Location = new Point(24, 232);
        nudTotalPrice.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
        nudTotalPrice.Name = "nudTotalPrice";
        nudTotalPrice.Size = new Size(196, 23);
        nudTotalPrice.TabIndex = 9;
        // 
        // btnSave
        // 
        btnSave.Location = new Point(244, 292);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(92, 30);
        btnSave.TabIndex = 10;
        btnSave.Text = "Save";
        btnSave.UseVisualStyleBackColor = true;
        btnSave.Click += btnSave_Click;
        // 
        // btnCancel
        // 
        btnCancel.Location = new Point(348, 292);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(92, 30);
        btnCancel.TabIndex = 11;
        btnCancel.Text = "Cancel";
        btnCancel.UseVisualStyleBackColor = true;
        btnCancel.Click += btnCancel_Click;
        // 
        // PurchaseItemDetailsForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(464, 349);
        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Controls.Add(nudTotalPrice);
        Controls.Add(lblTotalPrice);
        Controls.Add(nudUnitPrice);
        Controls.Add(lblUnitPrice);
        Controls.Add(nudQuantity);
        Controls.Add(lblQuantity);
        Controls.Add(cmbProduct);
        Controls.Add(lblProduct);
        Controls.Add(cmbPurchase);
        Controls.Add(lblPurchase);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "PurchaseItemDetailsForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Add Purchase Item";
        Load += PurchaseItemDetailsForm_Load;
        ((System.ComponentModel.ISupportInitialize)nudQuantity).EndInit();
        ((System.ComponentModel.ISupportInitialize)nudUnitPrice).EndInit();
        ((System.ComponentModel.ISupportInitialize)nudTotalPrice).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private Label lblPurchase;
    private ComboBox cmbPurchase;
    private Label lblProduct;
    private ComboBox cmbProduct;
    private Label lblQuantity;
    private NumericUpDown nudQuantity;
    private Label lblUnitPrice;
    private NumericUpDown nudUnitPrice;
    private Label lblTotalPrice;
    private NumericUpDown nudTotalPrice;
    private Button btnSave;
    private Button btnCancel;
}
