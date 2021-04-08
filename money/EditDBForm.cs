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

namespace money
{
    public partial class EditDBForm : Form
    {
        string selectDataGridViesEdit; //строка для таблицы ввода данных
        string command;//строка с командой
        String[] paramsNames; //массив имён параметров команды
        SqlDbType[] types;//длипы данных для команды
        object[] defaultRowItemArray; //дефолтные значения для таблицы
        int[] required; //массив номеров обязательных полей
        bool tag; //нужна ли теговая часть
        int idForTags; //или 0 для новых записей, или ид при редактировании записи
        CheckSum checkSum; //нужна ли прроверка на отрицательный баланс (для расходов)
        bool isReserve; //проводится ли операция для резерва или для основы

        ToolStripMenuItem[] clickedTags = new ToolStripMenuItem[0];//массив кликнутых тегов
        Color chosenTagColor = SystemColors.InactiveBorder;//цвет фона выбранных тегов        
        String strDBError = "Изменения не сохранены из-за ошибки. Возможно введены повторяющиеся данные.";//текст ошибки

        public EditDBForm(string selectDataGridViesEdit, 
                        object[] defaultRowItemArray,
                        int[] required,
                        string command, 
                        String[] paramsNames, 
                        SqlDbType[] types, 
                        bool tag, 
                        int idForTags,
                        CheckSum checkSum,
                        bool isReserve)//конструктор
        {
            InitializeComponent();
            this.selectDataGridViesEdit = selectDataGridViesEdit;
            this.defaultRowItemArray = defaultRowItemArray;
            this.required = required;
            this.command = command;
            this.paramsNames = paramsNames;
            this.types = types;
            this.tag = tag;
            this.idForTags = idForTags;
            this.checkSum = checkSum;
            this.isReserve = isReserve;

            if (selectDataGridViesEdit != null)//заполнение дгв
            {
                Program.SetDataGridViewDataSource(selectDataGridViesEdit, ref dataGridViewEdit);
                if (defaultRowItemArray != null)
                { ((DataTable)dataGridViewEdit.DataSource).Rows.Add(defaultRowItemArray); }

                try { dataGridViewEdit.Columns["Сумма"].DefaultCellStyle.Format = "F"; } catch { }
                this.Width = dataGridViewEdit.ColumnCount * 185;
            }
            else { HideSelect(); }

            
            if (tag)
            {
                UpdateTegs();
            }
            else { HideTags(); }
                                                
        }        
        

        private void HideSelect() //скрыть пол формы с полями
        {
            this.Size = new Size(400, 350);
            tableLayoutPanel.RowStyles[0].Height = 0;
            tableLayoutPanel.RowStyles[1].Height = 0;
            labelEdit.Hide();
            dataGridViewEdit.Hide();
        }

        private void HideTags()//скрыть пол формы с тегами
        {
            this.Size = new Size(this.Width, 150);
            labelSelect.Hide();
            addAndFindTaftableLayoutPanel.Hide();
            allTagsMenuStrip.Hide();
            chosenTagsMenuStrip.Hide();
            tableLayoutPanel.RowStyles[2].Height = 0;
            tableLayoutPanel.RowStyles[3].Height = 0;
            tableLayoutPanel.RowStyles[4].SizeType = SizeType.Absolute;
            tableLayoutPanel.RowStyles[4].Height = 0;
        }

        public void UpdateTegs()//берёт все теги из базы
        {
            for (int i = 0; i < allTagsMenuStrip.Items.Count;)//очистить меню
            {
                allTagsMenuStrip.Items.Remove(allTagsMenuStrip.Items[i]);
            }

            //теги для добавления
            string s = @"select name from Tag";
            SqlDataAdapter dataAdapter = new SqlDataAdapter(s, SqlC.c);
            DataTable tagDataTable = new DataTable();
            dataAdapter.Fill(tagDataTable);
            foreach (DataRow dr in tagDataTable.Rows)//заполнение менюшки
            {
                allTagsMenuStrip.Items.Add(new ToolStripMenuItem(dr.ItemArray[0].ToString().Trim(), null, null, dr.ItemArray[0].ToString().Trim()));
                            
            }
            allTagsMenuStrip.Items["Резерв"].Visible = false; 

            //добавленные теги
            s = "";
            if (isReserve)
            {
                s = @"select name from Tag
                    join Tag_of_flow on Tag.name = Tag_of_flow.tag_name
                    where flow_id = " + idForTags + " or  name = 'Резерв'";
            }
            else
            {
                s = @"select name from Tag
                    join Tag_of_flow on Tag.name = Tag_of_flow.tag_name
                    where flow_id = " + idForTags;
            }
            dataAdapter = new SqlDataAdapter(s, SqlC.c);
            tagDataTable = new DataTable();
            dataAdapter.Fill(tagDataTable);
            foreach (DataRow dr in tagDataTable.Rows)
            {
                for (int i = allTagsMenuStrip.Items.Count-1; i >= 0; i--)
                {
                    if (dr.ItemArray[0].ToString().Trim() == allTagsMenuStrip.Items[i].Text.Trim())
                    {
                        allTagsMenuStrip_ItemClicked(null, new ToolStripItemClickedEventArgs(allTagsMenuStrip.Items[i]));
                    }
                }
            }            
        }
        
        
        private void button_Click(object sender, EventArgs e)//главный клик!
        {
            if (required != null)//проверка что обязательные поля заполнены 
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
            
            if(checkSum != CheckSum.None && !SumIsOk(checkSum) && (decimal)dataGridViewEdit.Rows[0].Cells["Сумма"].Value==0)
            { MessageBox.Show("Сумма превышает баланс или равна нулю!"); return; }

            object[] values = new object[dataGridViewEdit.ColumnCount];//массив полей записи
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = dataGridViewEdit.Rows[0].Cells[i].Value;
            }
            //try//тк для красоты
            //{
                Program.SqlCommandExecute("insert", command,
                        paramsNames, types, values);//главная команда инсерт/апдейт из дгв
            //}
            //catch { MessageBox.Show(strDBError); }

