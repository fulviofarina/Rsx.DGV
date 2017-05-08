namespace Rsx.DGV
{
   partial class ucSearch
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
            this.box = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.fieldbox = new System.Windows.Forms.ComboBox();
            this.clr = new System.Windows.Forms.Button();
            this.close = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // box
            // 
            this.box.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.box.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.box.Dock = System.Windows.Forms.DockStyle.Fill;
            this.box.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.box.FormattingEnabled = true;
            this.box.Location = new System.Drawing.Point(282, 3);
            this.box.Name = "box";
            this.box.Size = new System.Drawing.Size(449, 26);
            this.box.TabIndex = 0;
            this.box.TextChanged += new System.EventHandler(this.box_TextChanged);
            this.box.KeyDown += new System.Windows.Forms.KeyEventHandler(this.box_KeyDown);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28.68044F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.408602F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 61.96236F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel1.Controls.Add(this.fieldbox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.box, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.clr, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.close, 3, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(766, 30);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // fieldbox
            // 
            this.fieldbox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.fieldbox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.fieldbox.BackColor = System.Drawing.Color.Thistle;
            this.fieldbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fieldbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fieldbox.ForeColor = System.Drawing.Color.Black;
            this.fieldbox.FormattingEnabled = true;
            this.fieldbox.Location = new System.Drawing.Point(3, 3);
            this.fieldbox.Name = "fieldbox";
            this.fieldbox.Size = new System.Drawing.Size(204, 26);
            this.fieldbox.TabIndex = 1;
            this.fieldbox.TextChanged += new System.EventHandler(this.fieldbox_TextChanged);
            this.fieldbox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.box_KeyDown);
            // 
            // clr
            // 
            this.clr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clr.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.150944F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clr.Location = new System.Drawing.Point(212, 2);
            this.clr.Margin = new System.Windows.Forms.Padding(2);
            this.clr.Name = "clr";
            this.clr.Size = new System.Drawing.Size(65, 26);
            this.clr.TabIndex = 2;
            this.clr.Text = "Clear";
            this.clr.UseVisualStyleBackColor = true;
            this.clr.Click += new System.EventHandler(this.clr_Click);
            // 
            // close
            // 
            this.close.BackColor = System.Drawing.Color.Black;
            this.close.Dock = System.Windows.Forms.DockStyle.Fill;
            this.close.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.830189F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.close.ForeColor = System.Drawing.Color.White;
            this.close.Location = new System.Drawing.Point(737, 3);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(26, 24);
            this.close.TabIndex = 3;
            this.close.Text = "X";
            this.close.UseVisualStyleBackColor = false;
            this.close.Click += new System.EventHandler(this.close_Click);
            // 
            // ucSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ucSearch";
            this.Size = new System.Drawing.Size(766, 30);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

	  }

	  #endregion

	  private System.Windows.Forms.ComboBox box;
      private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
      private System.Windows.Forms.ComboBox fieldbox;
      private System.Windows.Forms.Button clr;
      private System.Windows.Forms.Button close;
   }
}
