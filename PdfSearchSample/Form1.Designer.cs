namespace PdfSearchSample
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
        protected override void Dispose( bool disposing )
        {
            if (disposing && ( components != null ))
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pdfStorageSelector = new System.Windows.Forms.FolderBrowserDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this.folderNameLabel = new System.Windows.Forms.Label();
            this.selectPdfStorageBtn = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.searchDescription = new System.Windows.Forms.Label();
            this.searchText = new System.Windows.Forms.TextBox();
            this.startSearchBtn = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label1 = new System.Windows.Forms.Label();
            this.resultsView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pdfStorageSelector
            // 
            this.pdfStorageSelector.Description = "To select folder with pdf files.";
            this.pdfStorageSelector.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this.pdfStorageSelector.ShowNewFolderButton = false;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel1.Controls.Add(this.folderNameLabel);
            this.panel1.Controls.Add(this.selectPdfStorageBtn);
            this.panel1.Location = new System.Drawing.Point(10, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(762, 35);
            this.panel1.TabIndex = 1;
            // 
            // folderNameLabel
            // 
            this.folderNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.folderNameLabel.AutoSize = true;
            this.folderNameLabel.Location = new System.Drawing.Point(3, 11);
            this.folderNameLabel.Name = "folderNameLabel";
            this.folderNameLabel.Size = new System.Drawing.Size(83, 13);
            this.folderNameLabel.TabIndex = 2;
            this.folderNameLabel.Tag = "";
            this.folderNameLabel.Text = "(is not specified)";
            // 
            // selectPdfStorageBtn
            // 
            this.selectPdfStorageBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.selectPdfStorageBtn.AutoSize = true;
            this.selectPdfStorageBtn.Location = new System.Drawing.Point(638, 6);
            this.selectPdfStorageBtn.Name = "selectPdfStorageBtn";
            this.selectPdfStorageBtn.Size = new System.Drawing.Size(121, 23);
            this.selectPdfStorageBtn.TabIndex = 1;
            this.selectPdfStorageBtn.Text = "Select PDF Storage";
            this.selectPdfStorageBtn.UseVisualStyleBackColor = true;
            this.selectPdfStorageBtn.Click += new System.EventHandler(this.OnSelectStorage);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel2.Controls.Add(this.searchDescription);
            this.panel2.Controls.Add(this.searchText);
            this.panel2.Controls.Add(this.startSearchBtn);
            this.panel2.Location = new System.Drawing.Point(10, 59);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(762, 31);
            this.panel2.TabIndex = 2;
            // 
            // searchDescription
            // 
            this.searchDescription.AutoSize = true;
            this.searchDescription.Location = new System.Drawing.Point(3, 9);
            this.searchDescription.Name = "searchDescription";
            this.searchDescription.Size = new System.Drawing.Size(145, 13);
            this.searchDescription.TabIndex = 2;
            this.searchDescription.Text = "Please enter a text to search:";
            // 
            // searchText
            // 
            this.searchText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.searchText.Location = new System.Drawing.Point(154, 6);
            this.searchText.Name = "searchText";
            this.searchText.Size = new System.Drawing.Size(524, 20);
            this.searchText.TabIndex = 1;
            // 
            // startSearchBtn
            // 
            this.startSearchBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.startSearchBtn.Location = new System.Drawing.Point(684, 4);
            this.startSearchBtn.Name = "startSearchBtn";
            this.startSearchBtn.Size = new System.Drawing.Size(75, 23);
            this.startSearchBtn.TabIndex = 0;
            this.startSearchBtn.Text = "Search";
            this.startSearchBtn.UseVisualStyleBackColor = true;
            this.startSearchBtn.Click += new System.EventHandler(this.OnSearch);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(10, 103);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.resultsView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.pictureBox1);
            this.splitContainer1.Size = new System.Drawing.Size(762, 447);
            this.splitContainer1.SplitterDistance = 253;
            this.splitContainer1.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Search Results:";
            // 
            // resultsView
            // 
            this.resultsView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resultsView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.resultsView.FullRowSelect = true;
            this.resultsView.GridLines = true;
            this.resultsView.Location = new System.Drawing.Point(3, 25);
            this.resultsView.MultiSelect = false;
            this.resultsView.Name = "resultsView";
            this.resultsView.ShowItemToolTips = true;
            this.resultsView.Size = new System.Drawing.Size(258, 422);
            this.resultsView.TabIndex = 5;
            this.resultsView.UseCompatibleStateImageBehavior = false;
            this.resultsView.View = System.Windows.Forms.View.Tile;
            this.resultsView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.OnSelectResult);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Search Results";
            this.columnHeader1.Width = 200;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pictureBox1.Location = new System.Drawing.Point(8, 8);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(490, 430);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Apitron PDF search sample";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog pdfStorageSelector;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label folderNameLabel;
        private System.Windows.Forms.Button selectPdfStorageBtn;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label searchDescription;
        private System.Windows.Forms.TextBox searchText;
        private System.Windows.Forms.Button startSearchBtn;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView resultsView;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}

