namespace WindowsFormsApp
{
    partial class MainForm
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
            this.PortLab = new System.Windows.Forms.Label();
            this.StartButton = new System.Windows.Forms.Button();
            this.CloseButton = new System.Windows.Forms.Button();
            this.PortNum = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.PortNum)).BeginInit();
            this.SuspendLayout();
            // 
            // PortLab
            // 
            this.PortLab.AutoSize = true;
            this.PortLab.Location = new System.Drawing.Point(12, 17);
            this.PortLab.Name = "PortLab";
            this.PortLab.Size = new System.Drawing.Size(29, 12);
            this.PortLab.TabIndex = 1;
            this.PortLab.Text = "Port";
            // 
            // StartButton
            // 
            this.StartButton.Location = new System.Drawing.Point(161, 12);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(75, 23);
            this.StartButton.TabIndex = 2;
            this.StartButton.Text = "Start";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // CloseButton
            // 
            this.CloseButton.Location = new System.Drawing.Point(243, 12);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(75, 23);
            this.CloseButton.TabIndex = 3;
            this.CloseButton.Text = "Close";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // PortNum
            // 
            this.PortNum.Location = new System.Drawing.Point(48, 13);
            this.PortNum.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.PortNum.Minimum = new decimal(new int[] {
            80,
            0,
            0,
            0});
            this.PortNum.Name = "PortNum";
            this.PortNum.Size = new System.Drawing.Size(107, 21);
            this.PortNum.TabIndex = 4;
            this.PortNum.Value = new decimal(new int[] {
            8080,
            0,
            0,
            0});
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(331, 48);
            this.Controls.Add(this.PortNum);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.StartButton);
            this.Controls.Add(this.PortLab);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WebApi self hosting DEMO ";
            ((System.ComponentModel.ISupportInitialize)(this.PortNum)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label PortLab;
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.NumericUpDown PortNum;
    }
}

