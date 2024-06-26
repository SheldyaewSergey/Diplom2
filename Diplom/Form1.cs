using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace Diplom
{
    public partial class FormLogin : Form
    {
        // Переменная для хранения id вошедшего работника
        public static int LoggedInEmployeeId;

        public static NpgsqlConnection connection =
            new NpgsqlConnection("Server=localhost; Port=5432; User Id=postgres; Password=6387; Database=diplommm;");
        public static DataSet ds = new DataSet();

        public FormLogin()
        {
            InitializeComponent();
        }

        // Этот метод загружает данные из базы данных в таблицу внутри DataSet с указанным именем.
        // Если таблица с таким именем уже существует в DataSet, она очищается перед загрузкой новых данных.
        public static void Table_Fill(string name, string sql)
        {
            if (ds.Tables[name] != null)
                ds.Tables[name].Clear();
            NpgsqlDataAdapter dat = new NpgsqlDataAdapter(sql, connection);
            dat.Fill(ds, name);
            connection.Close();
        }

        // Этот метод загружает измененные данные в БД
        public static bool Modification_Execute(string sql)
        {
            NpgsqlCommand com = new NpgsqlCommand(sql, connection);
            connection.Open();
            try
            {
                com.ExecuteNonQuery();
            }
            catch (NpgsqlException)
            {
                MessageBox.Show("Обновление базы данных не было выполнено либо из-за некорректно указанных" +
                    " обновляемых данных либо отсутствующих, но при этом обязательных!!!", "Ошибка");
                connection.Close();
                return false;
            }
            connection.Close();
            return true;
        }

        // Метод для авторизации пользователя и загрузки данных
        private bool AuthenticateUser(string login, string password)
        {
            try
            {
                connection.Open();
                string query = "SELECT id FROM employees WHERE login = @login AND password = @password";
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("login", login);
                    cmd.Parameters.AddWithValue("password", password);
                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        // Присваиваем id вошедшего работника переменной LoggedInEmployeeId
                        LoggedInEmployeeId = Convert.ToInt32(result);

                        // Получить данные из всех таблиц
                        Table_Fill("customers", "SELECT id as \"Id\", full_name as \"ФИО\", passport_number as \"Паспортные данные\", " +
                                                "birth_date as \"Дата рождения\", phone_number as \"Номер телефона\", email as \"Электронная почта\", bank_card_number as \"Номер карты\" FROM customers");
                        ds.Tables["customers"].DefaultView.Sort = "id";

                        Table_Fill("employees", "SELECT id as \"Id\", full_name as \"Полное имя\", phone_number as \"Номер телефона\", birth_date as \"Дата рождения\", " +
                                                "passport_details as \"Номер паспорта\", position as \"Должность\", contract_number as \"Номер договора\", " +
                                                "employment_date as \"Дата приема на работу\", dismissal_date as \"Дата увольнения\", dismissal_reason as \"Причина увольнения\", " +
                                                "notes as \"Заметки\", login as \"Логин\", password as \"Пароль\" FROM employees");
                        ds.Tables["employees"].DefaultView.Sort = "id";

                        Table_Fill("orders", "SELECT id as \"Id\", created_at as \"Дата создания заказа\", status as \"Статус\", customer_id as \"Id клиента\", assembled_at as \"Дата сборки заказа\"" +
                            ", storage_deadline as \"Дата хранения\", issued_at as \"Дата выдачи\", employee_id as \"Id работника\" FROM orders");
                        ds.Tables["orders"].DefaultView.Sort = "id";

                        Table_Fill("products", "SELECT id as \"Id\", name as \"Название\", manufacturer as \"Производитель\", price as \"Цена\", weight as \"Вес\", " +
                                               "type as \"Категория\", description as \"Описание\", stock as \"Количество на складе\", color as \"Цвет\" , package_quantity as \"Количество в упаковке\" FROM products");
                        ds.Tables["products"].DefaultView.Sort = "id";

                        Table_Fill("cart", "SELECT id as \"Id\", order_id as \"Id заказа\", product_id as \"Id товара\", quantity as \"Количество\", " +
                                                 " customer_id as \"Id заказчика\", storage_location as \"Складское место\" FROM cart");
                        ds.Tables["cart"].DefaultView.Sort = "id";

                        Table_Fill("returns", "SELECT id as \"Id\", order_id as \"Id заказа\", return_reason as \"Причина\", product_id as \"Id товара\" FROM returns");
                        ds.Tables["returns"].DefaultView.Sort = "id";

                        Table_Fill("logs", "SELECT id as \"Id\", table_name as \"Название таблицы\", record_id as \"Id записи\", change_timestamp as \"Время изменения\", " +
                                           "change_description as \"Описание изменения\" FROM logs");
                        ds.Tables["logs"].DefaultView.Sort = "id";

                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Неправильный логин или пароль");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при подключении к базе данных: " + ex.Message);
                return false;
            }
            finally
            {
                connection.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string login = textBoxLogin.Text;
            string password = textBoxPassword.Text;

            if (AuthenticateUser(login, password))
            {
                // Открыть FormMain и закрыть текущую форму
                FormMain formMain = new FormMain();
                formMain.Show();
                this.Hide();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
                textBoxPassword.UseSystemPasswordChar = false;
            else
                textBoxPassword.UseSystemPasswordChar = true;
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            textBoxPassword.UseSystemPasswordChar = true;
        }
    }
}
