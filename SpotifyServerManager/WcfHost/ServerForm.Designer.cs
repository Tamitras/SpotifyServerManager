namespace WcfHost
{
    partial class ServerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.listBoxUsers = new System.Windows.Forms.ListBox();
            this.lblConnectedUser = new System.Windows.Forms.Label();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.listBoxMember = new System.Windows.Forms.ListBox();
            this.bindingSourceConnectedMember = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceConnectedMember)).BeginInit();
            this.SuspendLayout();
            // 
            // listBoxUsers
            // 
            this.listBoxUsers.FormattingEnabled = true;
            this.listBoxUsers.Location = new System.Drawing.Point(12, 36);
            this.listBoxUsers.Name = "listBoxUsers";
            this.listBoxUsers.Size = new System.Drawing.Size(218, 134);
            this.listBoxUsers.TabIndex = 0;
            // 
            // lblConnectedUser
            // 
            this.lblConnectedUser.AutoSize = true;
            this.lblConnectedUser.Location = new System.Drawing.Point(25, 20);
            this.lblConnectedUser.Name = "lblConnectedUser";
            this.lblConnectedUser.Size = new System.Drawing.Size(84, 13);
            this.lblConnectedUser.TabIndex = 1;
            this.lblConnectedUser.Text = "Connected User";
            // 
            // textBoxLog
            // 
            this.textBoxLog.Location = new System.Drawing.Point(236, 36);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.Size = new System.Drawing.Size(416, 358);
            this.textBoxLog.TabIndex = 2;
            // 
            // listBoxMember
            // 
            this.listBoxMember.DataSource = this.bindingSourceConnectedMember;
            this.listBoxMember.DisplayMember = "IPAddress";
            this.listBoxMember.FormattingEnabled = true;
            this.listBoxMember.Location = new System.Drawing.Point(12, 176);
            this.listBoxMember.Name = "listBoxMember";
            this.listBoxMember.Size = new System.Drawing.Size(218, 134);
            this.listBoxMember.TabIndex = 3;
            // 
            // ServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 406);
            this.Controls.Add(this.listBoxMember);
            this.Controls.Add(this.textBoxLog);
            this.Controls.Add(this.lblConnectedUser);
            this.Controls.Add(this.listBoxUsers);
            this.Name = "ServerForm";
            this.Text = "ServerForm";
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceConnectedMember)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxUsers;
        private System.Windows.Forms.Label lblConnectedUser;
        private System.Windows.Forms.TextBox textBoxLog;
        private System.Windows.Forms.ListBox listBoxMember;
        private System.Windows.Forms.BindingSource bindingSourceConnectedMember;
    }
}