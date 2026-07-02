namespace ZenCare.WinUI.Forms;

partial class UserRoleDetailsForm
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
        lblUser = new Label();
        cmbUser = new ComboBox();
        lblRole = new Label();
        cmbRole = new ComboBox();
        btnSave = new Button();
        btnCancel = new Button();
        SuspendLayout();
        // 
        // lblUser
        // 
        lblUser.AutoSize = true;
        lblUser.Location = new Point(24, 24);
        lblUser.Name = "lblUser";
        lblUser.Size = new Size(50, 15);
        lblUser.TabIndex = 0;
        lblUser.Text = "Korisnik";
        // 
        // cmbUser
        // 
        cmbUser.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbUser.FormattingEnabled = true;
        cmbUser.Location = new Point(24, 46);
        cmbUser.Name = "cmbUser";
        cmbUser.Size = new Size(416, 23);
        cmbUser.TabIndex = 1;
        // 
        // lblRole
        // 
        lblRole.AutoSize = true;
        lblRole.Location = new Point(24, 84);
        lblRole.Name = "lblRole";
        lblRole.Size = new Size(40, 15);
        lblRole.TabIndex = 2;
        lblRole.Text = "Uloga";
        // 
        // cmbRole
        // 
        cmbRole.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbRole.FormattingEnabled = true;
        cmbRole.Location = new Point(24, 106);
        cmbRole.Name = "cmbRole";
        cmbRole.Size = new Size(416, 23);
        cmbRole.TabIndex = 3;
        // 
        // btnSave
        // 
        btnSave.Location = new Point(264, 164);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(84, 32);
        btnSave.TabIndex = 4;
        btnSave.Text = "Sačuvaj";
        btnSave.UseVisualStyleBackColor = true;
        btnSave.Click += btnSave_Click;
        // 
        // btnCancel
        // 
        btnCancel.Location = new Point(356, 164);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(84, 32);
        btnCancel.TabIndex = 5;
        btnCancel.Text = "Odustani";
        btnCancel.UseVisualStyleBackColor = true;
        btnCancel.Click += btnCancel_Click;
        // 
        // UserRoleDetailsForm
        // 
        AcceptButton = btnSave;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(464, 221);
        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Controls.Add(cmbRole);
        Controls.Add(lblRole);
        Controls.Add(cmbUser);
        Controls.Add(lblUser);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "UserRoleDetailsForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Dodijeljena uloga";
        Load += UserRoleDetailsForm_Load;
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblUser;
    private ComboBox cmbUser;
    private Label lblRole;
    private ComboBox cmbRole;
    private Button btnSave;
    private Button btnCancel;
}
