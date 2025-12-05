using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Sportstore.AdminForm;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Menu;

namespace Sportstore
{
    public partial class ClientForm : Form
    {
        private void LoadCategoriesForFilter()
        {
            if (cmbFilterCategory == null) return;

            cmbFilterCategory.Items.Clear();
            cmbFilterCategory.Items.Add("Все категории");

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT DISTINCT Category FROM Products WHERE Category IS NOT NULL AND Category != '' ORDER BY Category";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cmbFilterCategory.Items.Add(reader["Category"].ToString());
                        }
                    }
                }
                cmbFilterCategory.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки категорий: {ex.Message}");
            }
        }
        private string connectionString = @"Data Source=localhost;Initial Catalog=sport;Integrated Security=True";
        private string currentUser;
        private string userRole;
        private int userId;
        private List<Product> products = new List<Product>();
        private List<CartItem> cart = new List<CartItem>();
        public class Product
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public string Category { get; set; }
            public int Stock { get; set; }
            public byte[] ImageData { get; set; }
            public string ImageType { get; set; }
        }

        public class CartItem
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
            public decimal Total => Price * Quantity;
        }
        public ClientForm()
        {
            InitializeComponent();
        }

        private void ClientForm_Load(object sender, EventArgs e)
        {
            LoadCart();
        }
        public ClientForm(string fullName, string role) : this()
        {
            currentUser = fullName;
            userRole = role;

            if (role == "Гость")
            {
                this.Text = "Просмотр товаров (Гость)";
                lblUserInfo.Text = "Гость";

                // Прячем ВСЕ элементы корзины
                lstCart.Visible = false;

                // Скрываем label5 (где сумма)
                if (lblCartTotal != null)
                    lblCartTotal.Visible = false;

                // Скрываем кнопки оформления и очистки
                if (btnCheckout != null)
                    btnCheckout.Visible = false;
                if (btnClearCart != null)
                    btnClearCart.Visible = false;
            }
            else
            {
                this.Text = $"Просмотр товаров - {fullName}";
                lblUserInfo.Text = $"{role}: {fullName}";
                userId = GetUserId(fullName);
                CheckDatabaseStructure();
                LoadCart(); // ТОЛЬКО для клиентов
            }

            InitializeSortComboBox();
            LoadCategoriesForFilter();
            LoadProducts();
            UpdateProductsList();

            if (cmbFilterCategory != null)
                cmbFilterCategory.SelectedIndexChanged += cmbFilterCategory_SelectedIndexChanged;
        }
        private void CheckDatabaseStructure()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Проверяем таблицу Cart
                    string checkCart = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Cart'";
                    using (SqlCommand cmd = new SqlCommand(checkCart, conn))
                    {
                        int cartExists = Convert.ToInt32(cmd.ExecuteScalar());
                        if (cartExists == 0)
                        {
                            MessageBox.Show("Таблица Cart не найдена в БД! Создайте таблицу через SQL запрос.",
                                          "Ошибка БД", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    // Проверяем своего пользователя
                    string checkUser = "SELECT COUNT(*) FROM Users WHERE Id = @userId";
                    using (SqlCommand cmd = new SqlCommand(checkUser, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        int userExists = Convert.ToInt32(cmd.ExecuteScalar());
                        if (userExists == 0)
                        {
                            MessageBox.Show("Пользователь не найден в БД!",
                                          "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка проверки БД: {ex.Message}");
            }
        }
        private int GetUserId(string fullName)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT Id FROM Users WHERE FullName = @fullName";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@fullName", fullName);
                        object result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : 0;
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        private void InitializeSortComboBox()
        {
            if (cmbSortBy != null)
            {
                cmbSortBy.Items.Clear();
                cmbSortBy.Items.AddRange(new string[] {
                    "По умолчанию",
                    "По цене (возр.)",
                    "По цене (убыв.)",
                    "По названию"
                });
                cmbSortBy.SelectedIndex = 0;
            }
        }

        private void LoadProducts(string search = "")
        {
            products.Clear();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT * FROM Products 
                                   WHERE (@search = '' OR Name LIKE @searchPattern OR Description LIKE @searchPattern)";

                    query += GetSortingQuery();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@search", search);
                        cmd.Parameters.AddWithValue("@searchPattern", $"%{search}%");

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Product product = new Product
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Name = reader["Name"].ToString(),
                                    Description = reader["Description"].ToString(),
                                    Price = Convert.ToDecimal(reader["Price"]),
                                    Category = reader["Category"]?.ToString() ?? "",
                                    Stock = Convert.ToInt32(reader["Stock"]),
                                };

                                if (reader["ImageData"] != DBNull.Value)
                                {
                                    product.ImageData = (byte[])reader["ImageData"];
                                    product.ImageType = reader["ImageType"].ToString();
                                }

                                products.Add(product);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки товаров: {ex.Message}");
            }
        }

        private string GetSortingQuery()
        {
            if (cmbSortBy == null || cmbSortBy.SelectedItem == null)
                return " ORDER BY Id DESC";

            switch (cmbSortBy.SelectedItem.ToString())
            {
                case "По цене (возр.)":
                    return " ORDER BY Price ASC";
                case "По цене (убыв.)":
                    return " ORDER BY Price DESC";
                case "По названию":
                    return " ORDER BY Name ASC";
                default:
                    return " ORDER BY Id DESC";
            }
        }

        private void UpdateProductsList()
        {
            flowProducts.Controls.Clear();

            foreach (var product in products)
            {
                AddProductCard(product);
            }
        }

        private void AddProductCard(Product product)
        {
            Panel productPanel = new Panel
            {
                Width = flowProducts.Width - 25,
                Height = 120,
                Margin = new Padding(0, 5, 0, 5),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Tag = product.Id
            };

            // Изображение товара
            PictureBox pic = new PictureBox
            {
                Location = new Point(10, 10),
                Size = new Size(100, 100),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle
            };

            if (product.ImageData != null && product.ImageData.Length > 0)
            {
                using (MemoryStream ms = new MemoryStream(product.ImageData))
                {
                    pic.Image = Image.FromStream(ms);
                }
            }
            else
            {
                pic.Image = CreateDefaultImage();
            }

            int rightColumnX = productPanel.Width - 160;

            Label lblTitle = new Label
            {
                Text = product.Name,
                Font = new Font("Arial", 11, FontStyle.Bold),
                Location = new Point(120, 10),
                Size = new Size(rightColumnX - 130, 25),
                ForeColor = Color.DarkBlue,
                TextAlign = ContentAlignment.TopLeft
            };

            // Описание
            string shortDesc = product.Description.Length > 60
                ? product.Description.Substring(0, 60) + "..."
                : product.Description;

            Label lblDesc = new Label
            {
                Text = shortDesc,
                Location = new Point(120, 35),
                Size = new Size(rightColumnX - 130, 30),
                ForeColor = Color.DarkGray,
                TextAlign = ContentAlignment.TopLeft
            };

            // Категория
            Label lblCategory = new Label
            {
                Text = product.Category,
                Location = new Point(120, 65),
                Size = new Size(rightColumnX - 130, 20),
                Font = new Font("Arial", 9, FontStyle.Italic),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.TopLeft
            };

            // Цена
            Label lblPrice = new Label
            {
                Text = $"{product.Price:N2} руб.",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Location = new Point(rightColumnX, 10),
                Size = new Size(150, 20),
                ForeColor = Color.Green,
                TextAlign = ContentAlignment.TopRight
            };

            // Наличие
            Label lblStock = new Label
            {
                Text = $"В наличии: {product.Stock} шт.",
                Location = new Point(rightColumnX, 40),
                Size = new Size(150, 20),
                Font = new Font("Arial", 9),
                TextAlign = ContentAlignment.TopRight
            };

            // Разделительная линия (сдвигаем в зависимости от наличия кнопки)
            int separatorY = 95;

            // Кнопка "В корзину" - ТОЛЬКО ДЛЯ НЕ ГОСТЕЙ
            if (userRole != "Гость")
            {
                Button btnAddToCart = new Button
                {
                    Text = "В корзину",
                    Location = new Point(rightColumnX, 70),
                    Size = new Size(150, 30),
                    Tag = product.Id,
                    BackColor = Color.LightGreen
                };
                btnAddToCart.Click += (s, e) =>
                {
                    AddToCart(product.Id);
                };
                productPanel.Controls.Add(btnAddToCart);
            }
            else
            {
                // Для гостя делаем наличие и цену ниже, так как нет кнопки
                lblPrice.Location = new Point(rightColumnX, 20);
                lblStock.Location = new Point(rightColumnX, 50);
                separatorY = 80; // Разделитель выше
            }

            // Разделительная линия
            Panel separator = new Panel
            {
                Location = new Point(120, separatorY),
                Size = new Size(productPanel.Width - 130, 1),
                BackColor = Color.LightGray
            };

            // Добавляем элементы (кнопка добавлялась выше только для не-гостей)
            productPanel.Controls.Add(pic);
            productPanel.Controls.Add(lblTitle);
            productPanel.Controls.Add(lblDesc);
            productPanel.Controls.Add(lblCategory);
            productPanel.Controls.Add(lblPrice);
            productPanel.Controls.Add(lblStock);
            productPanel.Controls.Add(separator);

            flowProducts.Controls.Add(productPanel);
        }

        private Image CreateDefaultImage()
        {
            Bitmap bmp = new Bitmap(100, 100);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.LightGray);
                g.DrawRectangle(Pens.DarkGray, 0, 0, 99, 99);

                using (Font font = new Font("Arial", 10, FontStyle.Bold))
                {
                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;

                    g.DrawString("Нет\nфото", font, Brushes.DarkGray,
                                new Rectangle(0, 0, 100, 100), sf);
                }
            }
            return bmp;
        }

        // Метод добавления в корзину
        private void AddToCart(int productId)
        {
            var product = products.FirstOrDefault(p => p.Id == productId);
            if (product == null) return;

            // Проверяем есть ли на складе
            if (product.Stock <= 0)
            {
                MessageBox.Show($"Товар \"{product.Name}\" отсутствует на складе!",
                              "Нет в наличии", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Проверяем есть ли уже в корзине
            var cartItem = cart.FirstOrDefault(c => c.ProductId == productId);

            if (cartItem != null)
            {
                // Проверяем не превышает ли количество доступное
                if (cartItem.Quantity >= product.Stock)
                {
                    MessageBox.Show($"Нельзя добавить больше {product.Stock} шт. товара \"{product.Name}\"!",
                                  "Превышен лимит", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                cartItem.Quantity++;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = 1
                });
            }

            // ОДИН раз сохраняем в БД (не дублируем)
            SaveCartToDatabase();
            UpdateCartDisplay();

            MessageBox.Show($"Товар \"{product.Name}\" добавлен в корзину!",
                          "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Отображение корзины
        private void UpdateCartDisplay()
        {
            lstCart.Items.Clear();

            decimal total = 0;
            foreach (var item in cart)
            {
                string itemText = $"{item.ProductName} x{item.Quantity} = {item.Total:N2} руб.";
                lstCart.Items.Add(itemText);
                total += item.Total;
            }

            lblCartTotal.Text = $"Итого: {total:N2} руб.";

            // Сохраняем в БД только если есть изменения
            // (убрали отсюда вызов SaveCartToDatabase)
        }

        // Улучшенное сохранение корзины в БД
        private void SaveCartToDatabase()
        {
            if (userId == 0 || userRole == "Гость" || cart.Count == 0) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Используем транзакцию для безопасности данных
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Удаляем старую корзину пользователя
                            string deleteQuery = "DELETE FROM Cart WHERE UserId = @userId";
                            using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn, transaction))
                            {
                                deleteCmd.Parameters.AddWithValue("@userId", userId);
                                deleteCmd.ExecuteNonQuery();
                            }

                            // Добавляем новую корзину БАТЧЕМ (пакетно)
                            foreach (var item in cart)
                            {
                                string insertQuery = @"INSERT INTO Cart (UserId, ProductId, Quantity, AddedDate) 
                                             VALUES (@userId, @productId, @quantity, GETDATE())";
                                using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn, transaction))
                                {
                                    insertCmd.Parameters.AddWithValue("@userId", userId);
                                    insertCmd.Parameters.AddWithValue("@productId", item.ProductId);
                                    insertCmd.Parameters.AddWithValue("@quantity", item.Quantity);
                                    insertCmd.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception("Ошибка транзакции: " + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения корзины: {ex.Message}");
            }
        }

        // Загрузка корзины из БД (оставить как есть, работает нормально)
        private void LoadCart()
        {
            if (userId == 0) return;

            cart.Clear();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT c.*, p.Name, p.Price 
                           FROM Cart c
                           JOIN Products p ON c.ProductId = p.Id
                           WHERE c.UserId = @userId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cart.Add(new CartItem
                                {
                                    ProductId = Convert.ToInt32(reader["ProductId"]),
                                    ProductName = reader["Name"].ToString(),
                                    Price = Convert.ToDecimal(reader["Price"]),
                                    Quantity = Convert.ToInt32(reader["Quantity"])
                                });
                            }
                        }
                    }
                }

                UpdateCartDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки корзины: {ex.Message}");
            }
        }


        private void gbSelection_Enter(object sender, EventArgs e)
        {

        }

        private void btnAddToCart_Click(object sender, EventArgs e)
        {

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            {
                LoadProducts(txtSearch?.Text ?? "");
                UpdateProductsList();
            }
        }

        private void cmbSortBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadProducts(txtSearch?.Text ?? "");
            UpdateProductsList();
        }

        private void btnCheckout_Click(object sender, EventArgs e)
        {
            {
                if (cart.Count == 0)
                {
                    MessageBox.Show("Корзина пуста!", "Внимание",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                decimal total = cart.Sum(item => item.Total);

                if (MessageBox.Show($"Оформить заказ на сумму {total:N2} руб.?",
                    "Подтверждение заказа", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // Здесь можно добавить логику оформления заказа
                    MessageBox.Show("Заказ оформлен успешно! Спасибо за покупку.",
                                  "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Очищаем корзину
                    cart.Clear();
                    UpdateCartDisplay();
                }
            }
        }

        private void btnClearCart_Click(object sender, EventArgs e)
        {
            {
                if (cart.Count == 0) return;

                if (MessageBox.Show("Очистить корзину?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cart.Clear();
                    UpdateCartDisplay();
                }
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            {
                LoginForm loginForm = new LoginForm();
                loginForm.Show();
                this.Close();
            }
        }

        private void ClientForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            {
                if (!Application.OpenForms.OfType<LoginForm>().Any())
                {
                    Application.Exit();
                }
            }
        }

        private void lstCart_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmbFilterCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadProducts(txtSearch?.Text ?? "");
            UpdateProductsList();
        }
    }
}
