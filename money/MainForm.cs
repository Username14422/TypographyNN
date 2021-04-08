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
using Microsoft.VisualBasic;

namespace money
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.Size = new Size(855, 515);
            UpdateArrivalAndExpenseTab();
            arrivalDateTimePicker2.Value = DateTime.Today;

            UpdateTagDataGridView();

        }

        #region main
        #region updates
        int currentRowMainDataGridView = 0;
        int countOfPoints = 3;

        public void UpdateArrivalAndExpenseTab()
        {
            string selectedValue = "";
            try
            {
                selectedValue = mainComboBox.SelectedValue.ToString();
            }
            catch { }
            Program.InitializeComboBox(@"select tag_name as name from Tag_of_flow
                                         group by tag_name",
                                    "name",
                                    "name",
                                    new object[] { "" },
                                    ref mainComboBox);
            mainComboBox.SelectedValue = selectedValue;
            UpdateFilters(null, null);
        }

        public void UpdateMainDataGridView()
        {
            string strArrivalSelect = @"select id as ID, date as 'Дата', arrival_or_expense as 'Это доход', 
                                    summa as 'Сумма', tags as 'Теги', description as 'Комментарий'
                                    from Cash_flow
                                    where basic_or_reserve = " + (radioButtonBasic.Checked ? 1 : 0) + @" and
                                          arrival_or_expense = " + (checkBoxArrival.Checked & !checkBoxExpense.Checked ? 1.ToString() :
                                    (!checkBoxArrival.Checked & checkBoxExpense.Checked ? 0.ToString() : "arrival_or_expense"));
            Program.SetDataGridViewDataSource(strArrivalSelect, ref mainDataGridView);
            mainDataGridView.Columns["ID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            mainDataGridView.Columns["Это доход"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            mainDataGridView.Sort(mainDataGridView.Columns["ID"], ListSortDirection.Descending);
            mainDataGridView.Columns["Сумма"].DefaultCellStyle.Format = "F";
            mainDataGridView.AlternatingRowsDefaultCellStyle.BackColor = SystemColors.InactiveBorder;

            string filterText = String.Format(@"[Дата] >= #{1}.{0}.{2}# and 
                                                [Дата] <= #{4}.{3}.{5}# and 
                                                [Сумма] >= {6} and 
                                                [Сумма] <= {7} and  
                                                [Теги] like '%{8}%' and
                                                [Комментарий] like '%{9}%'",                                                                                                                              //[Задержка по вине клиента] = {20}",
                    arrivalDateTimePicker1.Value.Day, arrivalDateTimePicker1.Value.Month, arrivalDateTimePicker1.Value.Year,                                                                                                                            //[Задержка по вине клиента] = {20}",
                    arrivalDateTimePicker2.Value.Day, arrivalDateTimePicker2.Value.Month, arrivalDateTimePicker2.Value.Year,
                    arrivalNumericUpDown1.Value,
                    arrivalNumericUpDown2.Value,
                    mainComboBox.SelectedValue,
                    arrivalTextBox.Text);
            Program.FilterDataGridView(filterText, ref mainDataGridView);
        }

        private void mainDataGridView_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                currentRowMainDataGridView = (int)mainDataGridView.CurrentRow.Cells["ID"].Value;
            }
            catch { }
        }

        public void UpdateChart()
        {
            mainChart.Series["arrivalSeries"].Points.Clear();
            mainChart.Series["expenseSeries"].Points.Clear();

            //arrivals
            if (checkBoxArrival.Checked | (!checkBoxArrival.Checked && !checkBoxExpense.Checked))
            {
                string arrivals = "";
                try
                {
                    arrivals = String.Format(@"select name, sum(summa) as sum from Tag
                            join Tag_of_flow on Tag.name = Tag_of_flow.tag_name
                            join Cash_flow on Cash_flow.id = Tag_of_flow.flow_id
                            where arrival_or_expense = 1 and
                                basic_or_reserve = {0} and  
								date >= '{1}-{2}-{3}' and
								date <= '{4}-{5}-{6}' and
								summa >= {7} and
								summa <= {8} and
								description like '%{9}%' and
								name like '%{10}%'
                            group by name
                            order by sum desc",                                                                                                                              //[Задержка по вине клиента] = {20}",
                      (radioButtonBasic.Checked ? "1" : "0 and name != 'Резерв'"),
                      arrivalDateTimePicker1.Value.Day, arrivalDateTimePicker1.Value.Month, arrivalDateTimePicker1.Value.Year,                                                                                                                            //[Задержка по вине клиента] = {20}",
                      arrivalDateTimePicker2.Value.Day, arrivalDateTimePicker2.Value.Month, arrivalDateTimePicker2.Value.Year,
                      arrivalNumericUpDown1.Value.ToString(),
                      arrivalNumericUpDown2.Value.ToString(),
                      arrivalTextBox.Text,
                      mainComboBox.SelectedValue.ToString());
                }
                catch
                {
                    arrivals = String.Format(@"select name, sum(summa) as sum from Tag
                            join Tag_of_flow on Tag.name = Tag_of_flow.tag_name
                            join Cash_flow on Cash_flow.id = Tag_of_flow.flow_id
                            where arrival_or_expense = 1 and
                                basic_or_reserve = {0} and  
								date >= '{1}-{2}-{3}' and
								date <= '{4}-{5}-{6}' and
								summa >= {7} and
								summa <= {8} and
								description like '%{9}%'
                            group by name
                            order by sum desc",                                                                                                                              //[Задержка по вине клиента] = {20}",
                     (radioButtonBasic.Checked ? "1" : "0 and name != 'Резерв'"),
                     arrivalDateTimePicker1.Value.Day, arrivalDateTimePicker1.Value.Month, arrivalDateTimePicker1.Value.Year,                                                                                                                            //[Задержка по вине клиента] = {20}",
                     arrivalDateTimePicker2.Value.Day, arrivalDateTimePicker2.Value.Month, arrivalDateTimePicker2.Value.Year,
                     arrivalNumericUpDown1.Value.ToString(),
                     arrivalNumericUpDown2.Value.ToString(),
                     arrivalTextBox.Text);
                }
                SqlDataAdapter dataAdapter = new SqlDataAdapter(arrivals, SqlC.c);
                DataTable popularTagsDataTable = new DataTable();
                dataAdapter.Fill(popularTagsDataTable);


                for (int i = 0; (i < popularTagsDataTable.Rows.Count) & (i < countOfPoints); i++)
                {
                    mainChart.Series["arrivalSeries"].Points.AddY(popularTagsDataTable.Rows[i][1]);
                    mainChart.Series["arrivalSeries"].Points[i].Label = popularTagsDataTable.Rows[i][0].ToString().Trim() +
                        String.Format("  +{0:C}", (popularTagsDataTable.Rows[i][1]));
                    mainChart.Series["arrivalSeries"].Points[i].Color = SystemColors.InactiveBorder;
                }
            }
            //expenses
            if (checkBoxExpense.Checked | (!checkBoxArrival.Checked && !checkBoxExpense.Checked))
            {
                string expenses = "";
                try
                {
                    expenses = String.Format(@"select name, sum(summa) as sum from Tag
                            join Tag_of_flow on Tag.name = Tag_of_flow.tag_name
                            join Cash_flow on Cash_flow.id = Tag_of_flow.flow_id
                            where arrival_or_expense = 0  and
                                basic_or_reserve = {0} and  
								date >= '{1}-{2}-{3}' and
								date <= '{4}-{5}-{6}' and
								summa >= {7} and
								summa <= {8} and
								description like '%{9}%' and
								name like '%{10}%'
                            group by name
                            order by sum desc",                                                                                                                              //[Задержка по вине клиента] = {20}",
                        (radioButtonBasic.Checked ? "1" : "0 and name != 'Резерв'"),                                                                                                                            //[Задержка по вине клиента] = {20}",
                        arrivalDateTimePicker1.Value.Day, arrivalDateTimePicker1.Value.Month, arrivalDateTimePicker1.Value.Year,                                                                                                                            //[Задержка по вине клиента] = {20}",
                        arrivalDateTimePicker2.Value.Day, arrivalDateTimePicker2.Value.Month, arrivalDateTimePicker2.Value.Year,
                        arrivalNumericUpDown1.Value.ToString(),
                        arrivalNumericUpDown2.Value.ToString(),
                        arrivalTextBox.Text,
                        mainComboBox.SelectedValue.ToString());
                }
                catch
                {
                    expenses = String.Format(@"select name, sum(summa) as sum from Tag
                            join Tag_of_flow on Tag.name = Tag_of_flow.tag_name
                            join Cash_flow on Cash_flow.id = Tag_of_flow.flow_id
                            where arrival_or_expense = 0  and
                                basic_or_reserve = {0} and  
								date >= '{1}-{2}-{3}' and
								date <= '{4}-{5}-{6}' and
								summa >= {7} and
								summa <= {8} and
								description like '%{9}%'
                            group by name
                            order by sum desc",                                                                                                                              //[Задержка по вине клиента] = {20}",
                        (radioButtonBasic.Checked ? "1" : "0 and name != 'Резерв'"),                                                                                                                            //[Задержка по вине клиента] = {20}",
                        arrivalDateTimePicker1.Value.Day, arrivalDateTimePicker1.Value.Month, arrivalDateTimePicker1.Value.Year,                                                                                                                            //[Задержка по вине клиента] = {20}",
                        arrivalDateTimePicker2.Value.Day, arrivalDateTimePicker2.Value.Month, arrivalDateTimePicker2.Value.Year,
                        arrivalNumericUpDown1.Value.ToString(),
                        arrivalNumericUpDown2.Value.ToString(),
                        arrivalTextBox.Text);
                }
                SqlDataAdapter dataAdapter = new SqlDataAdapter(expenses, SqlC.c);
                DataTable popularTagsDataTable = new DataTable();
                dataAdapter.Fill(popularTagsDataTable);

                for (int i = 0; (i < popularTagsDataTable.Rows.Count) & (i < countOfPoints); i++)
                {
                    mainChart.Series["expenseSeries"].Points.AddY(popularTagsDataTable.Rows[i][1]);
                    mainChart.Series["expenseSeries"].Points[i].Label = popularTagsDataTable.Rows[i][0].ToString().Trim() +
                        String.Format("  -{0:C}", (popularTagsDataTable.Rows[i][1]));
                    mainChart.Series["expenseSeries"].Points[i].Color = Color.Moccasin;
                }
            }
        }

        public void UpdateMenuItems()
        {
            if (radioButtonBasic.Checked)
            {
                toolStripMenuItemAddArrival.Text = "Добавить приход";
                toolStripMenuItemAddExpense.Text = "Добавить расход";
            }
            else
            {
                toolStripMenuItemAddArrival.Text = "Положить в резерв";
                toolStripMenuItemAddExpense.Text = "Достать из резерва";
            }
        }

        public void UpdateLabeles()
        {
            decimal arrivals = 0;
            decimal expenses = 0;
            int arrivalsCount = 0;
            int expensesCount = 0;
            for (int i = 0; i < mainDataGridView.RowCount; i++)
            {
                if ((bool)mainDataGridView.Rows[i].Cells["Это доход"].Value)
                {
                    arrivals += (decimal)mainDataGridView.Rows[i].Cells["Сумма"].Value;
                    arrivalsCount++;
                }
                else
                {
                    expenses += (decimal)mainDataGridView.Rows[i].Cells["Сумма"].Value;
                    expensesCount++;
                }
            }
            labelArrivalsSumValue.Text = arrivals.ToString("0.##");
            labelExpensesSumValue.Text = expenses.ToString("0.##");
            labelArrivalAndExpenseDifferenceValue.Text = (arrivals - expenses).ToString("0.##");

            try { labelArrivalsAvgValue.Text = (arrivals / arrivalsCount).ToString("0.##"); } catch { labelArrivalsAvgValue.Text = "-"; }

            try { labelExpenseAvgValue.Text = (expenses / expensesCount).ToString("0.##"); } catch { labelExpenseAvgValue.Text = "-"; }

            try { labelAvgValue.Text = ((arrivals - expenses) / mainDataGridView.RowCount).ToString("0.##"); } catch { labelAvgValue.Text = "-"; }

        }

        private void labelArrivalAndExpenseDifferenceValue_TextChanged(object sender, EventArgs e)
        {
            if (labelArrivalAndExpenseDifferenceValue.Text[0] == '-' | labelArrivalAndExpenseDifferenceValue.Text[0] == '0')
            {
                labelArrivalAndExpenseDifferenceValue.BackColor = Color.Moccasin;
            }
            else
            {
                labelArrivalAndExpenseDifferenceValue.BackColor = Color.White;
            }
        }

        private void MainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateArrivalAndExpenseTab();
            UpdateTagTab();

        }
        #endregion

        #region filters
        //filters 

        private void UpdateFilters(object sender, EventArgs e)
        {
            UpdateMainDataGridView();
            UpdateChart();
            UpdateLabeles();
            UpdateMenuItems();
            label.Text = (radioButtonBasic.Checked ? "Дневник" : "Резерв");
        }

        private void arrivalFiltersCancelButton_Click(object sender, EventArgs e)//сбросить фильтр
        {
            arrivalDateTimePicker1.Value = DateTime.Parse("01.01.2021");
            arrivalDateTimePicker2.Value = DateTime.Today;
            arrivalNumericUpDown1.Value = arrivalNumericUpDown1.Minimum;
            arrivalNumericUpDown2.Value = arrivalNumericUpDown2.Maximum;
            arrivalTextBox.Text = "";
            mainComboBox.SelectedValue = "";

            UpdateFilters(null, null);
        }
        #endregion

        #region menus
        //menus
        private void arrivalAddToolStripMenuItem_Click(object sender, EventArgs e) //add arrival
        {
            EditForm arrivalAdd;
            if (radioButtonBasic.Checked)
            {
                arrivalAdd = new EditForm(@"select date as 'Дата', summa as 'Сумма', description as 'Комментарий'
                                from Cash_flow
                                where  id = 0",
                                          new object[] { DateTime.Today, 0, " " },//defaults
                                          new int[] { 0, 1 },//req
                                          @"insert into Cash_flow 
                                (date, arrival_or_expense, summa, description, basic_or_reserve)  
                                values (@date, 1, @sum, TRIM (@desc), 1)",//insert/edit str
                                          new string[] { "@date", "@sum", "@desc" },
                                          new SqlDbType[] { SqlDbType.Date, SqlDbType.Money, SqlDbType.VarChar },
                                          true, 0, CheckSum.None, false);
                arrivalAdd.ShowDialog();
            }
            else//доход резерва (расход основы)
            {
                arrivalAdd = new EditForm(@"select date as 'Дата', summa as 'Сумма', description as 'Комментарий'
                                from Cash_flow
                                where  id = 0",
                                          new object[] { DateTime.Today, 0, " " },//defaults
                                          new int[] { 0, 1 },//req
                                          @"insert into Cash_flow 
                                (date, arrival_or_expense, summa, description, basic_or_reserve) 
                                values (@date, 1, @sum, TRIM (@desc), 0)",//insert/edit str
                                          new string[] { "@date", "@sum", "@desc" },
                                          new SqlDbType[] { SqlDbType.Date, SqlDbType.Money, SqlDbType.VarChar },
                                          true, 0, CheckSum.Balance, true);
                arrivalAdd.ShowDialog();

                //создать идентичную запись как расход  
                int idForTags = (int)Program.FindMaxId("Cash_Flow");//поиск нового ид                      
                //создать дт со значениями 
                string s = @"select date,  summa, description
                    from Cash_flow
                    where id =  " + idForTags;
                SqlDataAdapter dataAdapter = new SqlDataAdapter(s, SqlC.c);
                DataTable valuesDataTable = new DataTable();
                dataAdapter.Fill(valuesDataTable);
                //заполнить массив из строки
                object[] values = new object[valuesDataTable.Columns.Count];//массив полей записи
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = valuesDataTable.Rows[0][i];
                }
                //создать идентичную запись как расход
                Program.SqlCommandExecute("insert", @"insert into Cash_flow
                                (date, arrival_or_expense, summa, description, basic_or_reserve)
                                values (@date, 0, @sum, @desc, 1)",
                                new string[] { "@date", "@sum", "@desc" },
                                new SqlDbType[] { SqlDbType.Date, SqlDbType.Money, SqlDbType.VarChar },
                                values);//добавление новой записи

                //добавление тегов и строки из тегов в базу
                //дт из тегов старой записи
                s = @"select tag_name as name from Tag_of_flow 
                    where flow_id = " + idForTags;
                //поиск ид новой записи                
                dataAdapter = new SqlDataAdapter(s, SqlC.c);
                DataTable tagDataTable = new DataTable();
                dataAdapter.Fill(tagDataTable);
                //добавление тегов новой записи в базу
                idForTags = (int)Program.FindMaxId("Cash_Flow");
                foreach (DataRow dr in tagDataTable.Rows)
                {
                    Program.SqlCommandExecute("insert", @"insert into Tag_of_flow 
                                                    (tag_name, flow_id) 
                                                    values (@name, @flow)",
                               new string[] { @"name", @"flow" },
                               new SqlDbType[] { SqlDbType.VarChar, SqlDbType.Int },
                               new object[] { dr[0], idForTags });
                }
                //делаем строку из тегов
                string tags = Program.StringList(tagDataTable, '|');
                //апдейтим запись новой строкой тегов
                Program.SqlCommandExecute("update", @"update Cash_flow
                                                    set tags = @tags
                                                    where id = " + idForTags,
                                             new string[] { "@tags" },
                                             new SqlDbType[] { SqlDbType.VarChar },
                                             new object[] { tags });
            }
            UpdateArrivalAndExpenseTab();
        }

        private void toolStripMenuItemAddExpense_Click(object sender, EventArgs e)// add expense
        {
            EditForm arrivalAdd;
            if (radioButtonBasic.Checked) //расход основы
            {
                arrivalAdd = new EditForm(@"select date as 'Дата', summa as 'Сумма', description as 'Комментарий'
                                from Cash_flow
                                where  id = 0",
                                               new object[] { DateTime.Today, 0, " " },//defaults
                                               new int[] { 0, 1 },//req
                                               @"insert into Cash_flow 
                                (date, arrival_or_expense, summa, description, basic_or_reserve) 
                                values (@date, 0, @sum, TRIM (@desc), 1)",//insert/edit str
                                               new string[] { "@date", "@sum", "@desc" },
                                               new SqlDbType[] { SqlDbType.Date, SqlDbType.Money, SqlDbType.VarChar },
                                               true, 0, CheckSum.Balance, false);
                arrivalAdd.ShowDialog();
            }
            else //расход резерва
            {
                arrivalAdd = new EditForm(@"select date as 'Дата', summa as 'Сумма', description as 'Комментарий'
                                from Cash_flow
                                where  id = 0",
                                               new object[] { DateTime.Today, 0, " " },//defaults
                                               new int[] { 0, 1 },//req
                                               @"insert into Cash_flow 
                                (date, arrival_or_expense, summa, description, basic_or_reserve) 
                                values (@date, 0, @sum, TRIM (@desc), 0)",//insert/edit str
                                               new string[] { "@date", "@sum", "@desc" },
                                               new SqlDbType[] { SqlDbType.Date, SqlDbType.Money, SqlDbType.VarChar },
                                               true, 0, CheckSum.Reserve, true);
                arrivalAdd.ShowDialog();

                //создать идентичные записи как доход                
                int idForTags = (int)Program.FindMaxId("Cash_Flow");//поиск нового ид                
                //создать дт со значениями 
                string s = @"select date,  summa, description
                    from Cash_flow
                    where id =  " + idForTags;
                SqlDataAdapter dataAdapter = new SqlDataAdapter(s, SqlC.c);
                DataTable valuesDataTable = new DataTable();
                dataAdapter.Fill(valuesDataTable);
                //заполнить массив из строки дт
                object[] values = new object[valuesDataTable.Columns.Count];//массив полей записи
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = valuesDataTable.Rows[0][i];
                }
                //создать идентичные записи как доход
                Program.SqlCommandExecute("insert", @"insert into Cash_flow
                                (date, arrival_or_expense, summa, description, basic_or_reserve)
                                values (@date, 1, @sum, @desc, 1)",
                                new string[] { "@date", "@sum", "@desc" },
                                new SqlDbType[] { SqlDbType.Date, SqlDbType.Money, SqlDbType.VarChar },
                                values);//добавление новой записи дохода
                //добавить теги
                //дт из тегов старой записи
                s = @"select tag_name as name from Tag_of_flow 
                    where flow_id = " + idForTags;
                dataAdapter = new SqlDataAdapter(s, SqlC.c);
                DataTable tagDataTable = new DataTable();
                dataAdapter.Fill(tagDataTable);
                //поиск ид новой записи
                idForTags = (int)Program.FindMaxId("Cash_Flow");
                //добавление тегов новой записи в базу
                foreach (DataRow dr in tagDataTable.Rows)
                {
                    Program.SqlCommandExecute("insert", @"insert into Tag_of_flow 
                                                    (tag_name, flow_id) 
                                                    values (@name, @flow)",
                               new string[] { @"name", @"flow" },
                               new SqlDbType[] { SqlDbType.VarChar, SqlDbType.Int },
                               new object[] { dr[0], idForTags }); //для дохода
                }
                //делаем строку из тегов
                string tags = Program.StringList(tagDataTable, '|');
                //апдейтим запись новой строкой тегов
                Program.SqlCommandExecute("update", @"update Cash_flow
                                                    set tags = @tags
                                                    where id = " + (idForTags),
                                             new string[] { "@tags" },
                                             new SqlDbType[] { SqlDbType.VarChar },
                                             new object[] { tags });//для дохода
            }
            UpdateArrivalAndExpenseTab();
        }

        private void arrivalEditToolStripMenuItem_Click(object sender, EventArgs e) //edit
        {
            if (radioButtonBasic.Checked && (bool)mainDataGridView.CurrentRow.Cells["Это доход"].Value == false)//расход основы
            {
                EditForm arrivalEdit = new EditForm(@"select date as 'Дата', arrival_or_expense as 'это доход', summa as 'Сумма', description as 'Комментарий'
                                from Cash_flow
                                where  id =" + currentRowMainDataGridView,
                                              null,//defaults
                                              new int[] { 0, 2 },
                                              @"update Cash_flow 
                                        set date = @date,
                                        arrival_or_expense = @arex,
                                        summa = @sum,
                                        description = TRIM (@desc)
                                        where id =" + currentRowMainDataGridView,
                                              new string[] { "@date", "@arex", "sum", "@desc" },
                                              new SqlDbType[] { SqlDbType.Date, SqlDbType.Bit, SqlDbType.Money, SqlDbType.VarChar },
                                              true, currentRowMainDataGridView, CheckSum.Balance, false);
                arrivalEdit.ShowDialog();
            }
            else if (!radioButtonBasic.Checked && (bool)mainDataGridView.CurrentRow.Cells["Это доход"].Value == false)//расход резерва
            {
                EditForm arrivalEdit = new EditForm(@"select date as 'Дата', arrival_or_expense as 'это доход', summa as 'Сумма', description as 'Комментарий'
                                from Cash_flow
                                where  id =" + currentRowMainDataGridView,
                                              null,//defaults
                                              new int[] { 0, 2 },
                                              @"update Cash_flow 
                                        set date = @date,
                                        arrival_or_expense = @arex,
                                        summa = @sum,
                                        description = TRIM (@desc)
                                        where id =" + currentRowMainDataGridView,
                                              new string[] { "@date", "@arex", "sum", "@desc" },
                                              new SqlDbType[] { SqlDbType.Date, SqlDbType.Bit, SqlDbType.Money, SqlDbType.VarChar },
                                              true, currentRowMainDataGridView, CheckSum.Reserve, true);
                arrivalEdit.ShowDialog();
            }
            else if (!radioButtonBasic.Checked && (bool)mainDataGridView.CurrentRow.Cells["Это доход"].Value == true)//доход резерва(расход основы)
            {
                EditForm arrivalEdit = new EditForm(@"select date as 'Дата', arrival_or_expense as 'это доход', summa as 'Сумма', description as 'Комментарий'
                                from Cash_flow
                                where  id =" + currentRowMainDataGridView,
                                              null,//defaults
                                              new int[] { 0, 2 },
                                              @"update Cash_flow 
                                        set date = @date,
                                        arrival_or_expense = @arex,
                                        summa = @sum,
                                        description = TRIM (@desc)
                                        where id =" + currentRowMainDataGridView,
                                              new string[] { "@date", "@arex", "sum", "@desc" },
                                              new SqlDbType[] { SqlDbType.Date, SqlDbType.Bit, SqlDbType.Money, SqlDbType.VarChar },
                                              true, currentRowMainDataGridView, CheckSum.Balance, true);
                arrivalEdit.ShowDialog();
            }
            else //другое
            {
                if (radioButtonBasic.Checked)
                {
                    EditForm arrivalEdit = new EditForm(@"select date as 'Дата', arrival_or_expense as 'это доход', summa as 'Сумма', description as 'Комментарий'
                                from Cash_flow
                                where  id =" + currentRowMainDataGridView,
                                                  null,//defaults
                                                  new int[] { 0, 2 },
                                                  @"update Cash_flow 
                                        set date = @date,
                                        arrival_or_expense = @arex,
                                        summa = @sum,
                                        description = TRIM (@desc)
                                        where id =" + currentRowMainDataGridView,
                                                  new string[] { "@date", "@arex", "sum", "@desc" },
                                                  new SqlDbType[] { SqlDbType.Date, SqlDbType.Bit, SqlDbType.Money, SqlDbType.VarChar },
                                                  true, currentRowMainDataGridView, CheckSum.None, false);
                    arrivalEdit.ShowDialog();
                }
                else
                {
                    EditForm arrivalEdit = new EditForm(@"select date as 'Дата', arrival_or_expense as 'это доход', summa as 'Сумма', description as 'Комментарий'
                                from Cash_flow
                                where  id =" + currentRowMainDataGridView,
                                                  null,//defaults
                                                  new int[] { 0, 2 },
                                                  @"update Cash_flow 
                                        set date = @date,
                                        arrival_or_expense = @arex,
                                        summa = @sum,
                                        description = TRIM (@desc)
                                        where id =" + currentRowMainDataGridView,
                                                  new string[] { "@date", "@arex", "sum", "@desc" },
                                                  new SqlDbType[] { SqlDbType.Date, SqlDbType.Bit, SqlDbType.Money, SqlDbType.VarChar },
                                                  true, currentRowMainDataGridView, CheckSum.None, true);
                    arrivalEdit.ShowDialog();
                }
            }

            UpdateArrivalAndExpenseTab();
        }

        private void arrivalDelToolStripMenuItem_Click(object sender, EventArgs e) //delete
        {
            DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите удалить запись из базы?", "Удаление записи", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                //try
                //{
                Program.SqlCommandExecute("delete", "delete from Tag_of_flow where flow_id =  @id",
                    new string[] { "@id" }, new SqlDbType[] { SqlDbType.Int },
                    new object[] { currentRowMainDataGridView });
                Program.SqlCommandExecute("delete", "delete from Cash_flow where id =  @id",
                    new string[] { "@id" }, new SqlDbType[] { SqlDbType.Int },
                    new object[] { currentRowMainDataGridView });


                //}
                //catch { MessageBox.Show(strDBError); }
                UpdateArrivalAndExpenseTab();
            }
        }
        #endregion
        #endregion

        #region tag

        private void UpdateTagTab()
        {
            UpdateTagDataGridView();
        }

        public void UpdateTagDataGridView()
        {
            string strTagSelect = @"select name as 'ID', sum(IIF(arrival_or_expense = 1, summa, 0)) as 'Сумма приходов', 
                    sum(IIF(arrival_or_expense = 0, summa, 0)) as 'Сумма расходов'
                    from Tag
                    left join Tag_of_flow on name = tag_name
                    left join Cash_flow  on flow_id = id
                    where (basic_or_reserve = 1 or basic_or_reserve is NULL) and (name != 'Резерв')
                    group by name";
            Program.SetDataGridViewDataSource(strTagSelect, ref tagDataGridView);
            tagDataGridView.Columns["Сумма приходов"].DefaultCellStyle.Format = "F";
            tagDataGridView.Columns["Сумма расходов"].DefaultCellStyle.Format = "F";
            tagDataGridView.AlternatingRowsDefaultCellStyle.BackColor = SystemColors.InactiveBorder;

            //string filterText = String.Format(@"[Дата] >= #{1}.{0}.{2}# and 
            //                                    [Дата] <= #{4}.{3}.{5}# and 
            //                                    [Сумма] >= {6} and 
            //                                    [Сумма] <= {7} and  
            //                                    [Теги] like '%{8}%' and
            //                                    [Комментарий] like '%{9}%'",                                                                                                                              //[Задержка по вине клиента] = {20}",
            //        arrivalDateTimePicker1.Value.Day, arrivalDateTimePicker1.Value.Month, arrivalDateTimePicker1.Value.Year,                                                                                                                            //[Задержка по вине клиента] = {20}",
            //        arrivalDateTimePicker2.Value.Day, arrivalDateTimePicker2.Value.Month, arrivalDateTimePicker2.Value.Year,
            //        arrivalNumericUpDown1.Value,
            //        arrivalNumericUpDown2.Value,
            //        mainComboBox.SelectedValue,
            //        arrivalTextBox.Text);
            //Program.FilterDataGridView(filterText, ref mainDataGridView);
        }
        string currentRowTagDataGridView = "";
        private void TagDataGridView_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                currentRowTagDataGridView = tagDataGridView.CurrentRow.Cells["ID"].Value.ToString();

                if (((decimal)tagDataGridView.CurrentRow.Cells["Сумма приходов"].Value == 0) &&
                    ((decimal)tagDataGridView.CurrentRow.Cells["Сумма расходов"].Value == 0))
                { deleteTagToolStripMenuItem.Enabled = true; }
                else { deleteTagToolStripMenuItem.Enabled = false; }
            }
            catch { }
        }


        // menus
        private void addTagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditForm addTag = new EditForm(@"select * from Tag where name = 'qwerty'",
                                        new object[] { "" },
                                        new int[] { 0 },
                                        @"insert into Tag
                                            (name)
                                            values(@name)",
                                        new string[] { @"name" },
                                        new SqlDbType[] { SqlDbType.VarChar },
                                        false,
                                        0,
                                        CheckSum.None,
                                        false);
            addTag.ShowDialog();
            UpdateTagDataGridView();
        }

        private void editTagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditForm addTag = new EditForm(@"select * from Tag where name = 'qwerty'",
                                        new object[] { "" },
                                        new int[] { 0 },
                                        @"insert into Tag
                                            (name)
                                            values(@name)",
                                        new string[] { @"name" },
                                        new SqlDbType[] { SqlDbType.VarChar },
                                        false,
                                        0,
                                        CheckSum.None,
                                        false);
            addTag.ShowDialog();
            string newTag = (string) Program.FindMaxId("Tag");
            Program.SqlCommandExecute("update", @"update Tag_of_flow
                                                set tag_name = @name
                                                where tag_name = '" + currentRowTagDataGridView + "'",
                                        new string[] { @"name" },
                                        new SqlDbType[] { SqlDbType.VarChar },
                                        new object[] { newTag });

            SqlDataAdapter da = new SqlDataAdapter(@"select id from Cash_flow where tags like '%| "+currentRowTagDataGridView+" |%'", SqlC.c);
            DataTable cashFlows = new DataTable();
            da.Fill(cashFlows);
            for (int i = 0; i < cashFlows.Rows.Count; i++)
            {
                Program.AddTagString((int)cashFlows.Rows[i][0]);
            }
            UpdateTagDataGridView();

            Program.SqlCommandExecute("delete", "delete from Tag where name =  @id",
                    new string[] { "@id" }, new SqlDbType[] { SqlDbType.VarChar },
                    new object[] { currentRowTagDataGridView });
        }

        private void replaceTagToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void deleteTagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите удалить тег из базы?", "Удаление записи", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                //try
                //{
                Program.SqlCommandExecute("delete", "delete from Tag where name =  @id",
                    new string[] { "@id" }, new SqlDbType[] { SqlDbType.VarChar },
                    new object[] { currentRowTagDataGridView });
                //catch {}
            }
            UpdateTagTab();

        }
        #endregion
    }
}
