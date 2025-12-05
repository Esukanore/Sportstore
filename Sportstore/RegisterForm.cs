using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sportstore
{
    public partial class RegisterForm : Form
    {
        private string connectionString = @"Data Source=localhost;Initial Catalog=sport;Integrated Security=True";
        public RegisterForm()
        {
            InitializeComponent();
            lblMessage.Text = "";
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RegisterForm_Load(object sender, EventArgs e)
        {

        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            {
                string fullName = txtFullName.Text.Trim();
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text;
                string confirmPassword = txtConfirmPassword.Text;

                // Валидация
                if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(username) ||
                    string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
                {
                    lblMessage.Text = "Все поля обязательны для заполнения!";
                    return;
                }

                if (password != confirmPassword)
                {
                    lblMessage.Text = "Пароли не совпадают!";
                    return;
                }

                if (password.Length < 3)
                {
                    lblMessage.Text = "Пароль должен содержать минимум 3 символа!";
                    return;
                }

                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        // Проверка существующего пользователя
                        string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @username";
                        using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                        {
                            checkCmd.Parameters.AddWithValue("@username", username);
                            int count = (int)checkCmd.ExecuteScalar();

                            if (count > 0)
                            {
                                lblMessage.Text = "Пользователь с таким логином уже существует!";
                                return;
                            }
                        }

                        // Регистрация нового пользователя как Клиента
                        string insertQuery = @"INSERT INTO Users (FullName, Username, Password, UserRole) 
                                          VALUES (@fullName, @username, @password, 'Клиент')";

                        using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@fullName", fullName);
                            cmd.Parameters.AddWithValue("@username", username);
                            cmd.Parameters.AddWithValue("@password", password);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Регистрация успешна! Теперь вы можете войти.",
                                              "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                this.Close();
                            }
                            else
                            {
                                lblMessage.Text = "Ошибка регистрации!";
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    lblMessage.Text = $"Ошибка базы данных: {ex.Message}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}",
                                  "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
