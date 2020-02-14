namespace ShortCutSpotify
{
    partial class TestDllForm
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
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.textBoxCurrentSongName = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.buttonIncreaseVolume = new System.Windows.Forms.Button();
            this.buttonDecreaseVolume = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(183, 27);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Next";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(8, 27);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 2;
            this.button3.Text = "Previous";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // textBoxCurrentSongName
            // 
            this.textBoxCurrentSongName.Location = new System.Drawing.Point(12, 109);
            this.textBoxCurrentSongName.Name = "textBoxCurrentSongName";
            this.textBoxCurrentSongName.Size = new System.Drawing.Size(440, 20);
            this.textBoxCurrentSongName.TabIndex = 3;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(89, 27);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(88, 23);
            this.button4.TabIndex = 4;
            this.button4.Text = "StartStop";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // buttonIncreaseVolume
            // 
            this.buttonIncreaseVolume.Location = new System.Drawing.Point(361, 27);
            this.buttonIncreaseVolume.Name = "buttonIncreaseVolume";
            this.buttonIncreaseVolume.Size = new System.Drawing.Size(68, 59);
            this.buttonIncreaseVolume.TabIndex = 5;
            this.buttonIncreaseVolume.Text = "+";
            this.buttonIncreaseVolume.UseVisualStyleBackColor = true;
            this.buttonIncreaseVolume.Click += new System.EventHandler(this.buttonIncreaseVolume_Click);
            // 
            // buttonDecreaseVolume
            // 
            this.buttonDecreaseVolume.Location = new System.Drawing.Point(287, 27);
            this.buttonDecreaseVolume.Name = "buttonDecreaseVolume";
            this.buttonDecreaseVolume.Size = new System.Drawing.Size(68, 59);
            this.buttonDecreaseVolume.TabIndex = 6;
            this.buttonDecreaseVolume.Text = "-";
            this.buttonDecreaseVolume.UseVisualStyleBackColor = true;
            this.buttonDecreaseVolume.Click += new System.EventHandler(this.buttonDecreaseVolume_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 157);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "Reset";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // TestDllForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(477, 192);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonDecreaseVolume);
            this.Controls.Add(this.buttonIncreaseVolume);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.textBoxCurrentSongName);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Name = "TestDllForm";
            this.Text = "TestDllForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox textBoxCurrentSongName;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button buttonIncreaseVolume;
        private System.Windows.Forms.Button buttonDecreaseVolume;
        private System.Windows.Forms.Button button1;
    }
}