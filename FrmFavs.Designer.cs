namespace favs
{
    partial class FrmFavs
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.scMainView = new System.Windows.Forms.SplitContainer();
            this.scSizeView = new System.Windows.Forms.SplitContainer();
            this.trvFileSystem = new System.Windows.Forms.TreeView();
            this.lvFiles = new System.Windows.Forms.ListView();
            this.chName = new System.Windows.Forms.ColumnHeader();
            this.chSize = new System.Windows.Forms.ColumnHeader();
            this.trvFileChanges = new System.Windows.Forms.TreeView();
            ((System.ComponentModel.ISupportInitialize)(this.scMainView)).BeginInit();
            this.scMainView.Panel1.SuspendLayout();
            this.scMainView.Panel2.SuspendLayout();
            this.scMainView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scSizeView)).BeginInit();
            this.scSizeView.Panel1.SuspendLayout();
            this.scSizeView.Panel2.SuspendLayout();
            this.scSizeView.SuspendLayout();
            this.SuspendLayout();
            // 
            // scMainView
            // 
            this.scMainView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scMainView.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.scMainView.Location = new System.Drawing.Point(0, 0);
            this.scMainView.Name = "scMainView";
            this.scMainView.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scMainView.Panel1
            // 
            this.scMainView.Panel1.Controls.Add(this.scSizeView);
            // 
            // scMainView.Panel2
            // 
            this.scMainView.Panel2.Controls.Add(this.trvFileChanges);
            this.scMainView.Size = new System.Drawing.Size(800, 450);
            this.scMainView.SplitterDistance = 266;
            this.scMainView.TabIndex = 0;
            // 
            // scSizeView
            // 
            this.scSizeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scSizeView.Cursor = System.Windows.Forms.Cursors.VSplit;
            this.scSizeView.Location = new System.Drawing.Point(3, 3);
            this.scSizeView.Name = "scSizeView";
            // 
            // scSizeView.Panel1
            // 
            this.scSizeView.Panel1.Controls.Add(this.trvFileSystem);
            // 
            // scSizeView.Panel2
            // 
            this.scSizeView.Panel2.Controls.Add(this.lvFiles);
            this.scSizeView.Size = new System.Drawing.Size(794, 260);
            this.scSizeView.SplitterDistance = 472;
            this.scSizeView.TabIndex = 5;
            // 
            // trvFileSystem
            // 
            this.trvFileSystem.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trvFileSystem.Location = new System.Drawing.Point(0, 3);
            this.trvFileSystem.Name = "trvFileSystem";
            this.trvFileSystem.Size = new System.Drawing.Size(469, 257);
            this.trvFileSystem.TabIndex = 4;
            this.trvFileSystem.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.trvFileSystem_AfterExpand);
            this.trvFileSystem.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trvFileSystem_AfterSelect);
            // 
            // lvFiles
            // 
            this.lvFiles.AllowColumnReorder = true;
            this.lvFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chName,
            this.chSize});
            this.lvFiles.Location = new System.Drawing.Point(-1, 3);
            this.lvFiles.MultiSelect = false;
            this.lvFiles.Name = "lvFiles";
            this.lvFiles.Size = new System.Drawing.Size(319, 257);
            this.lvFiles.TabIndex = 5;
            this.lvFiles.UseCompatibleStateImageBehavior = false;
            this.lvFiles.View = System.Windows.Forms.View.Details;
            this.lvFiles.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvFiles_ColumnClick);
            // 
            // chName
            // 
            this.chName.Text = "File Name";
            this.chName.Width = 160;
            // 
            // chSize
            // 
            this.chSize.Text = "Size in Bytes";
            this.chSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.chSize.Width = 120;
            // 
            // trvFileChanges
            // 
            this.trvFileChanges.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trvFileChanges.Location = new System.Drawing.Point(3, 3);
            this.trvFileChanges.Name = "trvFileChanges";
            this.trvFileChanges.Size = new System.Drawing.Size(794, 174);
            this.trvFileChanges.TabIndex = 0;
            // 
            // FrmFavs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.scMainView);
            this.Name = "FrmFavs";
            this.Text = "favs";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmFavs_FormClosing);
            this.Load += new System.EventHandler(this.FrmFavs_Load);
            this.Shown += new System.EventHandler(this.FrmFavs_Shown);
            this.scMainView.Panel1.ResumeLayout(false);
            this.scMainView.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scMainView)).EndInit();
            this.scMainView.ResumeLayout(false);
            this.scSizeView.Panel1.ResumeLayout(false);
            this.scSizeView.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scSizeView)).EndInit();
            this.scSizeView.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SplitContainer scMainView;
        private TreeView trvFileSystem;
        private SplitContainer scSizeView;
        private ListView lvFiles;
        private ColumnHeader chName;
        private ColumnHeader chSize;
        private TreeView trvFileChanges;
    }
}