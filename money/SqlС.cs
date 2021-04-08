using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace money
{
    public class SqlC
    {
        public static SqlConnection c = new SqlConnection(@"Data Source=DESKTOP-LI129IP\MSSQLSERVER17;
Initial Catalog=money;Integrated Security=True");
        /*@"Data Source = KLASS309c6\SQLEXPRESS; 
            Initial Catalog = PublishingHouse; Persist Security Info = True; User ID = sa; Password = 123456"*/

        public static void Open()
        {
            try
            {
                c.Open();
            }
            catch
            {
                MessageBox.Show("Ошибка соединения с базой данных!"); 
            }
        }
        public static void Close()
        {
            try { c.Close(); } catch { MessageBox.Show("Ошибка закрытия соединения с базой данных!"); }
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

    }
}
