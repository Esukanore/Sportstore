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

namespace Sportstore
{
    public partial class AdminForm : Form
    {
        private string connectionString = @"Data Source=localhost;Initial Catalog=sport;Integrated Security=True";
        private string currentUser;
        private List<Product> products = new List<Product>();
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
            public bool IsSelected { get; set; }
        }
        public AdminForm()
        {
            InitializeComponent();
        }
        private void LoadProducts(string search = "")
        {
            products.Clear();
            if (clbProducts != null)
            {
                clbProducts.Items.Clear();
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Базовый запрос
                    string query = @"SELECT * FROM Products 
                           WHERE (@search = '' OR Name LIKE @searchPattern OR Description LIKE @searchPattern)";

                    // Добавляем фильтр по категории если выбран
                    if (cmbFilterCategory != null && cmbFilterCategory.SelectedIndex > 0 &&
                        cmbFilterCategory.SelectedItem.ToString() != "Все категории")
                    {
                        query += " AND Category = @category";
                    }

                    // Добавляем сортировку
                    query += GetSortingQuery();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@search", search);
                        cmd.Parameters.AddWithValue("@searchPattern", $"%{search}%");

                        if (cmbFilterCategory != null && cmbFilterCategory.SelectedIndex > 0 &&
                            cmbFilterCategory.SelectedItem.ToString() != "Все категории")
                        {
                            cmd.Parameters.AddWithValue("@category", cmbFilterCategory.Text);
                        }

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
                                    IsSelected = false
                                };

                                if (reader["ImageData"] != DBNull.Value)
                                {
                                    product.ImageData = (byte[])reader["ImageData"];
                                    product.ImageType = reader["ImageType"].ToString();
                                }

                                products.Add(product);

                                if (clbProducts != null)
                                {
                                    clbProducts.Items.Add($"{product.Id}: {product.Name}", false);
                                }
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

        // Метод для получения строки сортировки
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
                case "По количеству":
                    return " ORDER BY Stock DESC";
                default:
                    return " ORDER BY Id DESC";
            }
        }

        // СОБЫТИЕ: Изменение фильтра
        private void cmbFilterCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadProducts(txtSearch?.Text ?? "");
            UpdateProductsList();
        }

        // СОБЫТИЕ: Изменение сортировки
        private void cmbSortBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadProducts(txtSearch?.Text ?? "");
            UpdateProductsList();
        }
        // Обновление списка товаров в левой части
        private void UpdateProductsList()
        {
            flowProducts.Controls.Clear();

            foreach (var product in products)
            {
                AddProductCard(product);
            }
        }

        // Создание карточки товара (как на скрине)
        private void AddProductCard(Product product)
        {
            // Основная панель товара - УВЕЛИЧИВАЕМ ШИРИНУ
            Panel productPanel = new Panel
            {
                Width = flowProducts.Width - 25,
                Height = 120,
                Margin = new Padding(0, 5, 0, 5),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Tag = product.Id
            };

            // CheckBox для выбора
            CheckBox chkSelect = new CheckBox
            {
                Location = new Point(10, 50),
                Size = new Size(20, 20),
                Tag = product.Id,
                Checked = product.IsSelected
            };
            chkSelect.CheckedChanged += (s, e) =>
            {
                product.IsSelected = chkSelect.Checked;
                UpdateCheckedListBox();
            };

            // Изображение товара
            PictureBox pic = new PictureBox
            {
                Location = new Point(40, 10),
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

            // РАССЧИТЫВАЕМ ПОЗИЦИИ ДИНАМИЧЕСКИ
            int leftColumnX = 150;   // Левая колонка (название, описание, категория)
            int rightColumnX = productPanel.Width - 120; // Правая колонка (цена, наличие)

            // Название товара - ФИКСИРОВАННАЯ ШИРИНА
            Label lblTitle = new Label
            {
                Text = product.Name,
                Font = new Font("Arial", 11, FontStyle.Bold),
                Location = new Point(leftColumnX, 10),
                Size = new Size(rightColumnX - leftColumnX - 10, 25), // Фиксированная высота
                ForeColor = Color.DarkBlue,
                TextAlign = ContentAlignment.TopLeft
            };

            // Описание - ПРАВИЛЬНОЕ ОГРАНИЧЕНИЕ
            string shortDesc = product.Description.Length > 60
                ? product.Description.Substring(0, 60) + "..."
                : product.Description;

            Label lblDesc = new Label
            {
                Text = shortDesc,
                Location = new Point(leftColumnX, 35),
                Size = new Size(rightColumnX - leftColumnX - 10, 30), // Фиксированный размер
                ForeColor = Color.DarkGray,
                TextAlign = ContentAlignment.TopLeft
            };

            // Категория
            Label lblCategory = new Label
            {
                Text = product.Category,
                Location = new Point(leftColumnX, 65),
                Size = new Size(200, 20),
                Font = new Font("Arial", 9, FontStyle.Italic),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.TopLeft
            };

            // Цена - ПРАВАЯ КОЛОНКА
            Label lblPrice = new Label
            {
                Text = $"{product.Price:N2}",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(rightColumnX, 10),
                Size = new Size(100, 25),
                ForeColor = Color.Green,
                TextAlign = ContentAlignment.TopRight // Выравнивание по правому краю
            };

            // Надпись "руб."
            Label lblCurrency = new Label
            {
                Text = "руб.",
                Location = new Point(rightColumnX, 35),
                Size = new Size(100, 20),
                Font = new Font("Arial", 9),
                TextAlign = ContentAlignment.TopRight
            };

            // Наличие
            Label lblStock = new Label
            {
                Text = $"В наличии: {product.Stock} шт.",
                Location = new Point(rightColumnX, 60),
                Size = new Size(100, 20),
                TextAlign = ContentAlignment.TopRight
            };

            // Разделительная линия
            Panel separator = new Panel
            {
                Location = new Point(leftColumnX, 95),
                Size = new Size(productPanel.Width - leftColumnX - 10, 1),
                BackColor = Color.LightGray
            };

            // Добавляем все элементы на панель
            productPanel.Controls.Add(chkSelect);
            productPanel.Controls.Add(pic);
            productPanel.Controls.Add(lblTitle);
            productPanel.Controls.Add(lblDesc);
            productPanel.Controls.Add(lblCategory);
            productPanel.Controls.Add(lblPrice);
            productPanel.Controls.Add(lblCurrency);
            productPanel.Controls.Add(lblStock);
            productPanel.Controls.Add(separator);

            flowProducts.Controls.Add(productPanel);
        }

        // Создание заглушки для изображения
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
        private void InitializeSortComboBox()
        {
            if (cmbSortBy != null)
            {
                cmbSortBy.Items.Clear();
                cmbSortBy.Items.AddRange(new string[] {
            "По умолчанию",
            "По цене (возр.)",
            "По цене (убыв.)",
            "По названию",
            "По количеству"
        });
                cmbSortBy.SelectedIndex = 0;
            }
        }

        // Загрузка категорий для фильтра
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
        private void UpdateCheckedListBox()
        {
            for (int i = 0; i < products.Count; i++)
            {
                clbProducts.SetItemChecked(i, products[i].IsSelected);
            }
        }
        private void clbProducts_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.Index >= 0 && e.Index < products.Count)
            {
                products[e.Index].IsSelected = (e.NewValue == CheckState.Checked);

                // Обновляем CheckBox на карточке
                foreach (Control panel in flowProducts.Controls)
                {
                    if (panel is Panel productPanel && (int)productPanel.Tag == products[e.Index].Id)
                    {
                        foreach (Control control in productPanel.Controls)
                        {
                            if (control is CheckBox chk)
                            {
                                chk.Checked = products[e.Index].IsSelected;
                                break;
                            }
                        }
                        break;
                    }
                }
            }
        }
        private void AdminForm_Load(object sender, EventArgs e)
        {

        }
        public AdminForm(string fullName) : this() // Вызывает конструктор без параметров
        {
            {
                currentUser = fullName;
                lblUserInfo.Text = $"Администратор: {fullName}";
                // Инициализируем ComboBox сортировки
                InitializeSortComboBox();

                // Подписываемся на события
                cmbSortBy.SelectedIndexChanged += cmbSortBy_SelectedIndexChanged;
                cmbFilterCategory.SelectedIndexChanged += cmbFilterCategory_SelectedIndexChanged;
                LoadCategoriesForFilter();
                LoadProducts();
                UpdateProductsList();
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void flowProducts_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pnlTop_Paint(object sender, PaintEventArgs e)
        {

        }

        private void gbSelection_Enter(object sender, EventArgs e)
        {

        }

        private void pnlActions_Paint(object sender, PaintEventArgs e)
        {

        }

        private void clbProducts_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnEditProduct_Click(object sender, EventArgs e)
        {
            var selectedProducts = products.Where(p => p.IsSelected).ToList();

            if (selectedProducts.Count == 0)
            {
                MessageBox.Show("Выберите товар для изменения!", "Внимание",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (selectedProducts.Count > 1)
            {
                MessageBox.Show("Выберите только один товар для изменения!", "Внимание",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Product productToEdit = selectedProducts.First();
            ProductEditForm editForm = new ProductEditForm(productToEdit.Id);

            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadProducts();
                UpdateProductsList();
                MessageBox.Show("Товар изменен успешно!", "Успех",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnAddProduct_Click(object sender, EventArgs e)
        {
            {
                ProductEditForm editForm = new ProductEditForm();
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadProducts();
                    UpdateProductsList();
                    MessageBox.Show("Товар добавлен успешно!", "Успех",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            {
                LoadProducts(txtSearch?.Text ?? "");
                UpdateProductsList();
            }
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnSearch_Click(sender, e);
                e.Handled = true;
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }
        private void AdminForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!Application.OpenForms.OfType<LoginForm>().Any())
            {
                Application.Exit();
            }
        }

        private void btnDeleteProduct_Click(object sender, EventArgs e)
        {
            {
                var selectedProducts = products.Where(p => p.IsSelected).ToList();

                if (selectedProducts.Count == 0)
                {
                    MessageBox.Show("Выберите товары для удаления!", "Внимание",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string message = selectedProducts.Count == 1
                    ? $"Вы уверены, что хотите удалить товар:\n{selectedProducts[0].Name}?"
                    : $"Вы уверены, что хотите удалить {selectedProducts.Count} товаров?";

                if (MessageBox.Show(message, "Подтверждение удаления",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            conn.Open();

                            foreach (var product in selectedProducts)
                            {
                                string query = "DELETE FROM Products WHERE Id = @id";
                                using (SqlCommand cmd = new SqlCommand(query, conn))
                                {
                                    cmd.Parameters.AddWithValue("@id", product.Id);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }

                        LoadProducts();
                        UpdateProductsList();
                        MessageBox.Show("Товары удалены успешно!", "Успех",
                                      MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void cmbFilterCategory_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }   
}
