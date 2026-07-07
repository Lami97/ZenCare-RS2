namespace ZenCare.WinUI.Forms;

partial class SupplierDetailsForm
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
        lblName = new Label();
        txtName = new TextBox();
        lblContactEmail = new Label();
        txtContactEmail = new TextBox();
        lblPhoneNumber = new Label();
        txtPhoneNumber = new TextBox();
        lblAddress = new Label();
        txtAddress = new TextBox();
        chkIsActive = new CheckBox();
        btnSave = new Button();
        btnCancel = new Button();
        SuspendLayout();
        // 
        // lblName
        // 
        lblName.AutoSize = true;
        lblName.Location = new Point(24, 24);
        lblName.Name = "lblName";
        lblName.Size = new Size(39, 15);
        lblName.TabIndex = 0;
        lblName.Text = "Name";
        // 
        // txtName
        // 
        txtName.Location = new Point(24, 46);
        txtName.Name = "txtName";
        txtName.Size = new Size(416, 23);
        txtName.TabIndex = 1;
        // 
        // lblContactEmail
        // 
        lblContactEmail.AutoSize = true;
        lblContactEmail.Location = new Point(24, 86);
        lblContactEmail.Name = "lblContactEmail";
        lblContactEmail.Size = new Size(36, 15);
        lblContactEmail.TabIndex = 2;
        lblContactEmail.Text = "Email";
        // 
        // txtContactEmail
        // 
        txtContactEmail.Location = new Point(24, 108);
        txtContactEmail.Name = "txtContactEmail";
        txtContactEmail.Size = new Size(416, 23);
        txtContactEmail.TabIndex = 3;
        // 
        // lblPhoneNumber
        // 
        lblPhoneNumber.AutoSize = true;
        lblPhoneNumber.Location = new Point(24, 148);
        lblPhoneNumber.Name = "lblPhoneNumber";
        lblPhoneNumber.Size = new Size(41, 15);
        lblPhoneNumber.TabIndex = 4;
        lblPhoneNumber.Text = "Phone";
        // 
        // txtPhoneNumber
        // 
        txtPhoneNumber.Location = new Point(24, 170);
        txtPhoneNumber.Name = "txtPhoneNumber";
        txtPhoneNumber.Size = new Size(416, 23);
        txtPhoneNumber.TabIndex = 5;
        // 
        // lblAddress
        // 
        lblAddress.AutoSize = true;
        lblAddress.Location = new Point(24, 210);
        lblAddress.Name = "lblAddress";
        lblAddress.Size = new Size(49, 15);
        lblAddress.TabIndex = 6;
        lblAddress.Text = "Address";
        // 
        // txtAddress
        // 
        txtAddress.Location = new Point(24, 232);
        txtAddress.Multiline = true;
        txtAddress.Name = "txtAddress";
        txtAddress.Size = new Size(416, 68);
        txtAddress.TabIndex = 7;
        // 
        // chkIsActive
        // 
        chkIsActive.AutoSize = true;
        chkIsActive.Location = new Point(24, 318);
        chkIsActive.Name = "chkIsActive";
        chkIsActive.Size = new Size(59, 19);
        chkIsActive.TabIndex = 8;
        chkIsActive.Text = "Active";
        chkIsActive.UseVisualStyleBackColor = true;
        // 
        // btnSave
        // 
        btnSave.Location = new Point(244, 356);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(92, 30);
        btnSave.TabIndex = 9;
        btnSave.Text = "Save";
        btnSave.UseVisualStyleBackColor = true;
        btnSave.Click += btnSave_Click;
        // 
        // btnCancel
        // 
        btnCancel.Location = new Point(348, 356);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(92, 30);
        btnCancel.TabIndex = 10;
        btnCancel.Text = "Cancel";
        btnCancel.UseVisualStyleBackColor = true;
        btnCancel.Click += btnCancel_Click;
        // 
        // SupplierDetailsForm
        // 
        AcceptButton = btnSave;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(464, 412);
        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Controls.Add(chkIsActive);
        Controls.Add(txtAddress);
        Controls.Add(lblAddress);
        Controls.Add(txtPhoneNumber);
        Controls.Add(lblPhoneNumber);
        Controls.Add(txtContactEmail);
        Controls.Add(lblContactEmail);
        Controls.Add(txtName);
        Controls.Add(lblName);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "SupplierDetailsForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Supplier";
        Load += SupplierDetailsForm_Load;
        ResumeLayout(false);
        PerformLayout();
    }

    private Label lblName;
    private TextBox txtName;
    private Label lblContactEmail;
    private TextBox txtContactEmail;
    private Label lblPhoneNumber;
    private TextBox txtPhoneNumber;
    private Label lblAddress;
    private TextBox txtAddress;
    private CheckBox chkIsActive;
    private Button btnSave;
    private Button btnCancel;
}
