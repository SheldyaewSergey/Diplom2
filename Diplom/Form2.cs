using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using Npgsql;
using Point = System.Drawing.Point;
using Label = System.Windows.Forms.Label;
using Font = System.Drawing.Font;
using Button = System.Windows.Forms.Button;
using TextBox = System.Windows.Forms.TextBox;
using DataTable = System.Data.DataTable;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace Diplom
{
    public partial class FormMain : Form
    {
        private NpgsqlConnection connection = new NpgsqlConnection("Server=localhost; Port=5432; User Id=postgres; Password=6387; Database=diplommm;");
        public FormMain()
        {
            InitializeComponent();
            if (this.Controls["contentPanel"] == null)
            {
                contentPanel = new Panel
                {
                    Name = "contentPanel",
                    Location = new Point(210, 1),
                    Size = new Size(850, 660),
                    BorderStyle = BorderStyle.FixedSingle
                };
                this.Controls.Add(contentPanel);
            }

            // Вызов метода ShowBasePanel при загрузке формы
            this.Load += new EventHandler(FormMain_Load);

        }

        // Создаем обработчик событий для нажатия на лейбл Выдача заказа
        private void labelVidacha_Click(object sender, EventArgs e)
        {
            ResetForm();
            ShowIssueOrderForm();
        }

        private void ShowIssueOrderForm()
        {
            Panel contentPanel = this.Controls["contentPanel"] as Panel;
            if (contentPanel == null) return;

            // Создаем элементы для выдачи заказа
            Label labelOrderNumber = new Label
            {
                Text = "Введите номер заказа:",
                Location = new Point(225, 300),
                AutoSize = true,
                Font = new Font("Ebrima", 14)
            };
            TextBox textBoxOrderNumber = new TextBox
            {
                Name = "textBoxOrderNumber",
                Location = new Point(225, 335),
                Width = 350,
                Font = new Font("Ebrima", 14)
            };
            Button buttonNext = new Button
            {
                Text = "Вперед",
                Location = new Point(225, 385),
                Width = 125,
                Height = 45,
                Font = new Font("Ebrima", 14)
            };
            buttonNext.Click += ButtonNext_Click;

            // Добавляем элементы на панель
            contentPanel.Controls.Add(labelOrderNumber);
            contentPanel.Controls.Add(textBoxOrderNumber);
            contentPanel.Controls.Add(buttonNext);
        }

        private void ButtonNext_Click(object sender, EventArgs e)
        {
            Panel contentPanel = this.Controls["contentPanel"] as Panel;
            if (contentPanel == null) return;

            // Получение номера заказа из TextBox
            TextBox textBoxOrderNumber = contentPanel.Controls["textBoxOrderNumber"] as TextBox;
            if (textBoxOrderNumber == null) return;
            if (!int.TryParse(textBoxOrderNumber.Text, out int orderNumber))
            {
                MessageBox.Show("Введите корректный номер заказа.");
                return;
            }

            // Проверка, существует ли заказ с таким номером
            DataRow[] orderRows = FormLogin.ds.Tables["orders"].Select($"id = {orderNumber}");
            if (orderRows.Length == 0)
            {
                MessageBox.Show("Заказ с таким номером не найден.");
                return;
            }

            string status = orderRows[0]["Статус"].ToString();

            // Проверяем статус заказа и показываем сообщение при необходимости
            if (status == "Выполнен" || status == "Выполнен частично")
            {
                MessageBox.Show("Статус заказа указывает на то, что он уже выполнен или выполнен частично. Выберите другой заказ.");
                return;
            }
            else
            {
                // Очистка панели содержимого
                contentPanel.Controls.Clear();

                // Показать данные корзины для указанного номера заказа
                ShowCartData(orderNumber);
            }
        }

        private void ShowCartData(int orderNumber)
        {
            ResetForm();
            // Проверка наличия данных в DataSet
            if (FormLogin.ds.Tables["cart"].Rows.Count == 0)
            {
                MessageBox.Show("Таблица 'cart' пуста.");
            }
            if (FormLogin.ds.Tables["products"].Rows.Count == 0)
            {
                MessageBox.Show("Таблица 'products' пуста.");
            }

            // Создание новой таблицы для объединения данных из cart и products
            DataTable combinedTable = new DataTable();
            combinedTable.Columns.Add("Id", typeof(int));
            combinedTable.Columns.Add("Id заказа", typeof(int));
            combinedTable.Columns.Add("Id товара", typeof(int));
            combinedTable.Columns.Add("Количество", typeof(int));
            combinedTable.Columns.Add("Складское место", typeof(string));
            combinedTable.Columns.Add("Название", typeof(string));
            combinedTable.Columns.Add("Производитель", typeof(string));
            combinedTable.Columns.Add("Цена", typeof(decimal));
            combinedTable.Columns.Add("Вес", typeof(decimal));
            combinedTable.Columns.Add("Категория", typeof(string));
            combinedTable.Columns.Add("Цвет", typeof(string));
            combinedTable.Columns.Add("Количество в упаковке", typeof(int));

            // Объединение данных из таблиц cart и products
            foreach (DataRow cartRow in FormLogin.ds.Tables["cart"].Select($"[Id заказа] = {orderNumber}"))
            {
                DataRow productRow = FormLogin.ds.Tables["products"].Select($"id = {cartRow["Id товара"]}").FirstOrDefault();
                if (productRow != null)
                {

                    DataRow newRow = combinedTable.NewRow();
                    newRow["Id"] = cartRow["Id"];
                    newRow["Id заказа"] = cartRow["Id заказа"];
                    newRow["Id товара"] = cartRow["Id товара"];
                    newRow["Количество"] = cartRow["Количество"];
                    newRow["Складское место"] = cartRow["Складское место"];
                    newRow["Название"] = productRow["Название"];
                    newRow["Производитель"] = productRow["Производитель"];
                    newRow["Цена"] = productRow["Цена"];
                    newRow["Вес"] = productRow["Вес"];
                    newRow["Категория"] = productRow["Категория"];
                    newRow["Цвет"] = productRow["Цвет"];
                    newRow["Количество в упаковке"] = productRow["Количество в упаковке"];
                    combinedTable.Rows.Add(newRow);
                }
            }

            // Создание DataGridView для отображения данных покупок
            DataGridView dataGridView = new DataGridView
            {
                Location = new Point(4, 4),
                Size = new Size(840, 500),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AllowUserToAddRows = false,
                DataSource = combinedTable // Использовать объединенную таблицу
            };
            contentPanel.Controls.Add(dataGridView);

            // Создание динамически столбца CheckBox
            DataGridViewCheckBoxColumn checkBoxColumn = new DataGridViewCheckBoxColumn
            {
                HeaderText = "Выдать",
                Name = "checkBoxColumn",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader
            };
            dataGridView.Columns.Add(checkBoxColumn);

            // Создание столбца для причины возврата
            DataGridViewTextBoxColumn returnReasonColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Причина возврата",
                Name = "returnReasonColumn",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };
            dataGridView.Columns.Add(returnReasonColumn);

            // Скрытие ненужных столбцов
            dataGridView.Columns["Id заказа"].Visible = false;
            dataGridView.Columns["Id товара"].Visible = false;

            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                if (column.Name != "checkBoxColumn" && column.Name != "returnReasonColumn")
                {
                    column.ReadOnly = true;
                }
            }

            // Обработка событий для изменения состояния чекбоксов
            dataGridView.CellValueChanged += (s, args) =>
            {
                if (args.ColumnIndex == dataGridView.Columns["returnReasonColumn"].Index)
                {
                    var checkBoxCell = dataGridView.Rows[args.RowIndex].Cells["checkBoxColumn"] as DataGridViewCheckBoxCell;
                    if (checkBoxCell != null && !string.IsNullOrEmpty(dataGridView.Rows[args.RowIndex].Cells["returnReasonColumn"].Value?.ToString()))
                    {
                        checkBoxCell.Value = false;
                    }
                }
            };

            dataGridView.CellBeginEdit += (s, args) =>
            {
                if (args.ColumnIndex == dataGridView.Columns["checkBoxColumn"].Index)
                {
                    var reasonCellValue = dataGridView.Rows[args.RowIndex].Cells["returnReasonColumn"].Value?.ToString();
                    if (!string.IsNullOrEmpty(reasonCellValue))
                    {
                        MessageBox.Show("Уберите причину возврата перед выбором товара для выдачи.");
                        dataGridView.CancelEdit();
                    }
                }
            };

            Button buttonIssueOrder = new Button
            {
                Text = "Выдать заказ",
                Location = new Point(595, 612),
                Width = 150,
                Height = 30
            };
            // Создание кнопки для выдачи заказа
            buttonIssueOrder.Click += (s, args) =>
            {
                bool hasUnselectedItems = false;
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    // Проверяем, является ли текущая ячейка checkbox
                    if (row.Cells["checkBoxColumn"] is DataGridViewCheckBoxCell checkBox)
                    {
                        // Проверяем значение checkbox
                        bool isChecked = checkBox.Value != null && (bool)checkBox.Value;

                        // Если checkbox не выбран, проверяем причину возврата
                        if (!isChecked)
                        {
                            string returnReason = row.Cells["returnReasonColumn"].Value?.ToString();

                            // Если причина возврата не указана, показываем сообщение и прерываем выполнение
                            if (string.IsNullOrWhiteSpace(returnReason))
                            {
                                MessageBox.Show("Заполните причину возврата для товаров, которые не будут выданы.");
                                return;
                            }

                            // Если причина возврата указана, добавляем товар в таблицу возвратов
                            hasUnselectedItems = true;
                            DataRow returnRow = FormLogin.ds.Tables["returns"].NewRow();
                            returnRow["Id заказа"] = orderNumber;
                            returnRow["Id товара"] = row.Cells["Id товара"].Value;
                            returnRow["Причина"] = returnReason;
                            FormLogin.ds.Tables["returns"].Rows.Add(returnRow);
                        }
                    }
                }
                string currentDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                int employeeId = FormLogin.LoggedInEmployeeId;

                // Обновляем статус заказа в зависимости от наличия невыданных товаров
                if (hasUnselectedItems)
                {
                    string updateQuery = $"UPDATE orders SET status = 'Частично выполнен', issued_at = '{currentDateTime}', employee_id = {employeeId} WHERE id = {orderNumber}";
                    FormLogin.Modification_Execute(updateQuery);
                    MessageBox.Show("Статус заказа обновлен на 'Частично выполнен', а не выбранные позиции добавлены к возвратам.");
                }
                else
                {
                    string updateQuery = $"UPDATE orders SET status = 'Выполнен', issued_at = '{currentDateTime}', employee_id = {employeeId} WHERE id = {orderNumber}";
                    FormLogin.Modification_Execute(updateQuery);
                    MessageBox.Show("Статус заказа обновлен на 'Выполнен'.");
                }
            };


            contentPanel.Controls.Add(buttonIssueOrder);

            // Создание кнопки "Назад"
            Button buttonBack = new Button
            {
                Text = "Назад",
                Location = new Point(205, 612),
                Width = 150,
                Height = 30
            };
            buttonBack.Click += (s, args) =>
            {
                ResetForm();
            };
            contentPanel.Controls.Add(buttonBack);
        }


        private void ResetForm()
        {

            Panel contentPanel = this.Controls["contentPanel"] as Panel;
            if (contentPanel == null) return;
            contentPanel.Controls.Clear();
           
        }

        // Добавляем поле для хранения идентификатора выбранного продукта
        private int selectedProductId = -1;

        private void labelPriem_Click(object sender, EventArgs e)
        {
            // Очистка contentPanel
            contentPanel.Controls.Clear();

            Label labelInfo = new Label { Location = new Point(200, 70), Width = 300, Font = new Font("Ebrima", 14) };
            contentPanel.Controls.Add(labelInfo);
            labelInfo.Text = "Введита название товара";

            // Создание TextBox для ввода названия товара
            TextBox textBoxProductName = new TextBox
            {
                Location = new Point(200, 100),
                Width = 400,
                Font = new Font("Ebrima", 14)
            };
            contentPanel.Controls.Add(textBoxProductName);

            // Создание Label для отображения информации о товаре
            Label labelManufacturer = new Label { Location = new Point(200, 140), Width = 300, Font = new Font("Ebrima", 14) };
            Label labelPrice = new Label { Location = new Point(200, 170), Width = 300, Font = new Font("Ebrima", 14) };
            Label labelWeight = new Label { Location = new Point(200, 200), Width = 300, Font = new Font("Ebrima", 14) };
            Label labelCategory = new Label { Location = new Point(200, 230), Width = 300, Font = new Font("Ebrima", 14) };
            Label labelColor = new Label { Location = new Point(200, 260), Width = 300, Font = new Font("Ebrima", 14) };
            Label labelStock = new Label { Location = new Point(200, 290), Width = 300, Font = new Font("Ebrima", 14) };
            Label labelPackageQuantity = new Label { Location = new Point(200, 320), Width = 300, Font = new Font("Ebrima", 14) };

            contentPanel.Controls.Add(labelManufacturer);
            contentPanel.Controls.Add(labelPrice);
            contentPanel.Controls.Add(labelWeight);
            contentPanel.Controls.Add(labelCategory);
            contentPanel.Controls.Add(labelColor);
            contentPanel.Controls.Add(labelStock);
            contentPanel.Controls.Add(labelPackageQuantity);

            labelManufacturer.Text = "Производитель";
            labelPrice.Text = "Цена";
            labelWeight.Text = "Вес";
            labelCategory.Text = "Категория";
            labelColor.Text = "Цвет";
            labelStock.Text = "Остаток на складе";
            labelPackageQuantity.Text = "Кол-во в упаковке";

            // Настройка автозаполнения для TextBox
            AutoCompleteStringCollection productNames = new AutoCompleteStringCollection();
            foreach (DataRow row in FormLogin.ds.Tables["products"].Rows)
            {
                productNames.Add(row["Название"].ToString());
            }

            textBoxProductName.AutoCompleteCustomSource = productNames;
            textBoxProductName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            textBoxProductName.AutoCompleteSource = AutoCompleteSource.CustomSource;

            // Обработка выбора товара
            textBoxProductName.TextChanged += (s, args) =>
            {
                string enteredText = textBoxProductName.Text.Trim(); // Убираем пробелы в начале и конце строки

                var filteredRows = FormLogin.ds.Tables["products"].Select($"Название LIKE '{enteredText}%'");

                // Если введенное название точно совпадает с названием товара, заполняем поля
                if (filteredRows.Length == 1 && filteredRows[0]["Название"].ToString().Equals(enteredText, StringComparison.OrdinalIgnoreCase))
                {
                    DataRow selectedRow = filteredRows[0];
                    labelManufacturer.Text = $"Производитель: {selectedRow["Производитель"]}";
                    labelPrice.Text = $"Цена: {selectedRow["Цена"]}";
                    labelWeight.Text = $"Вес: {selectedRow["Вес"]}";
                    labelCategory.Text = $"Категория: {selectedRow["Категория"]}";
                    labelColor.Text = $"Цвет: {selectedRow["Цвет"]}";
                    labelStock.Text = $"Количество на складе: {selectedRow["Количество на складе"]}";
                    labelPackageQuantity.Text = $"Количество в упаковке: {selectedRow["Количество в упаковке"]}";

                    // Сохранение идентификатора выбранного продукта
                    selectedProductId = Convert.ToInt32(selectedRow["id"]);

                    // Добавление поля для ввода количества и кнопки
                    Label labelQuantity = new Label { Location = new Point(200, 400), Width = 300, Font = new Font("Ebrima", 14) };
                    contentPanel.Controls.Add(labelQuantity);
                    labelQuantity.Text = "Введите количество прибывшей на склад продукции:";

                    TextBox textBoxQuantity = new TextBox
                    {
                        Location = new Point(200, 430),
                        Width = 200,
                        Font = new Font("Ebrima", 14),
                        Name = "textBoxQuantity"
                    };
                    contentPanel.Controls.Add(textBoxQuantity);

                    Button buttonAddToStock = new Button
                    {
                        Text = "Добавить на склад",
                        Location = new Point(200, 470),
                        Font = new Font("Ebrima", 14),
                        AutoSize = true
                    };
                    buttonAddToStock.Click += ButtonAddToStock_Click;
                    contentPanel.Controls.Add(buttonAddToStock);
                }
                else
                {
                    // Очистка полей, если нет точного совпадения
                    labelManufacturer.Text = "Производитель";
                    labelPrice.Text = "Цена";
                    labelWeight.Text = "Вес";
                    labelCategory.Text = "Категория";
                    labelColor.Text = "Цвет";
                    labelStock.Text = "Остаток на складе";
                    labelPackageQuantity.Text = "Кол-во в упаковке";

                    // Очистка идентификатора выбранного продукта
                    selectedProductId = -1;
                }
            };
        }

        // Обработчик нажатия кнопки "Добавить на склад"
        private void ButtonAddToStock_Click(object sender, EventArgs e)
        {
            Panel contentPanel = this.Controls["contentPanel"] as Panel;
            if (contentPanel == null) return;

            // Получение введенного количества из TextBox
            TextBox textBoxQuantity = contentPanel.Controls["textBoxQuantity"] as TextBox;
            if (textBoxQuantity == null) return;

            // Проверка, является ли введенное значение числом
            if (!int.TryParse(textBoxQuantity.Text, out int quantity))
            {
                MessageBox.Show("Введите корректное количество продукции.");
                return;
            }

            // Проверка, был ли выбран продукт
            if (selectedProductId == -1)
            {
                MessageBox.Show("Сначала выберите продукт.");
                return;
            }

            // Получение выбранного продукта по id
            DataRow selectedProduct = FormLogin.ds.Tables["products"].Select($"id = {selectedProductId}").FirstOrDefault();
            if (selectedProduct == null)
            {
                MessageBox.Show("Выбранный продукт не найден.");
                return;
            }

            // Получение текущего остатка на складе
            int currentStock = Convert.ToInt32(selectedProduct["Количество на складе"]);

            // Прибавление введенного количества к текущему остатку
            int newStock = currentStock + quantity;

            // Обновление остатка на складе в базе данных
            string updateQuery = $"UPDATE products SET stock = {newStock} WHERE id = {selectedProductId}";

            try
            {
                FormLogin.Modification_Execute(updateQuery);
                // Оповещение пользователя об успешном добавлении на склад
                MessageBox.Show($"Количество продукции успешно добавлено на склад. Новый остаток: {newStock}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении базы данных: {ex.Message}");
                return;
            }

            // Очистка поля ввода количества
            textBoxQuantity.Text = "";
        }

        private void ShowBasePanel()
        {
            // Очистка contentPanel
            contentPanel.Controls.Clear();

            // Создание Label с базовой надписью
            Label baseLabel = new Label
            {
                Text = "Нажмите на интересующий вас раздел меню, чтобы начать работу",
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Font = new Font("Arial", 14, FontStyle.Bold)
            };

            contentPanel.Controls.Add(baseLabel);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ResetForm();
            ShowBasePanel();
        }

        private void labelSklad_Click(object sender, EventArgs e)
        {
            ResetForm();
            ShowBasePanel();
        }

        private void label1Moi_Click(object sender, EventArgs e)
        {
            ResetForm();
            ShowBasePanel();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            ResetForm();
            ShowBasePanel();
        }

        private void labelVozvrat_Click(object sender, EventArgs e)
        {
            // Очистка contentPanel
            contentPanel.Controls.Clear();

            // Создание и добавление элементов на панель
            Label labelInfo = new Label { Location = new Point(200, 140), Width = 300, Font = new Font("Ebrima", 14), Text = "Введите название товара" };
            TextBox textBoxProductName = new TextBox { Location = new Point(200, 170), Width = 400, Font = new Font("Ebrima", 14) };

            Label labelQuantity = new Label { Location = new Point(200, 200), Width = 300, Font = new Font("Ebrima", 14), Text = "Введите кол-во товара" };
            TextBox textBoxQuantity = new TextBox { Location = new Point(200, 230), Width = 200, Font = new Font("Ebrima", 14) };

            Label labelOrderNumber = new Label { Location = new Point(200, 260), Width = 300, Font = new Font("Ebrima", 14), Text = "Введите номер заказа" };
            TextBox textBoxOrderNumber = new TextBox { Location = new Point(200, 290), Width = 200, Font = new Font("Ebrima", 14) };

            Label labelReason = new Label { Location = new Point(200, 320), Width = 300, Font = new Font("Ebrima", 14), Text = "Введите причину возврата" };
            TextBox textBoxReason = new TextBox { Location = new Point(200, 350), Width = 400, Font = new Font("Ebrima", 14) };

            Button buttonReturnProduct = new Button { Text = "Оформить возврат", Location = new Point(200, 400), Font = new Font("Ebrima", 14), AutoSize = true };
            buttonReturnProduct.Click += ButtonReturnProduct_Click;

            contentPanel.Controls.Add(labelInfo);
            contentPanel.Controls.Add(textBoxProductName);
            contentPanel.Controls.Add(labelQuantity);
            contentPanel.Controls.Add(textBoxQuantity);
            contentPanel.Controls.Add(labelOrderNumber);
            contentPanel.Controls.Add(textBoxOrderNumber);
            contentPanel.Controls.Add(labelReason);
            contentPanel.Controls.Add(textBoxReason);
            contentPanel.Controls.Add(buttonReturnProduct);

            // Настройка автозаполнения для TextBox
            AutoCompleteStringCollection productNames = new AutoCompleteStringCollection();
            foreach (DataRow row in FormLogin.ds.Tables["products"].Rows)
            {
                productNames.Add(row["Название"].ToString());
            }

            textBoxProductName.AutoCompleteCustomSource = productNames;
            textBoxProductName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            textBoxProductName.AutoCompleteSource = AutoCompleteSource.CustomSource;

            textBoxProductName.TextChanged += (s, args) =>
            {
                string enteredText = textBoxProductName.Text.Trim();
                var filteredRows = FormLogin.ds.Tables["products"].Select($"Название LIKE '{enteredText}%'");

                if (filteredRows.Length == 1 && filteredRows[0]["Название"].ToString().Equals(enteredText, StringComparison.OrdinalIgnoreCase))
                {
                    DataRow selectedRow = filteredRows[0];
                    selectedProductId = Convert.ToInt32(selectedRow["id"]);
                }
                else
                {
                    selectedProductId = -1;
                }
            };
        }
        private void ButtonReturnProduct_Click(object sender, EventArgs e)
        {
            // Получение введенных данных
            string productName = contentPanel.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Location == new Point(200, 170))?.Text.Trim();
            string quantityText = contentPanel.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Location == new Point(200, 230))?.Text.Trim();
            string orderNumber = contentPanel.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Location == new Point(200, 290))?.Text.Trim();
            string returnReason = contentPanel.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Location == new Point(200, 350))?.Text.Trim();

            if (selectedProductId == -1)
            {
                MessageBox.Show("Сначала выберите продукт.");
                return;
            }

            if (!int.TryParse(quantityText, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество продукции.");
                return;
            }

            if (string.IsNullOrEmpty(orderNumber))
            {
                MessageBox.Show("Введите номер заказа.");
                return;
            }

            if (string.IsNullOrEmpty(returnReason))
            {
                MessageBox.Show("Введите причину возврата.");
                return;
            }

            try
            {
                // Добавление записей в DataSet
                for (int i = 0; i < quantity; i++)
                {
                    DataRow newRow = FormLogin.ds.Tables["returns"].NewRow();
                    newRow["Id заказа"] = orderNumber;
                    newRow["Причина"] = returnReason;
                    newRow["Id товара"] = selectedProductId;
                    FormLogin.ds.Tables["returns"].Rows.Add(newRow);
                }

                // Отправка изменений в базу данных
                foreach (DataRow row in FormLogin.ds.Tables["returns"].Rows)
                {
                    if (row.RowState == DataRowState.Added)
                    {
                        string insertQuery = $"INSERT INTO returns (order_id, return_reason, product_id) VALUES ({row["Id заказа"]}, '{row["Причина"]}', {row["Id товара"]})";
                        FormLogin.Modification_Execute(insertQuery);
                    }
                }

                MessageBox.Show("Возврат успешно зарегистрирован.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации возврата: {ex.Message}");
            }
        }

        private DataGridView dataGridViewOrders;

        private void labelZakaz_Click(object sender, EventArgs e)
        {
            // Обновление DataSet перед отображением
            UpdateDataSet();

            // Очистка contentPanel
            contentPanel.Controls.Clear();

            // Создание новой таблицы для отображения данных
            DataTable displayTable = new DataTable();
            displayTable.Columns.Add("ID заказа", typeof(int));
            displayTable.Columns.Add("Дата создания заказа", typeof(DateTime));
            displayTable.Columns.Add("Статус", typeof(string));
            displayTable.Columns.Add("ID клиента", typeof(int));
            displayTable.Columns.Add("Дата сборки заказа", typeof(DateTime));
            displayTable.Columns.Add("Дата хранения", typeof(DateTime));
            displayTable.Columns.Add("Дата выдачи", typeof(DateTime));
            displayTable.Columns.Add("ID работника", typeof(int));

            // Заполнение новыми данными из оригинальной таблицы
            foreach (DataRow row in FormLogin.ds.Tables["orders"].Rows)
            {
                DataRow newRow = displayTable.NewRow();
                newRow["ID заказа"] = row["id"];
                newRow["Дата создания заказа"] = row["created_at"];
                newRow["Статус"] = row["status"];
                newRow["ID клиента"] = row["customer_id"];
                newRow["Дата сборки заказа"] = row["assembled_at"];
                newRow["Дата хранения"] = row["storage_deadline"];
                newRow["Дата выдачи"] = row["issued_at"];
                newRow["ID работника"] = row["employee_id"];
                displayTable.Rows.Add(newRow);
            }

            // Удаление пустых строк
            for (int i = displayTable.Rows.Count - 1; i >= 0; i--)
            {
                DataRow row = displayTable.Rows[i];
                bool isEmpty = row.ItemArray.All(field => field == null || string.IsNullOrEmpty(field.ToString()));
                if (isEmpty)
                {
                    displayTable.Rows.RemoveAt(i);
                }
            }

            // Создание и добавление DataGridView для отображения данных заказов
            dataGridViewOrders = new DataGridView
            {
                Location = new Point(4, 4),
                Size = new Size(840, 500),
                DataSource = displayTable,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            contentPanel.Controls.Add(dataGridViewOrders);

            // Создание кнопки "Редактировать"
            Button buttonEdit = new Button
            {
                Text = "Редактировать",
                Location = new Point(320, 550),
                Font = new Font("Ebrima", 14),
                AutoSize = true
            };
            buttonEdit.Click += (s, args) =>
            {
                if (dataGridViewOrders.SelectedRows.Count > 0)
                {
                    int selectedOrderId = Convert.ToInt32(dataGridViewOrders.SelectedRows[0].Cells["ID заказа"].Value);
                    EditOrder(selectedOrderId);
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите заказ для редактирования.");
                }
            };
            contentPanel.Controls.Add(buttonEdit);

            // Создание кнопки "Выгрузить в Excel"
            Button buttonExport = new Button
            {
                Text = "Выгрузить в Excel",
                Location = new Point(500, 550),
                Font = new Font("Ebrima", 14),
                AutoSize = true
            };
            buttonExport.Click += ButtonExport_Click;
            contentPanel.Controls.Add(buttonExport);

            // Создание кнопки "Собрать заказ"
            Button buttonAssembleOrder = new Button
            {
                Text = "Собрать заказ",
                Location = new Point(150, 550),
                Font = new Font("Ebrima", 14),
                AutoSize = true
            };
            buttonAssembleOrder.Click += (s, args) =>
            {
                if (dataGridViewOrders.SelectedRows.Count > 0)
                {
                    int selectedOrderId = Convert.ToInt32(dataGridViewOrders.SelectedRows[0].Cells["ID заказа"].Value);
                    AssembleOrder(selectedOrderId);
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите заказ для сборки.");
                }
            };
            contentPanel.Controls.Add(buttonAssembleOrder);
        }

        private void EditOrder(int OrderId)
        {
            // Очистка contentPanel
            contentPanel.Controls.Clear();

            DataRow[] foundRows = FormLogin.ds.Tables["orders"].Select($"[Id] = {OrderId}");
            DataRow orderRow = foundRows.FirstOrDefault();
            if (orderRow == null)
            {
                MessageBox.Show("Выбранный заказ не найден.");
                return;
            }

            // Создание и добавление элементов для редактирования заказа
            Label labelOrderId = new Label { Location = new Point(300, 40), Width = 300, Font = new Font("Ebrima", 14), Text = $"ID заказа: {orderRow["Id"]}" };
            contentPanel.Controls.Add(labelOrderId);

            Label labelCreatedAt = new Label { Location = new Point(300, 70), Width = 300, Font = new Font("Ebrima", 14), Text = "Дата создания заказа" };
            DateTimePicker dateTimePickerCreatedAt = new DateTimePicker
            {
                Location = new Point(300, 100),
                Width = 200,
                Font = new Font("Ebrima", 14),
                Value = orderRow.IsNull("created_at") ? new DateTime(2000, 1, 1) : Convert.ToDateTime(orderRow["created_at"])
            };
            contentPanel.Controls.Add(labelCreatedAt);
            contentPanel.Controls.Add(dateTimePickerCreatedAt);

            Label labelStatus = new Label { Location = new Point(300, 130), Width = 300, Font = new Font("Ebrima", 14), Text = "Статус" };
            TextBox textBoxStatus = new TextBox { Location = new Point(300, 160), Width = 200, Font = new Font("Ebrima", 14), Text = orderRow["status"].ToString() };
            contentPanel.Controls.Add(labelStatus);
            contentPanel.Controls.Add(textBoxStatus);

            Label labelCustomerId = new Label { Location = new Point(300, 190), Width = 300, Font = new Font("Ebrima", 14), Text = "ID клиента" };
            TextBox textBoxCustomerId = new TextBox { Location = new Point(300, 220), Width = 200, Font = new Font("Ebrima", 14), Text = orderRow["customer_id"].ToString() };
            contentPanel.Controls.Add(labelCustomerId);
            contentPanel.Controls.Add(textBoxCustomerId);

            Label labelAssembledAt = new Label { Location = new Point(300, 250), Width = 300, Font = new Font("Ebrima", 14), Text = "Дата сборки заказа" };
            DateTimePicker dateTimePickerAssembledAt = new DateTimePicker
            {
                Location = new Point(300, 280),
                Width = 200,
                Font = new Font("Ebrima", 14),
                Value = orderRow.IsNull("assembled_at") ? new DateTime(2000, 1, 1) : Convert.ToDateTime(orderRow["assembled_at"])
            };
            contentPanel.Controls.Add(labelAssembledAt);
            contentPanel.Controls.Add(dateTimePickerAssembledAt);

            Label labelStorageDeadline = new Label { Location = new Point(300, 310), Width = 300, Font = new Font("Ebrima", 14), Text = "Дата хранения" };
            DateTimePicker dateTimePickerStorageDeadline = new DateTimePicker
            {
                Location = new Point(300, 340),
                Width = 200,
                Font = new Font("Ebrima", 14),
                Value = orderRow.IsNull("storage_deadline") ? new DateTime(2000, 1, 1) : Convert.ToDateTime(orderRow["storage_deadline"])
            };
            contentPanel.Controls.Add(labelStorageDeadline);
            contentPanel.Controls.Add(dateTimePickerStorageDeadline);

            Label labelIssuedAt = new Label { Location = new Point(300, 370), Width = 300, Font = new Font("Ebrima", 14), Text = "Дата выдачи" };
            DateTimePicker dateTimePickerIssuedAt = new DateTimePicker
            {
                Location = new Point(300, 400),
                Width = 200,
                Font = new Font("Ebrima", 14),
                Value = orderRow.IsNull("issued_at") ? new DateTime(2000, 1, 1) : Convert.ToDateTime(orderRow["issued_at"])
            };
            contentPanel.Controls.Add(labelIssuedAt);
            contentPanel.Controls.Add(dateTimePickerIssuedAt);

            Label labelEmployeeId = new Label { Location = new Point(300, 430), Width = 300, Font = new Font("Ebrima", 14), Text = "ID работника" };
            TextBox textBoxEmployeeId = new TextBox { Location = new Point(300, 460), Width = 200, Font = new Font("Ebrima", 14), Text = orderRow["employee_id"].ToString() };
            contentPanel.Controls.Add(labelEmployeeId);
            contentPanel.Controls.Add(textBoxEmployeeId);

            Button buttonSave = new Button
            {
                Text = "Сохранить изменения",
                Location = new Point(400, 540),
                Font = new Font("Ebrima", 14),
                AutoSize = true
            };
            buttonSave.Click += (s, args) =>
            {
                // Обновление данных в DataSet
                orderRow["created_at"] = dateTimePickerCreatedAt.Value.ToString("yyyy-MM-dd");
                orderRow["status"] = textBoxStatus.Text;
                orderRow["customer_id"] = textBoxCustomerId.Text;
                orderRow["assembled_at"] = dateTimePickerAssembledAt.Value.ToString("yyyy-MM-dd");
                orderRow["storage_deadline"] = dateTimePickerStorageDeadline.Value.ToString("yyyy-MM-dd");
                orderRow["issued_at"] = dateTimePickerIssuedAt.Value.ToString("yyyy-MM-dd");
                orderRow["employee_id"] = textBoxEmployeeId.Text;

                try
                {
                    // Обновление данных в базе данных
                    string updateQuery = $"UPDATE orders SET created_at = '{dateTimePickerCreatedAt.Value:yyyy-MM-dd}', status = '{textBoxStatus.Text}', " +
                    $"customer_id = {textBoxCustomerId.Text}, assembled_at = '{dateTimePickerAssembledAt.Value:yyyy-MM-dd}', storage_deadline = '{dateTimePickerStorageDeadline.Value:yyyy-MM-dd}'," +
                    $" issued_at = '{dateTimePickerIssuedAt.Value:yyyy-MM-dd}', employee_id = {textBoxEmployeeId.Text} WHERE id = {OrderId}";

                    FormLogin.Modification_Execute(updateQuery);
                    MessageBox.Show("Изменения успешно сохранены.");

                    // Обновление данных в DataSet после сохранения
                    UpdateDataSet();

                    // Повторное отображение данных редактирования
                    EditOrder(OrderId);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении изменений: {ex.Message}");
                }
            };

            Button buttonCancel = new Button
            {
                Text = "Отмена",
                Location = new Point(200, 540),
                Font = new Font("Ebrima", 14),
                AutoSize = true
            };
            buttonCancel.Click += (s, args) => labelZakaz_Click(null, null);

            contentPanel.Controls.Add(buttonSave);
            contentPanel.Controls.Add(buttonCancel);
        }

        private void AssembleOrder(int orderId)
        {
            // Очистка contentPanel перед добавлением новых элементов
            contentPanel.Controls.Clear();

            // Получение заказа по orderId
            DataRow orderRow = FormLogin.ds.Tables["orders"].Select($"Id = {orderId}").FirstOrDefault();
            if (orderRow == null)
            {
                MessageBox.Show("Выбранный заказ не найден.");
                return;
            }

            // Проверка статуса заказа
            string status = orderRow["Status"].ToString();
            if (status != "В обработке")
            {
                MessageBox.Show("Невозможно собрать заказ. Текущий статус не позволяет этого.");
                return;
            }

            // Получение customer_id из заказа
            int customerId = Convert.ToInt32(orderRow["customer_id"]);

            // Проверка наличия ячейки у клиента
            DataRow customerCart = FormLogin.ds.Tables["cart"].Select($"order_id = {orderId}").FirstOrDefault();
            string storageLocation;
            if (customerCart != null)
            {
                storageLocation = customerCart["storage_location"].ToString();
            }
            else
            {
                // Если у клиента нет ячейки, найти последнюю свободную ячейку
                DataRow lastFreeLocation = FormLogin.ds.Tables["cart"].Select("storage_location IS NOT NULL").OrderBy(r => Convert.ToInt32(r["storage_location"])).LastOrDefault();
                int nextStorageLocation = (lastFreeLocation != null) ? Convert.ToInt32(lastFreeLocation["storage_location"]) + 1 : 1;
                storageLocation = nextStorageLocation.ToString();
            }

            // Создание и добавление элементов для сборки заказа
            Label labelOrderId = new Label { Location = new Point(200, 140), Width = 300, Font = new Font("Ebrima", 14), Text = $"ID заказа: {orderRow["Id"]}" };
            contentPanel.Controls.Add(labelOrderId);

            Label labelStorageLocation = new Label { Location = new Point(200, 170), Width = 300, Font = new Font("Ebrima", 14), Text = $"Ячейка хранения: {storageLocation}" };
            contentPanel.Controls.Add(labelStorageLocation);

            // Создание таблицы для отображения товаров
            // Создание таблицы для отображения товаров
            DataGridView productsGridView = new DataGridView
            {
                Location = new Point(200, 200),
                Width = 400,
                Height = 200,
                Font = new Font("Ebrima", 14),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false, // Запрет добавления новых строк пользователем
                AllowUserToDeleteRows = false, // Запрет удаления строк пользователем
                ReadOnly = true // Таблица только для чтения
            };


            productsGridView.Columns.Clear();
            productsGridView.Columns.Add("ProductName", "Название товара");
            productsGridView.Columns.Add("Quantity", "Количество");

            // Проверка наличия таблицы order_items
            if (FormLogin.ds.Tables["cart"] != null && FormLogin.ds.Tables["products"] != null)
            {
                // Получение товаров, связанных с заказом
                DataRow[] orderItems = FormLogin.ds.Tables["cart"].Select($"order_id = {orderId}");
                foreach (DataRow item in orderItems)
                {
                    int productId = Convert.ToInt32(item["product_id"]);
                    DataRow productRow = FormLogin.ds.Tables["products"].Select($"id = {productId}").FirstOrDefault();
                    if (productRow != null)
                    {
                        string productName = productRow["name"].ToString();
                        int quantity = Convert.ToInt32(item["quantity"]);
                        productsGridView.Rows.Add(productName, quantity);
                    }
                }
            }
            else
            {
                MessageBox.Show("Таблица cart или products не найдена в DataSet.");
            }

            contentPanel.Controls.Add(productsGridView);

            // Кнопка "Ок" для сохранения изменений и обновления базы данных
            Button buttonOk = new Button
            {
                Text = "Ок",
                Location = new Point(525, 420),
                Font = new Font("Ebrima", 14),
                AutoSize = true
            };
            buttonOk.Click += (s, args) =>
            {
                try
                {
                    // Обновление таблицы cart
                    if (customerCart != null)
                    {
                        customerCart["Складское место"] = storageLocation; // Обновление в DataSet
                        string cartQuery = $"UPDATE cart SET Складское место = '{storageLocation}' WHERE order_id = {orderId}";
                        FormLogin.Modification_Execute(cartQuery);
                    }
                    else
                    {
                        DataRow newCartRow = FormLogin.ds.Tables["cart"].NewRow();
                        newCartRow["Id клиента"] = customerId;
                        newCartRow["Складское место"] = storageLocation;
                        newCartRow["Id товара"] = orderRow["Id"];
                        newCartRow["order_id"] = orderId;
                        FormLogin.ds.Tables["cart"].Rows.Add(newCartRow);

                        string cartQuery = $"INSERT INTO cart (customer_id, storage_location, product_id, order_id) VALUES ({customerId}, '{storageLocation}', {orderRow["Id"]}, {orderId})";
                        FormLogin.Modification_Execute(cartQuery);
                    }

                    // Обновление статуса заказа
                    orderRow["Статус"] = "Ожидает получения";
                    string orderQuery = $"UPDATE orders SET status = 'Ожидает получения' WHERE Id = {orderId}";
                    FormLogin.Modification_Execute(orderQuery);

                    // Синхронизация DataSet с базой данных
                    UpdateDataSet();
                    MessageBox.Show("Заказ успешно собран.");
                    labelZakaz_Click(null, null); // Возврат к предыдущей панели с таблицей
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сборке заказа: {ex.Message}");
                }
            };

            Button buttonCancel = new Button
            {
                Text = "Отмена",
                Location = new Point(200, 420),
                Font = new Font("Ebrima", 14),
                AutoSize = true
            };
            buttonCancel.Click += (s, args) => labelZakaz_Click(null, null);

            contentPanel.Controls.Add(buttonOk);
            contentPanel.Controls.Add(buttonCancel);
        }



        private void UpdateDataSet()
        {
            string ordersQuery = "SELECT * FROM orders";
            string cartQuery = "SELECT * FROM cart";
            string productsQuery = "SELECT * FROM products";

            NpgsqlDataAdapter ordersAdapter = new NpgsqlDataAdapter(ordersQuery, FormLogin.connection);
            NpgsqlDataAdapter cartAdapter = new NpgsqlDataAdapter(cartQuery, FormLogin.connection);
            NpgsqlDataAdapter productsAdapter = new NpgsqlDataAdapter(productsQuery, FormLogin.connection);

            FormLogin.ds.Tables["orders"].Clear();
            FormLogin.ds.Tables["cart"].Clear();
            FormLogin.ds.Tables["products"].Clear();

            ordersAdapter.Fill(FormLogin.ds.Tables["orders"]);
            cartAdapter.Fill(FormLogin.ds.Tables["cart"]);
            productsAdapter.Fill(FormLogin.ds.Tables["products"]);
        }




        private void ButtonExport_Click(object sender, EventArgs e)
        {
            // Создание объекта Excel
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();

            // Если Excel не установлен, выход с ошибкой
            if (excelApp == null)
            {
                MessageBox.Show("Excel не установлен на вашем компьютере.");
                return;
            }

            // Создание новой книги Excel
            Microsoft.Office.Interop.Excel.Workbook workbook = excelApp.Workbooks.Add();

            Microsoft.Office.Interop.Excel.Worksheet worksheet = null; // Объявление переменной

            try
            {
                // Создание нового листа в книге
                worksheet = workbook.Sheets[1];
                worksheet.Name = "Отчет по заказам";

                // Заголовки столбцов
                for (int col = 0; col < dataGridViewOrders.Columns.Count; col++)
                {
                    worksheet.Cells[1, col + 1] = dataGridViewOrders.Columns[col].HeaderText;
                }

                // Данные из DataGridView в Excel
                for (int row = 0; row < dataGridViewOrders.Rows.Count; row++)
                {
                    for (int col = 0; col < dataGridViewOrders.Columns.Count; col++)
                    {
                        worksheet.Cells[row + 2, col + 1] = dataGridViewOrders.Rows[row].Cells[col].Value;
                    }
                }

                // Автоизменение размеров столбцов для лучшей читаемости
                worksheet.Columns.AutoFit();

                // Отображение Excel
                excelApp.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте данных в Excel: {ex.Message}");
            }
            finally
            {
                // Освобождение ресурсов Excel
                ReleaseObject(worksheet);
                ReleaseObject(workbook);
                ReleaseObject(excelApp);
            }
        }

        // Метод для освобождения ресурсов COM объектов Excel
        private void ReleaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show($"Ошибка при освобождении объекта: {ex.ToString()}");
            }
            finally
            {
                GC.Collect();
            }
        }

        private Label labelId;
        private TextBox textBoxId;

        private Label labelName;
        private TextBox textBoxName;

        private Label labelManufacturer;
        private TextBox textBoxManufacturer;

        private Label labelPrice;
        private TextBox textBoxPrice;

        private Label labelWeight;
        private TextBox textBoxWeight;

        private Label labelCategory;
        private TextBox textBoxCategory;

        private Label labelDescription;
        private TextBox textBoxDescription;

        private Label labelStock;
        private TextBox textBoxStock;

        private Label labelColor;
        private TextBox textBoxColor;

        private Label labelPackageQuantity;
        private TextBox textBoxPackageQuantity;

        private Button buttonNext;
        private Button buttonPrevious;
        private Button buttonFirst;
        private Button buttonLast;
        private Button buttonSave;
        private Button buttonDelete;

        private int currentRowIndex = -1;

        private void labelOst_Click(object sender, EventArgs e)
        {
            // Обновление DataSet перед отображением
            UpdateDataSet();

            // Очистка contentPanel
            contentPanel.Controls.Clear();

            // Создание и добавление Label и TextBox для каждого поля
            AddLabelAndTextBox("ID", out textBoxId, new Point(225, 50));
            AddLabelAndTextBox("Название", out textBoxName, new Point(225, 80));
            AddLabelAndTextBox("Производитель", out textBoxManufacturer, new Point(225, 110));
            AddLabelAndTextBox("Цена", out textBoxPrice, new Point(225, 140));
            AddLabelAndTextBox("Вес", out textBoxWeight, new Point(225, 170));
            AddLabelAndTextBox("Категория", out textBoxCategory, new Point(225, 200));
            AddLabelAndTextBox("Описание", out textBoxDescription, new Point(225, 230));
            AddLabelAndTextBox("Количество на складе", out textBoxStock, new Point(225, 260));
            AddLabelAndTextBox("Цвет", out textBoxColor, new Point(225, 290));
            AddLabelAndTextBox("Количество в упаковке", out textBoxPackageQuantity, new Point(225, 320));

            // Добавление кнопок для навигации и управления
            AddButton("В начало", new Point(225, 350), new Size(100, 40), (s, args) => MoveFirst());
            AddButton("Назад", new Point(330, 350), new Size(100, 40), (s, args) => MovePrevious());
            AddButton("Вперед", new Point(435, 350), new Size(100, 40), (s, args) => MoveNext());
            AddButton("В конец", new Point(540, 350), new Size(100, 40), (s, args) => MoveLast());

            AddButton("Сохранить", new Point(225, 400), new Size(205, 40), (s, args) => SaveChanges());
            AddButton("Удалить", new Point(435, 400), new Size(205, 40), (s, args) => DeleteProduct());

            AddButton("Добавить новый продукт", new Point(225, 450), new Size(415, 40), (s, args) => AddNewProduct());

            // Показ первого продукта, если доступен
            if (FormLogin.ds.Tables["products"].Rows.Count > 0)
            {
                currentRowIndex = 0;
                UpdateTextBoxesFromDataRow(FormLogin.ds.Tables["products"].Rows[currentRowIndex]);
            }
            else
            {
                ClearTextBoxes();
            }
        }

        private void AddLabelAndTextBox(string labelText, out TextBox textBox, Point location)
        {
            Label label = new Label
            {
                Text = labelText,
                Location = location,
                AutoSize = true
            };
            contentPanel.Controls.Add(label);

            textBox = new TextBox
            {
                Location = new Point(location.X + 150, location.Y),
                Size = new Size(200, 20)
            };
            contentPanel.Controls.Add(textBox);
        }

        private void AddButton(string buttonText, Point location, Size size, EventHandler onClick)
        {
            Button button = new Button
            {
                Text = buttonText,
                Location = location,
                Size = size
            };
            button.Click += onClick;
            contentPanel.Controls.Add(button);
        }

        private void AddNewProduct()
        {
                // Находим максимальный существующий id в таблице
                int maxId = 0;
                foreach (DataRow row in FormLogin.ds.Tables["products"].Rows)
                {
                    int id = Convert.ToInt32(row["id"]);
                    if (id > maxId)
                    {
                        maxId = id;
                    }
                }

                // Создаем новую запись с id на единицу больше максимального
                int newId = maxId + 1;

                // Очищаем TextBox'ы, кроме TextBoxId
                textBoxName.Text = string.Empty;
                textBoxManufacturer.Text = string.Empty;
                textBoxPrice.Text = string.Empty;
                textBoxWeight.Text = string.Empty;
                textBoxCategory.Text = string.Empty;
                textBoxDescription.Text = string.Empty;
                textBoxStock.Text = string.Empty;
                textBoxColor.Text = string.Empty;
                textBoxPackageQuantity.Text = string.Empty;

                textBoxId.Text = newId.ToString();
        }

        private int GetNextAvailableId()
        {
            var maxId = FormLogin.ds.Tables["products"].AsEnumerable()
                        .Select(row => row.Field<int>("id"))
                        .DefaultIfEmpty(0)
                        .Max();
            return maxId + 1;
        }

        private void MoveNext()
        {
            if (currentRowIndex < FormLogin.ds.Tables["products"].Rows.Count - 1)
            {
                currentRowIndex++;
                UpdateTextBoxesFromDataRow(FormLogin.ds.Tables["products"].Rows[currentRowIndex]);
            }
            else
            {
                currentRowIndex = 0;
                UpdateTextBoxesFromDataRow(FormLogin.ds.Tables["products"].Rows[currentRowIndex]);
            }
        }

        private void MovePrevious()
        {
            if (currentRowIndex > 0)
            {
                currentRowIndex--;
                UpdateTextBoxesFromDataRow(FormLogin.ds.Tables["products"].Rows[currentRowIndex]);
            }
            else
            {
                ClearTextBoxes();
                textBoxId.Text = GetNextAvailableId().ToString();
            }
        }

        private void MoveLast()
        {
            currentRowIndex = FormLogin.ds.Tables["products"].Rows.Count - 1;
            UpdateTextBoxesFromDataRow(FormLogin.ds.Tables["products"].Rows[currentRowIndex]);
        }

        private void MoveFirst()
        {
            if (FormLogin.ds.Tables["products"].Rows.Count > 0)
            {
                currentRowIndex = 0;
                UpdateTextBoxesFromDataRow(FormLogin.ds.Tables["products"].Rows[currentRowIndex]);
            }
            else
            {
                ClearTextBoxes();
                textBoxId.Text = GetNextAvailableId().ToString();
            }
        }
        private void SaveChanges()
        {
            try
            {
                // Добавление новой записи в DataSet
                DataRow newRow = FormLogin.ds.Tables["products"].NewRow();
                newRow["id"] = Convert.ToInt32(textBoxId.Text); // Предполагаю, что ID вводится пользователем
                newRow["name"] = textBoxName.Text;
                newRow["manufacturer"] = textBoxManufacturer.Text;
                newRow["price"] = Convert.ToDecimal(textBoxPrice.Text);
                newRow["weight"] = Convert.ToDecimal(textBoxWeight.Text);
                newRow["type"] = textBoxCategory.Text;
                newRow["description"] = textBoxDescription.Text;
                newRow["stock"] = Convert.ToInt32(textBoxStock.Text);
                newRow["color"] = textBoxColor.Text;
                newRow["package_quantity"] = Convert.ToInt32(textBoxPackageQuantity.Text);

                FormLogin.ds.Tables["products"].Rows.Add(newRow);

                // Формирование SQL-запроса для вставки новой записи в базу данных
                string insertQuery = $"INSERT INTO products (id, name, manufacturer, price, weight, type, description, stock, color, package_quantity) " +
                                     $"VALUES ({newRow["id"]}, '{newRow["name"]}', '{newRow["manufacturer"]}', " +
                                     $"{newRow["price"]}, {newRow["weight"]}, '{newRow["type"]}', '{newRow["description"]}', " +
                                     $"{newRow["stock"]}, '{newRow["color"]}', {newRow["package_quantity"]})";

                // Выполнение SQL-запроса
                if (FormLogin.Modification_Execute(insertQuery))
                {
                    MessageBox.Show("Новый продукт успешно добавлен.");
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении нового продукта в базу данных.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }


        private void DeleteProduct()
        {
            if (currentRowIndex >= 0 && currentRowIndex < FormLogin.ds.Tables["products"].Rows.Count)
            {
                DataRow row = FormLogin.ds.Tables["products"].Rows[currentRowIndex];

                // Формирование SQL-запроса для удаления записи из базы данных
                string query = $"DELETE FROM products WHERE id = {row["id"]}";

                // Удаление продукта из DataSet
                row.Delete();

                // Выполнение SQL-запроса
                FormLogin.Modification_Execute(query);

                MessageBox.Show("Запись успешно удалена.");

                // Переход к следующему продукту или очистка TextBox, если продуктов не осталось
                if (currentRowIndex < FormLogin.ds.Tables["products"].Rows.Count)
                {
                    UpdateTextBoxesFromDataRow(FormLogin.ds.Tables["products"].Rows[currentRowIndex]);
                }
                else
                {
                    ClearTextBoxes();
                }
            }
        }
        private void UpdateTextBoxesFromDataRow(DataRow row)
        {
            if (row != null && row.RowState != DataRowState.Detached && row.RowState != DataRowState.Deleted)
            {
                textBoxId.Text = row["id"].ToString();
                textBoxName.Text = row["name"].ToString();
                textBoxManufacturer.Text = row["manufacturer"].ToString();
                textBoxPrice.Text = row["price"].ToString();
                textBoxWeight.Text = row["weight"].ToString();
                textBoxCategory.Text = row["type"].ToString();
                textBoxDescription.Text = row["description"].ToString();
                textBoxStock.Text = row["stock"].ToString();
                textBoxColor.Text = row["color"].ToString();
                textBoxPackageQuantity.Text = row["package_quantity"].ToString();
            }
            else
            {
                ClearTextBoxes(); 
            }
        }
        private void ClearTextBoxes()
        {
            textBoxId.Text = string.Empty;
            textBoxName.Text = string.Empty;
            textBoxManufacturer.Text = string.Empty;
            textBoxPrice.Text = string.Empty;
            textBoxWeight.Text = string.Empty;
            textBoxCategory.Text = string.Empty;
            textBoxDescription.Text = string.Empty;
            textBoxStock.Text = string.Empty;
            textBoxColor.Text = string.Empty;
            textBoxPackageQuantity.Text = string.Empty;
        }

        private void labelExit_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }
    }
}