            if (tag)
            {

                if (idForTags <= 0)//поиск ид новой записи
                {
                    idForTags = (int)Program.FindMaxId("Cash_Flow", "id");
                }

                UpdateTagsInDatabase(idForTags, clickedTags, chosenTagColor);

                Program.AddTagString(idForTags);

            }
                Close();
        }

        private bool SumIsOk(CheckSum cs)
        {
            string s = "";

            if (cs == CheckSum.Balance)
            {
                s = @"select(
                    select sum(summa) from Cash_flow
                    where arrival_or_expense = 1) 
                    -
                    (select sum(summa) from Cash_flow
                    where arrival_or_expense = 0)";
            }
            else if (cs == CheckSum.Reserve)
            {
                s = @"select(
                    select sum(summa) from Cash_flow
                    where arrival_or_expense = 1 and basic_or_reserve = 0)
                    -
                    (select sum(summa) from Cash_flow
                    where arrival_or_expense = 0 and basic_or_reserve = 0)";
            }
            SqlDataAdapter dataAdapter = new SqlDataAdapter(s, SqlC.c);
            DataTable balance = new DataTable();
            dataAdapter.Fill(balance);
            if ((decimal)balance.Rows[0][0] < (decimal)dataGridViewEdit.Rows[0].Cells["Сумма"].Value)
            { return false; }
            else { return true; }
            
        }

        

        
        private void UpdateTagsInDatabase(int idOfEntry, ToolStripMenuItem[] tags, Color chosenTagColor)//обновляет связи записей с тегами в бд 
        {
            for (int i = 0; i < tags.Length; i++)//сохраняет изменения в тегах
            {
                try//отметает лишние клики (повторные записи и удаление несуществующих)
                {
                    if (tags[i].BackColor == chosenTagColor)
                    {
                        Program.SqlCommandExecute("insert", @"insert into Tag_of_flow 
                                                                (tag_name, flow_id) 
                                                                values (@name, @flow)",
                        new string[] { @"name", @"flow" },
                        new SqlDbType[] { SqlDbType.VarChar, SqlDbType.Int },
                        new object[] { tags[i].Text.Trim(), idOfEntry });
                    }
                    else
                    {
                        Program.SqlCommandExecute("delete", @"delete from Tag_of_flow
                                                                where flow_id = @flow and tag_name = @tag",
                        new string[] { @"flow", @"tag" },
                        new SqlDbType[] { SqlDbType.Int, SqlDbType.VarChar },
                        new object[] { idOfEntry, tags[i].Text.Trim() });
                    }
                }
                catch { }
            }
        }

        private void tegAddButton_Click(object sender, EventArgs e)//добавить тег в базу
        {
            try//трай кетч для уже сохранённого
            {
                if (newTegTextBox.Text.Trim() != "")
                {
                    Program.SqlCommandExecute("insert", @"insert into Tag 
                                                (name) 
                                                values (@name)",
                                new string[] { @"name" },
                                new SqlDbType[] { SqlDbType.VarChar },
                                new object[] { newTegTextBox.Text.Trim() });
                    allTagsMenuStrip.Items.Add(newTegTextBox.Text.Trim());
                    allTagsMenuStrip_ItemClicked(null, new ToolStripItemClickedEventArgs(allTagsMenuStrip.Items[allTagsMenuStrip.Items.Count - 1]));

                    newTegTextBox.Text = "";
                }
            }
            catch { MessageBox.Show(strDBError); }
            
            
        }
        
        private void chosenTagsMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)//выделяет теги
        {
            e.ClickedItem.BackColor = allTagsMenuStrip.BackColor;

            allTagsMenuStrip.Items.Add(e.ClickedItem);

            Array.Resize(ref clickedTags, clickedTags.Length + 1);
            clickedTags[clickedTags.Length - 1] = (ToolStripMenuItem)e.ClickedItem;//массив кликнутых для работы с базой

        }

        private void allTagsMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)//отменить выделение
        {        
            e.ClickedItem.BackColor = chosenTagColor;
            
            chosenTagsMenuStrip.Items.Add(e.ClickedItem);

            Array.Resize(ref clickedTags, clickedTags.Length + 1);
            clickedTags[clickedTags.Length - 1] = (ToolStripMenuItem)e.ClickedItem;//массив кликнутых для работы с базой

        }

        private void findTegTextBox_TextChanged(object sender, EventArgs e)//фильтрует теги по поиску
        {
            for (int i = allTagsMenuStrip.Items.Count-1; i >= 0; i--)
            {
                if (allTagsMenuStrip.Items[i].Text.IndexOf(findTegTextBox.Text.Trim(), StringComparison.OrdinalIgnoreCase) >= 0)
                { allTagsMenuStrip.Items[i].Visible = true; }
                else { allTagsMenuStrip.Items[i].Visible = false; }
            }
        }

        private void findButton_Click(object sender, EventArgs e)//отменяет фильтр
        {
            findTegTextBox.Text = "";
        }
    }
    public enum CheckSum { Balance, Reserve, None}
}
    

   