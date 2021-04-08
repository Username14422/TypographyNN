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
using System.IO;

namespace TypographyNN
{
    public partial class ProductionManagerForm : System.Windows.Forms.Form
    {
        public ProductionManagerForm()
        {
            InitializeComponent();
            tabControlPM.ItemSize = new Size(new Point(0, 1));
            this.Size = new Size(900, 600);
            UpdateOrderTab();
            UpdateProductTab();  
        }
        String strDBError = "Изменения не сохранены из-за ошибки.";        

        #region Orders
        int currentRowOrderId = 0;
        #region select strings
        string strOrdersSelect = @"select productionOrderID as ID, productionStartDate as 'Дата начала производсва', 
                                estimatedCompletionDate as 'Плановая дата отгрузки', iif (productionStartDate is null, 'Оплачен', 'В процессе выполнения') as 'Статус'
                                from ProductionOrder
                                where dateOfPayment is not null and productionCompletionDate is null";
        string strProductsOfOrderSelect = @"select Product.productID as ID, ProductInOrder.productionOrderID, productName as 'Наименование', 
		iif(duration*numberOfCopiesOrdered>=360,
		CONVERT(varchar,duration*numberOfCopiesOrdered/60)+' (час.)',
		CONVERT(varchar,duration*numberOfCopiesOrdered)+' (мин.)') as 'Общая длительность',
		(CONVERT(varchar, numberOfCopiesOrdered) +'/'+ CONVERT(varchar,numberOfCopiesMade)) as 'Количество экземпляров'
        from Product
            left join ProductInOrder on (Product.productID = ProductInOrder.productID)            
            join 
            (	select productID,sum(dur) as duration
	            from
	            (	select ProductInOrder.productID, ProductOperation.operationID,[duration (minutes)] as dur
		            from ProductionOrder
		            left join ProductInOrder on (ProductionOrder.productionOrderID = ProductInOrder.productionOrderID)
		            left join ProductOperation on (ProductInOrder.productID = ProductOperation.productID)
		            group by ProductInOrder.productID, ProductOperation.operationID,[duration (minutes)]
	            ) as rightPricesDates 
	            group by productID
            ) as OperationPrices on (OperationPrices.productID = Product.productID)";
        string strContractChangesSelect = @"select contractChangeID as ID, productionOrderID, description as 'Описание изменения', contractChangeDate as 'Дата изменения'
                                          from ContractChange";
        #endregion
        public void UpdateOrderTab()
        {
            Program.SetDataGridViewDataSource(strOrdersSelect, ref dataGridViewOrders);
            dataGridViewOrders.Columns["ID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

            Program.SetDataGridViewDataSource(strProductsOfOrderSelect, ref dataGridViewProductsOfOrder);
            dataGridViewProductsOfOrder.Columns["ID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridViewProductsOfOrder.Columns["productionOrderID"].Visible = false;
            dataGridViewProductsOfOrder.Columns["Наименование"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

            Program.SetDataGridViewDataSource(strContractChangesSelect, ref dataGridViewContractChanges);
            dataGridViewContractChanges.Columns["ID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridViewContractChanges.Columns["productionOrderID"].Visible = false;

            FilterDataGridViewOrders(new Button(), null);
        }
        private void dataGridViewOrders_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                currentRowOrderId = (int)dataGridViewOrders.CurrentRow.Cells[0].Value;
                Program.FilterDataGridView(String.Format("ProductionOrderID = {0}", currentRowOrderId), ref dataGridViewProductsOfOrder);
                Program.FilterDataGridView(String.Format("ProductionOrderID = {0}", currentRowOrderId), ref dataGridViewContractChanges);
                SetMenuOrdersButtonsEnabeld();
            }
            catch { }
        }        
        private void dataGridViewOrders_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            toolStripMenuItemProducts_Click(null, null);
            comboBoxOrder.Text = dataGridViewOrders.CurrentRow.Cells["ID"].Value.ToString();
            comboBoxOrder_SelectedIndexChanged(null, null);
        }
        private void dataGridViewProductsOfOrder_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            toolStripMenuItemProducts_Click(null, null);
            comboBoxOrder.Text = dataGridViewProductsOfOrder.CurrentRow.Cells["productionOrderID"].Value.ToString();
            comboBoxOrder_SelectedIndexChanged(null, null);
            FilterDataGridViewOrders(null, null);
            int idClickedProductsOfOrderRow;
            idClickedProductsOfOrderRow = Int32.Parse(dataGridViewProductsOfOrder.CurrentRow.Cells["ID"].Value.ToString());
            try
            {
                for (int i = 0; i < dataGridViewProducts.RowCount; i++)
                {
                    if (dataGridViewProducts.Rows[i].Cells["ID"].Value.ToString() == idClickedProductsOfOrderRow.ToString())
                    {
                        dataGridViewProducts.CurrentCell = dataGridViewProducts.Rows[i].Cells["ID"];
                        dataGridViewProducts_CurrentCellChanged(null, null);
                        return;
                    }
                }
            }
            catch { }
        }
        //filts
        public void SetOrdersFiltersWidth(object sender, DataGridViewColumnEventArgs e)
        {
            try
            {
                for (int i = 0; i < dataGridViewOrders.ColumnCount; i++)
                {
                    tableLayoutPanelOrdersFilters.ColumnStyles[i].Width = dataGridViewOrders.Columns[i].Width;
                }
            }
            catch { }
        }
        public void FilterDataGridViewOrders(object sender, EventArgs e)
        {
            string filterText = "";
            try { object a = ((Button)sender); }
            catch
            {
                filterText = String.Format(@"[Дата начала производсва] {0} #{1}# and
                                            [Плановая дата отгрузки] {2} #{3}# and 
                                            ([Статус] like '{4}')",
                                      lO1.Text, dtpO1.Value.ToShortDateString(),
                                      lO2.Text, dtpO2.Value.ToShortDateString(),
                                      (cbO3.SelectedItem==null?"' or 'y'like 'y": cbO3.SelectedItem.ToString()));
            }
            //try
            //{
                Program.FilterDataGridView(filterText,
                    ref dataGridViewOrders);
            SetMenuOrdersButtonsEnabeld();
            //}
            //catch { }
        }
        //menus
        private void toolStripMenuItemProductionOrder_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Начать работу над заказом №" + currentRowOrderId + "?", "Добавление заказа", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    Program.SqlCommandExecute("update", "update ProductionOrder set productionStartDate=GETDATE() where productionOrderid =  @id",
                        new string[] { "@id" }, new SqlDbType[] { SqlDbType.Int },
                        new object[] { currentRowOrderId });
                }
                catch { MessageBox.Show(strDBError); }
                UpdateOrderTab();
                UpdateProductTab();
            }
        }
        private void toolStripMenuItemSendProductsOfOrder_Click(object sender, EventArgs e)
        {
            string productID = dataGridViewProductsOfOrder.CurrentRow.Cells["ID"].Value.ToString();
            EditForm senProd = new EditForm(@"select numberOfCopiesMade as 'Количество новых изготовленных экземпляров'
                                from ProductInOrder
                                where productID = 0",
                                          null,
                                          null,
                                          new int[] { 0 },
                                          null,
                                          @"update ProductInOrder
                                set numberOfCopiesMade = numberOfCopiesMade + @newCop
                                where productionOrderID = " + currentRowOrderId + "and productID=" + productID + @"
                                            insert into TransferOfProducts 
                                (transferType, transferDate, productID, quantity, productionOrderID, carriedOut)
                                values (1, GETDATE(), " + productID + ", @newCop, " + currentRowOrderId + ", 0)",
                                          new string[] { "@newCop" },
                                          new SqlDbType[] { SqlDbType.Int });
            senProd.ShowDialog();
            UpdateOrderTab();
            UpdateOrderTab();
            comboBoxOrder_SelectedIndexChanged(null, null);
        }
        //menus enableds
        public void SetMenuOrdersButtonsEnabeld()
        {
            if (dataGridViewOrders.RowCount > 0)
            {
                if (dataGridViewOrders.CurrentRow.Cells["Статус"].Value.ToString() == "Оплачен")
                {
                    toolStripMenuItemProductionOrder.Enabled = true;
                    toolStripMenuItemSendProductsOfOrder.Enabled = false;
                }
                else
                {
                    toolStripMenuItemProductionOrder.Enabled = false;
                    if (dataGridViewProductsOfOrder.CurrentRow != null)
                    { toolStripMenuItemSendProductsOfOrder.Enabled = true; }
                    else { toolStripMenuItemSendProductsOfOrder.Enabled = false; }
                }
            }
        }
        #endregion

        #region Products
        #region select strings
        //продукты
        string strAllProductsSelect = @"select Product.productID as ID, productName as 'Наименование', linkToPrintFile as 'Файл', 
                                     duration as 'Длительность производства (мин.)', isnull(completionOrderCount,0)as completionOrderCount
                            from Product           
                            join 
                            (	select productID,sum(dur) as duration
	                            from
	                            (	select ProductInOrder.productID, ProductOperation.operationID,[duration (minutes)] as dur
		                            from ProductionOrder
		                            left join ProductInOrder on (ProductionOrder.productionOrderID = ProductInOrder.productionOrderID)
		                            left join ProductOperation on (ProductInOrder.productID = ProductOperation.productID)
		                            group by ProductInOrder.productID, ProductOperation.operationID,[duration (minutes)]
	                            ) as rightPricesDates 
	                            group by productID
                            ) as OperationPrices on (OperationPrices.productID = Product.productID) 
							left join 
							(   select Product.productID, count(ProductionOrder.productionOrderID) as completionOrderCount from Product
								left join ProductInOrder on (Product.productID = ProductInOrder.productID)
								left join ProductionOrder on (ProductInOrder.productionOrderID = ProductionOrder.productionOrderID)
								where productionCompletionDate is not null
								group by Product.productID
							) as compl on (Product.productID = compl.productID)";
        string strProductsSelect = @"select Product.productID as ID, ProductInOrder.productionOrderID, productName as 'Наименование', linkToPrintFile as 'Файл', 
                                    duration as 'Длительность производства (мин.)', 
		                            iif(duration*numberOfCopiesOrdered>=360,
		                            CONVERT(varchar,duration*numberOfCopiesOrdered/60)+' (час.)',
		                            CONVERT(varchar,duration*numberOfCopiesOrdered)+' (мин.)') as 'Общая длительность',
		                            numberOfCopiesOrdered as 'Количество заказанных экземпляров', 
									numberOfCopiesMade as 'Количество изготовленных экземпляров',
									quanity as 'Количество экземпляров на складе', 
								iif (dateOfPayment is null, 'Ожидает оплаты',
								iif (productionStartDate is null, 'Оплачен',
								iif (productionCompletionDate is NULL, 'В процессе выполнения',
								iif (dateOfDelivery is null, 'Ожидает отгрузки','Завершен')))) as 'Статус заказа',
									isnull(completionOrderCount,0)as completionOrderCount
                            from Product
                            left join ProductInOrder on (Product.productID = ProductInOrder.productID)
							left join ProductionOrder on (ProductInOrder.productionOrderID = ProductionOrder.productionOrderID)
							join 
                            (	select productID,sum(dur) as duration
	                            from
	                            (	select ProductInOrder.productID, ProductOperation.operationID,[duration (minutes)] as dur
		                            from ProductionOrder
		                            left join ProductInOrder on (ProductionOrder.productionOrderID = ProductInOrder.productionOrderID)
		                            left join ProductOperation on (ProductInOrder.productID = ProductOperation.productID)
		                            group by ProductInOrder.productID, ProductOperation.operationID,[duration (minutes)]
	                            ) as duration 
	                            group by productID
                            ) as Duration on (Duration.productID = Product.productID)
							left join 
							(   select productionOrderId, productId, sum(quantity) as quanity
								from TransferOfProducts
								where transferType = 1 and carriedOut = 1
								group by productionOrderId, productId
							) as transfers on (transfers.productionOrderId = ProductInOrder.productionOrderID
										and transfers.productId = Product.productID)
							left join 
							(   select Product.productID, count(ProductionOrder.productionOrderID) as completionOrderCount from Product
								left join ProductInOrder on (Product.productID = ProductInOrder.productID)
								left join ProductionOrder on (ProductInOrder.productionOrderID = ProductionOrder.productionOrderID)
								where productionCompletionDate is not null
								group by Product.productID
							) as compl on (Product.productID = compl.productID)";
        //опрации
        string strOperationsOfProductSelect = @"select productID, Operation.operationID as ID, operationName as 'Наименование', 
                                            [duration (minutes)] as 'Длительность (мин.)', ISNULL (durationAvg,0) as 'Средняя длительность (мин.)',
				                            timesSum as 'Общее число обработанных экземпляров'
                            from ProductOperation
                            left join Operation on (Operation.operationID = ProductOperation.operationID)             
                            left join 
                            (   select operationID, avg([duration (minutes)]) as durationAvg
	                            from ProductOperation
	                            group by operationID
                            ) as avgDurationPerProduct
                            on (Operation.operationID = avgDurationPerProduct.operationID)
                            left join 
                            (   select executions.operationID, ISNULL (sum(times),0) as timesSum
	                            from
	                            (   select Operation.operationID, p.productionOrderID, sum(numberOfCopiesMade) as times
		                            from ProductOperation
		                            left join Operation  on (Operation.operationID = ProductOperation.operationID)
		                            left join 
                                    (   select ProductInOrder.productionOrderID, productID, numberOfCopiesMade
			                            from ProductInOrder
			                            left join ProductionOrder on (ProductionOrder.productionOrderID = ProductInOrder.productionOrderID)
		                            )as p  on (ProductOperation.productID = p.productID)
		                            group by Operation.operationID, p.productionOrderID
	                            ) as executions				
	                            group by executions.operationID
                            ) as AllTimes on (Operation.operationID = AllTimes.operationID)";
        string strOperationsOfOrderSelect = @"select productionOrderID, Operation.operationID as ID, operationName as 'Наименование', 
                                            sum([duration (minutes)]*numberOfCopiesOrdered) as 'Общая длительность (мин.)',
                                            timesSum as 'Общее число обработанных экземпляров'
                            from ProductInOrder
                            left join ProductOperation on (ProductInOrder.productID = ProductOperation.productID)
                            left join Operation on (Operation.operationID = ProductOperation.operationID) 
                            left join 
                            (   select executions.operationID, ISNULL (sum(times),0) as timesSum
	                            from
	                            (   select Operation.operationID, p.productionOrderID, sum(numberOfCopiesMade) as times
		                            from ProductOperation
		                            left join Operation  on (Operation.operationID = ProductOperation.operationID)
		                            left join 
                                    (   select ProductInOrder.productionOrderID, productID, numberOfCopiesMade
			                            from ProductInOrder
			                            left join ProductionOrder on (ProductionOrder.productionOrderID = ProductInOrder.productionOrderID)
		                            )as p  on (ProductOperation.productID = p.productID)
		                            group by Operation.operationID, p.productionOrderID
	                            ) as executions				
	                            group by executions.operationID
                            ) as AllTimes on (Operation.operationID = AllTimes.operationID)
                            group by productionOrderID, Operation.operationID, operationName, timesSum
                            order by productionOrderID";
        //материалы
        string strMaterialsOfProductSelect = @"select productID, Material.materialID as ID,materialName as 'Наименование', quantity as 'Расход', 
                                             qAvg as 'Средний расход на продукт'
							from ProductMaterial 
                            left join Material on(Material.materialID = ProductMaterial.materialID)
							left join 
			                (   select materialID, avg(quantity) as qAvg
				                from ProductMaterial
				                group by materialID
			                ) as avgConsumptionPerProduct on (Material.materialID = avgConsumptionPerProduct.materialID)";
        string strMaterialsOfOrderSelect = @"select ProductInOrder.productionOrderID, Material.materialID as ID,materialName as 'Наименование', 
                                            sum(ProductMaterial.quantity*numberOfCopiesOrdered) as 'Расход', ordered.quantity as 'Заказанное количество',
                                            carried.quantity as 'Доставленное количество'
                            from ProductMaterial 
                            left join Material on(Material.materialID = ProductMaterial.materialID)
                            left join ProductInOrder on (ProductMaterial.productID = ProductInOrder.productID)
                            left join 
                            (	select productionOrderID, materialID, sum(quantity) as quantity
	                            from TransferOfMaterials
	                            group by productionOrderID, materialID
                            ) as ordered on (ordered.productionOrderID = ProductInOrder.productionOrderID and 
	                            ordered.materialID = Material.materialID)
                            left join 
                            (	select productionOrderID, materialID, sum(quantity) as quantity
	                            from TransferOfMaterials
	                            where carriedOut = 1
	                            group by productionOrderID, materialID
                            ) as carried on (carried.productionOrderID = ProductInOrder.productionOrderID and 
	                            carried.materialID = Material.materialID)
                            group by ProductInOrder.productionOrderID, Material.materialID,materialName, ordered.quantity, carried.quantity";
        #endregion
        bool productsOfOrder = false;
        bool othersOfOrder = false;
        public void UpdateProductTab()
        {
            Program.InitializeComboBox(@"select convert(varchar, productionOrderId) as productionOrderIdVarchar, productionOrderId from ProductionOrder", //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                                       "productionOrderIdVarchar", "productionOrderId", 
                                       new object[] { "0", 0 }, ref comboBoxOrder);
            comboBoxOrder_SelectedIndexChanged(null, null);
        }
        public void UpdateProductDGV()
        {
            if (!productsOfOrder)
            {   //norm
                Program.SetDataGridViewDataSource(strAllProductsSelect, ref dataGridViewProducts);
            }
            else
            {   //of
                Program.SetDataGridViewDataSource(strProductsSelect, ref dataGridViewProducts);
                dataGridViewProducts.Columns["productionOrderID"].Visible = false;
            }
            dataGridViewProducts.Columns["ID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridViewProducts.Columns["completionOrderCount"].Visible = false;
        }
        public void UpdateDependentsDGV()
        {
            if (!othersOfOrder)
            {   //norm
                Program.SetDataGridViewDataSource(strOperationsOfProductSelect, ref dataGridViewOperationsOfProduct);
                Program.SetDataGridViewDataSource(strMaterialsOfProductSelect, ref dataGridViewMaterialsOfProduct);
                dataGridViewOperationsOfProduct.Columns["productID"].Visible = false;
                dataGridViewMaterialsOfProduct.Columns["productID"].Visible = false;
            }
            else
            {   //ofO
                Program.SetDataGridViewDataSource(strOperationsOfOrderSelect, ref dataGridViewOperationsOfProduct);
                Program.SetDataGridViewDataSource(strMaterialsOfOrderSelect, ref dataGridViewMaterialsOfProduct);
                dataGridViewOperationsOfProduct.Columns["productionOrderID"].Visible = false;
                dataGridViewMaterialsOfProduct.Columns["productionOrderID"].Visible = false;
            }
            dataGridViewOperationsOfProduct.Columns["ID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridViewOperationsOfProduct.Columns["Наименование"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridViewMaterialsOfProduct.Columns["ID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridViewMaterialsOfProduct.Columns["Наименование"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
        }

        int currentRowProductId = 0;
        private void dataGridViewProducts_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                if (othersOfOrder)
                {
                    othersOfOrder = false;
                    UpdateDependentsDGV();
                }
                currentRowProductId = (int)dataGridViewProducts.CurrentRow.Cells[0].Value;
                Program.FilterDataGridView(String.Format("productID = {0}", currentRowProductId), ref dataGridViewOperationsOfProduct);
                Program.FilterDataGridView(String.Format("productID = {0}", currentRowProductId), ref dataGridViewMaterialsOfProduct);
                SetMenuProductsButtonsEnabeld();
            }
            catch { }
        }
        int currentOrderId = 0;
        private void comboBoxOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxOrder.SelectedIndex != 0)
            {
                currentOrderId = (int)comboBoxOrder.SelectedValue;
                productsOfOrder = true;
                UpdateProductDGV();
                Program.FilterDataGridView(String.Format("productionOrderID = {0}", currentOrderId), ref dataGridViewProducts);
                othersOfOrder = true;
                UpdateDependentsDGV();
                Program.FilterDataGridView(String.Format("productionOrderID = {0}", currentOrderId), ref dataGridViewOperationsOfProduct);
                Program.FilterDataGridView(String.Format("productionOrderID = {0}", currentOrderId), ref dataGridViewMaterialsOfProduct);
                lP1.Enabled = false;
                lP2.Enabled = false;
                tbP1.Enabled = false;
                nudP2.Enabled = false;
            }
            else
            {
                productsOfOrder = false;
                othersOfOrder = false;
                UpdateProductDGV();
                UpdateDependentsDGV();
                dataGridViewProducts_CurrentCellChanged(null, null);
                lP1.Enabled = true;
                lP2.Enabled = true;
                tbP1.Enabled = true;
                nudP2.Enabled = true;
            }
            SetMenuProductsButtonsEnabeld();
        }////////////////////////////
        private void comboBoxOrder_TextUpdate(object sender, EventArgs e)
        {
            //comboBoxOrder.SelectedIndex = comboBoxOrder.FindString(comboBoxOrder.Text);
            ((DataTable)comboBoxOrder.DataSource).DefaultView.RowFilter = String.Format("productionOrderIdVarchar like '{0}%'", comboBoxOrder.Text);
            if (comboBoxOrder.Items.Count < 1)
            {
                ((DataTable)comboBoxOrder.DataSource).DefaultView.RowFilter = "";
                comboBoxOrder.SelectedIndex = 0;
            }
            comboBoxOrder.DroppedDown = true;
            comboBoxOrder.Select(comboBoxOrder.Text.Length, 0);
            SetMenuProductsButtonsEnabeld();
        }
        //filters        
        public void FilterDataGridViewProducts(object sender, EventArgs e)
        {
            string filterText = "";
            try { object a = ((Button)sender);
                comboBoxOrder.SelectedIndex = 0;}
            catch
            {
                filterText = String.Format(@"[Наименование] like '%{0}%' and
                                            [Длительность производства (мин.)] {1} {2}",
                                      tbP1.Text,
                                      lP2.Text, nudP2.Value.ToString());
            }
            //try
            //{
            Program.FilterDataGridView(filterText,
                ref dataGridViewProducts);
            SetMenuProductsButtonsEnabeld();
            //}
            //catch { }
        }
        //menus
        private void toolStripMenuItemSendProducts_Click(object sender, EventArgs e)
        {
            string orderID = dataGridViewProducts.CurrentRow.Cells["productionOrderID"].Value.ToString();
            EditForm senProd = new EditForm(@"select numberOfCopiesMade as 'Количество новых изготовленных экземпляров'
                                from ProductInOrder
                                where productID = 0",
                                          null,
                                          null,
                                          new int[] { 0 },
                                          null,
                                          @"update ProductInOrder
                                set numberOfCopiesMade = numberOfCopiesMade + @newCop
                                where productionOrderID = " + orderID + "and productID=" + currentRowProductId + @"
                                            insert into TransferOfProducts 
                                (transferType, transferDate, productID, quantity, productionOrderID, carriedOut)
                                values (1, GETDATE(), " + currentRowProductId + ", @newCop, " + orderID + ", 0)",
                                          new string[] { "@newCop" },
                                          new SqlDbType[] { SqlDbType.Int });
            senProd.ShowDialog();
            UpdateOrderTab();
            comboBoxOrder_SelectedIndexChanged(null, null);
        }
        private void toolStripMenuItemEditOperation_Click(object sender, EventArgs e)
        {
            string operationID = dataGridViewOperationsOfProduct.CurrentRow.Cells["ID"].Value.ToString();
            EditForm editOp = new EditForm(@"select [duration (minutes)] as 'Длительность (мин.)'
                                from ProductOperation
                                where productID = 0",
                                          new object[] { dataGridViewOperationsOfProduct.CurrentRow.Cells["Длительность (мин.)"].Value },
                                          null,
                                          new int[] { 0 },
                                          null,
                                          @"update ProductOperation
                                set [duration (minutes)] = @dur
                                where productID = " + currentRowProductId + "and operationID=" + operationID,
                                          new string[] { "@dur" },
                                          new SqlDbType[] { SqlDbType.Int });
            editOp.ShowDialog();
            UpdateOrderTab();
            comboBoxOrder_SelectedIndexChanged(null, null);
        }
        private void toolStripMenuItemEditMaterial_Click(object sender, EventArgs e)
        {
            string materialID = dataGridViewMaterialsOfProduct.CurrentRow.Cells["ID"].Value.ToString();
            EditForm editOp = new EditForm(@"select quantity as 'Расход'
                                from ProductMaterial
                                where productID = 0",
                                          new object[] { dataGridViewMaterialsOfProduct.CurrentRow.Cells["Расход"].Value },
                                          null,
                                          new int[] { 0 },
                                          null,
                                          @"update ProductMaterial
                                set quantity = @q
                                where productID = " + currentRowProductId + "and materialID=" + materialID,
                                          new string[] { "@q" },
                                          new SqlDbType[] { SqlDbType.Float });
            editOp.ShowDialog();
            UpdateOrderTab();
            comboBoxOrder_SelectedIndexChanged(null, null);
        }
        private void toolStripMenuItemAddOperation_Click(object sender, EventArgs e)
        {
            EditForm addOperationToProduct = new EditForm(@"select operationName from Operation where operationID=0",
                                          null,
                                          @"select operationId as ID, operationName as'Наименование' from Operation",
                                          new int[] { 0 },
                                          null,
                                          @"insert into Operation 
                                       (operationName)
                                       values (@name)",
                                          new string[] { "@name", "@notNeededID" },
                                          new SqlDbType[] { SqlDbType.VarChar, SqlDbType.Int });
            addOperationToProduct.ShowDialog();
        }
        private void toolStripMenuItemOrderMaterial_Click(object sender, EventArgs e)
        {
            string materialID = dataGridViewMaterialsOfProduct.CurrentRow.Cells["ID"].Value.ToString();
            EditForm senProd = new EditForm(@"select quantity as 'Количество заказываемого материала'
                                from TransferOfMaterials
                                where productionOrderID = 0",
                                          null,
                                          null,
                                          new int[] { 0 },
                                          null,
                                          @"insert into TransferOfMaterials 
                                (transferType, transferDate, materialID, quantity, productionOrderID, carriedOut)
                                values (0, GETDATE(), " + materialID + ", @q, " + currentOrderId + ", 0)",
                                          new string[] { "@q" },
                                          new SqlDbType[] { SqlDbType.Int });
            senProd.ShowDialog();
            UpdateOrderTab();
            comboBoxOrder_SelectedIndexChanged(null, null);
        }
        //menus enableds
        public void SetMenuProductsButtonsEnabeld()
        {
            if (productsOfOrder &&
                dataGridViewProducts.RowCount>0 &&
                (dataGridViewProducts.Rows[0].Cells["Статус заказа"].Value.ToString() == "В процессе выполнения"))
            {
               toolStripMenuItemSendProducts.Enabled = true;
                toolStripMenuItemEditOperation.Enabled = false;
                toolStripMenuItemEditMaterial.Enabled = false;
                if (othersOfOrder && dataGridViewMaterialsOfProduct.CurrentRow!=null)
                { toolStripMenuItemOrderMaterial.Enabled = true; }
                else { toolStripMenuItemOrderMaterial.Enabled = false; }
            }
            else
            {
                toolStripMenuItemSendProducts.Enabled = false;
                toolStripMenuItemOrderMaterial.Enabled = false;
                if (!othersOfOrder&&dataGridViewProducts.CurrentRow.Cells["completionOrderCount"].Value.ToString() =="0")
                {
                    if (dataGridViewOperationsOfProduct.CurrentRow != null)
                    { toolStripMenuItemEditOperation.Enabled = true; }
                    else { toolStripMenuItemEditOperation.Enabled = false; }
                    if (dataGridViewMaterialsOfProduct.CurrentRow != null)
                    { toolStripMenuItemEditMaterial.Enabled = true; }
                    else { toolStripMenuItemEditMaterial.Enabled = false; }
                }
                else
                {
                    toolStripMenuItemEditOperation.Enabled = false;
                    toolStripMenuItemEditMaterial.Enabled = false;
                }
            }
        }

        #endregion

        #region MenuClics
        private void toolStripMenuItemOrders_Click(object sender, EventArgs e)
        {
            if (tabControlPM.SelectedTab != tabPageOrders)
            { tabControlPM.SelectedTab = tabPageOrders; }
        }
        private void toolStripMenuItemProducts_Click(object sender, EventArgs e)
        {
            if (tabControlPM.SelectedTab != tabPageProducts)
            { tabControlPM.SelectedTab = tabPageProducts; }
        }
        #endregion        

        public void FilterLabelClick(object sender, EventArgs e)
        {
            Program.FilterLabelClick(sender, e);
        }

        

        
    }
}
