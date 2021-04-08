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
using DGVPrinterHelper;

namespace TypographyNN
{
    public partial class FinishedProductsStorekeeperForm : System.Windows.Forms.Form
    {
        public FinishedProductsStorekeeperForm()
        {
            InitializeComponent();
            tabControlFGS.ItemSize = new Size(new Point(0, 1));
            this.Size = new Size(900, 600);
            UpdateAll();
        }
        String strDBError = "Изменения не сохранены из-за ошибки.";
        public void UpdateAll()
        {
            UpdateClientTab();
            UpdateOrderTab();
            UpdateProductTab();
        }

        #region Clients
        int currentRowClientId = 0;
        #region select strings
        string strClientsSelect = @"select customerID as ID, customerName as 'Имя / Наименование организации', contactNumber as 'Контактный номер' from Customer";
        string strOrdersOfClientSelect = @"select ProductionOrder.productionOrderID as ID, ProductionOrder.customerID, recipientName as 'Имя получателя',                
                                iif (productionCompletionDate is NULL, 'В процессе выполнения',
                                iif (dateOfDelivery is null, 'Ожидает отгрузки','Завершен')) as 'Статус',sumM as 'Сумма', 
                                linkToTheContractFile as 'Договор', contractDate as 'Дата заключения договора', 
								linkToPaymentDocumentFile as 'Документ об оплате', 
                                estimatedCompletionDate as 'Плановая дата отгрузки', dateOfDelivery 'Фактическая дата отгрузки',
                                delayByCustomer as 'Задержка по вине клиента'
                            from ProductionOrder
							left join (
                            select ProductInOrder.productionOrderID, sum((matP+opP)*numberOfCopiesOrdered) as sumM
                                    from Product
                                        left join ProductInOrder on (Product.productID = ProductInOrder.productID)
                                        join
                                        (   select productID, productionOrderID, sum(ISNULL (quantity*materialPrice,0)) as matP 
	                                        from
	                                        (	select ProductInOrder.productID, ProductionOrder.productionOrderID, ProductMaterial.materialID, quantity, max(dateOfMaterialPriceChange) as datP
		                                        from ProductionOrder
		                                        left join ProductInOrder on (ProductionOrder.productionOrderID = ProductInOrder.productionOrderID)
		                                        left join ProductMaterial on (ProductInOrder.productID = ProductMaterial.productID)
		                                        left join MaterialPriceChange on (ProductMaterial.materialID = MaterialPriceChange.materialID)
		                                        where dateOfMaterialPriceChange <= contractDate
		                                        group by ProductInOrder.productID, ProductionOrder.productionOrderID, ProductMaterial.materialID, quantity
	                                        ) as rightPricesDates left join MaterialPriceChange 
	                                        on (rightPricesDates.materialID = MaterialPriceChange.materialID
	                                        and rightPricesDates.datP = MaterialPriceChange.dateOfMaterialPriceChange)
	                                        group by productID, productionOrderID
                                        ) as MaterialPrices on (MaterialPrices.productID = Product.productID and MaterialPrices.productionOrderID = ProductInOrder.productionOrderID)
                                        join 
                                        (	select productID, productionOrderID, sum(ISNULL (operationPrice,0)) as opP
	                                        from
	                                        (	select ProductInOrder.productID, ProductionOrder.productionOrderID, ProductOperation.operationID,  max(dateOfOperationPriceChange) as datP
		                                        from ProductionOrder
		                                        left join ProductInOrder on (ProductionOrder.productionOrderID = ProductInOrder.productionOrderID)
		                                        left join ProductOperation on (ProductInOrder.productID = ProductOperation.productID)
		                                        left join OperationPriceChange on (ProductOperation.operationID = OperationPriceChange.operationID)
		                                        where dateOfOperationPriceChange <= contractDate
		                                        group by ProductInOrder.productID, ProductionOrder.productionOrderID, ProductOperation.operationID
	                                        ) as rightPricesDates left join OperationPriceChange 
	                                        on (rightPricesDates.operationID = OperationPriceChange.operationID
	                                        and rightPricesDates.datP = OperationPriceChange.dateOfOperationPriceChange)
	                                        group by productID, productionOrderID
                                        ) as OperationPrices on (OperationPrices.productID = Product.productID and 
			                            OperationPrices.productionOrderID = ProductInOrder.productionOrderID)
			                            group by ProductInOrder.productionOrderID
                            ) as prodStats on (prodStats.productionOrderID = ProductionOrder.productionOrderID)  
				            where productionStartDate is not null
							order by estimatedCompletionDate";
        #endregion
        public void UpdateClientTab()
        {
            //документы
            Program.SetDataGridViewDataSource(strClientsSelect, ref dataGridViewClients);
            dataGridViewClients.Columns["ID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

            Program.SetDataGridViewDataSource(strOrdersOfClientSelect, ref dataGridViewOrdersOfClient);
            dataGridViewOrdersOfClient.Columns["ID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridViewOrdersOfClient.Columns["Сумма"].DefaultCellStyle.Format = "F";
            dataGridViewOrdersOfClient.Columns["customerID"].Visible = false;

            FilterDataGridViewClients(new Button(), null);
        }
        private void dataGridViewClients_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                currentRowClientId = (int)dataGridViewClients.CurrentRow.Cells[0].Value;
                Program.FilterDataGridView(String.Format("customerID = {0}", currentRowClientId), ref dataGridViewOrdersOfClient);                
            }
            catch { }
        }
        private void dataGridViewOrdersOfClient_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            toolStripMenuItemOrders_Click(null, null);
            //btnOrdersFilterClear_Click(null, null);
            int idClickedOrderOfClientRow;
            idClickedOrderOfClientRow = Int32.Parse(dataGridViewOrdersOfClient.CurrentRow.Cells["ID"].Value.ToString());
            for (int i = 0; i < dataGridViewOrders.RowCount; i++)
            {
                if (dataGridViewOrders.Rows[i].Cells["ID"].Value.ToString() == idClickedOrderOfClientRow.ToString())
                {
                    dataGridViewOrders.CurrentCell = dataGridViewOrders.Rows[i].Cells["ID"];
                    return;
                }
            }
        }
        //filts
        public void SetClientsFiltersWidth(object sender, DataGridViewColumnEventArgs e)
        {
            try
            {
                for (int i = 0; i < dataGridViewClients.ColumnCount; i++)
                {
                    tableLayoutPanelClientsFilters.ColumnStyles[i].Width = dataGridViewClients.Columns[i].Width;
                }
            }
            catch { }
        }
        public void FilterDataGridViewClients(object sender, EventArgs e)
        {
            string filterText = "";
            try { object a = ((Button)sender); }
            catch
            {
                filterText = String.Format(@"[Имя / Наименование организации] like '%{0}%' and
                                             [Контактный номер] like '%{1}%'",
                                             tbC1.Text,
                                             tbC2.Text);
                                     // (cbO3.SelectedItem == null ? "' or 'y'like 'y" : cbO3.SelectedItem.ToString()));
            }
            //try
            //{
            Program.FilterDataGridView(filterText,
                ref dataGridViewClients);
            //SetMenuOperationsButtonsEnabeld();
            //}
            //catch { }
        }
        //menus
        private void toolStripMenuItemPrintClients_Click(object sender, EventArgs e)
        {
            DGVPrinter printer = Program.DefaultPrinter();
            printer.Title = "Клиенты";
            printer.Footer = "Складской отдел";
            printer.PrintPreviewDataGridView(dataGridViewClients);
        }
        private void toolStripMenuItemMarkClientsDelay_Click(object sender, EventArgs e)
        {
            try
            {
                Program.SqlCommandExecute("update",
                    @"update ProductionOrder
                set delayByCustomer = 1
                where productionOrderID = @id",
                    new string[] { "@id" }, new SqlDbType[] { SqlDbType.Int },
                    new object[] { dataGridViewOrdersOfClient.CurrentRow.Cells["ID"].Value.ToString() });
            }
            catch { MessageBox.Show(strDBError); }
            UpdateAll();
        }
        #endregion

