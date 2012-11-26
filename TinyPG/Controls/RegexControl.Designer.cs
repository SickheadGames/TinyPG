namespace TinyPG.Controls
{
    partial class RegExControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegExControl));
            this.Splitter = new System.Windows.Forms.Splitter();
            this.panel2 = new System.Windows.Forms.Panel();
            this.textMatches = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.checkIgnoreCase = new System.Windows.Forms.CheckBox();
            this.checkMultiline = new System.Windows.Forms.CheckBox();
            this.textExpression = new System.Windows.Forms.TextBox();
            this.textBox = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.statusText = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Splitter
            // 
            this.Splitter.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.Splitter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Splitter.Location = new System.Drawing.Point(0, 421);
            this.Splitter.Name = "Splitter";
            this.Splitter.Size = new System.Drawing.Size(331, 5);
            this.Splitter.TabIndex = 11;
            this.Splitter.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.textMatches);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 426);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(331, 57);
            this.panel2.TabIndex = 12;
            // 
            // textMatches
            // 
            this.textMatches.BackColor = System.Drawing.SystemColors.Window;
            this.textMatches.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textMatches.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textMatches.Font = new System.Drawing.Font("Courier New", 9.75F);
            this.textMatches.HideSelection = false;
            this.textMatches.Location = new System.Drawing.Point(0, 20);
            this.textMatches.Name = "textMatches";
            this.textMatches.ReadOnly = true;
            this.textMatches.Size = new System.Drawing.Size(331, 37);
            this.textMatches.TabIndex = 6;
            this.textMatches.Text = "";
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(331, 20);
            this.label2.TabIndex = 7;
            this.label2.Text = "Match results (displays group names only)";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // checkIgnoreCase
            // 
            this.checkIgnoreCase.AutoSize = true;
            this.checkIgnoreCase.Checked = true;
            this.checkIgnoreCase.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkIgnoreCase.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.checkIgnoreCase.Location = new System.Drawing.Point(8, 46);
            this.checkIgnoreCase.Name = "checkIgnoreCase";
            this.checkIgnoreCase.Size = new System.Drawing.Size(86, 19);
            this.checkIgnoreCase.TabIndex = 6;
            this.checkIgnoreCase.Text = "Ignore case";
            this.checkIgnoreCase.UseVisualStyleBackColor = true;
            this.checkIgnoreCase.CheckedChanged += new System.EventHandler(this.checkIgnoreCase_CheckedChanged);
            // 
            // checkMultiline
            // 
            this.checkMultiline.AutoSize = true;
            this.checkMultiline.Checked = true;
            this.checkMultiline.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkMultiline.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.checkMultiline.Location = new System.Drawing.Point(8, 30);
            this.checkMultiline.Name = "checkMultiline";
            this.checkMultiline.Size = new System.Drawing.Size(73, 19);
            this.checkMultiline.TabIndex = 5;
            this.checkMultiline.Text = "Multiline";
            this.checkMultiline.UseVisualStyleBackColor = true;
            this.checkMultiline.CheckedChanged += new System.EventHandler(this.checkMultiline_CheckedChanged);
            // 
            // textExpression
            // 
            this.textExpression.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textExpression.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textExpression.HideSelection = false;
            this.textExpression.Location = new System.Drawing.Point(96, 8);
            this.textExpression.Name = "textExpression";
            this.textExpression.Size = new System.Drawing.Size(220, 26);
            this.textExpression.TabIndex = 1;
            this.textExpression.TextChanged += new System.EventHandler(this.textExpression_TextChanged);
            // 
            // textBox
            // 
            this.textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox.HideSelection = false;
            this.textBox.Location = new System.Drawing.Point(0, 68);
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(331, 353);
            this.textBox.TabIndex = 9;
            this.textBox.Text = resources.GetString("textBox.Text");
            this.textBox.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            this.textBox.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.panel1.Controls.Add(this.statusText);
            this.panel1.Controls.Add(this.checkIgnoreCase);
            this.panel1.Controls.Add(this.checkMultiline);
            this.panel1.Controls.Add(this.textExpression);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(331, 68);
            this.panel1.TabIndex = 8;
            // 
            // statusText
            // 
            this.statusText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.statusText.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.statusText.Location = new System.Drawing.Point(93, 35);
            this.statusText.Name = "statusText";
            this.statusText.Size = new System.Drawing.Size(223, 30);
            this.statusText.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.label1.Location = new System.Drawing.Point(8, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 21);
            this.label1.TabIndex = 4;
            this.label1.Text = "Expression:";
            // 
            // RegExControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.Splitter);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "RegExControl";
            this.Size = new System.Drawing.Size(331, 483);
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Splitter Splitter;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RichTextBox textMatches;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkIgnoreCase;
        private System.Windows.Forms.CheckBox checkMultiline;
        private System.Windows.Forms.TextBox textExpression;
        private System.Windows.Forms.RichTextBox textBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label statusText;

    }
}
