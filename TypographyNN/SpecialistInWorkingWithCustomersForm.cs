using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using DGVPrinterHelper;

namespace TypographyNN
{
    public partial class SpecialistInWorkingWithCustomersForm : System.Windows.Forms.Form
    {
        public SpecialistInWorkingWithCustomersForm()
        {
            InitializeComponent();
            tabControlSIWWC.ItemSize = new Size(new Point(0, 1));
            this.Size = new Size(750, 450);
            UpdateClientTab();
            UpdateOrderTab();
            UpdateProductTab();
            UpdateOperationTab();
            UpdateMaterialTab();
        }
        public void UpdateAll()
        {
            UpdateClientTab();
            UpdateOrderTab();
            UpdateProductTab();
            UpdateOperationTab();
            UpdateMaterialTab();
        }
        String strDBError = "Изменения не сохранены из-за ошибки.";
        #region Clients
        int currentRowClientId = 0;
        #region selectStrings
        string strClientsSelect = @"select Customer.customerID  as ID, customerName as 'Имя / Наименование организации', 
                                    isnull(sums,0) as 'Общая сумма заказов', isnull(countOrd,0) as 'Количество заказов', 
                                    isnull((countOrd-countOrdFinished),0) as 'Количество текущих заказов', 
                                    isnull(countDelay,0) as 'Количество задержек по вине клиента',
                                    contactNumber as 'Контактный номер', email as 'E-mail' 
                from Customer
                left join 
                (   select customerID, sum(sumM) as sums,count (ProductionOrder.productionOrderID) as countOrd, count (dateOfDelivery) as countOrdFinished, 
	                count(iif(delayByCustomer = 0, null, 1)) as countDelay
	                from ProductionOrder
	                left join 
	                (   select ProductInOrder.productionOrderID, sum((matP+opP)*numberOfCopiesOrdered) as sumM
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
                    ) as prodSums on (prodSums.productionOrderID = ProductionOrder.productionOrderID)
	                group by customerID
                ) as clientStats on (clientStats.customerID = Customer.customerID)";
        string strOrdersOfClientSelect = @"select ProductionOrder.productionOrderID as ID, ProductionOrder.customerID, recipientName as 'Имя получателя', 
                iif(dateOfPayment is null, 'Ожидает оплаты',
                iif (productionStartDate is null, 'Оплачен',
                iif (productionCompletionDate is NULL, 'В процессе выполнения',
                iif (dateOfDelivery is null, 'Ожидает отгрузки','Завершен')))) as 'Статус', sumM as 'Сумма', linkToTheContractFile as 'Файл договора', contractDate as 'Дата заключения договора', 
                estimatedCompletionDate as 'Плановая дата отгрузки', dateOfDelivery 'Фактическая дата отгрузки', delayByCustomer as 'Задержка по вине клиента'
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
                ) as prodStats on (prodStats.productionOrderID = ProductionOrder.productionOrderID)";
        string strProductsOfClientSelect = @"select Product.productID as ID, customerID, productName as 'Наименование',  
                                            sum(numberOfCopiesOrdered) as 'Количество экземпляров'
                                            from ProductionOrder
                                            join ProductInOrder on ProductionOrder.productionOrderID = ProductInOrder.productionOrderID
                                            join Product on ProductInOrder.productID = Product.productID
                                            group by Product.productID, customerID, productName";
        #endregion
        public void UpdateClientTab()
        {
            Program.SetDataGridViewDataSource(strClientsSelect, ref dataGridViewClients);
            dataGridViewClients.Columns["ID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewClients.Columns["ID"].ReadOnly = true;
            dataGridViewClients.Columns["Имя / Наименование организации"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridViewClients.Columns["Общая сумма заказов"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            dataGridViewClients.Columns["Общая сумма заказов"].DefaultCellStyle.Format = "F";
            dataGridViewClients.Columns["Общая сумма заказов"].ReadOnly = true;
            dataGridViewClients.Columns["Количество заказов"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridViewClients.Columns["Количество заказов"].Width = 75;
            dataGridViewClients.Columns["Количество заказов"].ReadOnly = true;
            dataGridViewClients.Columns["Количество текущих заказов"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridViewClients.Columns["Количество текущих заказов"].Width = 75;
            dataGridViewClients.Columns["Количество текущих заказов"].ReadOnly = true;
            dataGridViewClients.Columns["Количество задержек по вине клиента"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridViewClients.Columns["Количество задержек по вине клиента"].Width = 80;
            dataGridViewClients.Columns["Количество задержек по вине клиента"].ReadOnly = true;
            dataGridViewClients.Columns["Контактный номер"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridViewClients.Columns["Контактный номер"].Width = 75;

            dataGridViewOrdersOfClient_Update();
            dataGridViewOrdersOfClient.Columns["ID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            dataGridViewOrdersOfClient.Columns["customerID"].Visible = false;
            dataGridViewOrdersOfClient.Columns["Имя получателя"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridViewOrdersOfClient.Columns["Статус"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            dataGridViewOrdersOfClient.Columns["Сумма"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            dataGridViewOrdersOfClient.Columns["Сумма"].DefaultCellStyle.Format = "F";
            dataGridViewOrdersOfClient.Columns["Файл договора"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridViewOrdersOfClient.Columns["Файл договора"].Width = 60;
            dataGridViewOrdersOfClient.Columns["Дата заключения договора"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridViewOrdersOfClient.Columns["Дата заключения договора"].Width = 120;
            dataGridViewOrdersOfClient.Columns["Плановая дата отгрузки"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridViewOrdersOfClient.Columns["Плановая дата отгрузки"].Width = 110;
            dataGridViewOrdersOfClient.Columns["Фактическая дата отгрузки"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridViewOrdersOfClient.Columns["Фактическая дата отгрузки"].Width = 110;
            dataGridViewOrdersOfClient.Columns["Задержка по вине клиента"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridViewOrdersOfClient.Columns["Задержка по вине клиента"].Width = 80;

            Program.SetDataGridViewDataSource(strProductsOfClientSelect, ref dataGridViewProductsOfClient);
            dataGridViewProductsOfClient.Columns["ID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            dataGridViewProductsOfClient.Columns["customerID"].Visible = false;
            dataGridViewProductsOfClient.Columns["Наименование"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridViewProductsOfClient.Columns["Количество экземпляров"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridViewProductsOfClient.Columns["Количество экземпляров"].Width = 75;

            SetMenuClientsButtonsEnabeld();
        }
        private void dataGridViewClients_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                currentRowClientId = (int)dataGridViewClients.CurrentRow.Cells[0].Value;
                Program.FilterDataGridView(String.Format("customerID = {0}", currentRowClientId), ref dataGridViewOrdersOfClient);
                SetMenuClientsButtonsEnabeld();
            }
            catch { }
        }
        private void dataGridViewProductsOfClient_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            FilterDataGridViewProducts(new Button(), null);
            toolStripMenuItemProducts_Click(null, null);
            int idClickedProductsOfClientRow;
            idClickedProductsOfClientRow = Int32.Parse(dataGridViewProductsOfClient.CurrentRow.Cells["ID"].Value.ToString());
            for (int i = 0; i < dataGridViewProducts.RowCount; i++)
            {
                if (dataGridViewProducts.Rows[i].Cells["ID"].Value.ToString() == idClickedProductsOfClientRow.ToString())
                {
                    dataGridViewProducts.CurrentCell = dataGridViewProducts.Rows[i].Cells["ID"];
                    return;
                }
            }
        }
        //filters        
        public void SetClientsFiltersWidth(object sender, DataGridViewColumnEventArgs e)
        {
            try
            {
                for (int i = 0; i < dataGridViewClients.ColumnCount - 1; i++)
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
                                        [Общая сумма заказов] {1} {2} and 
                                        [Количество заказов] {3} {4} and 
                                        [Количество текущих заказов] {5} {6} and
                                        [Количество задержек по вине клиента] {7} {8} and
                                        [Контактный номер] like '%{9}%' and
                                        [E-mail] like '%{10}%'",
                                      tbC1.Text,
                                      lC2.Text, nudC2.Value.ToString(),
                                      lC3.Text, nudC3.Value.ToString(),
                                      lC4.Text, nudC4.Value.ToString(),
                                      lC5.Text, nudC5.Value.ToString(),
                                      tbC6.Text,
                                      tbC7.Text);
            }
            try
            {
                Program.FilterDataGridView(filterText,
                    ref dataGridViewClients);
            }
            catch { }
            SetMenuClientsButtonsEnabeld();
        }
        // ClientsMenuActions    
        public void SetMenuClientsButtonsEnabeld()
        {
            if (dataGridViewClients.CurrentRow == null)
            {
                toolStripMenuItemPrintClients.Enabled = false;
                toolStripMenuItemEditClient.Enabled = false;
                toolStripMenuItemDeleteClient.Enabled = false;
                toolStripMenuItemAddOrderOfClient.Enabled = false;
                toolStripMenuItemEditOrderOfClient.Enabled = false;
                toolStripMenuItemDeleteOrderOfClient.Enabled = false;

            }
            else if (dataGridViewClients.CurrentRow.Cells["Количество заказов"].Value.ToString() != "0")
            {
                toolStripMenuItemPrintClients.Enabled = true;
                toolStripMenuItemEditClient.Enabled = true;
                toolStripMenuItemDeleteClient.Enabled = false;
                toolStripMenuItemAddOrderOfClient.Enabled = true;
            }
            else
            {
                toolStripMenuItemPrintClients.Enabled = true;
                toolStripMenuItemEditClient.Enabled = true;
                toolStripMenuItemDeleteClient.Enabled = true;
                toolStripMenuItemAddOrderOfClient.Enabled = true;
                
            }
            if ((dataGridViewOrdersOfClient.CurrentRow != null)&&
                (dataGridViewOrdersOfClient.CurrentRow.Cells["Фактическая дата отгрузки"].Value.ToString() == ""))
            {
                toolStripMenuItemEditOrderOfClient.Enabled = true;
                toolStripMenuItemDeleteOrderOfClient.Enabled = true;
            }
            else
            {
                toolStripMenuItemEditOrderOfClient.Enabled = false;
                toolStripMenuItemDeleteOrderOfClient.Enabled = false;
            }
        }
        public void SetMenuClientsButtonsEnabeld(object sender, EventArgs e)
        { SetMenuClientsButtonsEnabeld(); }
        private void toolStripMenuItemPrintClient_Click(object sender, EventArgs e)
        {
            DGVPrinter printer = Program.DefaultPrinter();
            printer.Title = "Клиенты";
            printer.Footer = "Отдел заказов";
            printer.PrintPreviewDataGridView(dataGridViewClients);
        }
        private void toolStripMenuItemAddClient_Click(object sender, EventArgs e)
        {
            EditForm addClient = new EditForm(@"select customerName as 'Имя / Наименование организации', 
                                contactNumber as 'Контактный номер', email as 'E-mail'
                                from Customer
                                where customerID = 0",
                                          new object[] { "", "", "" },
                                          null,
                                          new int[] { 0, 1 },
                                          null,
                                          @"insert into Customer 
                                (customerName, contactNumber, email) 
                                values (@name, @number, @email)",
                                          new string[] { "@name", "@number", "@email" },
                                          new SqlDbType[] { SqlDbType.VarChar, SqlDbType.VarChar, SqlDbType.VarChar });
            addClient.ShowDialog();
            UpdateAll();
        }
        private void toolStripMenuItemEditClient_Click(object sender, EventArgs e)
        {
            EditForm editClient = new EditForm(@"select customerName as 'Имя / Наименование организации', 
                                contactNumber as 'Контактный номер', email as 'E-mail'
                                from Customer
                                where customerID = " + dataGridViewClients.CurrentRow.Cells["ID"].Value.ToString(),
                                          null,
                                          null,
                                          new int[] { 0, 1 },
                                          null,
                                          @"update Customer 
                                        set customerName = @name,
                                        contactNumber = @number,
                                        email = @email
                                        where customerID = " + dataGridViewClients.CurrentRow.Cells["ID"].Value.ToString(),
                                          new string[] { "@name", "@number", "@email" },
                                          new SqlDbType[] { SqlDbType.VarChar, SqlDbType.VarChar, SqlDbType.VarChar });
            editClient.ShowDialog();
            UpdateAll();
        }
        private void toolStripMenuItemDeleteClient_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите удалить клиента из базы?", "Удаление записи", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    Program.SqlCommandExecute("delete", "delete from Customer where customerid =  @id",
                        new string[] { "@id" }, new SqlDbType[] { SqlDbType.Int },
                        new object[] { dataGridViewClients.CurrentRow.Cells["ID"].Value });
                    Program.SetDataGridViewDataSource(strClientsSelect, ref dataGridViewClients);
                }
                catch { MessageBox.Show(strDBError); }
                UpdateAll();
            }
        }
        //OrdersOfClientMenuAction
        public void dataGridViewOrdersOfClient_Update()
        {
            Program.SetDataGridViewDataSource(strOrdersOfClientSelect, ref dataGridViewOrdersOfClient);
            dataGridViewClients_CurrentCellChanged(null, null);
        }
        private void dataGridViewOrdersOfClient_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            toolStripMenuItemOrders_Click(null, null);
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
        private void toolStripMenuItemAddOrderOfClient_Click(object sender, EventArgs e)
        {
            EditForm addOrder = new EditForm(@"select recipientName as 'Имя получателя',
                    linkToTheContractFile as 'Файл договора', contractDate as 'Дата заключения договора',
                    estimatedCompletionDate as 'Плановая дата отгрузки'                                
                                from ProductionOrder
                                where productionOrderID = 0",
                                          new object[] { "", "", DateTime.Today, DateTime.Today.AddDays(21) },
                                          null,
                                          new int[] { 1, 2, 3 },
                                          new int[] { 1 },
                                          @"insert into ProductionOrder 
                    (customerID, recipientName, linkToTheContractFile, contractDate, estimatedCompletionDate) 
                    values (" + currentRowClientId + ", @recipientName, @link, @contractDate, @estDate)",
                                          new string[] { "@recipientName", "@link", "@contractDate", "@estDate" },
                                          new SqlDbType[] { SqlDbType.VarChar, SqlDbType.VarChar, SqlDbType.Date, SqlDbType.Date });
            addOrder.ShowDialog();
            UpdateAll();
        }
        private void toolStripMenuItemEditOrderOfClient_Click(object sender, EventArgs e)
        {
            EditForm editOrder = new EditForm(@"select recipientName as 'Имя получателя',
                    linkToPaymentDocumentFile as 'Документ об оплате', isnull(dateOfPayment,GETDATE()) as 'Дата оплаты', 
                    delayByCustomer as 'Задержка по вине клиента'
                                from ProductionOrder
                                where productionOrderID = " + dataGridViewOrdersOfClient.CurrentRow.Cells["ID"].Value.ToString(),
                                          null,
                                          null,
                                          null,
                                          new int[] { 1 },
                                          @"update ProductionOrder
                                        set recipientName = @name,
                                        linkToPaymentDocumentFile = @link,
                                        dateOfPayment = @payDate,
                                        delayByCustomer = @cDelay
                                        where productionOrderID = " + dataGridViewOrdersOfClient.CurrentRow.Cells["ID"].Value.ToString() + @"
                                        insert into ContractChange 
                                        (productionOrderID, contractChangeDate, description) 
                                        values (" + dataGridViewOrdersOfClient.CurrentRow.Cells["ID"].Value.ToString() + 
                                                ", GETDATE(), 'Заказ был отредактирован')",
                                          new string[] { "@name", "@link", "@payDate", "@cDelay" },
                                          new SqlDbType[] { SqlDbType.VarChar, SqlDbType.VarChar, SqlDbType.Date, SqlDbType.Bit });
            editOrder.ShowDialog();
            UpdateAll();
        }
        private void toolStripMenuItemDeleteOrderOfClient_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите удалить заказ из базы?", "Удаление записи", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    Program.SqlCommandExecute("delete",
                        @"delete from 
                    ProductInOrder where productionOrderID =  @id
                    delete from 
                    ContractChange where productionOrderID =  @id
                    delete from 
                    ProductionOrder where productionOrderID =  @id",
                        new string[] { "@id" }, new SqlDbType[] { SqlDbType.Int },
                        new object[] { dataGridViewOrdersOfClient.CurrentRow.Cells["ID"].Value });
                }

                catch { MessageBox.Show(strDBError); }
                UpdateAll();
            }
        }        
        #endregion
        #region Orders
        int currentRowOrderId = 0;
        #region selectStrings
        string strOrdersSelect = @"select ProductionOrder.productionOrderID as ID, customerName as 'Заказчик', recipientName as 'Имя получателя', sumM as 'Сумма',         
                                linkToTheContractFile as 'Файл договора', contractDate as 'Дата заключения договора', 
                                linkToPaymentDocumentFile as 'Документ об оплате', dateOfPayment as 'Дата оплаты',
                                productionStartDate as 'Дата начала производсва',
								CEILING(1.0*sumD/480) as 'Длительность производства (дн.)', 
								productionCompletionDate as 'Дата завершения производства',
                                estimatedCompletionDate as 'Плановая дата отгрузки', dateOfDelivery 'Фактическая дата отгрузки',
                                delayByCustomer as 'Задержка по вине клиента'
                                from ProductionOrder
                                left join Customer on(ProductionOrder.customerID = Customer.customerID)
								left join (
                                select ProductInOrder.productionOrderID, sum((matP+opP)*numberOfCopiesOrdered) as sumM, 
                                        sum(duration*numberOfCopiesOrdered) as sumD
                                        from Product
                                            left join ProductInOrder on (Product.productID = ProductInOrder.productID)
                                            join
                                            (   select productID, productionOrderID, sum(ISNULL (quantity*materialPrice,0)) as matP 
	                                            from
	                                            (	select ProductInOrder.productID, ProductionOrder.productionOrderID, 
                                                    ProductMaterial.materialID, quantity, max(dateOfMaterialPriceChange) as datP
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
                                            (	select productID, productionOrderID, sum(ISNULL (operationPrice,0)) as opP, sum(dur) as duration
	                                            from
	                                            (	select ProductInOrder.productID, ProductionOrder.productionOrderID, ProductOperation.operationID,[duration (minutes)] as dur,  max(dateOfOperationPriceChange) as datP
		                                            from ProductionOrder
		                                            left join ProductInOrder on (ProductionOrder.productionOrderID = ProductInOrder.productionOrderID)
		                                            left join ProductOperation on (ProductInOrder.productID = ProductOperation.productID)
		                                            left join OperationPriceChange on (ProductOperation.operationID = OperationPriceChange.operationID)
		                                            where dateOfOperationPriceChange <= contractDate
		                                            group by ProductInOrder.productID, ProductionOrder.productionOrderID, ProductOperation.operationID,[duration (minutes)]
	                                            ) as rightPricesDates left join OperationPriceChange 
	                                            on (rightPricesDates.operationID = OperationPriceChange.operationID
	                                            and rightPricesDates.datP = OperationPriceChange.dateOfOperationPriceChange)
	                                            group by productID, productionOrderID
                                            ) as OperationPrices on (OperationPrices.productID = Product.productID and 
			                                OperationPrices.productionOrderID = ProductInOrder.productionOrderID)
			                                group by ProductInOrder.productionOrderID
                                ) as prodStats on (prodStats.productionOrderID = ProductionOrder.productionOrderID)";
        string strProductsOfOrderSelect = @"select Product.productID as ID, ProductInOrder.productionOrderID, productName as 'Наименование', (matP+opP) as 'Стоимость', 
        duration as 'Длительность производства (мин.)', numberOfCopiesOrdered as 'Количество заказанных экземпляров', numberOfCopiesMade as 'Количество изготовленных экземпляров',
		isnull(quantity,0) as 'Количество экземпляров на складе'
        from Product
            left join ProductInOrder on (Product.productID = ProductInOrder.productID)
			left join
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
			left join 
            (	select productID, productionOrderID, sum(ISNULL (operationPrice,0)) as opP, sum(dur) as duration
	            from
	            (	select ProductInOrder.productID, ProductionOrder.productionOrderID, ProductOperation.operationID,[duration (minutes)] as dur,  
                    max(dateOfOperationPriceChange) as datP
		            from ProductionOrder
		            left join ProductInOrder on (ProductionOrder.productionOrderID = ProductInOrder.productionOrderID)
		            left join ProductOperation on (ProductInOrder.productID = ProductOperation.productID)
		            left join OperationPriceChange on (ProductOperation.operationID = OperationPriceChange.operationID)
		            where dateOfOperationPriceChange <= contractDate
		            group by ProductInOrder.productID, ProductionOrder.productionOrderID, ProductOperation.operationID,[duration (minutes)]
	            ) as rightPricesDates left join OperationPriceChange 
	            on (rightPricesDates.operationID = OperationPriceChange.operationID
	            and rightPricesDates.datP = OperationPriceChange.dateOfOperationPriceChange)
	            group by productID, productionOrderID
            ) as OperationPrices on (OperationPrices.productID = Product.productID and OperationPrices.productionOrderID = ProductInOrder.productionOrderID)
			left join 
			(   select productionOrderId, productId, sum(quantity) as quantity
				from TransferOfProducts
				where transferType = 1 and carriedOut = 1
				group by productionOrderId, productId
			) as transfers on (transfers.productionOrderId = ProductInOrder.productionOrderID
										and transfers.productId = Product.productID)";
        string strContractChangesSelect = @"select contractChangeID as ID, productionOrderID, description as 'Описание изменения', 
                                          linkToContractChangeFile as 'Файл', contractChangeDate as 'Дата изменения'
                                          from ContractChange";
        #endregion
        public void UpdateOrderTab()
        {
            Program.SetDataGridViewDataSource(strOrdersSelect, ref dataGridViewOrders);
            dataGridViewOrders.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridViewOrders.Columns[0].Width = 40;
            dataGridViewOrders.Columns[3].DefaultCellStyle.Format = "F";

            Program.SetDataGridViewDataSource(strProductsOfOrderSelect, ref dataGridViewProductsOfOrder);
            dataGridViewProductsOfOrder.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewProductsOfOrder.Columns[1].Visible = false;
            dataGridViewProductsOfOrder.Columns[3].DefaultCellStyle.Format = "F";

            Program.SetDataGridViewDataSource(strContractChangesSelect, ref dataGridViewContractChanges);
            dataGridViewContractChanges.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewContractChanges.Columns[1].Visible = false;

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
        private void dataGridViewProductsOfOrder_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            FilterDataGridViewProducts(new Button(), null);
            toolStripMenuItemProducts_Click(null, null);
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
        //filters
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
                filterText = String.Format(@"[Заказчик] like '%{0}%' and
                            [Имя получателя] like '%{1}%' and                                            
                            [Сумма] {2} {3} and 
                            [Файл договора] like '%{4}%' and    
                            [Дата заключения договора] {5} #{6}# and 
                            [Документ об оплате] like '%{7}%' and    
                            [Дата оплаты] {8} #{9}# and  
                            [Дата начала производсва] {10} #{11}# and 
                            [Длительность производства (дн.)] {12} {13} and 
                            [Дата завершения производства] {14} #{15}# and  
                            [Плановая дата отгрузки] {16} #{17}# and 
                            [Фактическая дата отгрузки] {18} #{19}# and
                            [Задержка по вине клиента] = {20}",                                                                                                                              //[Задержка по вине клиента] = {20}",
                                        tbOr1.Text,
                                        tbOr2.Text,
                                        lOr3.Text, nudOr3.Value.ToString(),
                                        tbOr4.Text,
                                        lOr5.Text, dtpOr5.Value.ToShortDateString(),
                                        tbOr6.Text,
                                        lOr7.Text, dtpOr7.Value.ToShortDateString(),
                                        lOr8.Text, dtpOr8.Value.ToShortDateString(),
                                        lOr9.Text, nudOr9.Value.ToString(),
                                        lOr10.Text, dtpOr10.Value.ToShortDateString(),
                                        lOr11.Text, dtpOr11.Value.ToShortDateString(),
                                        lOr12.Text, dtpOr12.Value.ToShortDateString(),
                                        chbOr13.Checked ? 1 : 0);
            }
            try
            {
                Program.FilterDataGridView(filterText,
                    ref dataGridViewOrders);
                SetMenuOrdersButtonsEnabeld();
            }
            catch { }
        }
        //OrdersMenuActions
        public void SetMenuOrdersButtonsEnabeld()
        {
            if (dataGridViewOrders.CurrentRow == null)
            {
                toolStripMenuItemPrintOrders.Enabled = false;
                toolStripMenuItemEditOrder.Enabled = false;
                toolStripMenuItemDeleteOrder.Enabled = false;
                toolStripMenuItemCopyOrder.Enabled = false;
                toolStripMenuItemProductOfOrderFrom.Enabled = false;
                toolStripMenuItemProductOfOrderAdd.Enabled = false;
                toolStripMenuItemAddContractChange.Enabled = false;
                toolStripMenuItemEditContractChange.Enabled = false;
            }
            else if (dataGridViewOrders.CurrentRow.Cells["Фактическая дата отгрузки"].Value.ToString() != "")
            {
                toolStripMenuItemPrintOrders.Enabled = true;
                toolStripMenuItemEditOrder.Enabled = false;
                toolStripMenuItemDeleteOrder.Enabled = false;
                toolStripMenuItemCopyOrder.Enabled = true;
                toolStripMenuItemProductOfOrderFrom.Enabled = false;
                toolStripMenuItemProductOfOrderAdd.Enabled = false;
                toolStripMenuItemProductOfOrderEdit.Enabled = false;
                toolStripMenuItemProductOfOrderDelete.Enabled = false;
                toolStripMenuItemAddContractChange.Enabled = false;
                toolStripMenuItemEditContractChange.Enabled = false;
            }
            else
            {
                toolStripMenuItemPrintOrders.Enabled = true;
                toolStripMenuItemEditOrder.Enabled = true;
                toolStripMenuItemDeleteOrder.Enabled = true;
                toolStripMenuItemCopyOrder.Enabled = true;
                toolStripMenuItemProductOfOrderAdd.Enabled = true;
                toolStripMenuItemProductOfOrderFrom.Enabled = true;
                toolStripMenuItemAddContractChange.Enabled = true;
                if (dataGridViewProductsOfOrder.CurrentRow != null)
                {
                    toolStripMenuItemProductOfOrderEdit.Enabled = true;
                    toolStripMenuItemProductOfOrderDelete.Enabled = true;
                }
                else
                {
                    toolStripMenuItemProductOfOrderEdit.Enabled = false;
                    toolStripMenuItemProductOfOrderDelete.Enabled = false;
                }
                if (dataGridViewContractChanges.CurrentRow != null)
                { toolStripMenuItemEditContractChange.Enabled = true; }
                else
                { toolStripMenuItemEditContractChange.Enabled = false; }
            }
        }        
        private void toolStripMenuItemPrintOrders_Click(object sender, EventArgs e)
        {
            DGVPrinter printer = Program.DefaultPrinter();
            printer.Title = "Заказы";
            printer.Footer = "Отдел заказов";
            dataGridViewOrders.Columns["Файл договора"].Visible = false;
            dataGridViewOrders.Columns["Документ об оплате"].Visible = false;
            printer.PrintPreviewDataGridView(dataGridViewOrders);
            dataGridViewOrders.Columns["Файл договора"].Visible = true;
            dataGridViewOrders.Columns["Документ об оплате"].Visible = true;
        }
        private void toolStripMenuItemAddOrder_Click(object sender, EventArgs e)
        {
            EditForm addOrder = new EditForm(@"select recipientName as 'Имя получателя',
                    linkToTheContractFile as 'Файл договора', contractDate as 'Дата заключения договора',
                    estimatedCompletionDate as 'Плановая дата отгрузки'                                
                                from ProductionOrder
                                where productionOrderID = 0",
                                          new object[] { "", "", DateTime.Today, DateTime.Today.AddDays(21) },
                                          @"select customerID as ID, customerName as 'Имя / Наименование организации', 
                                contactNumber as 'Контактный номер', email as 'E-mail'
                                from Customer",
                                          new int[] { 1, 2, 3 },
                                          new int[] { 1 },
                                          @"insert into ProductionOrder 
                    (customerID, recipientName, linkToTheContractFile, contractDate, estimatedCompletionDate) 
                    values (@cID, @recipientName, @link, @contractDate, @estDate)",
                                          new string[] { "@recipientName", "@link", "@contractDate", "@estDate", "@cID" },
                                          new SqlDbType[] { SqlDbType.VarChar, SqlDbType.VarChar, SqlDbType.Date, SqlDbType.Date, SqlDbType.Int });
            addOrder.ShowDialog();
            UpdateAll();
        }
        private void toolStripMenuItemCopyOrder_Click(object sender, EventArgs e)
        {
            EditForm addOrder = new EditForm(@"select recipientName as 'Имя получателя',
                linkToTheContractFile as 'Файл договора', contractDate as 'Дата заключения договора',
                estimatedCompletionDate as 'Плановая дата отгрузки'                                
                from ProductionOrder
                where productionOrderID = 0",
                                new object[] { "", "", DateTime.Today, DateTime.Today.AddDays(21) },
                                @"select customerID as ID, customerName as 'Имя / Наименование организации', 
                contactNumber as 'Контактный номер', email as 'E-mail'
                from Customer",
                                new int[] { 1, 2, 3 },
                                new int[] { 1 },
                                @"insert into ProductionOrder 
                (customerID, recipientName, linkToTheContractFile, contractDate, estimatedCompletionDate) 
                values (@cID, @recipientName, @link, @contractDate, @estDate)                
                insert into ProductInOrder
                select (select max(productionOrderID) from ProductionOrder),
                productID, numberOfCopiesOrdered, 0
                from ProductInOrder where productionOrderID = "+currentRowOrderId,
                                new string[] { "@recipientName", "@link", "@contractDate", "@estDate", "@cID" },
                                new SqlDbType[] { SqlDbType.VarChar, SqlDbType.VarChar, SqlDbType.Date, SqlDbType.Date, SqlDbType.Int });
            addOrder.ShowDialog();
            UpdateAll();
        }
        private void toolStripMenuItemEditOrder_Click(object sender, EventArgs e)
        {
            EditForm editOrder = new EditForm(@"select recipientName as 'Имя получателя',
                    linkToPaymentDocumentFile as 'Документ об оплате', isnull(dateOfPayment,GETDATE()) as 'Дата оплаты', 
                    delayByCustomer as 'Задержка по вине клиента'
                                from ProductionOrder
                                where productionOrderID = " + currentRowOrderId,
                                          null,
                                          null,
                                          null,
                                          new int[] { 1 },
                                          @"update ProductionOrder
                                        set recipientName = @name,
                                        linkToPaymentDocumentFile = @link,
                                        dateOfPayment = @payDate,
                                        delayByCustomer = @cDelay
                                        where productionOrderID = " + currentRowOrderId + @"
                                insert into ContractChange 
                                (productionOrderID, contractChangeDate, description) 
                                values (" + currentRowOrderId + ", GETDATE(), 'Заказ был отредактирован')",
                                          new string[] { "@name", "@link", "@payDate", "@cDelay" },
                                          new SqlDbType[] { SqlDbType.VarChar, SqlDbType.VarChar, SqlDbType.Date, SqlDbType.Bit });
            editOrder.ShowDialog();
            UpdateAll();
        }
        private void toolStripMenuItemDeleteOrder_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите удалить заказ из базы?", "Удаление записи", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    Program.SqlCommandExecute("delete",
                        @"delete from 
                    ProductInOrder where productionOrderID =  @id
                    delete from 
                    ContractChange where productionOrderID =  @id
                    delete from 
                    ProductionOrder where productionOrderID =  @id",
                        new string[] { "@id" }, new SqlDbType[] { SqlDbType.Int },
                        new object[] { currentRowOrderId });
                }
                catch { MessageBox.Show(strDBError); }
                UpdateAll();
            }
        }        
        //productsOfOrderMenu
        private void toolStripMenuItemProductOfOrderFrom_Click(object sender, EventArgs e)
        {
            EditForm addProductToOrder = new EditForm(@"select numberOfCopiesOrdered as 'Количество экземпляров' 
                                from ProductInOrder 
                                where productionOrderID = 0",
                                          null,
                                          @"select Product.productID as ID, productName as 'Наименование',linkToPrintFile as 'Файл',
            note as 'Примечание',(matP+opP) as 'Текущая стоимость', dur as 'Длительность производства (мин.)'
            from Product
            left join
            (   select Product.productID, sum(materialPrice* quantity) as matP
	            from Product
	            left join ProductMaterial on (Product.productID = ProductMaterial.productID)
	            left join
	            (   select Material.materialID, materialPrice
                    from (select materialID, max(dateOfMaterialPriceChange) as dat
                            from MaterialPriceChange 
                            group by materialID) as lastChange
                    left join Material on(Material.materialID = lastChange.materialID)
                    left join MaterialPriceChange on(Material.materialID = MaterialPriceChange.materialID 
                                                        and dat = MaterialPriceChange.dateOfMaterialPriceChange)
	            ) as materialPrices on (ProductMaterial.materialID = materialPrices.materialID)											
	            group by Product.productID	
            ) as MaterialPrices on (MaterialPrices.productID = Product.productID)
            left join 
            (	select Product.productID, sum(operationPrice) as opP, sum(ProductOperation.[duration (minutes)]) as dur
	            from Product
	            left join ProductOperation on (Product.productID = ProductOperation.productID)
	            left join
	            (   select Operation.operationID, operationPrice
		            from (  select operationID, max(dateOfOperationPriceChange) as dat
				            from OperationPriceChange 
				            group by operationID ) as lastChange
		            left join Operation on(Operation.operationID = lastChange.operationID)
		            left join OperationPriceChange on(Operation.operationID = OperationPriceChange.operationID 
														            and dat = OperationPriceChange.dateOfOperationPriceChange)
	            ) as operationPrices on (ProductOperation.operationID = OperationPrices.operationID)											
	            group by Product.productID	
            ) as OperationPrices on (OperationPrices.productID = Product.productID)",
                                          new int[] { 0 },
                                          null,
                                          @"insert into ProductInOrder 
            (productionOrderID, productID, numberOfCopiesOrdered, numberOfCopiesMade)
            values ("+currentRowOrderId+ @",@pID, @copies, 0)
                                insert into ContractChange 
                                (productionOrderID, contractChangeDate, description) 
                                values (" + currentRowOrderId + ", GETDATE(),'Добавлен продукт:'+ convert(varchar,@pID)+'; '+convert(varchar,@copies)+' экз.')",
                                          new string[] { "@copies", "@pID" },
                                          new SqlDbType[] { SqlDbType.Int, SqlDbType.Int });
            addProductToOrder.ShowDialog();
            UpdateAll();
        }
        private void toolStripMenuItemProductOfOrderAdd_Click(object sender, EventArgs e)
        {
            EditForm addProductToOrder = new EditForm(@"select productName as 'Наименование',linkToPrintFile as 'Файл',
                    note as 'Примечание', null as 'Количество экземпляров'
                    from Product where productID = 0",
                                null,
                                null,
                                new int[] { 0,1,3},
                                new int[] { 1 },
                                @"insert into Product
                    (productName, linkToPrintFile, note) 
                    values (@name, @file, @note)
                                insert into ProductInOrder 
                    (productionOrderID, productID, numberOfCopiesOrdered, numberOfCopiesMade)
                    values (" + currentRowOrderId + @",(select max(productID) from Product), @copies, 0)
                                insert into ContractChange 
                    (productionOrderID, contractChangeDate, description) 
                    values (" + currentRowOrderId + ", GETDATE(), 'Добавлен новый продукт: '+(select max(productID) from Product)+'; '+convert(varchar,@copies)+' экз.')",
                                new string[] { "@name", "file", "note", "@copies"},
                                new SqlDbType[] { SqlDbType.VarChar, SqlDbType.VarChar, SqlDbType.VarChar, SqlDbType.Int });
            addProductToOrder.ShowDialog();
            UpdateAll();
        }
        private void toolStripMenuItemProductOfOrderEdit_Click(object sender, EventArgs e)
        {
            EditForm editProductOfOrder = new EditForm(@"select numberOfCopiesOrdered as 'Количество экземпляров' 
                from ProductInOrder 
                where productionOrderID = 0",
                            new object[] { dataGridViewProductsOfOrder.CurrentRow.Cells["Количество заказанных экземпляров"].Value.ToString()},
                            null,
                            new int[] { 0 },
                            null,
                            @"update ProductInOrder
                set numberOfCopiesOrdered = @copies
                where productionOrderID = "+currentRowOrderId+" and productID = "+ 
                                            dataGridViewProductsOfOrder.CurrentRow.Cells["ID"].Value.ToString()+@"
                            insert into ContractChange 
                (productionOrderID, contractChangeDate, description)
                values(" + currentRowOrderId + ", GETDATE(), 'Изменено количество экземпляров: "+
                                            dataGridViewProductsOfOrder.CurrentRow.Cells["ID"].Value.ToString() +
                "; '+convert(varchar,@copies)+' экз.')",
                            new string[] { "@copies"},
                            new SqlDbType[] { SqlDbType.Int });
            editProductOfOrder.ShowDialog();
            UpdateAll();
        }
        private void toolStripMenuItemProductOfOrderDelete_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите удалить продукт из заказа?", "Удаление записи", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    Program.SqlCommandExecute("delete",
                        @"delete from 
                    ProductInOrder where productionOrderID =  @order
                    and productID =  @product
                    insert into ContractChange 
                    (productionOrderID, contractChangeDate, description) 
                    values (@order, GETDATE(), 'Удален продукт: '+convert(varchar,@product))",
                        new string[] { "@order", "@product" }, new SqlDbType[] { SqlDbType.Int, SqlDbType.Int },
                        new object[] { currentRowOrderId, dataGridViewProductsOfOrder.CurrentRow.Cells["ID"].Value.ToString()});
                }
                catch { MessageBox.Show(strDBError); }
                UpdateAll();
            }
        }
        //ContractChangesActions        
        private void dataGridViewContractChanges_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridViewContractChanges.Columns[e.ColumnIndex].Name == "Файл")
            {
                Program.fileDialog = FileDialogResults.open;
                Program.FileDialog(dataGridViewContractChanges.CurrentCell.Value.ToString());
            }
            UpdateAll();
        }
        private void toolStripMenuItemAddContractChanges_Click(object sender, EventArgs e)
        {
            EditForm addCC = new EditForm(@"select description as 'Описание изменения', 
                                linkToContractChangeFile as 'Файл'
                                from ContractChange
                                where contractChangeID = 0",
                                          new object[] { "", "" },
                                          null,
                                          new int[] { 0 },
                                          new int[] { 1 },
                                          @"insert into ContractChange 
                                (productionOrderID, contractChangeDate, description, linkToContractChangeFile) 
                                values (" + currentRowOrderId + ", GETDATE(), @description, @link)",
                                          new string[] { "@description", "@link" },
                                          new SqlDbType[] { SqlDbType.VarChar, SqlDbType.VarChar });
            addCC.ShowDialog();
            UpdateAll();
        }
        private void toolStripMenuItemEditContractChanges_Click(object sender, EventArgs e)
        {
            EditForm editCC = new EditForm(@"select description as 'Описание изменения', 
                                linkToContractChangeFile as 'Файл'
                                from ContractChange
                                where contractChangeID = " + dataGridViewContractChanges.CurrentRow.Cells["ID"].Value.ToString(),
                                          null,
                                          null,
                                          new int[] { 0 },
                                          new int[] { 1 },
                                          @"update ContractChange
                                        set description = @description,
                                        linkToContractChangeFile = @link
                                        where contractChangeID = " + dataGridViewContractChanges.CurrentRow.Cells["ID"].Value.ToString(),
                                          new string[] { "@description", "@link" },
                                          new SqlDbType[] { SqlDbType.VarChar, SqlDbType.VarChar });
            editCC.ShowDialog();
            UpdateAll();
        }
        private void toolStripMenuItemDeleteContractChange_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите удалить запись об изменениях из базы?", "Удаление записи", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    Program.SqlCommandExecute("delete", "delete from ContractChange where ContractChangeid =  @id",
                        new string[] { "@id" }, new SqlDbType[] { SqlDbType.Int },
                        new object[] { dataGridViewContractChanges.CurrentRow.Cells["ID"].Value });
                    UpdateAll();
                }
                catch { MessageBox.Show(strDBError); }
            }
        }
        #endregion
        #region Products
        int currentRowProductId = 0;
        #region selectStrings
        string strProductsSelect = @"select Product.productID as ID, productName as 'Наименование',linkToPrintFile as 'Файл',note as 'Примечание',
                                    (matP+opP) as 'Текущая стоимость', dur as 'Длительность производства (мин.)', copies as 'Количество произведённых экземпляров',
            dat as 'Последний заказ'
            from Product
            left join
            (   select Product.productID, sum(materialPrice* quantity) as matP
	            from Product
	            left join ProductMaterial on (Product.productID = ProductMaterial.productID)
	            left join
	            (   select Material.materialID, materialPrice
                    from (select materialID, max(dateOfMaterialPriceChange) as dat
                            from MaterialPriceChange 
                            group by materialID) as lastChange
                    left join Material on(Material.materialID = lastChange.materialID)
                    left join MaterialPriceChange on(Material.materialID = MaterialPriceChange.materialID 
                                                        and dat = MaterialPriceChange.dateOfMaterialPriceChange)
	            ) as materialPrices on (ProductMaterial.materialID = materialPrices.materialID)											
	            group by Product.productID	
            ) as MaterialPrices on (MaterialPrices.productID = Product.productID)
            left join 
            (	select Product.productID, sum(operationPrice) as opP, sum(ProductOperation.[duration (minutes)]) as dur
	            from Product
	            left join ProductOperation on (Product.productID = ProductOperation.productID)
	            left join
	            (   select Operation.operationID, operationPrice
		            from (  select operationID, max(dateOfOperationPriceChange) as dat
				            from OperationPriceChange 
				            group by operationID ) as lastChange
		            left join Operation on(Operation.operationID = lastChange.operationID)
		            left join OperationPriceChange on(Operation.operationID = OperationPriceChange.operationID 
														            and dat = OperationPriceChange.dateOfOperationPriceChange)
	            ) as operationPrices on (ProductOperation.operationID = OperationPrices.operationID)											
	            group by Product.productID	
            ) as OperationPrices on (OperationPrices.productID = Product.productID)										
            left join
            (   select productID, sum(numberOfCopiesOrdered) as copies, max(contractDate) as dat
	            from ProductInOrder
	            join ProductionOrder on (ProductInOrder.productionOrderID = ProductionOrder.productionOrderID)
	            group by productID
            ) as orders on (Product.productID = orders.productID)";
        string strOperationsOfProductSelect = @"select Operations.operationID as ID,productID, operationName as 'Наименование', 
                                              operationPrice as 'Стоимость', [duration (minutes)] as 'Длительность (мин.)'
                                        from ProductOperation join
                                        (select Operation.operationID, operationName, operationPrice
                                                        from(select operationID, max(dateOfOperationPriceChange) as dat
                                                                from OperationPriceChange
                                                                group by operationID) as lastChange
                                                        left join Operation on(Operation.operationID = lastChange.operationID)
                                                        left join OperationPriceChange on(Operation.operationID = OperationPriceChange.operationID
                                                                                            and dat = OperationPriceChange.dateOfOperationPriceChange))
                                        as Operations on ProductOperation.operationID = Operations.operationID";
        string strMaterialsOfProductSelect = @"select Materials.materialID as ID, productID, materialName as 'Наименование', materialPrice as 'Цена', quantity as 'Расход', (materialPrice*quantity) as 'Стоимость'
                                        from ProductMaterial join
                                        (select Material.materialID, materialName, materialPrice
                                                        from(select materialID, max(dateOfMaterialPriceChange) as dat
                                                                from MaterialPriceChange
                                                                group by materialID) as lastChange
                                                        left join Material on(Material.materialID = lastChange.materialID)
                                                        left join MaterialPriceChange on(Material.materialID = MaterialPriceChange.materialID
                                                                                            and dat = MaterialPriceChange.dateOfMaterialPriceChange))
                                        as Materials on ProductMaterial.materialID = Materials.materialID";
        #endregion
        public void UpdateProductTab()
        {
            Program.SetDataGridViewDataSource(strProductsSelect, ref dataGridViewProducts);
            dataGridViewProducts.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewProducts.Columns[4].DefaultCellStyle.Format = "F";

            Program.SetDataGridViewDataSource(strOperationsOfProductSelect, ref dataGridViewOperationsOfProduct);
            dataGridViewOperationsOfProduct.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewOperationsOfProduct.Columns[1].Visible = false;
            dataGridViewOperationsOfProduct.Columns[3].DefaultCellStyle.Format = "F";

            Program.SetDataGridViewDataSource(strMaterialsOfProductSelect, ref dataGridViewMaterialsOfProduct);
            dataGridViewMaterialsOfProduct.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewMaterialsOfProduct.Columns[1].Visible = false;
            dataGridViewMaterialsOfProduct.Columns[3].DefaultCellStyle.Format = "F";
            dataGridViewMaterialsOfProduct.Columns[5].DefaultCellStyle.Format = "F";

            FilterDataGridViewProducts(new Button(), null);
        }
        private void dataGridViewProducts_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                currentRowProductId = (int)dataGridViewProducts.CurrentRow.Cells[0].Value;
                Program.FilterDataGridView(String.Format("productID = {0}", currentRowProductId), ref dataGridViewOperationsOfProduct);
                Program.FilterDataGridView(String.Format("productID = {0}", currentRowProductId), ref dataGridViewMaterialsOfProduct);
                SetMenuProductsButtonsEnabeld();
            }
            catch { }
        }
        private void dataGridViewOperationsOfProduct_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            FilterDataGridViewOperations(new Button(), null);
            toolStripMenuItemOperations_Click(null, null);
            int idClickedOperationsOfProductRow;
            idClickedOperationsOfProductRow = Int32.Parse(dataGridViewOperationsOfProduct.CurrentRow.Cells["ID"].Value.ToString());
            for (int i = 0; i < dataGridViewOperations.RowCount; i++)
            {
                if (dataGridViewOperations.Rows[i].Cells["ID"].Value.ToString() == idClickedOperationsOfProductRow.ToString())
                {
                    dataGridViewOperations.CurrentCell = dataGridViewOperations.Rows[i].Cells["ID"];
                    return;
                }
            }
        }
        private void dataGridViewMaterialsOfProduct_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            FilterDataGridViewMaterials(new Button(), null);
            toolStripMenuItemMaterials_Click(null, null);
            int idClickedMaterialsOfProductRow;
            idClickedMaterialsOfProductRow = Int32.Parse(dataGridViewMaterialsOfProduct.CurrentRow.Cells["ID"].Value.ToString());
            for (int i = 0; i < dataGridViewMaterials.RowCount; i++)
            {
                if (dataGridViewMaterials.Rows[i].Cells["ID"].Value.ToString() == idClickedMaterialsOfProductRow.ToString())
                {
                    dataGridViewMaterials.CurrentCell = dataGridViewMaterials.Rows[i].Cells["ID"];
                    return;
                }
            }
            FilterDataGridViewMaterials(new Button(), null);
        }
        //filters
        public void SetProductsFiltersWidth(object sender, DataGridViewColumnEventArgs e)
        {
            try
            {
                for (int i = 0; i < dataGridViewProducts.ColumnCount; i++)
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
                                        [Примечание] like '%{2}%' and
                                        [Текущая стоимость] {3} {4} and 
                                        [Длительность производства (мин.)] {5} {6} and 
                                        [Количество произведённых экземпляров] {7} {8} and
                                        [Последний заказ] {9} #{10}#",
                                      tbP1.Text,
                                      tbP2.Text,
                                      tbP3.Text,
                                      lP4.Text, nudP4.Value.ToString(),
                                      lP5.Text, nudP5.Value.ToString(),
                                      lP6.Text, nudP6.Value.ToString(),
                                      lP7.Text, dtpP7.Value.ToString("dd-MM-yyyy"));
            }
            try
            {
                Program.FilterDataGridView(filterText,
                    ref dataGridViewProducts);
                SetMenuProductsButtonsEnabeld();
            }
            catch { }
        }
        //menus prod
        public void SetMenuProductsButtonsEnabeld()
        {
            if (dataGridViewProducts.CurrentRow == null)
            {
                toolStripMenuItemPrintProducts.Enabled = false;
                toolStripMenuItemCopyProduct.Enabled = false;
                toolStripMenuItemDeleteProduct.Enabled = false;
                toolStripMenuItemAddOperationToProduct.Enabled = false;
                toolStripMenuItemAddMaterialToProduct.Enabled = false;
                toolStripMenuItemDeleteOperationFromProduct.Enabled = false;
                toolStripMenuItemDeleteMaterialFromProduct.Enabled = false;
            }
            else if ((dataGridViewProducts.CurrentRow.Cells["Количество произведённых экземпляров"].Value.ToString() != "0")&&
                (dataGridViewProducts.CurrentRow.Cells["Количество произведённых экземпляров"].Value.ToString() != ""))
            {
                toolStripMenuItemPrintProducts.Enabled = true;
                toolStripMenuItemCopyProduct.Enabled = true;
                toolStripMenuItemDeleteProduct.Enabled = false;
                toolStripMenuItemAddOperationToProduct.Enabled = false;
                toolStripMenuItemAddMaterialToProduct.Enabled = false;
                toolStripMenuItemDeleteOperationFromProduct.Enabled = false;
                toolStripMenuItemDeleteMaterialFromProduct.Enabled = false;
            }
            else
            {
                toolStripMenuItemPrintProducts.Enabled = true;
                toolStripMenuItemCopyProduct.Enabled = true;
                toolStripMenuItemDeleteProduct.Enabled = true;
                toolStripMenuItemAddOperationToProduct.Enabled = true;
                toolStripMenuItemAddMaterialToProduct.Enabled = true;
                if (dataGridViewOperationsOfProduct.CurrentRow != null)
                { toolStripMenuItemDeleteOperationFromProduct.Enabled = true; }
                else
                {  toolStripMenuItemDeleteOperationFromProduct.Enabled = false; }
                if (dataGridViewMaterialsOfProduct.CurrentRow != null)
                { toolStripMenuItemDeleteMaterialFromProduct.Enabled = true; }
                else
                { toolStripMenuItemDeleteMaterialFromProduct.Enabled = false; }
            }
        }
        private void toolStripMenuItemPrintProducts_Click(object sender, EventArgs e)
        {
            DGVPrinter printer = Program.DefaultPrinter();
            printer.Title = "Продукты";
            printer.Footer = "Отдел заказов";
            printer.PrintPreviewDataGridView(dataGridViewProducts);
        }
        private void toolStripMenuItemAddProduct_Click(object sender, EventArgs e)
        {
            EditForm addProduct = new EditForm(@"select productName as 'Наименование',linkToPrintFile as 'Файл',
                    note as 'Примечание'
                    from Product where productID = 0",
                                null,
                                null,
                                new int[] { 0, 1},
                                new int[] { 1 },
                                @"insert into Product
                    (productName, linkToPrintFile, note) 
                    values (@name, @file, @note)",
                                new string[] { "@name", "file", "note" },
                                new SqlDbType[] { SqlDbType.VarChar, SqlDbType.VarChar, SqlDbType.VarChar});
            addProduct.ShowDialog();
            UpdateAll();
        }
        private void toolStripMenuItemCopyProduct_Click(object sender, EventArgs e)
        {
            EditForm addProduct = new EditForm(@"select productName as 'Наименование',linkToPrintFile as 'Файл',
                    note as 'Примечание'
                    from Product where productID = 0",
                                null,
                                null,
                                new int[] { 0, 1},
                                new int[] { 1 },
                                @"insert into Product
                    (productName, linkToPrintFile, note) 
                    values (@name, @file, @note)
                    insert into ProductOperation
                    select (select max(productID) from Product), operationID, [duration (minutes)] 
                    from ProductOperation 
                    where productID = "+ currentRowProductId + @"
                    insert into ProductMaterial
                    select (select max(productID) from Product), materialID, quantity 
                    from ProductMaterial 
                    where productID = " + currentRowProductId,
                                new string[] { "@name", "file", "note" },
                                new SqlDbType[] { SqlDbType.VarChar, SqlDbType.VarChar, SqlDbType.VarChar });
            addProduct.ShowDialog();
            UpdateAll();
        }
        private void toolStripMenuItemDeleteProduct_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите удалить продукт из базы?", "Удаление записи", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    Program.SqlCommandExecute("delete",
                        @"delete from 
                    ProductMaterial where productID =  @id
                    delete from 
                    ProductOperation where productID =  @id
                    delete from 
                    Product where productID =  @id",
                        new string[] { "@id" }, new SqlDbType[] { SqlDbType.Int },
                        new object[] { currentRowProductId });
                }
                catch { MessageBox.Show(strDBError); }
                UpdateAll();
            }
        }
        //menus op
        private void toolStripMenuItemAddOperationToProduct_Click(object sender, EventArgs e)
        {
            EditForm addOperationToProduct = new EditForm(@"select null as 'Длительность (мин.)' where 1=2",
                                          null,
                                          @"select Operation.operationID as ID, operationName as 'Наименование', operationPrice as 'Текущая цена',
                                     ISNULL (durationAvg,0) as 'Средняя длительность выполнения (мин.)', timesSum as 'Количество выполнений', moneySum as 'Выручка'
            from (  select operationID, max(dateOfOperationPriceChange) as dat
                    from OperationPriceChange 
                    group by operationID) as lastChange
            left join Operation on(Operation.operationID = lastChange.operationID)
            left join OperationPriceChange on(Operation.operationID = OperationPriceChange.operationID and dat = OperationPriceChange.dateOfOperationPriceChange)
			left join 
			(   select operationID, avg([duration (minutes)]) as durationAvg
				from ProductOperation
				group by operationID
			) as avgDurationPerProduct
			on (Operation.operationID = avgDurationPerProduct.operationID)			
			left join 
			(   select executions.operationID, ISNULL (sum(times),0) as timesSum, ISNULL (sum (times* price),0) as moneySum
				from
				(   select Operation.operationID, p.productionOrderID, sum(numberOfCopiesMade) as times
					from Operation 
					left join ProductOperation on (Operation.operationID = ProductOperation.operationID)
					left join 
                    (   select ProductInOrder.productionOrderID, productID, numberOfCopiesMade
						from ProductInOrder
						left join ProductionOrder on (ProductionOrder.productionOrderID = ProductInOrder.productionOrderID)
					)as p  on (ProductOperation.productID = p.productID)
					group by Operation.operationID, p.productionOrderID
				) as executions
				left join
				(   select productionOrderID, rightPricesDates.operationID, ISNULL (operationPrice,0) price  
                    from
					(   select ProductionOrder.productionOrderID, ProductOperation.operationID, max(dateOfOperationPriceChange) as datP
						from ProductionOrder
						left join ProductInOrder on (ProductionOrder.productionOrderID = ProductInOrder.productionOrderID)
						left join ProductOperation on (ProductInOrder.productID = ProductOperation.productID)
						left join OperationPriceChange on (ProductOperation.operationID = OperationPriceChange.operationID)
						where dateOfOperationPriceChange <= contractDate
						group by ProductionOrder.productionOrderID, ProductOperation.operationID
					) as rightPricesDates left join OperationPriceChange on (rightPricesDates.operationID = OperationPriceChange.operationID 
                                                                         and rightPricesDates.datP = OperationPriceChange.dateOfOperationPriceChange)
				) as prices on (executions.operationID = prices.operationID	and executions.productionOrderID = prices.productionOrderID)
				group by executions.operationID
			) as indicators on (Operation.operationID = indicators.operationID)",
                                          null,
                                          null,
                                          @"insert into ProductOperation 
            (productID, operationID, [duration (minutes)])
            values (" + currentRowProductId + ",@opID, @dur)",
                                          new string[] { "@dur", "@opID" },
                                          new SqlDbType[] { SqlDbType.Int, SqlDbType.Int });
            addOperationToProduct.ShowDialog();
            UpdateAll();
        }
        private void toolStripMenuItemDeleteOperationFromProduct_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите отменить операцию над продуктом?", "Удаление записи", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    Program.SqlCommandExecute("delete",
                        @"delete from 
                    ProductOperation where productID =  @product
                    and operationID =  @operation",
                        new string[] {"@product",  "@operation" }, new SqlDbType[] { SqlDbType.Int, SqlDbType.Int },
                        new object[] { currentRowProductId, dataGridViewOperationsOfProduct.CurrentRow.Cells["ID"].Value.ToString() });
                }
                catch { MessageBox.Show(strDBError); }
                UpdateAll();
            }
        }
        //menus mat
        private void toolStripMenuItemAddMaterialToProduct_Click(object sender, EventArgs e)
        {
            EditForm addMaterialToProduct = new EditForm(@"select quantity as 'Расходуемое количество' 
                                        from ProductMaterial
                                        where 1=2",
                                          null,
                                          @"select Material.materialID as ID, materialName as 'Наименование', materialPrice as 'Текущая цена',
                                    ISNULL (qAvg,0) as 'Средний расход на продукт', qcSum as 'Общий расход', qcp as 'Выручка', balance as 'Доступный остаток'
            from (select materialID, max(dateOfMaterialPriceChange) as dat
                    from MaterialPriceChange 
                    group by materialID) as lastChange
            left join Material on(Material.materialID = lastChange.materialID)
            left join MaterialPriceChange on(Material.materialID = MaterialPriceChange.materialID and dat = MaterialPriceChange.dateOfMaterialPriceChange)
			left join 
			(   select materialID, avg(quantity) as qAvg
				from ProductMaterial
				group by materialID
			) as avgConsumptionPerProduct on (Material.materialID = avgConsumptionPerProduct.materialID)
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
				    where transferType = 0
				    group by materialID
				) as consumption on (arrival.materialID = consumption.materialID)
				group by arrival.materialID, a, b
			) as stokBalancee on (Material.materialID = stokBalancee.materialID)
			left join 
			(   select consumption.materialID, sum(qc) as qcSum, ISNULL (sum (qc* price),0) as qcp
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
				left join
				(   select productionOrderID, rightPricesDates.materialID, ISNULL (materialPrice,0) price  
                    from
					(   select ProductionOrder.productionOrderID, ProductMaterial.materialID, max(dateOfMaterialPriceChange) as datP
						from ProductionOrder
						left join ProductInOrder on (ProductionOrder.productionOrderID = ProductInOrder.productionOrderID)
						left join ProductMaterial on (ProductInOrder.productID = ProductMaterial.productID)
						left join MaterialPriceChange on (ProductMaterial.materialID = MaterialPriceChange.materialID)
						where dateOfMaterialPriceChange <= contractDate
						group by ProductionOrder.productionOrderID, ProductMaterial.materialID
					) as rightPricesDates left join MaterialPriceChange on (rightPricesDates.materialID = MaterialPriceChange.materialID
					                                                    and rightPricesDates.datP = MaterialPriceChange.dateOfMaterialPriceChange)
				) as prices on (consumption.materialID = prices.materialID and consumption.productionOrderID = prices.productionOrderID)
				group by consumption.materialID
			) as indicators on (Material.materialID = indicators.materialID)",
                                          null,
                                          null,
                                          @"insert into ProductMaterial 
            (productID, materialID, quantity)
            values (" + currentRowProductId + ",@mID, @q)",
                                          new string[] { "@q", "@mID" },
                                          new SqlDbType[] { SqlDbType.Float, SqlDbType.Int });
            addMaterialToProduct.ShowDialog();
            UpdateAll();
        }
        private void toolStripMenuItemDeleteMaterialFromProduct_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите удалить материал продукта?", "Удаление записи", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    Program.SqlCommandExecute("delete",
                        @"delete from 
                    ProductMaterial where productID =  @product
                    and materialID =  @material",
                        new string[] { "@product", "@material" }, new SqlDbType[] { SqlDbType.Int, SqlDbType.Int },
                        new object[] { currentRowProductId, dataGridViewMaterialsOfProduct.CurrentRow.Cells["ID"].Value.ToString() });
                }
                catch { MessageBox.Show(strDBError); }
                UpdateAll();
            }
        }
        #endregion
        #region Operations
        int currentRowOperationId = 0;
        #region selectStrings
        string strOperationsSelect = @"select Operation.operationID as ID, operationName as 'Наименование', operationPrice as 'Текущая цена',
                                     ISNULL (durationAvg,0) as 'Средняя длительность выполнения (мин.)', timesSum as 'Количество выполнений', moneySum as 'Выручка'
            from (  select operationID, max(dateOfOperationPriceChange) as dat
                    from OperationPriceChange 
                    group by operationID) as lastChange
            left join Operation on(Operation.operationID = lastChange.operationID)
            left join OperationPriceChange on(Operation.operationID = OperationPriceChange.operationID and dat = OperationPriceChange.dateOfOperationPriceChange)
			left join 
			(   select operationID, avg([duration (minutes)]) as durationAvg
				from ProductOperation
				group by operationID
			) as avgDurationPerProduct
			on (Operation.operationID = avgDurationPerProduct.operationID)			
			left join 
			(   select executions.operationID, ISNULL (sum(times),0) as timesSum, ISNULL (sum (times* price),0) as moneySum
				from
				(   select Operation.operationID, p.productionOrderID, sum(numberOfCopiesMade) as times
					from Operation 
					left join ProductOperation on (Operation.operationID = ProductOperation.operationID)
					left join 
                    (   select ProductInOrder.productionOrderID, productID, numberOfCopiesMade
						from ProductInOrder
						left join ProductionOrder on (ProductionOrder.productionOrderID = ProductInOrder.productionOrderID)
					)as p  on (ProductOperation.productID = p.productID)
					group by Operation.operationID, p.productionOrderID
				) as executions
				left join
				(   select productionOrderID, rightPricesDates.operationID, ISNULL (operationPrice,0) price  
                    from
					(   select ProductionOrder.productionOrderID, ProductOperation.operationID, max(dateOfOperationPriceChange) as datP
						from ProductionOrder
						left join ProductInOrder on (ProductionOrder.productionOrderID = ProductInOrder.productionOrderID)
						left join ProductOperation on (ProductInOrder.productID = ProductOperation.productID)
						left join OperationPriceChange on (ProductOperation.operationID = OperationPriceChange.operationID)
						where dateOfOperationPriceChange <= contractDate
						group by ProductionOrder.productionOrderID, ProductOperation.operationID
					) as rightPricesDates left join OperationPriceChange on (rightPricesDates.operationID = OperationPriceChange.operationID 
                                                                         and rightPricesDates.datP = OperationPriceChange.dateOfOperationPriceChange)
				) as prices on (executions.operationID = prices.operationID	and executions.productionOrderID = prices.productionOrderID)
				group by executions.operationID
			) as indicators on (Operation.operationID = indicators.operationID)";
        string strPricesOfOperationSelect = @"select operationPriceChangeID as 'ID', operationID, operationPrice as 'Назначенная цена', dateOfOperationPriceChange as 'Дата изменения'
                                            from OperationPriceChange
											order by 'Дата изменения' desc";
        #endregion
        public void UpdateOperationTab()
        {
            Program.SetDataGridViewDataSource(strOperationsSelect, ref dataGridViewOperations);
            dataGridViewOperations.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewOperations.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewOperations.Columns[2].DefaultCellStyle.Format = "F";
            dataGridViewOperations.Columns[5].DefaultCellStyle.Format = "F";
            Program.SetDataGridViewDataSource(strPricesOfOperationSelect, ref dataGridViewPricesOfOperation);
            dataGridViewPricesOfOperation.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewPricesOfOperation.Columns[1].Visible = false;
            dataGridViewPricesOfOperation.Columns[2].DefaultCellStyle.Format = "F";
            FilterDataGridViewOperations(new Button(), null);
        }
        private void dataGridViewOperations_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                currentRowOperationId = (int)dataGridViewOperations.CurrentRow.Cells[0].Value;
                Program.FilterDataGridView(String.Format("OperationID = {0}", currentRowOperationId), ref dataGridViewPricesOfOperation);
            }
            catch { }
        }
        //filters
        public void SetOperationsFiltersWidth(object sender, DataGridViewColumnEventArgs e)
        {
            try
            {
                for (int i = 0; i < dataGridViewOperations.ColumnCount - 1; i++)
                {
                    tableLayoutPanelOperationsFilters.ColumnStyles[i].Width = dataGridViewOperations.Columns[i].Width;
                }
            }
            catch { }
        }
        public void FilterDataGridViewOperations(object sender, EventArgs e)
        {
            string filterText = "";
            try { object a = ((Button)sender); }
            catch
            {
                filterText = String.Format(@"[Наименование] like '%{0}%' and
                                                [Текущая цена] {1} {2} and 
                                                [Средняя длительность выполнения (мин.)] {3} {4} and 
                                                [Количество выполнений] {5} {6} and  
                                                [Выручка] {7} {8}",
                                      tbOp.Text,
                                      lOp1.Text, nudOp1.Value.ToString(),
                                      lOp2.Text, nudOp2.Value.ToString(),
                                      lOp3.Text, nudOp3.Value.ToString(),
                                      lOp4.Text, nudOp4.Value.ToString());
            }
            try
            {
                Program.FilterDataGridView(filterText,
                    ref dataGridViewOperations);
                SetMenuOperationsButtonsEnabeld();
            }
            catch { }
        }
        //menus
        public void SetMenuOperationsButtonsEnabeld()
        {
            if (dataGridViewOperations.CurrentRow == null)
            {
                toolStripMenuItemPrintOperations.Enabled = false;
                toolStripMenuItemEditOperationPrice.Enabled = false;
            }
            else
            {
                toolStripMenuItemPrintOperations.Enabled = true;
                toolStripMenuItemEditOperationPrice.Enabled = true;
            }
        }
        private void toolStripMenuItemPrintOperations_Click(object sender, EventArgs e)
        {
            DGVPrinter printer = Program.DefaultPrinter();
            printer.Title = "Операции";
            printer.Footer = "Отдел заказов";
            printer.PrintPreviewDataGridView(dataGridViewOperations);
        }
        private void toolStripMenuItemEditOperationPrice_Click(object sender, EventArgs e)
        {
            EditForm addOperationPrice = new EditForm(@"select OperationPrice as 'Текущая цена'
                    from OperationPriceChange where operationID = 0",
                                new object[] { Decimal.Parse(dataGridViewOperations.CurrentRow.Cells["Текущая цена"].Value.ToString())+5 },
                                null,
                                new int[] { 0 },
                                null,
                                @"insert into OperationPriceChange
                    (operationID, dateOfOperationPriceChange, operationPrice) 
                    values ("+currentRowOperationId+",  GETDATE(), @price)",
                                new string[] {"price" },
                                new SqlDbType[] { SqlDbType.Money });
            addOperationPrice.ShowDialog();
            UpdateAll();
        }
        #endregion
        #region Materials
        int currentRowMaterialId = 0;
        #region select strings
        string strMaterialsSelect = @"select Material.materialID as ID, materialName as 'Наименование', materialPrice as 'Текущая цена',
                                    ISNULL (qAvg,0) as 'Средний расход на продукт', qcSum as 'Общий расход', qcp as 'Выручка', balance as 'Доступный остаток'
            from (select materialID, max(dateOfMaterialPriceChange) as dat
                    from MaterialPriceChange 
                    group by materialID) as lastChange
            left join Material on(Material.materialID = lastChange.materialID)
            left join MaterialPriceChange on(Material.materialID = MaterialPriceChange.materialID and dat = MaterialPriceChange.dateOfMaterialPriceChange)
			left join 
			(   select materialID, avg(quantity) as qAvg
				from ProductMaterial
				group by materialID
			) as avgConsumptionPerProduct on (Material.materialID = avgConsumptionPerProduct.materialID)
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
				    where transferType = 0
				    group by materialID
				) as consumption on (arrival.materialID = consumption.materialID)
				group by arrival.materialID, a, b
			) as stokBalancee on (Material.materialID = stokBalancee.materialID)
			left join 
			(   select consumption.materialID, sum(qc) as qcSum, ISNULL (sum (qc* price),0) as qcp
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
				left join
				(   select productionOrderID, rightPricesDates.materialID, ISNULL (materialPrice,0) price  
                    from
					(   select ProductionOrder.productionOrderID, ProductMaterial.materialID, max(dateOfMaterialPriceChange) as datP
						from ProductionOrder
						left join ProductInOrder on (ProductionOrder.productionOrderID = ProductInOrder.productionOrderID)
						left join ProductMaterial on (ProductInOrder.productID = ProductMaterial.productID)
						left join MaterialPriceChange on (ProductMaterial.materialID = MaterialPriceChange.materialID)
						where dateOfMaterialPriceChange <= contractDate
						group by ProductionOrder.productionOrderID, ProductMaterial.materialID
					) as rightPricesDates left join MaterialPriceChange on (rightPricesDates.materialID = MaterialPriceChange.materialID
					                                                    and rightPricesDates.datP = MaterialPriceChange.dateOfMaterialPriceChange)
				) as prices on (consumption.materialID = prices.materialID and consumption.productionOrderID = prices.productionOrderID)
				group by consumption.materialID
			) as indicators on (Material.materialID = indicators.materialID)";
        string strPricesOfMaterialSelect = @"select materialPriceChangeID as 'ID', materialID, 
                    materialPrice as 'Назначенная цена', dateOfMaterialPriceChange as 'Дата изменения'
                                            from MaterialPriceChange
											order by 'Дата изменения' desc";
        #endregion
        public void UpdateMaterialTab()
        {
            Program.SetDataGridViewDataSource(strMaterialsSelect, ref dataGridViewMaterials);
            dataGridViewMaterials.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewMaterials.Columns[2].DefaultCellStyle.Format = "F";
            dataGridViewMaterials.Columns[3].DefaultCellStyle.Format = "F";
            dataGridViewMaterials.Columns[4].DefaultCellStyle.Format = "F";
            dataGridViewMaterials.Columns[5].DefaultCellStyle.Format = "F";

            Program.SetDataGridViewDataSource(strPricesOfMaterialSelect, ref dataGridViewPricesOfMaterial);
            dataGridViewPricesOfMaterial.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewPricesOfMaterial.Columns[1].Visible = false;
            dataGridViewPricesOfMaterial.Columns[2].DefaultCellStyle.Format = "F";

            FilterDataGridViewMaterials(new Button(), null);
        }
        private void dataGridViewMaterials_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                currentRowMaterialId = (int)dataGridViewMaterials.CurrentRow.Cells[0].Value;
                Program.FilterDataGridView(String.Format("MaterialID = {0}", currentRowMaterialId), ref dataGridViewPricesOfMaterial);
            }
            catch { }
        }
        //
        public void FilterDataGridViewMaterials(object sender, EventArgs e)
        {
            string filterText = "";
            try { object a = ((Button)sender); }
            catch
            {
                filterText = String.Format(@"[Наименование] like '%{0}%' and
                                                [Текущая цена] {1} {2} and 
                                                [Средний расход на продукт] {3} {4} and 
                                                [Общий расход] {5} {6} and  
                                                [Выручка] {7} {8} and  
                                                [Доступный остаток] {9} {10}",
                                      tbM1.Text,
                                      lM2.Text, nudM2.Value.ToString(),
                                      lM3.Text, nudM3.Value.ToString(),
                                      lM4.Text, nudM4.Value.ToString(),
                                      lM5.Text, nudM5.Value.ToString(),
                                      lM6.Text, nudM6.Value.ToString());
            }
            try
            {
                Program.FilterDataGridView(filterText,
                    ref dataGridViewMaterials);
                //set enableds
                if (dataGridViewMaterials.CurrentRow == null)
                { toolStripMenuItemPrintMaterials.Enabled = false; }
                else
                { toolStripMenuItemPrintMaterials.Enabled = true; }
            }
            catch { }
        }
        public void SetMaterialsFiltersWidth(object sender, DataGridViewColumnEventArgs e)
        {
            try
            {
                for (int i = 0; i < dataGridViewMaterials.ColumnCount - 1; i++)
                {
                    tableLayoutPanelMaterialsFilters.ColumnStyles[i].Width = dataGridViewMaterials.Columns[i].Width;
                }
            }
            catch { }
        }
        //menus
        private void toolStripMenuItemPrintMaterials_Click(object sender, EventArgs e)
        {
            DGVPrinter printer = Program.DefaultPrinter();
            printer.Title = "Материалы";
            printer.Footer = "Отдел заказов";
            printer.PrintPreviewDataGridView(dataGridViewMaterials);
        }
        #endregion
        #region MenuClics
        private void toolStripMenuItemClients_Click(object sender, EventArgs e)
        {
            if (tabControlSIWWC.SelectedTab != tabPageClients)
            { tabControlSIWWC.SelectedTab = tabPageClients; }
        }
        private void toolStripMenuItemOrders_Click(object sender, EventArgs e)
        {
            if (tabControlSIWWC.SelectedTab != tabPageOrders)
            { tabControlSIWWC.SelectedTab = tabPageOrders; }
        }
        private void toolStripMenuItemProducts_Click(object sender, EventArgs e)
        {
            if (tabControlSIWWC.SelectedTab != tabPageProducts)
            { tabControlSIWWC.SelectedTab = tabPageProducts; }
        }
        private void toolStripMenuItemOperations_Click(object sender, EventArgs e)
        {
            if (tabControlSIWWC.SelectedTab != tabPageOperations)
            { tabControlSIWWC.SelectedTab = tabPageOperations; }
        }
        private void toolStripMenuItemMaterials_Click(object sender, EventArgs e)
        {
            if (tabControlSIWWC.SelectedTab != tabPageMaterials)
            { tabControlSIWWC.SelectedTab = tabPageMaterials; }
        }











        #endregion
        public void FilterLabelClick(object sender, EventArgs e)
        {
            Program.FilterLabelClick(sender, e);
        }
    }
}