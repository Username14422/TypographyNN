using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TypographyNN
{
    public partial class EditForm : Form
    {
        string selectDataGridViesEdit;
        string command;
        String[] paramsNames;
        SqlDbType[] types;
        object[] defaultRowItemArray;
        string selectDataGridViewSelect;
        int[] required;
        int[] files;
        
        String strDBError = "Изменения не сохранены из-за ошибки.";

        public EditForm(string selectDataGridViesEdit, object[] defaultRowItemArray, string selectDataGridViewSelect,
        int[] required, int[] files,
        string command, String[] paramsNames, SqlDbType[] types)
        {
            InitializeComponent();
            this.selectDataGridViesEdit = selectDataGridViesEdit;
            this.defaultRowItemArray = defaultRowItemArray;
            this.selectDataGridViewSelect = selectDataGridViewSelect;
            this.required = required;
            this.files = files;
            this.command = command;
            this.paramsNames = paramsNames;
            this.types = types;

            if  (selectDataGridViesEdit != null)
            {
                Program.SetDataGridViewDataSource(selectDataGridViesEdit, ref dataGridViewEdit);
                if (defaultRowItemArray != null)
                { ((DataTable)dataGridViewEdit.DataSource).Rows.Add(defaultRowItemArray);}
                else { ((DataTable)dataGridViewEdit.DataSource).Rows.Add(); }
            }
            else { HideEdit(); }                      

            if (selectDataGridViewSelect != null)
            { Program.SetDataGridViewDataSource(selectDataGridViewSelect, ref dataGridViewSelect);
                dataGridViewSelect.Columns["ID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                dataGridViewSelect.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            }
            else { HideSelect(); }

            
        }
        //public EditForm(string selectDataGridViewSelect)
        //{
        //    InitializeComponent();
        //    HideEdit();
        //    Program.SetDataGridViewDataSource(selectDataGridViewSelect, ref dataGridViewSelect);
        //}
        public EditForm()
        {
            InitializeComponent();
        }

        private void HideEdit()
        {
            tableLayoutPanel.RowStyles[0].Height = 0;
            tableLayoutPanel.RowStyles[1].Height = 0;
            labelEdit.Hide();
            dataGridViewEdit.Hide();
        }
        private void HideSelect()
        {
            this.Size = new Size(this.Size.Width, 135);
            tableLayoutPanel.RowStyles[2].Height = 0;
            tableLayoutPanel.RowStyles[3].Height = 0;
            labelSelect.Hide();
            dataGridViewSelect.Hide();
        }

        private void button_Click(object sender, EventArgs e)
        {
            if (required != null)
            {
                foreach (int req in required)
                {
                    if (dataGridViewEdit.Rows[0].Cells[req].Value.ToString() == "")
                    {
                        MessageBox.Show("Поле '" + dataGridViewEdit.Columns[req].Name + "' обязательно к заполнению!");
                        return;
                    }
                }
            }
            object[] values = new object[dataGridViewEdit.ColumnCount + (dataGridViewSelect.ColumnCount > 0 ? 1 : 0)];
            for (int i = 0; i < values.Length; i++)
            {
                if (i < dataGridViewEdit.ColumnCount)
                { values[i] = dataGridViewEdit.Rows[0].Cells[i].Value; }
                else { values[i] = dataGridViewSelect.CurrentRow.Cells["ID"].Value; }
            }
            //try
            //{
                Program.SqlCommandExecute("insert", command,
                        paramsNames, types, values);
            //}
            //catch { MessageBox.Show(strDBError); }
            Close();

        }

        private void dataGridViewEdit_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (files != null)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i] == e.ColumnIndex)
                    {
                        Program.fileDialog = FileDialogResults.edit;
                        dataGridViewEdit.Rows[0].Cells[files[i]].Value 
                            = Program.FileDialog(dataGridViewEdit.Rows[0].Cells[files[i]].Value.ToString());
                        return;
                    }
                }
            }
        }
    }
}
    

   