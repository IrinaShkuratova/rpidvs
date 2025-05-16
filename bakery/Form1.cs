using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;

namespace bakery
{
    public partial class Form1 : Form
    {
        private string connString = "Host=127.0.0.1;Username=postgres;Password=1234;Database=ira2";


        public Form1()
        {
            InitializeComponent();
            InitializeDataTabs();
            LoadSuppliers();
            LoadBakeryItems();
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            try
            {
                using (var con = GetConnection())
                {
                    // Очищаем предыдущие данные в графике
                    chart1.Series.Clear();

                    // 1. Статистика по статусам заказов
                    string sql = @"SELECT 
                    CASE 
                        WHEN completed THEN 'Завершенные'
                        ELSE 'Активные'
                    END as status,
                    COUNT(*) as count
                   FROM production_orders
                   GROUP BY completed";

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, con);
                    NpgsqlDataReader dr = cmd.ExecuteReader();

                    // Добавляем серию для статусов заказов
                    var series1 = new System.Windows.Forms.DataVisualization.Charting.Series("Orders");
                    series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
                    chart1.Series.Add(series1);

                    while (dr.Read())
                    {
                        series1.Points.AddXY(dr["status"].ToString(), dr["count"]);
                    }
                    dr.Close();

                    // 2. Статистика по популярным продуктам
                    //sql = @"SELECT p.product_name, SUM(r.quantity * poi.quantity) as total_used
                    //       FROM production_order_items poi
                    //       JOIN production_orders po ON poi.order_id = po.order_id
                    //       JOIN recipes r ON poi.item_id = r.item_id
                    //       JOIN products p ON r.product_id = p.product_id
                    //       GROUP BY p.product_name
                    //       ORDER BY total_used DESC
                    //       LIMIT 5";

                    //cmd = new NpgsqlCommand(sql, con);
                    //dr = cmd.ExecuteReader();

                    //// Добавляем серию для популярных продуктов
                    //var series2 = new System.Windows.Forms.DataVisualization.Charting.Series("Products");
                    //series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Bar;
                    //chart1.Series.Add(series2);

                    //while (dr.Read())
                    //{
                    //    series2.Points.AddXY(dr["product_name"].ToString(), dr["total_used"]);
                    //}
                    //dr.Close();

                    //// Настройка внешнего вида графика
                    //chart1.Titles.Clear();
                    //chart1.Titles.Add("Статистика производства");
                    //chart1.ChartAreas[0].AxisX.Title = "Категории";
                    //chart1.ChartAreas[0].AxisY.Title = "Количество";
                    //chart1.Legends[0].Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки статистики: " + ex.Message);
            }
        }

        private NpgsqlConnection GetConnection()
        {
            var con = new NpgsqlConnection(connString);
            con.Open();
            return con;
        }

        private void InitializeDataTabs()
        {
            // Основные таблицы для редактирования
            TabPage suppliersTab = new TabPage("Поставщики");
            TabPage productsTab = new TabPage("Продукты");
            TabPage bakeryItemsTab = new TabPage("Виды продукции");
            TabPage recipesTab = new TabPage("Рецепты");
            TabPage invoicesTab = new TabPage("Накладные");
            // TabPage ordersTab = new TabPage("Заказы");

            // Инициализируем все вкладки с редактированием
            InitializeEditableTab(suppliersTab, "suppliers", new[] { "supplier_id", "supplier_name", "contact_info" });
            InitializeEditableTab(productsTab, "products", new[] { "product_id", "product_name", "unit_of_measure", "current_quantity" });
            InitializeEditableTab(bakeryItemsTab, "bakery_items", new[] { "item_id", "item_name", "description" });
            InitializeEditableTab(recipesTab, "recipes", new[] { "recipe_id", "item_id", "product_id", "quantity" });
            InitializeEditableTab(invoicesTab, "incoming_invoices", new[] { "invoice_id", "supplier_id", "invoice_date", "invoice_number" });
            // InitializeEditableTab(ordersTab, "production_orders", new[] { "order_id", "order_date", "completed" });

            // Добавляем вкладки в TabControl
            tabControl1.TabPages.Add(suppliersTab);
            tabControl1.TabPages.Add(productsTab);
            tabControl1.TabPages.Add(bakeryItemsTab);
            tabControl1.TabPages.Add(recipesTab);
            tabControl1.TabPages.Add(invoicesTab);
            // tabControl1.TabPages.Add(ordersTab);
        }

