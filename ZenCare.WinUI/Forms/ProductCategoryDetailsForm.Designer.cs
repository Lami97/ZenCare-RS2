namespace ZenCare.WinUI.Forms;

partial class ProductCategoryDetailsForm
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
        chkIsActive = new CheckBox();
        btnSave = new Button();
        btnCancel = new Button();
        SuspendLayout();
        // 
        // lblName
        // 
        lblName.AutoSize = true;
        lblName.Location = new Point(24, 30);
        lblName.Name = "lblName";
        lblName.Size = new Size(39, 15);
        lblName.TabIndex = 0;
        lblName.Text = "Name";
        // 
        // txtName
        // 
        txtName.Location = new Point(24, 52);
        txtName.Name = "txtName";
        txtName.Size = new Size(336, 23);
        txtName.TabIndex = 1;
        // 
        // chkIsActive
        // 
        chkIsActive.AutoSize = true;
        chkIsActive.Location = new Point(24, 96);
        chkIsActive.Name = "chkIsActive";
        chkIsActive.Size = new Size(68, 19);
        chkIsActive.TabIndex = 2;
        chkIsActive.Text = "IsActive";
        chkIsActive.UseVisualStyleBackColor = true;
        // 
        // btnSave
        // 
        btnSave.Location = new Point(184, 144);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(84, 32);
        btnSave.TabIndex = 3;
        btnSave.Text = "Save";
        btnSave.UseVisualStyleBackColor = true;
        btnSave.Click += btnSave_Click;
        // 
        // btnCancel
        // 
        btnCancel.Location = new Point(276, 144);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(84, 32);
        btnCancel.TabIndex = 4;
        btnCancel.Text = "Cancel";
        btnCancel.UseVisualStyleBackColor = true;
        btnCancel.Click += btnCancel_Click;
        // 
        // ProductCategoryDetailsForm
        // 
        AcceptButton = btnSave;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(384, 201);
        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Controls.Add(chkIsActive);
        Controls.Add(txtName);
        Controls.Add(lblName);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "ProductCategoryDetailsForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Product Category";
        Load += ProductCategoryDetailsForm_Load;
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblName;
    private TextBox txtName;
    private CheckBox chkIsActive;
    private Button btnSave;
    private Button btnCancel;
}
