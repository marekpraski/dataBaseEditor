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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DBEditorMainForm));
            this.btnWyswietl = new System.Windows.Forms.Button();
            this.undoButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tbSqlQuery = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.loadNextButton = new System.Windows.Forms.Button();
            this.remainingRowsLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbLike = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbOrderBy = new System.Windows.Forms.TextBox();
            this.labelZatwierdzone = new System.Windows.Forms.Label();
            this.cbZatwierdzone = new System.Windows.Forms.ComboBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsPomoc = new System.Windows.Forms.ToolStripButton();
            this.tsUruchomModulGraficzny = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsWyswietlOryginalne = new System.Windows.Forms.ToolStripButton();
            this.cbOryginalneCzyZmienione = new System.Windows.Forms.ToolStripComboBox();
            this.cbKolor = new System.Windows.Forms.ToolStripComboBox();
            this.cbFituj = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnWyswietl
            // 
            this.btnWyswietl.Enabled = false;
            this.btnWyswietl.Location = new System.Drawing.Point(464, 232);
            this.btnWyswietl.Name = "btnWyswietl";
            this.btnWyswietl.Size = new System.Drawing.Size(75, 23);
            this.btnWyswietl.TabIndex = 0;
            this.btnWyswietl.Text = "wyświetl";
            this.btnWyswietl.UseVisualStyleBackColor = true;
            this.btnWyswietl.Click += new System.EventHandler(this.btnWyswietl_Click);
            // 
            // undoButton
            // 
            this.undoButton.Enabled = false;
            this.undoButton.Location = new System.Drawing.Point(464, 278);
            this.undoButton.Name = "undoButton";
            this.undoButton.Size = new System.Drawing.Size(75, 23);
            this.undoButton.TabIndex = 1;
            this.undoButton.Text = "cofnij";
            this.undoButton.UseVisualStyleBackColor = true;
            this.undoButton.Visible = false;
            this.undoButton.Click += new System.EventHandler(this.UndoButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Enabled = false;
            this.saveButton.Location = new System.Drawing.Point(464, 328);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 2;
            this.saveButton.Text = "zapisz";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Visible = false;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4});
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView1.Location = new System.Drawing.Point(12, 172);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(444, 458);
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
            // tbSqlQuery
            // 
            this.tbSqlQuery.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tbSqlQuery.Location = new System.Drawing.Point(12, 51);
            this.tbSqlQuery.Name = "tbSqlQuery";
            this.tbSqlQuery.Size = new System.Drawing.Size(527, 90);
            this.tbSqlQuery.TabIndex = 4;
            this.tbSqlQuery.Text = "";
            this.tbSqlQuery.TextChanged += new System.EventHandler(this.sqlQueryTextBox_TextChangedEvent);
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
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(412, 149);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "order by";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(144, 149);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(141, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "and nazwa wyrobiska like \'%";
            // 
            // tbLike
            // 
            this.tbLike.Location = new System.Drawing.Point(291, 145);
            this.tbLike.Name = "tbLike";
            this.tbLike.Size = new System.Drawing.Size(100, 20);
            this.tbLike.TabIndex = 14;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(392, 149);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "%\'";
            // 
            // tbOrderBy
            // 
            this.tbOrderBy.Location = new System.Drawing.Point(463, 145);
            this.tbOrderBy.Name = "tbOrderBy";
            this.tbOrderBy.Size = new System.Drawing.Size(28, 20);
            this.tbOrderBy.TabIndex = 16;
            this.tbOrderBy.Text = "1";
            // 
            // labelZatwierdzone
            // 
            this.labelZatwierdzone.AutoSize = true;
            this.labelZatwierdzone.Location = new System.Drawing.Point(12, 149);
            this.labelZatwierdzone.Name = "labelZatwierdzone";
            this.labelZatwierdzone.Size = new System.Drawing.Size(81, 13);
            this.labelZatwierdzone.TabIndex = 17;
            this.labelZatwierdzone.Text = "zatwierdzone = ";
            // 
            // cbZatwierdzone
            // 
            this.cbZatwierdzone.FormattingEnabled = true;
            this.cbZatwierdzone.Items.AddRange(new object[] {
            "0",
            "1"});
            this.cbZatwierdzone.Location = new System.Drawing.Point(91, 145);
            this.cbZatwierdzone.Name = "cbZatwierdzone";
            this.cbZatwierdzone.Size = new System.Drawing.Size(35, 21);
            this.cbZatwierdzone.TabIndex = 19;
            this.cbZatwierdzone.SelectedIndexChanged += new System.EventHandler(this.cbZatwierdzone_SelectedIndexChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsPomoc,
            this.tsUruchomModulGraficzny,
            this.toolStripSeparator1,
            this.tsWyswietlOryginalne,
            this.cbOryginalneCzyZmienione,
            this.cbKolor});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(553, 25);
            this.toolStrip1.TabIndex = 20;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsPomoc
            // 
            this.tsPomoc.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsPomoc.Image = global::DataBaseEditor.Properties.Resources.InfoObiekt;
            this.tsPomoc.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsPomoc.Name = "tsPomoc";
            this.tsPomoc.Size = new System.Drawing.Size(23, 22);
            this.tsPomoc.Text = "toolStripButton3";
            this.tsPomoc.ToolTipText = "Pomoc";
            this.tsPomoc.Click += new System.EventHandler(this.PomocToolStripMenuItem_Click);
            // 
            // tsUruchomModulGraficzny
            // 
            this.tsUruchomModulGraficzny.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsUruchomModulGraficzny.Image = ((System.Drawing.Image)(resources.GetObject("tsUruchomModulGraficzny.Image")));
            this.tsUruchomModulGraficzny.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsUruchomModulGraficzny.Name = "tsUruchomModulGraficzny";
            this.tsUruchomModulGraficzny.Size = new System.Drawing.Size(23, 22);
            this.tsUruchomModulGraficzny.Text = "toolStripButton1";
            this.tsUruchomModulGraficzny.ToolTipText = "Uruchom moduł graficzny";
            this.tsUruchomModulGraficzny.Click += new System.EventHandler(this.mapaToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsWyswietlOryginalne
            // 
            this.tsWyswietlOryginalne.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsWyswietlOryginalne.Image = global::DataBaseEditor.Properties.Resources.PokazMape_23x23;
            this.tsWyswietlOryginalne.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsWyswietlOryginalne.Name = "tsWyswietlOryginalne";
            this.tsWyswietlOryginalne.Size = new System.Drawing.Size(23, 22);
            this.tsWyswietlOryginalne.Text = "toolStripButton2";
            this.tsWyswietlOryginalne.ToolTipText = "Wyświetl na mapie";
            this.tsWyswietlOryginalne.Click += new System.EventHandler(this.tsWyswietlNaMapie_Click);
            // 
            // cbOryginalneCzyZmienione
            // 
            this.cbOryginalneCzyZmienione.Items.AddRange(new object[] {
            "oryginalne",
            "zmienione"});
            this.cbOryginalneCzyZmienione.Name = "cbOryginalneCzyZmienione";
            this.cbOryginalneCzyZmienione.Size = new System.Drawing.Size(100, 25);
            // 
            // cbKolor
            // 
            this.cbKolor.Name = "cbKolor";
            this.cbKolor.Size = new System.Drawing.Size(121, 25);
            // 
            // cbFituj
            // 
            this.cbFituj.AutoSize = true;
            this.cbFituj.Location = new System.Drawing.Point(316, 8);
            this.cbFituj.Name = "cbFituj";
            this.cbFituj.Size = new System.Drawing.Size(132, 17);
            this.cbFituj.TabIndex = 21;
            this.cbFituj.Text = "usuń poprzednie i zbliż";
            this.cbFituj.UseVisualStyleBackColor = true;
            // 
            // DBEditorMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(553, 640);
            this.Controls.Add(this.cbFituj);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.cbZatwierdzone);
            this.Controls.Add(this.labelZatwierdzone);
            this.Controls.Add(this.tbOrderBy);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tbLike);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.remainingRowsLabel);
            this.Controls.Add(this.loadNextButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbSqlQuery);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.undoButton);
            this.Controls.Add(this.btnWyswietl);
            this.Name = "DBEditorMainForm";
            this.Text = "Edytor osi wyrobisk";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DBEditorMainForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnWyswietl;
        private System.Windows.Forms.Button undoButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.RichTextBox tbSqlQuery;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button loadNextButton;
        private System.Windows.Forms.Label remainingRowsLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbLike;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbOrderBy;
        private System.Windows.Forms.Label labelZatwierdzone;
        private System.Windows.Forms.ComboBox cbZatwierdzone;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsPomoc;
        private System.Windows.Forms.ToolStripButton tsUruchomModulGraficzny;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsWyswietlOryginalne;
        private System.Windows.Forms.ToolStripComboBox cbOryginalneCzyZmienione;
        private System.Windows.Forms.ToolStripComboBox cbKolor;
        private System.Windows.Forms.CheckBox cbFituj;
    }
}

