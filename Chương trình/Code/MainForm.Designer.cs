namespace TextureClassification
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
            this.waitingProgressBar = new System.Windows.Forms.ProgressBar();
            this.lbpImage = new System.Windows.Forms.PictureBox();
            this.originalImage = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnLoadPartern = new System.Windows.Forms.Button();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.lblResult = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.lbpImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.originalImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // waitingProgressBar
            // 
            this.waitingProgressBar.Location = new System.Drawing.Point(57, 524);
            this.waitingProgressBar.Name = "waitingProgressBar";
            this.waitingProgressBar.Size = new System.Drawing.Size(429, 13);
            this.waitingProgressBar.TabIndex = 0;
            this.waitingProgressBar.Visible = false;
            // 
            // lbpImage
            // 
            this.lbpImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbpImage.Location = new System.Drawing.Point(0, 0);
            this.lbpImage.Name = "lbpImage";
            this.lbpImage.Size = new System.Drawing.Size(247, 314);
            this.lbpImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.lbpImage.TabIndex = 0;
            this.lbpImage.TabStop = false;
            // 
            // originalImage
            // 
            this.originalImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.originalImage.Location = new System.Drawing.Point(0, 0);
            this.originalImage.Name = "originalImage";
            this.originalImage.Size = new System.Drawing.Size(245, 314);
            this.originalImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.originalImage.TabIndex = 0;
            this.originalImage.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 128);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(89, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Mở file ảnh";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(0, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 651);
            this.splitter1.TabIndex = 3;
            this.splitter1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(113, 482);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Ảnh gốc";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(368, 482);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Ảnh LBP";
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(540, 301);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(88, 23);
            this.btnRun.TabIndex = 5;
            this.btnRun.Text = "Xác định mẫu";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnLoadPartern
            // 
            this.btnLoadPartern.Location = new System.Drawing.Point(634, 26);
            this.btnLoadPartern.Name = "btnLoadPartern";
            this.btnLoadPartern.Size = new System.Drawing.Size(101, 23);
            this.btnLoadPartern.TabIndex = 2;
            this.btnLoadPartern.Text = "Mở tập ảnh mẫu";
            this.btnLoadPartern.UseVisualStyleBackColor = true;
            this.btnLoadPartern.Click += new System.EventHandler(this.btnLoadPartern_Click);
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeColumns = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.BackgroundColor = System.Drawing.Color.Silver;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Location = new System.Drawing.Point(634, 55);
            this.dataGridView.Margin = new System.Windows.Forms.Padding(0);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridView.ShowCellErrors = false;
            this.dataGridView.ShowCellToolTips = false;
            this.dataGridView.ShowEditingIcon = false;
            this.dataGridView.ShowRowErrors = false;
            this.dataGridView.Size = new System.Drawing.Size(637, 584);
            this.dataGridView.TabIndex = 6;
            // 
            // lblResult
            // 
            this.lblResult.AutoSize = true;
            this.lblResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResult.Location = new System.Drawing.Point(53, 562);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(28, 24);
            this.lblResult.TabIndex = 7;
            this.lblResult.Text = "...";
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Location = new System.Drawing.Point(12, 163);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.originalImage);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lbpImage);
            this.splitContainer1.Size = new System.Drawing.Size(500, 316);
            this.splitContainer1.SplitterDistance = 247;
            this.splitContainer1.TabIndex = 8;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1275, 651);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.btnLoadPartern);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.waitingProgressBar);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Texture classification";
            ((System.ComponentModel.ISupportInitialize)(this.lbpImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.originalImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar waitingProgressBar;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox lbpImage;
        private System.Windows.Forms.PictureBox originalImage;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnLoadPartern;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}

