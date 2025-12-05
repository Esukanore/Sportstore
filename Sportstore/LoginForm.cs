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
    public partial class LoginForm : Form
    {
            
        private string connectionString = @"Data Source=localhost;Initial Catalog=sport;Integrated Security=True";
        private int failedAttempts = 0;
        private string captchaText = "";
        private DateTime? blockUntil = null;
        private Timer blockTimer;

        private void InitializeTimer()
        {
            blockTimer = new Timer();
            blockTimer.Interval = 1000;
            blockTimer.Tick += BlockTimer_Tick;
        }

        private void BlockTimer_Tick(object sender, EventArgs e)
        {
            if (blockUntil.HasValue && DateTime.Now >= blockUntil.Value)
            {
                blockUntil = null;
                blockTimer.Stop();
                UpdateUIBlockStatus(false);
            }
        }

        private void UpdateUIBlockStatus(bool isBlocked)
        {
            btnLogin.Enabled = !isBlocked;
            btnGuest.Enabled = !isBlocked;
            btnRegister.Enabled = !isBlocked;

            if (isBlocked)
            {
                btnLogin.Text = "Заблокировано";
            }
            else
            {
                btnLogin.Text = "Войти";
                lblMessage.Text = "";
            }
        }
        public LoginForm()
        {
            InitializeComponent();
            InitializeTimer();
            // Скрываем CAPTCHA при запуске
            pnlCaptcha.Visible = false;
            lblMessage.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            GenerateCaptcha();
        }
        private void GenerateCaptcha()
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            captchaText = "";

            for (int i = 0; i < 4; i++)
            {
                captchaText += chars[random.Next(chars.Length)];
            }

            // Создаем изображение CAPTCHA
            Bitmap bmp = new Bitmap(picCaptcha.Width, picCaptcha.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);

                // Добавляем шум
                for (int i = 0; i < 100; i++)
                {
                    int x = random.Next(bmp.Width);
                    int y = random.Next(bmp.Height);
                    bmp.SetPixel(x, y, Color.LightGray);
                }

                // Рисуем текст
                Font font = new Font("Arial", 18, FontStyle.Bold);
                for (int i = 0; i < captchaText.Length; i++)
                {
                    float x = 10 + i * 30 + random.Next(-5, 5);
                    float y = 5 + random.Next(-5, 5);

                    g.TranslateTransform(x, y);
                    g.RotateTransform(random.Next(-15, 15));

                    g.DrawString(captchaText[i].ToString(), font,
                                Brushes.Black, -10, -10);

                    g.ResetTransform();
                }

                // Добавляем линии
                for (int i = 0; i < 3; i++)
                {
                    g.DrawLine(Pens.Red,
                              random.Next(bmp.Width), random.Next(bmp.Height),
                              random.Next(bmp.Width), random.Next(bmp.Height));
                }
            }

            picCaptcha.Image = bmp;
            txtCaptcha.Text = "";
        }
        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void lblMessage_Click(object sender, EventArgs e)
        {

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            {
                // Проверка блокировки
                if (blockUntil.HasValue)
                {
                    lblMessage.Text = $"Система заблокирована до {blockUntil.Value:HH:mm:ss}";
                    return;
                }

                // Проверка CAPTCHA после первой неудачной попытки
                if (failedAttempts >= 1 && pnlCaptcha.Visible)
                {
                    if (txtCaptcha.Text.ToUpper() != captchaText)
                    {
                        lblMessage.Text = "Неверная CAPTCHA! Попробуйте снова.";
                        GenerateCaptcha();
                        return;
                    }
                }

                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text.Trim();

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    lblMessage.Text = "Введите логин и пароль!";
                    return;
                }

                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "SELECT FullName, UserRole FROM Users WHERE Username = @username AND Password = @password";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@username", username);
                            cmd.Parameters.AddWithValue("@password", password);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    string fullName = reader["FullName"].ToString();
                                    string userRole = reader["UserRole"].ToString();

                                    // Сброс счетчика неудачных попыток
                                    failedAttempts = 0;
                                    pnlCaptcha.Visible = false;

                                    // Открываем соответствующую форму в зависимости от роли
                                    if (userRole == "Администратор")
                                    {
                                        AdminForm adminForm = new AdminForm(fullName);
                                        adminForm.Show();
                                    }
                                    else if (userRole == "Клиент" || userRole == "Менеджер")
                                    {
                                        ClientForm clientForm = new ClientForm(fullName, userRole);
                                        clientForm.Show();
                                    }

                                    this.Hide(); // Скрываем форму входа
                                }
                                else
                                {
                                    failedAttempts++;

                                    if (failedAttempts == 1)
                                    {
                                        lblMessage.Text = "Неверный логин или пароль! Введите CAPTCHA.";
                                        pnlCaptcha.Visible = true;
                                        GenerateCaptcha();
                                    }
                                    else if (failedAttempts >= 2)
                                    {
                                        // Блокировка на 10 секунд
                                        blockUntil = DateTime.Now.AddSeconds(10);
                                        blockTimer.Start();
                                        UpdateUIBlockStatus(true);
                                        lblMessage.Text = $"Неверная авторизация! Система заблокирована на 10 секунд.";
                                    }
                                    else
                                    {
                                        lblMessage.Text = "Неверный логин или пароль!";
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка подключения: {ex.Message}");
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            {
                // Тестовые данные для удобства
                txtUsername.Text = "admin";
                txtPassword.Text = "admin";
            }
        }

        private void btnGuest_Click(object sender, EventArgs e)
        {
            {
                ClientForm guestForm = new ClientForm("Гость", "Гость");
                guestForm.Show();
                this.Hide();
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            {
                RegisterForm registerForm = new RegisterForm();
                registerForm.ShowDialog();
            }
        }
    }
}
