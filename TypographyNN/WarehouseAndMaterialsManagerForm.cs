using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using DGVPrinterHelper;

namespace TypographyNN
{
    public partial class WarehouseAndMaterialsManagerForm : System.Windows.Forms.Form
    {
        public WarehouseAndMaterialsManagerForm()
        {
            InitializeComponent();
            this.Size = new Size(900, 600);
            UpdateMaterialTab();
        }
        String strDBError = "Изменения не сохранены из-за ошибки.";

        #region Materials
        #region select strings
        string strMaterialsSelect = @"select Material.materialID as ID, materialName as 'Наименование', isnull (ordered.quantity,0) as 'Заказанно производством', 
                                    balance as 'Остаток на складе', isnull(bought.quantity,0) as 'Из них у поставщиков', qcSum as 'Общий расход', materialPrice as 'Текущая цена'
            from (select materialID, max(dateOfMaterialPriceChange) as dat
                    from MaterialPriceChange 
                    group by materialID) as lastChange
            right join Material on(Material.materialID = lastChange.materialID)
            left join MaterialPriceChange on(Material.materialID = MaterialPriceChange.materialID and dat = MaterialPriceChange.dateOfMaterialPriceChange)
			left join 
			(   select
				materialID, sum(quantity) as quantity
				from TransferOfMaterials	
				where transferType = 0 and carriedOut = 0	
				group by materialID
			) as ordered on (Material.materialID = ordered.materialID)
			left join
			(   select arrival.materialID, (a- ISNULL ( b , 0 )  ) as balance 
                from
				(
				    Select materialID, sum (quantity) as a from TransferOfMaterials
				    where transferType = 1
				    group by materialID
				) as arrival
				full join
				(   Select materialID, sum (quantity) as b from TransferOfMaterials
				    where transferType = 0 and carriedOut = 1
				    group by materialID
				) as consumption on (arrival.materialID = consumption.materialID)
				group by arrival.materialID, a, b
			) as stokBalancee on (Material.materialID = stokBalancee.materialID)
			left join 
			(   select
				materialID, sum(quantity) as quantity
				from TransferOfMaterials	
				where transferType = 1 and carriedOut = 0	
				group by materialID
			) as bought on (Material.materialID = bought.materialID)
			left join 
			(   select consumption.materialID, sum(qc) as qcSum
				from
				(   select Material.materialID, p.productionOrderID, ISNULL (sum(quantity*numberOfCopiesMade),0) as qc
					from Material 
					left join ProductMaterial on (Material.materialID = ProductMaterial.materialID)
					left join 
                    (   select ProductInOrder.productionOrderID, productID, numberOfCopiesMade
						from ProductInOrder
						left join ProductionOrder on (ProductionOrder.productionOrderID = ProductInOrder.productionOrderID)
					)as p  on (ProductMaterial.productID = p.productID)
					group by Material.materialID, p.productionOrderID
				) as consumption				
				group by consumption.materialID
			) as indicators on (Material.materialID = indicators.materialID)";
        string strTransferOfMaterialSelect = @"select transferOfMaterialsID as ID, Material.materialId, 
                                            IIf(transferType = 1, 'приход', 'расход') as 'Тип', quantity as 'Количество', 
                                            carriedOut as 'Выполнено', transferDate as 'Дата' 
                                        from TransferOfMaterials
                                        left join Material on (Material.materialID = TransferOfMaterials.materialID)
                                        order by 'Дата' desc";
        string strPricesOfMaterialSelect = @"select materialPriceChangeID as ID, materialID, materialPrice as 'Назначенная цена', dateOfMaterialPriceChange as 'Дата изменения'
                                            from MaterialPriceChange
											order by 'Дата изменения' desc";
        #endregion

