using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using Microsoft.VisualBasic;

namespace money
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
            Application.Run(new MainForm());

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
        public static int FindMaxId(string table, string columnName)//ищет максимальный ид указанной таблицы 
        {
            string s = @"select max("+columnName+") from " + table;
            SqlDataAdapter dataAdapter = new SqlDataAdapter(s, SqlC.c);
            DataTable maxFlowId = new DataTable();
            dataAdapter.Fill(maxFlowId);
            return (int) maxFlowId.Rows[0][0];
        }

        public static string StringList(DataTable dt, Char separator)
        {
            string sl = separator +" ";
            foreach (DataRow dr in dt.Rows)
            {
                sl += dr.ItemArray[0].ToString() + " "+separator+" ";
            }
            return sl;
        }

        public static void AddTagString(int idOfEntry)//создаёт и сохраняет строку из тегов 
        {
            //гуглим теги этого чувака
            string tagsQuery = @"select tag_name from Tag_of_flow
                            where flow_id   = " + idOfEntry;
            SqlDataAdapter dataAdapter2 = new SqlDataAdapter(tagsQuery, SqlC.c);
            DataTable tagsTable = new DataTable();
            dataAdapter2.Fill(tagsTable);
            //делаем строку
            string tags = Program.StringList(tagsTable, '|');
            //аптудейтим чуввака
            Program.SqlCommandExecute("update", @"update Cash_flow
                                                    set tags = @tags
                                                    where id = " + idOfEntry,
                                         new string[] { "@tags" },
                                         new SqlDbType[] { SqlDbType.VarChar },
                                         new object[] { tags });

        }
    }
}
