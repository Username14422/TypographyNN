using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using DGVPrinterHelper;
using Microsoft.VisualBasic;

namespace TypographyNN
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SqlC.Open();
            //Application.Run(new SpecialistInWorkingWithCustomersForm());
            Application.Run(new AuthorizationForm());

        }        
        public static FileDialogResults fileDialog = FileDialogResults.exit;
        public static string FileDialog(string path)
        {
            if (fileDialog == FileDialogResults.edit)
            {
                OpenFileDialog ofd = new OpenFileDialog();//диалог открытия файла
                ofd.RestoreDirectory = true;
                if (ofd.ShowDialog() == DialogResult.OK) { path = ofd.FileName; }
            }
            else if (fileDialog == FileDialogResults.open)
            {
                try { System.Diagnostics.Process.Start(path); } catch { }
            }
            fileDialog = FileDialogResults.exit;
            return path;           
        }
        public static void SetDataGridViewDataSource(String s, ref DataGridView dgw)
        {
            int currentRow;
            int currentColumn; 
            if (dgw.CurrentCell == null)
            {
                currentRow = 0;
                currentColumn = 1;
            }
            else
            {
                currentRow = dgw.CurrentCell.RowIndex;
                currentColumn = dgw.CurrentCell.ColumnIndex;
            }
            SqlDataAdapter dataAdapter = new SqlDataAdapter(s, SqlC.c);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            dgw.DataSource = dataTable;
            try { dgw.CurrentCell = dgw.Rows[currentRow].Cells[currentColumn]; } catch { }
        }
        public static void FilterDataGridView(String filterText, ref DataGridView filterDgw)
        {
            ((DataTable)filterDgw.DataSource).DefaultView.RowFilter = filterText;
        }
        public static void SqlCommandExecute(String commandName, String commandText, String[] paramsNames, SqlDbType[] types, Object[] values)
        {
            switch (commandName.ToLower())
            {
                case "insert":
                    {
                        SqlDataAdapter dataAdapter = new SqlDataAdapter();
                        dataAdapter.InsertCommand = new SqlCommand(commandText, SqlC.c);
                        for (int i = 0; i < paramsNames.Length; i++)
                        {
                            dataAdapter.InsertCommand.Parameters.Add(new SqlParameter(paramsNames[i], types[i]));
                            dataAdapter.InsertCommand.Parameters[i].Value = values[i];
                        }
                        dataAdapter.InsertCommand.ExecuteNonQuery();
                    }
                    break;
                case "update":
                    {
                        SqlDataAdapter dataAdapter = new SqlDataAdapter();
                        dataAdapter.UpdateCommand = new SqlCommand(commandText, SqlC.c);
                        for (int i = 0; i < paramsNames.Length; i++)
                        {
                            dataAdapter.UpdateCommand.Parameters.Add(new SqlParameter(paramsNames[i], types[i]));
                            dataAdapter.UpdateCommand.Parameters[i].Value = values[i];
                        }
                        dataAdapter.UpdateCommand.ExecuteNonQuery();
                    }
                    break;
                case "delete":
                    {
                        SqlDataAdapter dataAdapter = new SqlDataAdapter();
                        dataAdapter.DeleteCommand = new SqlCommand(commandText, SqlC.c);
                        for (int i = 0; i < paramsNames.Length; i++)
                        {
                            dataAdapter.DeleteCommand.Parameters.Add(new SqlParameter(paramsNames[i], types[i]));
                            dataAdapter.DeleteCommand.Parameters[i].Value = values[i];
                        }
                        dataAdapter.DeleteCommand.ExecuteNonQuery();
                    }
                    break;
                default: MessageBox.Show("invalid Sql command name"); break;
            }
        }
        public static void InitializeComboBox(String dataAdapterapterString, String displayMem,
           String valueMem, object[] defaultDataRowParams, ref ComboBox combBx)
        {
            try
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter(@dataAdapterapterString, SqlC.c);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                DataRow defaultDataRow = dataTable.NewRow();
                defaultDataRow.ItemArray = defaultDataRowParams;
                dataTable.Rows.InsertAt(defaultDataRow, 0);

                double symbolScale = 6.45;
                int maxItemLenght = combBx.Width;
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    if (maxItemLenght < ((int)(dataTable.Rows[i][displayMem].ToString().Length * symbolScale)))
                    { maxItemLenght = ((int)(dataTable.Rows[i][displayMem].ToString().Length * symbolScale)); }
                }
                combBx.DropDownWidth = maxItemLenght;

                combBx.DataSource = dataTable;
                combBx.DisplayMember = displayMem;
                combBx.ValueMember = valueMem;
                combBx.SelectedIndex = 0;
            }
            catch { }
        }
        public static void InitializeComboBox(String dataAdapterapterString, String displayMem,
          String valueMem, object[] defaultDataRowParams, ref DataGridViewComboBoxColumn combBx)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter(@dataAdapterapterString, SqlC.c);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);

            DataRow defaultDataRow = dataTable.NewRow();
            defaultDataRow.ItemArray = defaultDataRowParams;
            dataTable.Rows.InsertAt(defaultDataRow, 0);

            double symbolScale = 6.45;
            int maxItemLenght = combBx.Width;
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                if (maxItemLenght < ((int)(dataTable.Rows[i][displayMem].ToString().Length * symbolScale)))
                { maxItemLenght = ((int)(dataTable.Rows[i][displayMem].ToString().Length * symbolScale)); }
            }
            combBx.DropDownWidth = maxItemLenght;

            combBx.DataSource = dataTable;
            combBx.DisplayMember = displayMem;
            combBx.ValueMember = valueMem;
        }
        public static void FilterLabelClick(object sender, EventArgs e)
        {
            switch (((Label)sender).Text)
            {
                case ">=":
                    ((Label)sender).Text = "<="; break;
                case "<=":
                    ((Label)sender).Text = "="; break;
                case "=":
                    ((Label)sender).Text = ">="; break;
            }
        }
        public static DGVPrinter DefaultPrinter()
        {
            DGVPrinter printer = new DGVPrinter();
            printer.SubTitle = Interaction.InputBox("Введите название отчета");
            printer.SubTitleFormatFlags = StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            printer.PorportionalColumns = true;
            printer.HeaderCellAlignment = StringAlignment.Near;
            printer.FooterSpacing = 15;
            return printer;
        }
    }
    enum FileDialogResults { edit, open, exit}    
}