        #region Orders
        int currentRowOrderId = 0;
        #region select strings
        string strOrdersSelect = @"select productionOrderID as ID, customerName as 'Заказчик',                
                                iif (productionCompletionDate is NULL, 'В процессе выполнения',
                                iif (dateOfDelivery is null, 'Ожидает отгрузки','Завершен')) as 'Статус',
                                productionStartDate as 'Дата начала производсва', productionCompletionDate as 'Дата завершения производства',
                                estimatedCompletionDate as 'Плановая дата отгрузки', dateOfDelivery 'Фактическая дата отгрузки'
                            from ProductionOrder
			                left join Customer on(ProductionOrder.customerID = Customer.customerID)
			                where productionStartDate is not null";
        string strProductsOfOrderSelect = @"select Product.productID as ID, ProductInOrder.productionOrderID, productName as 'Наименование', linkToPrintFile as 'Файл', 
                                        (CONVERT(varchar, numberOfCopiesOrdered) +'/'+ CONVERT(varchar,numberOfCopiesMade)) as 'Количество изготовленных экземпляров',
		                                quanityW as 'Перемещено на склад', quanityC as 'Передано клиенту'
                                    from Product
                                    left join ProductInOrder on (Product.productID = ProductInOrder.productID)	
	                                left join 
	                                (   select productionOrderId, productId, sum(quantity) as quanityW
		                                from TransferOfProducts
		                                where transferType = 1 and carriedOut = 1
		                                group by productionOrderId, productId
	                                ) as transfersW on (transfersW.productionOrderId = ProductInOrder.productionOrderID
										                                and transfersW.productId = Product.productID)
	                                left join 
	                                (   select productionOrderId, productId, sum(quantity) as quanityC
		                                from TransferOfProducts
		                                where transferType = 0 and carriedOut = 1
		                                group by productionOrderId, productId
	                                ) as transfersC on (transfersC.productionOrderId = ProductInOrder.productionOrderID
										                                and transfersC.productId = Product.productID)";
        string strContractChangesSelect = @"select contractChangeID as ID, productionOrderID, description as 'Описание изменения', 
                                          linkToContractChangeFile as 'Файл', contractChangeDate as 'Дата изменения'
                                          from ContractChange";
        #endregion
        public void UpdateOrderTab()
        {
            Program.SetDataGridViewDataSource(strOrdersSelect, ref dataGridViewOrders);
            dataGridViewOrders.Columns["ID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridViewOrders.Columns["Заказчик"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

            Program.SetDataGridViewDataSource(strProductsOfOrderSelect, ref dataGridViewProductsOfOrder);
            dataGridViewProductsOfOrder.Columns["ID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridViewProductsOfOrder.Columns["productionOrderID"].Visible = false;
            dataGridViewProductsOfOrder.Columns["Наименование"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

            Program.SetDataGridViewDataSource(strContractChangesSelect, ref dataGridViewContractChanges);
            dataGridViewContractChanges.Columns["ID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridViewContractChanges.Columns["productionOrderID"].Visible = false;

            FilterDataGridViewOrders(null, null);
        }
        private void dataGridViewOrders_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                currentRowOrderId = (int)dataGridViewOrders.CurrentRow.Cells[0].Value;
                Program.FilterDataGridView(String.Format("ProductionOrderID = {0}", currentRowOrderId), ref dataGridViewProductsOfOrder);
                Program.FilterDataGridView(String.Format("ProductionOrderID = {0}", currentRowOrderId), ref dataGridViewContractChanges);
                //enabled
            }
            catch { }
        }
        private void dataGridViewProductsOfOrder_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            toolStripMenuItemProducts_Click(null, null);
            //btnOrdersFilterClear_Click(null, null);
            int idClickedProductsOfOrderRow;
            idClickedProductsOfOrderRow = Int32.Parse(dataGridViewProductsOfOrder.CurrentRow.Cells["ID"].Value.ToString());
            for (int i = 0; i < dataGridViewProducts.RowCount; i++)
            {
                if (dataGridViewProducts.Rows[i].Cells["ID"].Value.ToString() == idClickedProductsOfOrderRow.ToString())
                {
                    dataGridViewProducts.CurrentCell = dataGridViewProducts.Rows[i].Cells["ID"];
                    return;
                }
            }
        }
        //filt                
        public void SetOrdersFiltersWidth(object sender, DataGridViewColumnEventArgs e)
        {
            try
            {
                for (int i = 0; i < dataGridViewOrders.ColumnCount-1; i++)
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
                filterText = String.Format(@"[Заказчик] like '%{0}%' and
                                            ([Статус] like '{1}') and
                                            [Дата начала производсва] {2} #{3}# and
                                            [Дата завершения производства] {4} #{5}# and
                                            [Плановая дата отгрузки] {6} #{7}# and 
                                            [Фактическая дата отгрузки] {8} #{9}#",
                                            tbO1.Text,
                                            (cbO2.SelectedItem == null ? "' or 'y'like 'y" : cbO2.SelectedItem.ToString()),
                                            lO3.Text, dtpO3.Value.ToShortDateString(),
                                            lO4.Text, dtpO4.Value.ToShortDateString(),
                                            lO5.Text, dtpO5.Value.ToShortDateString(),
                                            lO6.Text, dtpO6.Value.ToShortDateString());
            }
            //try
            //{
            Program.FilterDataGridView(filterText,
                ref dataGridViewOrders);
            //SetMenuOrdersButtonsEnabeld();
            //}
            //catch { }
        }
        //menus
        #endregion

        #region Products
        int currentRowProductId = 0;
        #region select strings
        string strProductsSelect = @"select Product.productID as ID, productName as 'Наименование', linkToPrintFile as 'Файл',
                                    quantityN as 'Требуется переместить на склад',
		                           (quantityW-quantityC) as 'Остаток на складе', quantityC as 'Передано клиентам'
                                from Product	
	                            left join 
	                            (   select productId, sum(quantity) as quantityW
		                            from TransferOfProducts
		                            where transferType = 1 and carriedOut = 1
		                            group by productId
	                            ) as transfersW on (transfersW.productId = Product.productID)
	                            left join 
	                            (   select productId, sum(quantity) as quantityC
		                            from TransferOfProducts
		                            where transferType = 0 and carriedOut = 1
		                            group by productId
	                            ) as transfersC on (transfersC.productId = Product.productID)
	                            left join
	                            (   select productId, sum(quantity) as quantityN
		                            from TransferOfProducts
		                            where transferType = 1 and carriedOut = 0
		                            group by productId
	                            ) as transferN on (transferN.productId = Product.productID)";
        string strTransfersOfProductSelect = @"select transferOfProductsId as ID, productId, productionOrderId as 'Заказ',
                                            IIf(transferType = 1, 'на склад', 'клиенту') as 'Тип', quantity as 'Количество', 
                                            carriedOut as 'Выполнено', transferDate as 'Дата'  
                                    from TransferOfProducts
                                    order by 'Дата' desc";
        #endregion
        public void UpdateProductTab()
        {
            Program.SetDataGridViewDataSource(strProductsSelect, ref dataGridViewProducts);
            dataGridViewProducts.Columns["ID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridViewProducts.Columns["Наименование"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

            Program.SetDataGridViewDataSource(strTransfersOfProductSelect, ref dataGridViewTransfersOfProduct);
            dataGridViewTransfersOfProduct.Columns["ID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridViewTransfersOfProduct.Columns["productId"].Visible = false;

            FilterDataGridViewProducts(new Button(), null);
        }
        private void dataGridViewProducts_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                currentRowProductId = (int)dataGridViewProducts.CurrentRow.Cells["ID"].Value;
                Program.FilterDataGridView(String.Format("productID = {0}", currentRowProductId), ref dataGridViewTransfersOfProduct);
            }
            catch { }
            //enabled
        }
        //filt
        public void SetProductsFiltersWidth(object sender, DataGridViewColumnEventArgs e)
        {
            try
            {
                for (int i = 0; i < dataGridViewProducts.ColumnCount - 1; i++)
                {
                    tableLayoutPanelProductsFilters.ColumnStyles[i].Width = dataGridViewProducts.Columns[i].Width;
                }
            }
            catch { }
        }
        public void FilterDataGridViewProducts(object sender, EventArgs e)
        {
            string filterText = "";
            try { object a = ((Button)sender); }
            catch
            {
                filterText = String.Format(@"[Наименование] like '%{0}%' and
                                            [Файл] like '%{1}%' and
                                            [Требуется переместить на склад] {2} {3} and
                                            [Остаток на складе] {4} {5} and
                                            [Передано клиентам] {6} {7}",
                                            tbP1.Text,
                                            tbP2.Text,
                                            lP3.Text, nudP3.Value.ToString(),
                                            lP4.Text, nudP4.Value.ToString(),
                                            lP5.Text, nudP5.Value.ToString());
            }
            //try
            //{
            Program.FilterDataGridView(filterText,
                ref dataGridViewProducts);
            //SetMenuOrdersButtonsEnabeld();
            //}
            //catch { }
        }
        //menu
        #endregion                                 


        #region MenuClics
        private void toolStripMenuItemClients_Click(object sender, EventArgs e)
        {
            if (tabControlFGS.SelectedTab != tabPageClients)
            { tabControlFGS.SelectedTab = tabPageClients; }
        }
        private void toolStripMenuItemOrders_Click(object sender, EventArgs e)
        {
            if (tabControlFGS.SelectedTab != tabPageOrders)
            { tabControlFGS.SelectedTab = tabPageOrders; }
        }
        private void toolStripMenuItemProducts_Click(object sender, EventArgs e)
        {
            if (tabControlFGS.SelectedTab != tabPageProducts)
            { tabControlFGS.SelectedTab = tabPageProducts; }
        }
        #endregion

        public void FilterLabelClick(object sender, EventArgs e)
        {
            Program.FilterLabelClick(sender, e);
        }

    }

}
