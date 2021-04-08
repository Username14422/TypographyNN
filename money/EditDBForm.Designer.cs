namespace money
{
    partial class EditDBForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditForm));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.labelSelect = new System.Windows.Forms.Label();
            this.dataGridViewEdit = new System.Windows.Forms.DataGridView();
            this.labelEdit = new System.Windows.Forms.Label();
            this.button = new System.Windows.Forms.Button();
            this.addAndFindTaftableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.newTegTextBox = new System.Windows.Forms.TextBox();
            this.tegAddButton = new System.Windows.Forms.Button();
            this.findTegTextBox = new System.Windows.Forms.TextBox();
            this.findButton = new System.Windows.Forms.Button();
            this.tagsSplitContainer = new System.Windows.Forms.SplitContainer();
            this.allTagsMenuStrip = new System.Windows.Forms.MenuStrip();
            this.ghhToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uuuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ghhToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.chosenTagsMenuStrip = new System.Windows.Forms.MenuStrip();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEdit)).BeginInit();
            this.addAndFindTaftableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tagsSplitContainer)).BeginInit();
            this.tagsSplitContainer.Panel1.SuspendLayout();
            this.tagsSplitContainer.Panel2.SuspendLayout();
            this.tagsSplitContainer.SuspendLayout();
            this.allTagsMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.labelSelect, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.dataGridViewEdit, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.labelEdit, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.button, 0, 5);
            this.tableLayoutPanel.Controls.Add(this.addAndFindTaftableLayoutPanel, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.tagsSplitContainer, 0, 4);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 6;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 41F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(684, 361);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // labelSelect
            // 
            this.labelSelect.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelSelect.AutoSize = true;
            this.labelSelect.BackColor = System.Drawing.Color.Transparent;
            this.labelSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelSelect.ForeColor = System.Drawing.SystemColors.WindowText;
            this.labelSelect.Location = new System.Drawing.Point(0, 85);
            this.labelSelect.Margin = new System.Windows.Forms.Padding(0);
            this.labelSelect.Name = "labelSelect";
            this.labelSelect.Size = new System.Drawing.Size(78, 13);
            this.labelSelect.TabIndex = 5;
            this.labelSelect.Text = "Отметте теги:";
            // 
            // dataGridViewEdit
            // 
            this.dataGridViewEdit.AllowUserToAddRows = false;
            this.dataGridViewEdit.AllowUserToDeleteRows = false;
            this.dataGridViewEdit.AllowUserToResizeRows = false;
            this.dataGridViewEdit.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewEdit.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dataGridViewEdit.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(252)))), ((int)(((byte)(252)))));
            this.dataGridViewEdit.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridViewEdit.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleVertical;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.NullValue = "0";
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewEdit.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewEdit.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(252)))), ((int)(((byte)(252)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewEdit.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewEdit.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridViewEdit.EnableHeadersVisualStyles = false;
            this.dataGridViewEdit.GridColor = System.Drawing.SystemColors.Highlight;
            this.dataGridViewEdit.Location = new System.Drawing.Point(4, 32);
            this.dataGridViewEdit.Margin = new System.Windows.Forms.Padding(4);
            this.dataGridViewEdit.MultiSelect = false;
            this.dataGridViewEdit.Name = "dataGridViewEdit";
            this.dataGridViewEdit.RowHeadersVisible = false;
            this.dataGridViewEdit.RowTemplate.Height = 17;
            this.dataGridViewEdit.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridViewEdit.Size = new System.Drawing.Size(676, 42);
            this.dataGridViewEdit.TabIndex = 4;
            // 
            // labelEdit
            // 
            this.labelEdit.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelEdit.AutoSize = true;
            this.labelEdit.BackColor = System.Drawing.SystemColors.Control;
            this.labelEdit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelEdit.ForeColor = System.Drawing.SystemColors.WindowText;
            this.labelEdit.Location = new System.Drawing.Point(0, 7);
            this.labelEdit.Margin = new System.Windows.Forms.Padding(0);
            this.labelEdit.Name = "labelEdit";
            this.labelEdit.Size = new System.Drawing.Size(110, 13);
            this.labelEdit.TabIndex = 1;
            this.labelEdit.Text = "Заполните столбцы:";
            // 
            // button
            // 
            this.button.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button.BackColor = System.Drawing.SystemColors.ControlLight;
            this.button.Location = new System.Drawing.Point(292, 329);
            this.button.Margin = new System.Windows.Forms.Padding(4);
            this.button.Name = "button";
            this.button.Size = new System.Drawing.Size(100, 25);
            this.button.TabIndex = 7;
            this.button.Text = "Подтвердить";
            this.button.UseVisualStyleBackColor = false;
            this.button.Click += new System.EventHandler(this.button_Click);
            // 
            // addAndFindTaftableLayoutPanel
            // 
            this.addAndFindTaftableLayoutPanel.ColumnCount = 4;
            this.addAndFindTaftableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 175F));
            this.addAndFindTaftableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.addAndFindTaftableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 175F));
            this.addAndFindTaftableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.addAndFindTaftableLayoutPanel.Controls.Add(this.newTegTextBox, 2, 0);
            this.addAndFindTaftableLayoutPanel.Controls.Add(this.tegAddButton, 3, 0);
            this.addAndFindTaftableLayoutPanel.Controls.Add(this.findTegTextBox, 0, 0);
            this.addAndFindTaftableLayoutPanel.Controls.Add(this.findButton, 1, 0);
            this.addAndFindTaftableLayoutPanel.Location = new System.Drawing.Point(4, 110);
            this.addAndFindTaftableLayoutPanel.Margin = new System.Windows.Forms.Padding(4);
            this.addAndFindTaftableLayoutPanel.Name = "addAndFindTaftableLayoutPanel";
            this.addAndFindTaftableLayoutPanel.RowCount = 1;
            this.addAndFindTaftableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.addAndFindTaftableLayoutPanel.Size = new System.Drawing.Size(674, 33);
            this.addAndFindTaftableLayoutPanel.TabIndex = 9;
            // 
            // newTegTextBox
            // 
            this.newTegTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.newTegTextBox.Location = new System.Drawing.Point(341, 6);
            this.newTegTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.newTegTextBox.Name = "newTegTextBox";
            this.newTegTextBox.Size = new System.Drawing.Size(167, 21);
            this.newTegTextBox.TabIndex = 0;
            this.toolTip.SetToolTip(this.newTegTextBox, "Новый тег...");
            // 
            // tegAddButton
            // 
            this.tegAddButton.Location = new System.Drawing.Point(516, 4);
            this.tegAddButton.Margin = new System.Windows.Forms.Padding(4);
            this.tegAddButton.Name = "tegAddButton";
            this.tegAddButton.Size = new System.Drawing.Size(100, 25);
            this.tegAddButton.TabIndex = 1;
            this.tegAddButton.Text = "Добавить тег";
            this.tegAddButton.UseVisualStyleBackColor = true;
            this.tegAddButton.Click += new System.EventHandler(this.tegAddButton_Click);
            // 
            // findTegTextBox
            // 
            this.findTegTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.findTegTextBox.Location = new System.Drawing.Point(4, 6);
            this.findTegTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.findTegTextBox.Name = "findTegTextBox";
            this.findTegTextBox.Size = new System.Drawing.Size(167, 21);
            this.findTegTextBox.TabIndex = 2;
            this.toolTip.SetToolTip(this.findTegTextBox, "Поиск...");
            this.findTegTextBox.TextChanged += new System.EventHandler(this.findTegTextBox_TextChanged);
            // 
            // findButton
            // 
            this.findButton.Location = new System.Drawing.Point(179, 4);
            this.findButton.Margin = new System.Windows.Forms.Padding(4);
            this.findButton.Name = "findButton";
            this.findButton.Size = new System.Drawing.Size(100, 25);
            this.findButton.TabIndex = 3;
            this.findButton.Text = "Отменить фильтр";
            this.findButton.UseVisualStyleBackColor = true;
            this.findButton.Click += new System.EventHandler(this.findButton_Click);
            // 
            // tagsSplitContainer
            // 
            this.tagsSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tagsSplitContainer.Location = new System.Drawing.Point(4, 151);
            this.tagsSplitContainer.Margin = new System.Windows.Forms.Padding(4);
            this.tagsSplitContainer.Name = "tagsSplitContainer";
            // 
            // tagsSplitContainer.Panel1
            // 
            this.tagsSplitContainer.Panel1.Controls.Add(this.allTagsMenuStrip);
            // 
            // tagsSplitContainer.Panel2
            // 
            this.tagsSplitContainer.Panel2.Controls.Add(this.chosenTagsMenuStrip);
            this.tagsSplitContainer.Size = new System.Drawing.Size(676, 168);
            this.tagsSplitContainer.SplitterDistance = 338;
            this.tagsSplitContainer.TabIndex = 10;
            // 
            // allTagsMenuStrip
            // 
            this.allTagsMenuStrip.BackColor = System.Drawing.Color.White;
            this.allTagsMenuStrip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.allTagsMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ghhToolStripMenuItem,
            this.uuuToolStripMenuItem,
            this.ghhToolStripMenuItem1});
            this.allTagsMenuStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.allTagsMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.allTagsMenuStrip.Name = "allTagsMenuStrip";
            this.allTagsMenuStrip.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.allTagsMenuStrip.Size = new System.Drawing.Size(338, 168);
            this.allTagsMenuStrip.TabIndex = 8;
            this.allTagsMenuStrip.Text = "TagsMenuStrip";
            this.allTagsMenuStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.allTagsMenuStrip_ItemClicked);
            // 
            // ghhToolStripMenuItem
            // 
            this.ghhToolStripMenuItem.Name = "ghhToolStripMenuItem";
            this.ghhToolStripMenuItem.Size = new System.Drawing.Size(40, 19);
            this.ghhToolStripMenuItem.Text = "ghh";
            // 
            // uuuToolStripMenuItem
            // 
            this.uuuToolStripMenuItem.Name = "uuuToolStripMenuItem";
            this.uuuToolStripMenuItem.Size = new System.Drawing.Size(40, 19);
            this.uuuToolStripMenuItem.Text = "uuu";
            // 
            // ghhToolStripMenuItem1
            // 
            this.ghhToolStripMenuItem1.Name = "ghhToolStripMenuItem1";
            this.ghhToolStripMenuItem1.Size = new System.Drawing.Size(40, 19);
            this.ghhToolStripMenuItem1.Text = "ghh";
            // 
            // chosenTagsMenuStrip
            // 
            this.chosenTagsMenuStrip.BackColor = System.Drawing.Color.White;
            this.chosenTagsMenuStrip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chosenTagsMenuStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.chosenTagsMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.chosenTagsMenuStrip.Name = "chosenTagsMenuStrip";
            this.chosenTagsMenuStrip.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.chosenTagsMenuStrip.Size = new System.Drawing.Size(334, 168);
            this.chosenTagsMenuStrip.TabIndex = 9;
            this.chosenTagsMenuStrip.Text = "TagsMenuStrip";
            this.chosenTagsMenuStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.chosenTagsMenuStrip_ItemClicked);
            // 
            // EditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 361);
            this.Controls.Add(this.tableLayoutPanel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "EditForm";
            this.Text = "Добавить/изменить";
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEdit)).EndInit();
            this.addAndFindTaftableLayoutPanel.ResumeLayout(false);
            this.addAndFindTaftableLayoutPanel.PerformLayout();
            this.tagsSplitContainer.Panel1.ResumeLayout(false);
            this.tagsSplitContainer.Panel1.PerformLayout();
            this.tagsSplitContainer.Panel2.ResumeLayout(false);
            this.tagsSplitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tagsSplitContainer)).EndInit();
            this.tagsSplitContainer.ResumeLayout(false);
            this.allTagsMenuStrip.ResumeLayout(false);
            this.allTagsMenuStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label labelEdit;
        private System.Windows.Forms.Label labelSelect;
        private System.Windows.Forms.DataGridView dataGridViewEdit;
        private System.Windows.Forms.Button button;
        private System.Windows.Forms.MenuStrip allTagsMenuStrip;
        private System.Windows.Forms.TableLayoutPanel addAndFindTaftableLayoutPanel;
        private System.Windows.Forms.TextBox newTegTextBox;
        private System.Windows.Forms.Button tegAddButton;
        private System.Windows.Forms.TextBox findTegTextBox;
        private System.Windows.Forms.Button findButton;
        private System.Windows.Forms.SplitContainer tagsSplitContainer;
        private System.Windows.Forms.MenuStrip chosenTagsMenuStrip;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ToolStripMenuItem ghhToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uuuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ghhToolStripMenuItem1;
    }
}