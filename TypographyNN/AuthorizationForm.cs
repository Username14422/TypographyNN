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
    public partial class AuthorizationForm : System.Windows.Forms.Form
    {
        public AuthorizationForm()
        {
            InitializeComponent();
        }

        private void buttonAuthorization_Click(object sender, EventArgs e)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter("select * from \"User\"", SqlC.c);
            DataTable users = new DataTable();
            dataAdapter.Fill(users);

            users.Columns[0].ColumnName = "login";

            DataRow[] selectedRows;
            selectedRows = users.Select("login = '" + textBoxLogin.Text+ "'");

            if ((selectedRows.Length != 0)&&(selectedRows[0][1].ToString()==textBoxPassword.Text))
            {
                this.Hide();
                Form f;
                switch (selectedRows[0][2].ToString())
                {
                    case "1": { f = new SpecialistInWorkingWithCustomersForm(); break; }
                    case "2": { f = new ProductionManagerForm(); break; }
                    case "3": { f = new WarehouseAndMaterialsManagerForm(); break; }
                    case "4": { f = new FinishedProductsStorekeeperForm(); break; }
                    default: { f = new RegistrationForm(); break; }
                }
                f.ShowDialog();
                this.Show();
                //this.Close();
            }
            else
            { MessageBox.Show("Неверно введён логин ли пароль."); }
        }
    }
}
