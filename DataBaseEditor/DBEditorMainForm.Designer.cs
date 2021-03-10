namespace DataBaseEditor
{
    partial class DBEditorMainForm
    {
        /// <summary>
        /// Wymagana zmienna projektanta.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Wyczyść wszystkie używane zasoby.
        /// </summary>
        /// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod generowany przez Projektanta formularzy systemu Windows

        /// <summary>
        /// Metoda wymagana do obsługi projektanta — nie należy modyfikować
        /// jej zawartości w edytorze kodu.
        /// </summary>
        private void InitializeComponent()
        {
            this.displayButton = new System.Windows.Forms.Button();
            this.undoButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sqlQueryTextBox = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.pomocToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mapaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label2 = new System.Windows.Forms.Label();
            this.loadNextButton = new System.Windows.Forms.Button();
            this.remainingRowsLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // displayButton
            // 
            this.displayButton.Enabled = false;
            this.displayButton.Location = new System.Drawing.Point(464, 147);
            this.displayButton.Name = "displayButton";
            this.displayButton.Size = new System.Drawing.Size(75, 23);
            this.displayButton.TabIndex = 0;
            this.displayButton.Text = "wyświetl";
            this.displayButton.UseVisualStyleBackColor = true;
            this.displayButton.Click += new System.EventHandler(this.displayButton_Click);
            // 
            // undoButton
            // 
            this.undoButton.Enabled = false;
            this.undoButton.Location = new System.Drawing.Point(464, 193);
            this.undoButton.Name = "undoButton";
            this.undoButton.Size = new System.Drawing.Size(75, 23);
            this.undoButton.TabIndex = 1;
            this.undoButton.Text = "cofnij";
            this.undoButton.UseVisualStyleBackColor = true;
            this.undoButton.Click += new System.EventHandler(this.UndoButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Enabled = false;
            this.saveButton.Location = new System.Drawing.Point(464, 243);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 2;
            this.saveButton.Text = "zapisz";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4});
            this.dataGridView1.Location = new System.Drawing.Point(12, 147);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(444, 483);
            this.dataGridView1.TabIndex = 3;
            this.dataGridView1.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridView1_CellBeginEdit);
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            this.dataGridView1.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_RowHeaderMouseClick);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Column1";
            this.Column1.Name = "Column1";
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Column2";
            this.Column2.Name = "Column2";
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Column3";
            this.Column3.Name = "Column3";
            // 
            // Column4
            // 
            this.Column4.HeaderText = "Column4";
            this.Column4.Name = "Column4";
            // 
            // sqlQueryTextBox
            // 
            this.sqlQueryTextBox.Location = new System.Drawing.Point(12, 51);
            this.sqlQueryTextBox.Name = "sqlQueryTextBox";
            this.sqlQueryTextBox.Size = new System.Drawing.Size(527, 90);
            this.sqlQueryTextBox.TabIndex = 4;
            this.sqlQueryTextBox.Text = "";
            this.sqlQueryTextBox.TextChanged += new System.EventHandler(this.sqlQueryTextBox_TextChangedEvent);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "kwerenda sql";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pomocToolStripMenuItem,
            this.mapaToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(558, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // pomocToolStripMenuItem
            // 
            this.pomocToolStripMenuItem.Name = "pomocToolStripMenuItem";
            this.pomocToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.pomocToolStripMenuItem.Text = "Pomoc";
            this.pomocToolStripMenuItem.Click += new System.EventHandler(this.PomocToolStripMenuItem_Click);
            // 
            // mapaToolStripMenuItem
            // 
            this.mapaToolStripMenuItem.Name = "mapaToolStripMenuItem";
            this.mapaToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.mapaToolStripMenuItem.Text = "Mapa";
            this.mapaToolStripMenuItem.Click += new System.EventHandler(this.mapaToolStripMenuItem_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(136, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(236, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "połączenie z serwerem nie zostało zdefiniowane";
            this.label2.Visible = false;
            // 
            // loadNextButton
            // 
            this.loadNextButton.Location = new System.Drawing.Point(463, 606);
            this.loadNextButton.Name = "loadNextButton";
            this.loadNextButton.Size = new System.Drawing.Size(83, 23);
            this.loadNextButton.TabIndex = 8;
            this.loadNextButton.Text = "button1";
            this.loadNextButton.UseVisualStyleBackColor = true;
            this.loadNextButton.Visible = false;
            this.loadNextButton.Click += new System.EventHandler(this.LoadNexButton_Click);
            // 
            // remainingRowsLabel
            // 
            this.remainingRowsLabel.AutoSize = true;
            this.remainingRowsLabel.Location = new System.Drawing.Point(483, 590);
            this.remainingRowsLabel.Name = "remainingRowsLabel";
            this.remainingRowsLabel.Size = new System.Drawing.Size(35, 13);
            this.remainingRowsLabel.TabIndex = 9;
            this.remainingRowsLabel.Text = "label3";
            this.remainingRowsLabel.Visible = false;
            // 
            // DBEditorMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(558, 642);
            this.Controls.Add(this.remainingRowsLabel);
            this.Controls.Add(this.loadNextButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.sqlQueryTextBox);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.undoButton);
            this.Controls.Add(this.displayButton);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "DBEditorMainForm";
            this.Text = "Edytor Bazy Danych";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DBEditorMainForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button displayButton;
        private System.Windows.Forms.Button undoButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.RichTextBox sqlQueryTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem pomocToolStripMenuItem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button loadNextButton;
        private System.Windows.Forms.Label remainingRowsLabel;
        private System.Windows.Forms.ToolStripMenuItem mapaToolStripMenuItem;
    }
}

