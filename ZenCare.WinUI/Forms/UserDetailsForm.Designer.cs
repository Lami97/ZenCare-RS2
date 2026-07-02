namespace ZenCare.WinUI.Forms;

partial class UserDetailsForm
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
        lblFirstName = new Label();
        txtFirstName = new TextBox();
        lblLastName = new Label();
        txtLastName = new TextBox();
        lblEmail = new Label();
        txtEmail = new TextBox();
        lblUsername = new Label();
        txtUsername = new TextBox();
        lblPhoneNumber = new Label();
        txtPhoneNumber = new TextBox();
        lblPassword = new Label();
        txtPassword = new TextBox();
        lblPasswordConfirm = new Label();
        txtPasswordConfirm = new TextBox();
        chkIsActive = new CheckBox();
        btnSave = new Button();
        btnCancel = new Button();
        SuspendLayout();
        // 
        // lblFirstName
        // 
        lblFirstName.AutoSize = true;
        lblFirstName.Location = new Point(24, 24);
        lblFirstName.Name = "lblFirstName";
        lblFirstName.Size = new Size(27, 15);
        lblFirstName.TabIndex = 0;
        lblFirstName.Text = "Ime";
        // 
        // txtFirstName
        // 
        txtFirstName.Location = new Point(24, 46);
        txtFirstName.Name = "txtFirstName";
        txtFirstName.Size = new Size(196, 23);
        txtFirstName.TabIndex = 1;
        // 
        // lblLastName
        // 
        lblLastName.AutoSize = true;
        lblLastName.Location = new Point(244, 24);
        lblLastName.Name = "lblLastName";
        lblLastName.Size = new Size(48, 15);
        lblLastName.TabIndex = 2;
        lblLastName.Text = "Prezime";
        // 
        // txtLastName
        // 
        txtLastName.Location = new Point(244, 46);
        txtLastName.Name = "txtLastName";
        txtLastName.Size = new Size(196, 23);
        txtLastName.TabIndex = 3;
        // 
        // lblEmail
        // 
        lblEmail.AutoSize = true;
        lblEmail.Location = new Point(24, 84);
        lblEmail.Name = "lblEmail";
        lblEmail.Size = new Size(36, 15);
        lblEmail.TabIndex = 4;
        lblEmail.Text = "Email";
        // 
        // txtEmail
        // 
        txtEmail.Location = new Point(24, 106);
        txtEmail.Name = "txtEmail";
        txtEmail.Size = new Size(416, 23);
        txtEmail.TabIndex = 5;
        // 
        // lblUsername
        // 
        lblUsername.AutoSize = true;
        lblUsername.Location = new Point(24, 144);
        lblUsername.Name = "lblUsername";
        lblUsername.Size = new Size(89, 15);
        lblUsername.TabIndex = 6;
        lblUsername.Text = "Korisničko ime";
        // 
        // txtUsername
        // 
        txtUsername.Location = new Point(24, 166);
        txtUsername.Name = "txtUsername";
        txtUsername.Size = new Size(416, 23);
        txtUsername.TabIndex = 7;
        // 
        // lblPhoneNumber
        // 
        lblPhoneNumber.AutoSize = true;
        lblPhoneNumber.Location = new Point(24, 204);
        lblPhoneNumber.Name = "lblPhoneNumber";
        lblPhoneNumber.Size = new Size(43, 15);
        lblPhoneNumber.TabIndex = 8;
        lblPhoneNumber.Text = "Telefon";
        // 
        // txtPhoneNumber
        // 
        txtPhoneNumber.Location = new Point(24, 226);
        txtPhoneNumber.Name = "txtPhoneNumber";
        txtPhoneNumber.Size = new Size(416, 23);
        txtPhoneNumber.TabIndex = 9;
        // 
        // lblPassword
        // 
        lblPassword.AutoSize = true;
        lblPassword.Location = new Point(24, 264);
        lblPassword.Name = "lblPassword";
        lblPassword.Size = new Size(47, 15);
        lblPassword.TabIndex = 10;
        lblPassword.Text = "Lozinka";
        // 
        // txtPassword
        // 
        txtPassword.Location = new Point(24, 286);
        txtPassword.Name = "txtPassword";
        txtPassword.Size = new Size(416, 23);
        txtPassword.TabIndex = 11;
        txtPassword.UseSystemPasswordChar = true;
        // 
        // lblPasswordConfirm
        // 
        lblPasswordConfirm.AutoSize = true;
        lblPasswordConfirm.Location = new Point(24, 324);
        lblPasswordConfirm.Name = "lblPasswordConfirm";
        lblPasswordConfirm.Size = new Size(84, 15);
        lblPasswordConfirm.TabIndex = 12;
        lblPasswordConfirm.Text = "Potvrda lozinke";
        // 
        // txtPasswordConfirm
        // 
        txtPasswordConfirm.Location = new Point(24, 346);
        txtPasswordConfirm.Name = "txtPasswordConfirm";
        txtPasswordConfirm.Size = new Size(416, 23);
        txtPasswordConfirm.TabIndex = 13;
        txtPasswordConfirm.UseSystemPasswordChar = true;
        // 
        // chkIsActive
        // 
        chkIsActive.AutoSize = true;
        chkIsActive.Location = new Point(24, 388);
        chkIsActive.Name = "chkIsActive";
        chkIsActive.Size = new Size(105, 19);
        chkIsActive.TabIndex = 14;
        chkIsActive.Text = "Aktivan korisnik";
        chkIsActive.UseVisualStyleBackColor = true;
        // 
        // btnSave
        // 
        btnSave.Location = new Point(264, 428);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(84, 32);
        btnSave.TabIndex = 15;
        btnSave.Text = "Sačuvaj";
        btnSave.UseVisualStyleBackColor = true;
        btnSave.Click += btnSave_Click;
        // 
        // btnCancel
        // 
        btnCancel.Location = new Point(356, 428);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(84, 32);
        btnCancel.TabIndex = 16;
        btnCancel.Text = "Odustani";
        btnCancel.UseVisualStyleBackColor = true;
        btnCancel.Click += btnCancel_Click;
        // 
        // UserDetailsForm
        // 
        AcceptButton = btnSave;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(464, 485);
        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Controls.Add(chkIsActive);
        Controls.Add(txtPasswordConfirm);
        Controls.Add(lblPasswordConfirm);
        Controls.Add(txtPassword);
        Controls.Add(lblPassword);
        Controls.Add(txtPhoneNumber);
        Controls.Add(lblPhoneNumber);
        Controls.Add(txtUsername);
        Controls.Add(lblUsername);
        Controls.Add(txtEmail);
        Controls.Add(lblEmail);
        Controls.Add(txtLastName);
        Controls.Add(lblLastName);
        Controls.Add(txtFirstName);
        Controls.Add(lblFirstName);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "UserDetailsForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Korisnik";
        Load += UserDetailsForm_Load;
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblFirstName;
    private TextBox txtFirstName;
    private Label lblLastName;
    private TextBox txtLastName;
    private Label lblEmail;
    private TextBox txtEmail;
    private Label lblUsername;
    private TextBox txtUsername;
    private Label lblPhoneNumber;
    private TextBox txtPhoneNumber;
    private Label lblPassword;
    private TextBox txtPassword;
    private Label lblPasswordConfirm;
    private TextBox txtPasswordConfirm;
    private CheckBox chkIsActive;
    private Button btnSave;
    private Button btnCancel;
}