        public void UpdateMaterialTab()
        {
            Program.SetDataGridViewDataSource(strMaterialsSelect, ref dataGridViewMaterials);
            dataGridViewMaterials.Columns["ID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridViewMaterials.Columns["Наименование"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridViewMaterials.Columns["Текущая цена"].DefaultCellStyle.Format = "F";
            dataGridViewMaterials.Columns["Общий расход"].DefaultCellStyle.Format = "F3";

            Program.SetDataGridViewDataSource(strTransferOfMaterialSelect, ref dataGridViewTransferOfMaterial);
            dataGridViewTransferOfMaterial.Columns["ID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridViewTransferOfMaterial.Columns["materialId"].Visible = false;

            Program.SetDataGridViewDataSource(strPricesOfMaterialSelect, ref dataGridViewPricesOfMaterial);
            dataGridViewPricesOfMaterial.Columns["ID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridViewPricesOfMaterial.Columns["materialId"].Visible = false;
            dataGridViewPricesOfMaterial.Columns["Назначенная цена"].DefaultCellStyle.Format = "F";

            FilterDataGridViewMaterials(new Button(), null);
        }
        int currentRowMaterialId = 0;
        private void dataGridViewMaterials_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                currentRowMaterialId = (int)dataGridViewMaterials.CurrentRow.Cells[0].Value;
                Program.FilterDataGridView(String.Format("materialId = {0}", currentRowMaterialId), ref dataGridViewTransferOfMaterial);
                Program.FilterDataGridView(String.Format("materialId = {0}", currentRowMaterialId), ref dataGridViewPricesOfMaterial);
            }
            catch { }
        }
        //filt
        public void SetMaterialsFiltersWidth(object sender, DataGridViewColumnEventArgs e)
        {
            try
            {
                for (int i = 0; i < dataGridViewMaterials.ColumnCount; i++)
                {
                    tableLayoutPanelMaterialsFilters.ColumnStyles[i].Width = dataGridViewMaterials.Columns[i].Width;
                }
            }
            catch { }
        } 
        public void FilterLabelClick(object sender, EventArgs e)
        {
            Program.FilterLabelClick(sender, e);
        }
        public void FilterDataGridViewMaterials(object sender, EventArgs e)
        {
            string filterText = "";
            try { object a = ((Button)sender); }
            catch
            {
                filterText = String.Format(@"[Наименование] like '%{0}%' and 
                                                [Заказанно производством] {1} {2} and 
                                                [Остаток на складе] {3} {4} and  
                                                [Из них у поставщиков] {5} {6} and 
                                                [Общий расход] {7} {8}  and
                                                [Текущая цена] {9} {10}",
                                      tbM1.Text,
                                      lM2.Text, nudM2.Value.ToString(),
                                      lM3.Text, nudM3.Value.ToString(),
                                      lM4.Text, nudM4.Value.ToString(),
                                      lM5.Text, nudM5.Value.ToString(),
                                      lM6.Text, nudM6.Value.ToString());
            }
            try
            {
                Program.FilterDataGridView(filterText, ref dataGridViewMaterials);
                SetMenuMaterialsButtonsEnabeld(null, null);
            }
            catch { }
        }
        //menus mat
        private void toolStripMenuItemPrintMaterials_Click(object sender, EventArgs e)
        {
            DGVPrinter printer = Program.DefaultPrinter();
            printer.Title = "Материалы";
            printer.Footer = "Складской отдел";
            printer.PrintPreviewDataGridView(dataGridViewMaterials);
        }
        private void toolStripMenuItemAddMaterial_Click(object sender, EventArgs e)
        {
            EditForm addMat = new EditForm(@"select materialName as 'Наименование', 1.00 as 'Цена'
                    from Material 
                    where materialID = 0",
                        null,
                        null,
                        new int[] { 0, 1 },
                        null,
                        @"insert into Material
                    (materialName) values (@name)
                        insert into MaterialPriceChange
                    (materialID, dateOfMaterialPriceChange, materialPrice) 
                    values ((select max(materialID) from Material), GETDATE(), @price)",
                        new string[] { "@name", "@price" },
                        new SqlDbType[] { SqlDbType.VarChar, SqlDbType.Money });
            addMat.ShowDialog();
            UpdateMaterialTab();
        }
        private void toolStripMenuItemEditMaterial_Click(object sender, EventArgs e)
        {
            EditForm editMat = new EditForm(@"select materialName as 'Наименование' 
                    from Material where materialID = 0",
                        new object[] { dataGridViewMaterials.CurrentRow.Cells["Наименование"].Value.ToString() },
                        null,
                        new int[] { 0 },
                        null,
                        @"update Material
                    set materialName = @name
                    where materialID =" + currentRowMaterialId,
                        new string[] { "@name"},
                        new SqlDbType[] { SqlDbType.VarChar});
            editMat.ShowDialog();
            UpdateMaterialTab();
        }
        private void toolStripMenuItemDeleteMaterial_Click(object sender, EventArgs e)//obshiy ras = 0
        {
            DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите удалить материал из базы?", "Удаление записи", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    Program.SqlCommandExecute("delete",
                        @"delete from ProductMaterial where materialID =  @id
                    delete from MaterialPriceChange where materialID =  @id
                    delete from TransferOfMaterials where materialID =  @id
                    delete from Material where materialID =  @id",
                        new string[] { "@id" }, new SqlDbType[] { SqlDbType.Int },
                        new object[] { dataGridViewMaterials.CurrentRow.Cells["ID"].Value });
                }
                catch { MessageBox.Show(strDBError); }
                UpdateMaterialTab();
            }
        }
        //menus dep
        private void toolStripMenuItemAddTransfer_Click(object sender, EventArgs e)
        {
            EditForm addTrans = new EditForm(@"select quantity
                    from TransferOfMaterials 
                    where materialID = 0",
                        null,
                        null,
                        new int[] { 0},
                        null,
                        @"insert into TransferOfMaterials
                    (transferType, transferDate, materialID, quantity, carriedOut) 
                    values (1, GETDATE(), "+currentRowMaterialId+@", @q, 0)",
                        new string[] { "@q" },
                        new SqlDbType[] { SqlDbType.Float });
            addTrans.ShowDialog();
            UpdateMaterialTab();
        }
        private void toolStripMenuItemCarriedOutTransfer_Click(object sender, EventArgs e)
        {
            try
            {
                Program.SqlCommandExecute("update",
                    @"update TransferOfMaterials
                set carriedOut = 1
                where transferOfMaterialsID = @id",
                    new string[] { "@id" }, new SqlDbType[] { SqlDbType.Int },
                    new object[] { dataGridViewTransferOfMaterial.CurrentRow.Cells["ID"].Value.ToString() });

            }
            catch { MessageBox.Show(strDBError); }
            UpdateMaterialTab();            
        }
        private void toolStripMenuItemDeleteTransfer_Click(object sender, EventArgs e)//если приход и не проведён
        {
            DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите удалить информацию о перемещении из базы?", "Удаление записи", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    Program.SqlCommandExecute("delete",
                        @"delete from TransferOfMaterials where transferOfMaterialsID =  @id",
                        new string[] { "@id" }, new SqlDbType[] { SqlDbType.Int },
                        new object[] { dataGridViewTransferOfMaterial.CurrentRow.Cells["ID"].Value });
                }
                catch { MessageBox.Show(strDBError); }
                UpdateMaterialTab();
            }
        }
        private void toolStripMenuItemAddPrice_Click(object sender, EventArgs e)
        {
            EditForm addPrice = new EditForm(@"select materialPrice as 'Цена'
                    from MaterialPriceChange 
                    where materialID = 0",
                        null,
                        null,
                        new int[] { 0},
                        null,
                        @"insert into MaterialPriceChange
                    (materialID, dateOfMaterialPriceChange, materialPrice) 
                    values ("+currentRowMaterialId+", GETDATE(), @price)",
                        new string[] { "@price" },
                        new SqlDbType[] { SqlDbType.Money });
            addPrice.ShowDialog();
            UpdateMaterialTab();
        }
        //enableds
        public void SetMenuMaterialsButtonsEnabeld(object sender, EventArgs e)
        {
            if (dataGridViewMaterials.CurrentRow != null)
            {
                toolStripMenuItemPrintMaterials.Enabled = true;
                toolStripMenuItemEditMaterial.Enabled = true;
                toolStripMenuItemAddTransfer.Enabled = true;
                toolStripMenuItemAddPrice.Enabled = true;

                if (dataGridViewMaterials.CurrentRow.Cells["Общий расход"].Value.ToString() == "0")
                { toolStripMenuItemDeleteMaterial.Enabled = true; }
                else { toolStripMenuItemDeleteMaterial.Enabled = false; }

                if (dataGridViewTransferOfMaterial.CurrentRow != null)
                {
                    //MessageBox.Show(dataGridViewTransferOfMaterial.CurrentRow.Cells["Выполнено"].Value.ToString()+
                    //    dataGridViewTransferOfMaterial.CurrentRow.Cells["Тип"].Value.ToString());
                    if (dataGridViewTransferOfMaterial.CurrentRow.Cells["Выполнено"].Value.ToString() == "False")
                    { toolStripMenuItemCarriedOutTransfer.Enabled = true; }
                    else { toolStripMenuItemCarriedOutTransfer.Enabled = false; }

                    if (dataGridViewTransferOfMaterial.CurrentRow.Cells["Тип"].Value.ToString() == "приход" &&
                        dataGridViewTransferOfMaterial.CurrentRow.Cells["Выполнено"].Value.ToString() == "False")
                    { toolStripMenuItemDeleteTransfer.Enabled = true; }
                    else { toolStripMenuItemDeleteTransfer.Enabled = false; }
                }
                else
                {
                    toolStripMenuItemCarriedOutTransfer.Enabled = false;
                    toolStripMenuItemDeleteTransfer.Enabled = false;
                }
            }
            else
            {
                toolStripMenuItemPrintMaterials.Enabled = false;
                toolStripMenuItemEditMaterial.Enabled = false;
                toolStripMenuItemAddTransfer.Enabled = false;
                toolStripMenuItemAddPrice.Enabled = false;
                toolStripMenuItemDeleteMaterial.Enabled = false;
                toolStripMenuItemCarriedOutTransfer.Enabled = false;
                toolStripMenuItemDeleteTransfer.Enabled = false;
            }        
        }


        #endregion


    }

}
