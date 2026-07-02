namespace ZenCare.WinUI.Forms;

partial class PurchaseDetailsForm
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
        lblUser = new Label();
        cmbUser = new ComboBox();
        lblPurchaseNumber = new Label();
        txtPurchaseNumber = new TextBox();
        lblStatus = new Label();
        cmbStatus = new ComboBox();
        lblPaymentStatus = new Label();
        cmbPaymentStatus = new ComboBox();
        lblTotalAmount = new Label();
        nudTotalAmount = new NumericUpDown();
        lblStripePaymentIntentId = new Label();
        txtStripePaymentIntentId = new TextBox();
        chkPaidAt = new CheckBox();
        dtpPaidAt = new DateTimePicker();
        btnSave = new Button();
        btnCancel = new Button();
        ((System.ComponentModel.ISupportInitialize)nudTotalAmount).BeginInit();
        SuspendLayout();
        // 
        // fields
        // 
        lblUser.AutoSize = true;
        lblUser.Location = new Point(24, 24);
        lblUser.Name = "lblUser";
        lblUser.Size = new Size(30, 15);
        lblUser.Text = "User";
        cmbUser.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbUser.FormattingEnabled = true;
        cmbUser.Location = new Point(24, 46);
        cmbUser.Name = "cmbUser";
        cmbUser.Size = new Size(416, 23);
        cmbUser.TabIndex = 1;
        lblPurchaseNumber.AutoSize = true;
        lblPurchaseNumber.Location = new Point(24, 84);
        lblPurchaseNumber.Name = "lblPurchaseNumber";
        lblPurchaseNumber.Size = new Size(99, 15);
        lblPurchaseNumber.Text = "Purchase number";
        txtPurchaseNumber.Location = new Point(24, 106);
        txtPurchaseNumber.Name = "txtPurchaseNumber";
        txtPurchaseNumber.Size = new Size(416, 23);
        txtPurchaseNumber.TabIndex = 3;
        lblStatus.AutoSize = true;
        lblStatus.Location = new Point(24, 144);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(39, 15);
        lblStatus.Text = "Status";
        cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbStatus.FormattingEnabled = true;
        cmbStatus.Location = new Point(24, 166);
        cmbStatus.Name = "cmbStatus";
        cmbStatus.Size = new Size(196, 23);
        cmbStatus.TabIndex = 5;
        lblPaymentStatus.AutoSize = true;
        lblPaymentStatus.Location = new Point(244, 144);
        lblPaymentStatus.Name = "lblPaymentStatus";
        lblPaymentStatus.Size = new Size(87, 15);
        lblPaymentStatus.Text = "Payment status";
        cmbPaymentStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbPaymentStatus.FormattingEnabled = true;
        cmbPaymentStatus.Location = new Point(244, 166);
        cmbPaymentStatus.Name = "cmbPaymentStatus";
        cmbPaymentStatus.Size = new Size(196, 23);
        cmbPaymentStatus.TabIndex = 7;
        lblTotalAmount.AutoSize = true;
        lblTotalAmount.Location = new Point(24, 204);
        lblTotalAmount.Name = "lblTotalAmount";
        lblTotalAmount.Size = new Size(77, 15);
        lblTotalAmount.Text = "Total amount";
        nudTotalAmount.DecimalPlaces = 2;
        nudTotalAmount.Location = new Point(24, 226);
        nudTotalAmount.Maximum = 1000000;
        nudTotalAmount.Name = "nudTotalAmount";
        nudTotalAmount.Size = new Size(196, 23);
        nudTotalAmount.TabIndex = 9;
        lblStripePaymentIntentId.AutoSize = true;
        lblStripePaymentIntentId.Location = new Point(24, 264);
        lblStripePaymentIntentId.Name = "lblStripePaymentIntentId";
        lblStripePaymentIntentId.Size = new Size(130, 15);
        lblStripePaymentIntentId.Text = "Stripe payment intent";
        txtStripePaymentIntentId.Location = new Point(24, 286);
        txtStripePaymentIntentId.Name = "txtStripePaymentIntentId";
        txtStripePaymentIntentId.Size = new Size(416, 23);
        txtStripePaymentIntentId.TabIndex = 11;
        chkPaidAt.AutoSize = true;
        chkPaidAt.Location = new Point(24, 328);
        chkPaidAt.Name = "chkPaidAt";
        chkPaidAt.Size = new Size(62, 19);
        chkPaidAt.TabIndex = 12;
        chkPaidAt.Text = "Paid at";
        chkPaidAt.UseVisualStyleBackColor = true;
        dtpPaidAt.CustomFormat = "yyyy-MM-dd HH:mm";
        dtpPaidAt.Format = DateTimePickerFormat.Custom;
        dtpPaidAt.Location = new Point(104, 326);
        dtpPaidAt.Name = "dtpPaidAt";
        dtpPaidAt.Size = new Size(180, 23);
        dtpPaidAt.TabIndex = 13;
        btnSave.Location = new Point(264, 384);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(84, 32);
        btnSave.TabIndex = 14;
        btnSave.Text = "Save";
        btnSave.UseVisualStyleBackColor = true;
        btnSave.Click += btnSave_Click;
        btnCancel.Location = new Point(356, 384);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(84, 32);
        btnCancel.TabIndex = 15;
        btnCancel.Text = "Cancel";
        btnCancel.UseVisualStyleBackColor = true;
        btnCancel.Click += btnCancel_Click;
        // 
        // PurchaseDetailsForm
        // 
        AcceptButton = btnSave;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(464, 441);
        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Controls.Add(dtpPaidAt);
        Controls.Add(chkPaidAt);
        Controls.Add(txtStripePaymentIntentId);
        Controls.Add(lblStripePaymentIntentId);
        Controls.Add(nudTotalAmount);
        Controls.Add(lblTotalAmount);
        Controls.Add(cmbPaymentStatus);
        Controls.Add(lblPaymentStatus);
        Controls.Add(cmbStatus);
        Controls.Add(lblStatus);
        Controls.Add(txtPurchaseNumber);
        Controls.Add(lblPurchaseNumber);
        Controls.Add(cmbUser);
        Controls.Add(lblUser);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "PurchaseDetailsForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Purchase";
        Load += PurchaseDetailsForm_Load;
        ((System.ComponentModel.ISupportInitialize)nudTotalAmount).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private Label lblUser;
    private ComboBox cmbUser;
    private Label lblPurchaseNumber;
    private TextBox txtPurchaseNumber;
    private Label lblStatus;
    private ComboBox cmbStatus;
    private Label lblPaymentStatus;
    private ComboBox cmbPaymentStatus;
    private Label lblTotalAmount;
    private NumericUpDown nudTotalAmount;
    private Label lblStripePaymentIntentId;
    private TextBox txtStripePaymentIntentId;
    private CheckBox chkPaidAt;
    private DateTimePicker dtpPaidAt;
    private Button btnSave;
    private Button btnCancel;
}
