using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace TypographyNN
{
    public partial class RegistrationForm : Form
    {
        public RegistrationForm()
        {
            InitializeComponent();
            InitializeCombBx(ref comboBoxType);
        }
        
        public static void InitializeCombBx(ref ComboBox combBx)
        {
            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("position");
            dataTable.Columns.Add("value");
            string displayMem = "position";

            dataTable.Rows.Add(new object[] { "Администратор", 0 });
            dataTable.Rows.Add(new object[] {"Специалист по работе с клиентами", 1 });
            dataTable.Rows.Add(new object[] { "Руководитель производственного отдела", 2 });
            dataTable.Rows.Add(new object[] { "Заведующий материалами", 3 });
            dataTable.Rows.Add(new object[] { "Кладовщик готовой продукции", 4 });

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
            combBx.ValueMember = "value";
            combBx.SelectedIndex = 0;            
        }
        
        private void buttonRegistration_Click(object sender, EventArgs e)
        {
            //проверка и обрезка данных
            string login = textBoxLogin.Text.Trim();
            string password = textBoxPassword.Text.Trim();
            if ((login.Length >= 5) &&
                (password.Length >= 5) &&
                (password == textBoxPassword2.Text.ToString()))
            {
                try
                {
                    SqlC.SqlCommandExecute("insert", "insert into \"User\" (login, password, type) values (@login, @password, @type)",
                                       new String[] { @"login", @"password", @"type" },
                                       new SqlDbType[] { SqlDbType.VarChar, SqlDbType.VarChar, SqlDbType.Int },
                                       new object[] { login, password, Int16.Parse(comboBoxType.SelectedValue.ToString()) });
                    MessageBox.Show("Пользователь успешно добавлен");
                    this.Close();
                }
                catch { MessageBox.Show("Пользователь с таким именем уже существует!"); }
            }
            else MessageBox.Show("Проверьте введённые данные: логин и пароль должны содержать " +
                "от 5 до 20 символов, оба раза должен быть введён один и тот же пароль");
        }

        //
        
    
    }

}