        private void InitializeEditableTab(TabPage tab, string tableName, string[] columns)
        {
            DataGridView dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Name = tableName + "_grid"
            };

            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50
            };

            Button addButton = new Button { Text = "Добавить", Dock = DockStyle.Left, Width = 100 };
            Button editButton = new Button { Text = "Изменить", Dock = DockStyle.Left, Width = 100 };
            Button deleteButton = new Button { Text = "Удалить", Dock = DockStyle.Left, Width = 100 };
            Button refreshButton = new Button { Text = "Обновить", Dock = DockStyle.Right, Width = 100 };

            addButton.Click += (sender, e) => ShowAddEditForm(tableName, null, dataGridView);
            editButton.Click += (sender, e) =>
            {
                if (dataGridView.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Выберите строку для редактирования");
                    return;
                }
                var selectedId = dataGridView.SelectedRows[0].Cells[0].Value;
                if (selectedId != null)
                {
                    ShowAddEditForm(tableName, selectedId, dataGridView);
                }
            };
            deleteButton.Click += (sender, e) =>
            {
                if (dataGridView.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Выберите строку для удаления");
                    return;
                }
                var selectedId = dataGridView.SelectedRows[0].Cells[0].Value;
                if (selectedId != null)
                {
                    if (MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Подтверждение",
                        MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        DeleteRecord(tableName, selectedId, dataGridView);
                    }
                }
            };
            refreshButton.Click += (sender, e) => LoadData(tableName, dataGridView);

            buttonPanel.Controls.Add(deleteButton);
            buttonPanel.Controls.Add(editButton);
            buttonPanel.Controls.Add(addButton);
            buttonPanel.Controls.Add(refreshButton);

            tab.Controls.Add(dataGridView);
            tab.Controls.Add(buttonPanel);

