namespace MemberDetection
{
    partial class Screen
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
            this.btnExportGeometry = new System.Windows.Forms.Button();
            this.btnDetectType = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.btnCalculate = new System.Windows.Forms.Button();
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // btnExportGeometry
            // 
            this.btnExportGeometry.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnExportGeometry.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnExportGeometry.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExportGeometry.Location = new System.Drawing.Point(12, 53);
            this.btnExportGeometry.Name = "btnExportGeometry";
            this.btnExportGeometry.Size = new System.Drawing.Size(216, 80);
            this.btnExportGeometry.TabIndex = 0;
            this.btnExportGeometry.Text = "Export to Geometry";
            this.btnExportGeometry.UseVisualStyleBackColor = false;
            this.btnExportGeometry.Click += new System.EventHandler(this.btnExportGeometry_Click);
            // 
            // btnDetectType
            // 
            this.btnDetectType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btnDetectType.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDetectType.Location = new System.Drawing.Point(278, 53);
            this.btnDetectType.Name = "btnDetectType";
            this.btnDetectType.Size = new System.Drawing.Size(216, 80);
            this.btnDetectType.TabIndex = 1;
            this.btnDetectType.Text = "Detect Type";
            this.btnDetectType.UseVisualStyleBackColor = false;
            this.btnDetectType.Click += new System.EventHandler(this.btnDetectType_Click);
            // 
            // btnCalculate
            // 
            this.btnCalculate.BackColor = System.Drawing.Color.Yellow;
            this.btnCalculate.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCalculate.Location = new System.Drawing.Point(543, 53);
            this.btnCalculate.Name = "btnCalculate";
            this.btnCalculate.Size = new System.Drawing.Size(216, 80);
            this.btnCalculate.TabIndex = 2;
            this.btnCalculate.Text = "Calculate Parameters";
            this.btnCalculate.UseVisualStyleBackColor = false;
            this.btnCalculate.Click += new System.EventHandler(this.btnCalculate_Click);
            // 
            // richTextBox
            // 
            this.richTextBox.Location = new System.Drawing.Point(12, 189);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.Size = new System.Drawing.Size(747, 239);
            this.richTextBox.TabIndex = 3;
            this.richTextBox.Text = "";
            // 
            // Screen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.richTextBox);
            this.Controls.Add(this.btnCalculate);
            this.Controls.Add(this.btnDetectType);
            this.Controls.Add(this.btnExportGeometry);
            this.Name = "Screen";
            this.Text = "Screen";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnExportGeometry;
        private System.Windows.Forms.Button btnDetectType;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Button btnCalculate;
        private System.Windows.Forms.RichTextBox richTextBox;
    }
}