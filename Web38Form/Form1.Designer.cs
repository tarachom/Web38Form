namespace Web38Form
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.checkBoxVisibleLogXml = new System.Windows.Forms.CheckBox();
            this.notifyIconWeb38Form = new System.Windows.Forms.NotifyIcon(this.components);
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Location = new System.Drawing.Point(12, 35);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(737, 452);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // checkBoxVisibleLogXml
            // 
            this.checkBoxVisibleLogXml.AutoSize = true;
            this.checkBoxVisibleLogXml.Location = new System.Drawing.Point(12, 12);
            this.checkBoxVisibleLogXml.Name = "checkBoxVisibleLogXml";
            this.checkBoxVisibleLogXml.Size = new System.Drawing.Size(216, 17);
            this.checkBoxVisibleLogXml.TabIndex = 1;
            this.checkBoxVisibleLogXml.Text = "Виводити детальну інформацію  в лог";
            this.checkBoxVisibleLogXml.UseVisualStyleBackColor = true;
            // 
            // notifyIconWeb38Form
            // 
            this.notifyIconWeb38Form.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIconWeb38Form.Icon")));
            this.notifyIconWeb38Form.Text = "Web38Form";
            this.notifyIconWeb38Form.Visible = true;
            this.notifyIconWeb38Form.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIconWeb38Form_MouseDoubleClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(761, 499);
            this.Controls.Add(this.checkBoxVisibleLogXml);
            this.Controls.Add(this.richTextBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Web38";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.CheckBox checkBoxVisibleLogXml;
        private System.Windows.Forms.NotifyIcon notifyIconWeb38Form;
    }
}