            LoadData(tableName, dataGridView);
        }

        private void LoadData(string tableName, DataGridView dataGridView)
        {
            try
            {
                using (var con = GetConnection())
                {
                    string sql = $"SELECT * FROM {tableName}";

                    // Для таблиц с внешними ключами добавляем JOIN для удобства просмотра
                    if (tableName == "recipes")
                    {
                        sql = @"SELECT r.recipe_id, b.item_name, p.product_name, r.quantity 
                               FROM recipes r
                               JOIN bakery_items b ON r.item_id = b.item_id
                               JOIN products p ON r.product_id = p.product_id";
                    }
                    else if (tableName == "incoming_invoices")
                    {
                        sql = @"SELECT i.invoice_id, s.supplier_name, i.invoice_date, i.invoice_number 
                               FROM incoming_invoices i
                               JOIN suppliers s ON i.supplier_id = s.supplier_id";
                    }
                    else if (tableName == "production_orders")
                    {
                        sql = @"SELECT order_id, order_date, completed,
                              CASE WHEN completed THEN 'Да' ELSE 'Нет' END as completed_text
                              FROM production_orders";
                    }

                    NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView.DataSource = dt;

                    // Скрываем технические колонки
                    foreach (DataGridViewColumn column in dataGridView.Columns)
                    {
                        if (column.Name.EndsWith("_id") || column.Name == "completed")
                        {
                            column.Visible = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных из таблицы {tableName}: {ex.Message}");
            }
        }

        private void ShowAddEditForm(string tableName, object id, DataGridView dataGridView)
        {
            Form editForm = new Form
            {
                Text = id == null ? "Добавить запись" : "Изменить запись",
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false,
                Width = 400,
                Height = 300
            };

            try
            {
                using (var con = GetConnection())
                {
                    string sql = id == null ?
                        $"SELECT * FROM {tableName} WHERE 1=0" :
                        $"SELECT * FROM {tableName} WHERE {GetIdColumn(tableName)} = @id";

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, con);
                    if (id != null) cmd.Parameters.AddWithValue("@id", id);

                    NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    TableLayoutPanel tableLayout = new TableLayoutPanel
                    {
                        Dock = DockStyle.Fill,
                        ColumnCount = 2,
                        RowCount = dt.Columns.Count + 1,
                        AutoScroll = true
                    };

                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        if (dt.Columns[i].ColumnName == GetIdColumn(tableName)) continue;

                        tableLayout.Controls.Add(new Label
                        {
                            Text = GetDisplayName(dt.Columns[i].ColumnName),
                            Dock = DockStyle.Fill,
                            TextAlign = System.Drawing.ContentAlignment.MiddleLeft
                        }, 0, i);

                        Control inputControl = CreateInputControl(dt.Columns[i].DataType, dt.Columns[i].ColumnName, con);

                        if (id != null && dt.Rows.Count > 0 && dt.Rows[0][i] != DBNull.Value)
                        {
                            SetControlValue(inputControl, dt.Rows[0][i]);
                        }

                        tableLayout.Controls.Add(inputControl, 1, i);
                    }

                    Button saveButton = new Button { Text = "Сохранить", Dock = DockStyle.Fill };
                    saveButton.Click += (sender, e) =>
                    {
                        SaveRecord(tableName, id, tableLayout, editForm, dataGridView);
                    };

                    tableLayout.Controls.Add(saveButton, 0, dt.Columns.Count);
                    tableLayout.SetColumnSpan(saveButton, 2);

                    editForm.Controls.Add(tableLayout);
                    editForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии формы редактирования: {ex.Message}");
            }
        }

        private string GetDisplayName(string columnName)
        {
            switch (columnName)
            {
                case "supplier_name": return "Название поставщика";
                case "contact_info": return "Контактная информация";
                case "product_name": return "Название продукта";
                case "unit_of_measure": return "Единица измерения";
                case "current_quantity": return "Текущее количество";
                case "item_name": return "Название продукции";
                case "description": return "Описание";
                case "item_id": return "Вид продукции";
                case "product_id": return "Продукт";
                case "quantity": return "Количество";
                case "supplier_id": return "Поставщик";
                case "invoice_date": return "Дата накладной";
                case "invoice_number": return "Номер накладной";
                case "order_date": return "Дата заказа";
                case "completed": return "Завершено";
                default: return columnName;
            }
        }

        private Control CreateInputControl(Type dataType, string columnName, NpgsqlConnection con)
        {
            if (columnName.EndsWith("_id"))
            {
                string relatedTable = columnName.StartsWith("item_") ? "bakery_items" :
                                   columnName.StartsWith("product_") ? "products" :
                                   columnName.StartsWith("supplier_") ? "suppliers" :
                                   columnName.Split('_')[0] + "s"; // общий случай
                ComboBox comboBox = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };

                string sql = $"SELECT {GetIdColumn(relatedTable)}, {GetNameColumn(relatedTable)} FROM {relatedTable}";
                NpgsqlCommand cmd = new NpgsqlCommand(sql, con);
                NpgsqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    comboBox.Items.Add(new ComboboxItem(dr[1].ToString(), dr[0]));
                }
                dr.Close();

                return comboBox;
            }
            else if (dataType == typeof(DateTime))
            {
                return new DateTimePicker { Dock = DockStyle.Fill };
            }
            else if (dataType == typeof(bool))
            {
                return new CheckBox { Dock = DockStyle.Fill };
            }
            else if (dataType == typeof(decimal) || dataType == typeof(double))
            {
                return new NumericUpDown { Dock = DockStyle.Fill, DecimalPlaces = 3, Maximum = 10000 };
            }
            else
            {
                return new TextBox { Dock = DockStyle.Fill };
            }
        }

        private string GetNameColumn(string tableName)
        {
            switch (tableName)
            {
                case "suppliers": return "supplier_name";
                case "products": return "product_name";
                case "bakery_items": return "item_name";
                default: return "name";
            }
        }

        private void SetControlValue(Control control, object value)
        {
            if (control is DateTimePicker)
                ((DateTimePicker)control).Value = (DateTime)value;
            else if (control is CheckBox)
                ((CheckBox)control).Checked = (bool)value;
            else if (control is NumericUpDown)
                ((NumericUpDown)control).Value = Convert.ToDecimal(value);
            else if (control is TextBox)
                ((TextBox)control).Text = value.ToString();
            else if (control is ComboBox)
            {
                foreach (ComboboxItem item in ((ComboBox)control).Items)
                {
                    if (item.Value.ToString() == value.ToString())
                    {
                        ((ComboBox)control).SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private string GetIdColumn(string tableName)
        {
            switch (tableName)
            {
                case "suppliers": return "supplier_id";
                case "products": return "product_id";
                case "bakery_items": return "item_id";
                case "recipes": return "recipe_id";
                case "incoming_invoices": return "invoice_id";
                case "production_orders": return "order_id";
                default: return "id";
            }
        }

        private void SaveRecord(string tableName, object id, TableLayoutPanel tableLayout, Form editForm, DataGridView dataGridView)
        {
            try
            {
                using (var con = GetConnection())
                {
                    string sql;
                    NpgsqlCommand cmd;

                    if (id == null)
                    {
                        sql = $"INSERT INTO {tableName} ({GetColumnsForInsert(tableName, tableLayout)}) VALUES ({GetValuesForInsert(tableName, tableLayout)})";
                    }
                    else
                    {
                        sql = $"UPDATE {tableName} SET {GetSetClause(tableName, tableLayout)} WHERE {GetIdColumn(tableName)} = @id";
                    }

                    cmd = new NpgsqlCommand(sql, con);
                    AddParameters(cmd, tableName, tableLayout);
                    if (id != null) cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();
                    LoadData(tableName, dataGridView);
                    editForm.Close();
                    LoadStatistics(); // Обновляем статистику после изменений
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}");
            }
        }

        private string GetColumnsForInsert(string tableName, TableLayoutPanel tableLayout)
        {
            string columns = "";
            for (int i = 0; i < tableLayout.RowCount - 1; i++)
            {
                var label = tableLayout.GetControlFromPosition(0, i) as Label;
                if (label == null) continue;

                string columnName = GetColumnNameFromDisplayName(label.Text);
                if (columnName == GetIdColumn(tableName)) continue;
                if (columns != "") columns += ", ";
                columns += columnName;
            }
            return columns;
        }

        private string GetColumnNameFromDisplayName(string displayName)
        {
            switch (displayName)
            {
                case "Название поставщика": return "supplier_name";
                case "Контактная информация": return "contact_info";
                case "Название продукта": return "product_name";
                case "Единица измерения": return "unit_of_measure";
                case "Текущее количество": return "current_quantity";
                case "Название продукции": return "item_name";
                case "Описание": return "description";
                case "Вид продукции": return "item_id";
                case "Продукт": return "product_id";
                case "Количество": return "quantity";
                case "Поставщик": return "supplier_id";
                case "Дата накладной": return "invoice_date";
                case "Номер накладной": return "invoice_number";
                case "Дата заказа": return "order_date";
                case "Завершено": return "completed";
                default: return displayName;
            }
        }

        private string GetValuesForInsert(string tableName, TableLayoutPanel tableLayout)
        {
            string values = "";
            for (int i = 0; i < tableLayout.RowCount - 1; i++)
            {
                var label = tableLayout.GetControlFromPosition(0, i) as Label;
                if (label == null) continue;

                string columnName = GetColumnNameFromDisplayName(label.Text);
                if (columnName == GetIdColumn(tableName)) continue;
                if (values != "") values += ", ";
                values += $"@{columnName}";
            }
            return values;
        }

        private string GetSetClause(string tableName, TableLayoutPanel tableLayout)
        {
            string setClause = "";
            for (int i = 0; i < tableLayout.RowCount - 1; i++)
            {
                var label = tableLayout.GetControlFromPosition(0, i) as Label;
                if (label == null) continue;

                string columnName = GetColumnNameFromDisplayName(label.Text);
                if (columnName == GetIdColumn(tableName)) continue;
                if (setClause != "") setClause += ", ";
                setClause += $"{columnName} = @{columnName}";
            }
            return setClause;
        }

        private void AddParameters(NpgsqlCommand cmd, string tableName, TableLayoutPanel tableLayout)
        {
            for (int i = 0; i < tableLayout.RowCount - 1; i++)
            {
                var label = tableLayout.GetControlFromPosition(0, i) as Label;
                if (label == null) continue;

                string columnName = GetColumnNameFromDisplayName(label.Text);
                if (columnName == GetIdColumn(tableName)) continue;

                Control inputControl = tableLayout.GetControlFromPosition(1, i);
                if (inputControl == null) continue;

                object value = GetControlValue(inputControl);
                cmd.Parameters.AddWithValue($"@{columnName}", value ?? DBNull.Value);
            }
        }

        private object GetControlValue(Control control)
        {
            if (control is DateTimePicker)
                return ((DateTimePicker)control).Value;
            else if (control is CheckBox)
                return ((CheckBox)control).Checked;
            else if (control is NumericUpDown)
                return ((NumericUpDown)control).Value;
            else if (control is TextBox)
                return string.IsNullOrEmpty(((TextBox)control).Text) ? null : ((TextBox)control).Text;
            else if (control is ComboBox)
                return ((ComboboxItem)((ComboBox)control).SelectedItem)?.Value;

            return null;
        }

        private void DeleteRecord(string tableName, object id, DataGridView dataGridView)
        {
            try
            {
                using (var con = GetConnection())
                {
                    string sql = $"DELETE FROM {tableName} WHERE {GetIdColumn(tableName)} = @id";
                    NpgsqlCommand cmd = new NpgsqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                    LoadData(tableName, dataGridView);
                    LoadStatistics(); // Обновляем статистику после удаления
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении записи: {ex.Message}");
            }
        }

        private void LoadSuppliers()
        {
            try
            {
                using (var con = GetConnection())
                {
                    string sql = "SELECT supplier_id, supplier_name FROM suppliers ORDER BY supplier_name";
                    NpgsqlCommand cmd = new NpgsqlCommand(sql, con);
                    NpgsqlDataReader dr = cmd.ExecuteReader();

                    checkedListBoxSuppliers.Items.Clear();
                    while (dr.Read())
                    {
                        checkedListBoxSuppliers.Items.Add(new ComboboxItem(dr["supplier_name"].ToString(), dr["supplier_id"]));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки поставщиков: " + ex.Message);
            }
        }

        private void LoadBakeryItems()
        {
            try
            {
                using (var con = GetConnection())
                {
                    string sql = "SELECT item_id, item_name FROM bakery_items ORDER BY item_name";
                    NpgsqlCommand cmd = new NpgsqlCommand(sql, con);
                    NpgsqlDataReader dr = cmd.ExecuteReader();

                    checkedListBoxBakeryItems.Items.Clear();
                    while (dr.Read())
                    {
                        checkedListBoxBakeryItems.Items.Add(new ComboboxItem(dr["item_name"].ToString(), dr["item_id"]));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки продукции: " + ex.Message);
            }
        }

        private void GenerateProductUsageReport(DateTime startDate, DateTime endDate, string filePath)
        {
            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook workbook = excelApp.Workbooks.Add();
            Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Sheets[1];

            try
            {
                using (var con = GetConnection())
                {
                    // Заголовок отчета
                    worksheet.Cells[1, 1] = "Отчет по использованию продуктов";
                    worksheet.Range["A1:F1"].Merge();
                    worksheet.Cells[2, 1] = $"За период с {startDate:dd.MM.yyyy} по {endDate:dd.MM.yyyy}";
                    worksheet.Range["A2:F2"].Merge();

                    // Заголовки таблицы
                    worksheet.Cells[4, 1] = "Продукт";
                    worksheet.Cells[4, 2] = "Ед. изм.";
                    worksheet.Cells[4, 3] = "Остаток на начало";
                    worksheet.Cells[4, 4] = "Поступило";
                    worksheet.Cells[4, 5] = "Использовано";
                    worksheet.Cells[4, 6] = "Остаток на конец";

                    // Получаем выбранные виды продукции
                    string bakeryItemsFilter = "";
                    foreach (ComboboxItem item in checkedListBoxBakeryItems.CheckedItems)
                    {
                        if (bakeryItemsFilter != "") bakeryItemsFilter += ",";
                        bakeryItemsFilter += item.Value.ToString();
                    }

                    // 1. Получаем остатки на начало периода
                    string sql = @"
                SELECT p.product_id, p.product_name, p.unit_of_measure, 
                       COALESCE(p.current_quantity, 0) AS beginning_quantity
                FROM products p
                WHERE p.product_id IN (
                    SELECT DISTINCT r.product_id 
                    FROM recipes r 
                    WHERE r.item_id IN (" + bakeryItemsFilter + @")
                )";

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, con);
                    NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                    DataTable dtProducts = new DataTable();
                    da.Fill(dtProducts);

                    // 2. Получаем количество поступивших продуктов за период
                    sql = @"
                SELECT iii.product_id, SUM(iii.quantity) AS received_quantity
                FROM incoming_invoice_items iii
                JOIN incoming_invoices ii ON iii.invoice_id = ii.invoice_id
                WHERE ii.invoice_date BETWEEN @startDate AND @endDate
                AND iii.product_id IN (
                    SELECT DISTINCT r.product_id 
                    FROM recipes r 
                    WHERE r.item_id IN (" + bakeryItemsFilter + @")
                )
                GROUP BY iii.product_id";

                    NpgsqlCommand cmdReceived = new NpgsqlCommand(sql, con);
                    cmdReceived.Parameters.AddWithValue("@startDate", startDate);
                    cmdReceived.Parameters.AddWithValue("@endDate", endDate);
                    da = new NpgsqlDataAdapter(cmdReceived);
                    DataTable dtReceived = new DataTable();
                    da.Fill(dtReceived);

                    // 3. Получаем использованное количество продуктов
                    sql = @"
                SELECT r.product_id, SUM(r.quantity * poi.quantity) AS used_quantity
                FROM production_order_items poi
                JOIN production_orders po ON poi.order_id = po.order_id
                JOIN recipes r ON poi.item_id = r.item_id
                WHERE po.order_date BETWEEN @startDate AND @endDate
                AND poi.item_id IN (" + bakeryItemsFilter + @")
                GROUP BY r.product_id";

                    NpgsqlCommand cmdUsed = new NpgsqlCommand(sql, con);
                    cmdUsed.Parameters.AddWithValue("@startDate", startDate);
                    cmdUsed.Parameters.AddWithValue("@endDate", endDate);
                    da = new NpgsqlDataAdapter(cmdUsed);
                    DataTable dtUsed = new DataTable();
                    da.Fill(dtUsed);

                    // Заполняем данные в Excel
                    int row = 5;
                    foreach (DataRow product in dtProducts.Rows)
                    {
                        decimal beginningQty = Convert.ToDecimal(product["beginning_quantity"]);
                        worksheet.Cells[row, 1] = product["product_name"].ToString();
                        worksheet.Cells[row, 2] = product["unit_of_measure"].ToString();
                        worksheet.Cells[row, 3] = beginningQty;

                        // Получаем количество поступивших товаров
                        decimal receivedQty = 0;
                        DataRow[] receivedRows = dtReceived.Select($"product_id = {product["product_id"]}");
                        if (receivedRows.Length > 0)
                        {
                            receivedQty = Convert.ToDecimal(receivedRows[0]["received_quantity"]);
                        }
                        worksheet.Cells[row, 4] = receivedQty;

                        // Получаем использованное количество
                        decimal usedQty = 0;
                        DataRow[] usedRows = dtUsed.Select($"product_id = {product["product_id"]}");
                        if (usedRows.Length > 0)
                        {
                            usedQty = Convert.ToDecimal(usedRows[0]["used_quantity"]);
                        }
                        worksheet.Cells[row, 5] = usedQty;

                        // Рассчитываем остаток на конец периода
                        decimal endingQty = beginningQty + receivedQty - usedQty;
                        worksheet.Cells[row, 6] = endingQty;

                        row++;
                    }

                    // Форматирование
                    Excel.Range range = worksheet.Range["A4:F" + (row - 1)];
                    range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    range.Font.Bold = false;

                    Excel.Range headerRange = worksheet.Range["A4:F4"];
                    headerRange.Font.Bold = true;
                    headerRange.Interior.Color = Excel.XlRgbColor.rgbLightGray;

                    worksheet.Columns.AutoFit();

                    // Сохраняем файл
                    workbook.SaveAs(filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при формировании отчета: " + ex.Message);
            }
            finally
            {
                workbook.Close(false);
                excelApp.Quit();
                ReleaseObject(worksheet);
                ReleaseObject(workbook);
                ReleaseObject(excelApp);
            }
        }

        private void GenerateSupplierReport(DateTime startDate, DateTime endDate, string filePath)
        {
            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook workbook = excelApp.Workbooks.Add();
            Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Sheets[1];

            try
            {
                using (var con = GetConnection())
                {
                    // Заголовок отчета
                    worksheet.Cells[1, 1] = "Отчет по поставкам продуктов";
                    worksheet.Range["A1:D1"].Merge();
                    worksheet.Cells[2, 1] = $"За период с {startDate:dd.MM.yyyy} по {endDate:dd.MM.yyyy}";
                    worksheet.Range["A2:D2"].Merge();

                    // Заголовки таблицы
                    worksheet.Cells[4, 1] = "Поставщик";
                    worksheet.Cells[4, 2] = "Продукт";
                    worksheet.Cells[4, 3] = "Ед. изм.";
                    worksheet.Cells[4, 4] = "Количество";
                    worksheet.Cells[4, 5] = "Сумма поставки";

                    // Получаем выбранных поставщиков
                    string suppliersFilter = "";
                    foreach (ComboboxItem item in checkedListBoxSuppliers.CheckedItems)
                    {
                        if (suppliersFilter != "") suppliersFilter += ",";
                        suppliersFilter += item.Value.ToString();
                    }

                    // Получаем данные о поставках
                    string sql = @"
                        SELECT s.supplier_name, p.product_name, p.unit_of_measure, 
                               SUM(iii.quantity) AS total_quantity, 
                               SUM(iii.quantity * iii.price) AS total_amount
                        FROM incoming_invoice_items iii
                        JOIN incoming_invoices ii ON iii.invoice_id = ii.invoice_id
                        JOIN suppliers s ON ii.supplier_id = s.supplier_id
                        JOIN products p ON iii.product_id = p.product_id
                        WHERE ii.invoice_date BETWEEN @startDate AND @endDate
                        AND ii.supplier_id IN (" + suppliersFilter + @")
                        GROUP BY s.supplier_name, p.product_name, p.unit_of_measure
                        ORDER BY s.supplier_name, p.product_name";

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@startDate", startDate);
                    cmd.Parameters.AddWithValue("@endDate", endDate);
                    NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Заполняем данные в Excel
                    int row = 5;
                    foreach (DataRow dr in dt.Rows)
                    {
                        worksheet.Cells[row, 1] = dr["supplier_name"].ToString();
                        worksheet.Cells[row, 2] = dr["product_name"].ToString();
                        worksheet.Cells[row, 3] = dr["unit_of_measure"].ToString();
                        worksheet.Cells[row, 4] = Convert.ToDecimal(dr["total_quantity"]);
                        worksheet.Cells[row, 5] = Convert.ToDecimal(dr["total_amount"]);
                        row++;
                    }

                    // Форматирование
                    Excel.Range range = worksheet.Range["A4:E" + (row - 1)];
                    range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    range.Font.Bold = false;

                    Excel.Range headerRange = worksheet.Range["A4:E4"];
                    headerRange.Font.Bold = true;
                    headerRange.Interior.Color = Excel.XlRgbColor.rgbLightGray;

                    // Форматирование денежных значений
                    Excel.Range amountRange = worksheet.Range["E5:E" + (row - 1)];
                    amountRange.NumberFormat = "#,##0.00";

                    // Итоговая строка
                    worksheet.Cells[row, 1] = "Итого:";
                    worksheet.Range[$"A{row}:D{row}"].Merge();
                    worksheet.Cells[row, 5].Formula = $"=SUM(E5:E{row - 1})";
                    worksheet.Range[$"A{row}:E{row}"].Font.Bold = true;

                    worksheet.Columns.AutoFit();

                    // Сохраняем файл
                    workbook.SaveAs(filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при формировании отчета: " + ex.Message);
            }
            finally
            {
                workbook.Close(false);
                excelApp.Quit();
                ReleaseObject(worksheet);
                ReleaseObject(workbook);
                ReleaseObject(excelApp);
            }
        }

        private void ReleaseObject(object obj)
        {
            try
            {
                if (obj != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                    obj = null;
                }
            }
            catch (Exception)
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }

        private void btnGenerateSupplierReport_Click(object sender, EventArgs e)
        {
            if (checkedListBoxSuppliers.CheckedItems.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы одного поставщика");
                return;
            }

            DateTime startDate = dateTimePickerStart.Value.Date;
            DateTime endDate = dateTimePickerEnd.Value.Date;

            if (endDate < startDate)
            {
                MessageBox.Show("Конечная дата не может быть раньше начальной");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Files|*.xlsx";
            saveFileDialog.Title = "Сохранить отчет по поставкам";
            saveFileDialog.FileName = $"Отчет_по_поставкам_{DateTime.Now:yyyyMMdd}.xlsx";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                GenerateSupplierReport(startDate, endDate, saveFileDialog.FileName);
                MessageBox.Show("Отчет успешно сформирован!");
            }
        }

        private void btnGenerateProductUsage_Click(object sender, EventArgs e)
        {
            if (checkedListBoxBakeryItems.CheckedItems.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы один вид продукции");
                return;
            }

            DateTime startDate = dateTimePickerStart.Value.Date;
            DateTime endDate = dateTimePickerEnd.Value.Date;

            if (endDate < startDate)
            {
                MessageBox.Show("Конечная дата не может быть раньше начальной");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Files|*.xlsx";
            saveFileDialog.Title = "Сохранить отчет по использованию продуктов";
            saveFileDialog.FileName = $"Отчет_по_использованию_продуктов_{DateTime.Now:yyyyMMdd}.xlsx";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                GenerateProductUsageReport(startDate, endDate, saveFileDialog.FileName);
                MessageBox.Show("Отчет успешно сформирован!");
            }
        }

        private void checkedListBoxBakeryItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Обработчик события изменения выбора в списке видов продукции
        }
    }

    public class ComboboxItem
    {
        public string Text { get; set; }
        public object Value { get; set; }

        public ComboboxItem(string text, object value)
        {
            Text = text;
            Value = value;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}