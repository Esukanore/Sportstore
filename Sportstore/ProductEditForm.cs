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
    public partial class ProductEditForm : Form
    {
        private string connectionString = @"Data Source=localhost;Initial Catalog=sport;Integrated Security=True";
        private int productId = 0;
        private byte[] imageData;
        private string imageType;
        public ProductEditForm()
        {
            InitializeComponent();
            this.Text = "Добавить товар";
            this.Size = new Size(702,541);
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void ProductEditForm_Load(object sender, EventArgs e)
        {

        }
        public ProductEditForm(int id) : this() // вызывает конструктор без параметров
        {
            productId = id;
            this.Text = "Редактировать товар";
            LoadProductData();
        }
        private void LoadProductData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM Products WHERE Id = @id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", productId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtName.Text = reader["Name"].ToString();
                                txtDescription.Text = reader["Description"].ToString();
                                txtCategory.Text = reader["Category"].ToString();
                                txtPrice.Text = reader["Price"].ToString();
                                numStock.Value = Convert.ToInt32(reader["Stock"]);

                                if (reader["ImageData"] != DBNull.Value)
                                {
                                    imageData = (byte[])reader["ImageData"];
                                    imageType = reader["ImageType"].ToString();

                                    using (MemoryStream ms = new MemoryStream(imageData))
                                    {
                                        picProduct.Image = Image.FromStream(ms);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        private void btnLoadImage_Click(object sender, EventArgs e)
        {
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                    ofd.Title = "Выберите изображение товара";

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            Image image = Image.FromFile(ofd.FileName);

                            // Масштабируем если нужно
                            if (image.Width > 400 || image.Height > 400)
                            {
                                image = ResizeImage(image, 400, 400);
                            }

                            picProduct.Image = image;

                            // Сохраняем в байтах
                            using (MemoryStream ms = new MemoryStream())
                            {
                                image.Save(ms, image.RawFormat);
                                imageData = ms.ToArray();
                                imageType = Path.GetExtension(ofd.FileName).ToLower();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка: {ex.Message}");
                        }
                    }
                }
            }
        }

        private Image ResizeImage(Image image, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(result))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(image, 0, 0, width, height);
            }

            return result;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            // Валидация
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название товара!", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Введите корректную цену!", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrice.Focus();
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    if (productId == 0) // Новый товар
                    {
                        string query = @"INSERT INTO Products (Name, Description, Price, Category, ImageData, ImageType, Stock) 
                                       VALUES (@name, @desc, @price, @category, @imageData, @imageType, @stock)";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            AddParameters(cmd);
                            cmd.ExecuteNonQuery();
                        }

                        MessageBox.Show("Товар добавлен успешно!", "Успех",
                                      MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else // Редактирование
                    {
                        string query = @"UPDATE Products 
                                       SET Name = @name, Description = @desc, Price = @price, 
                                           Category = @category, ImageData = @imageData, 
                                           ImageType = @imageType, Stock = @stock
                                       WHERE Id = @id";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", productId);
                            AddParameters(cmd);
                            cmd.ExecuteNonQuery();
                        }

                        MessageBox.Show("Товар обновлен успешно!", "Успех",
                                      MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Ошибка базы данных: {ex.Message}", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
            

        private void AddParameters(SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
            cmd.Parameters.AddWithValue("@desc", txtDescription.Text.Trim());
            cmd.Parameters.AddWithValue("@price", decimal.Parse(txtPrice.Text));
            cmd.Parameters.AddWithValue("@category", txtCategory.Text.Trim());
            cmd.Parameters.AddWithValue("@stock", (int)numStock.Value);

            if (imageData != null && imageData.Length > 0)
            {
                cmd.Parameters.AddWithValue("@imageData", imageData);
                cmd.Parameters.AddWithValue("@imageType", imageType);
            }
            else
            {
                cmd.Parameters.AddWithValue("@imageData", DBNull.Value);
                cmd.Parameters.AddWithValue("@imageType", DBNull.Value);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void picProduct_Click(object sender, EventArgs e)
        {

        }
    } 
}
