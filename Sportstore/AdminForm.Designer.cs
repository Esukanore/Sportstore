namespace Sportstore
{
    partial class AdminForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlMain = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnLogout = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutMain = new System.Windows.Forms.TableLayoutPanel();
            this.pnlProductsList = new System.Windows.Forms.Panel();
            this.flowProducts = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlActions = new System.Windows.Forms.Panel();
            this.btnAddProduct = new System.Windows.Forms.Button();
            this.btnDeleteProduct = new System.Windows.Forms.Button();
            this.btnEditProduct = new System.Windows.Forms.Button();
            this.gbSelection = new System.Windows.Forms.GroupBox();
            this.clbProducts = new System.Windows.Forms.CheckedListBox();
            this.lblUserInfo = new System.Windows.Forms.Label();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbFilterCategory = new System.Windows.Forms.ComboBox();
            this.cmbSortBy = new System.Windows.Forms.ComboBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.pnlMain.SuspendLayout();
            this.tableLayoutMain.SuspendLayout();
            this.pnlProductsList.SuspendLayout();
            this.pnlActions.SuspendLayout();
            this.gbSelection.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.label3);
            this.pnlMain.Controls.Add(this.label2);
            this.pnlMain.Controls.Add(this.btnLogout);
            this.pnlMain.Controls.Add(this.label1);
            this.pnlMain.Controls.Add(this.tableLayoutMain);
            this.pnlMain.Controls.Add(this.lblUserInfo);
            this.pnlMain.Controls.Add(this.pnlTop);
            this.pnlMain.Location = new System.Drawing.Point(1, 0);
            this.pnlMain.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Padding = new System.Windows.Forms.Padding(12);
            this.pnlMain.Size = new System.Drawing.Size(1021, 606);
            this.pnlMain.TabIndex = 0;
            this.pnlMain.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 82);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "Сортировка:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "Поиск:";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // btnLogout
            // 
            this.btnLogout.Location = new System.Drawing.Point(910, 35);
            this.btnLogout.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(97, 33);
            this.btnLogout.TabIndex = 4;
            this.btnLogout.Text = "Выход";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Comic Sans MS", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(153, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(198, 35);
            this.label1.TabIndex = 2;
            this.label1.Text = "Список товаров";
            // 
            // tableLayoutMain
            // 
            this.tableLayoutMain.ColumnCount = 2;
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 72.2332F));
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.7668F));
            this.tableLayoutMain.Controls.Add(this.pnlProductsList, 0, 0);
            this.tableLayoutMain.Controls.Add(this.pnlActions, 1, 0);
            this.tableLayoutMain.Location = new System.Drawing.Point(0, 117);
            this.tableLayoutMain.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tableLayoutMain.Name = "tableLayoutMain";
            this.tableLayoutMain.RowCount = 1;
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutMain.Size = new System.Drawing.Size(1012, 489);
            this.tableLayoutMain.TabIndex = 0;
            // 
            // pnlProductsList
            // 
            this.pnlProductsList.AutoScroll = true;
            this.pnlProductsList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlProductsList.Controls.Add(this.flowProducts);
            this.pnlProductsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlProductsList.Location = new System.Drawing.Point(4, 3);
            this.pnlProductsList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pnlProductsList.Name = "pnlProductsList";
            this.pnlProductsList.Size = new System.Drawing.Size(723, 483);
            this.pnlProductsList.TabIndex = 0;
            // 
            // flowProducts
            // 
            this.flowProducts.AutoScroll = true;
            this.flowProducts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowProducts.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowProducts.Location = new System.Drawing.Point(0, 0);
            this.flowProducts.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.flowProducts.Name = "flowProducts";
            this.flowProducts.Size = new System.Drawing.Size(721, 481);
            this.flowProducts.TabIndex = 0;
            this.flowProducts.WrapContents = false;
            this.flowProducts.Paint += new System.Windows.Forms.PaintEventHandler(this.flowProducts_Paint);
            // 
            // pnlActions
            // 
            this.pnlActions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlActions.Controls.Add(this.btnAddProduct);
            this.pnlActions.Controls.Add(this.btnDeleteProduct);
            this.pnlActions.Controls.Add(this.btnEditProduct);
            this.pnlActions.Controls.Add(this.gbSelection);
            this.pnlActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlActions.Location = new System.Drawing.Point(735, 3);
            this.pnlActions.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(273, 483);
            this.pnlActions.TabIndex = 1;
            this.pnlActions.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlActions_Paint);
            // 
            // btnAddProduct
            // 
            this.btnAddProduct.BackColor = System.Drawing.Color.LightGreen;
            this.btnAddProduct.Location = new System.Drawing.Point(4, 285);
            this.btnAddProduct.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnAddProduct.Name = "btnAddProduct";
            this.btnAddProduct.Size = new System.Drawing.Size(209, 40);
            this.btnAddProduct.TabIndex = 3;
            this.btnAddProduct.Text = "+ Добавить товар";
            this.btnAddProduct.UseVisualStyleBackColor = false;
            this.btnAddProduct.Click += new System.EventHandler(this.btnAddProduct_Click);
            // 
            // btnDeleteProduct
            // 
            this.btnDeleteProduct.BackColor = System.Drawing.Color.LightCoral;
            this.btnDeleteProduct.Location = new System.Drawing.Point(4, 332);
            this.btnDeleteProduct.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnDeleteProduct.Name = "btnDeleteProduct";
            this.btnDeleteProduct.Size = new System.Drawing.Size(209, 40);
            this.btnDeleteProduct.TabIndex = 2;
            this.btnDeleteProduct.Text = "🗑 Удалить товары";
            this.btnDeleteProduct.UseVisualStyleBackColor = false;
            this.btnDeleteProduct.Click += new System.EventHandler(this.btnDeleteProduct_Click);
            // 
            // btnEditProduct
            // 
            this.btnEditProduct.Location = new System.Drawing.Point(4, 238);
            this.btnEditProduct.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnEditProduct.Name = "btnEditProduct";
            this.btnEditProduct.Size = new System.Drawing.Size(209, 40);
            this.btnEditProduct.TabIndex = 1;
            this.btnEditProduct.Text = "✏ Изменить товар";
            this.btnEditProduct.UseVisualStyleBackColor = true;
            this.btnEditProduct.Click += new System.EventHandler(this.btnEditProduct_Click);
            // 
            // gbSelection
            // 
            this.gbSelection.Controls.Add(this.clbProducts);
            this.gbSelection.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbSelection.Location = new System.Drawing.Point(0, 0);
            this.gbSelection.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gbSelection.Name = "gbSelection";
            this.gbSelection.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gbSelection.Size = new System.Drawing.Size(271, 231);
            this.gbSelection.TabIndex = 0;
            this.gbSelection.TabStop = false;
            this.gbSelection.Text = "Выбор товаров";
            this.gbSelection.Enter += new System.EventHandler(this.gbSelection_Enter);
            // 
            // clbProducts
            // 
            this.clbProducts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbProducts.FormattingEnabled = true;
            this.clbProducts.Location = new System.Drawing.Point(4, 19);
            this.clbProducts.Margin = new System.Windows.Forms.Padding(12);
            this.clbProducts.Name = "clbProducts";
            this.clbProducts.Size = new System.Drawing.Size(263, 209);
            this.clbProducts.TabIndex = 0;
            this.clbProducts.SelectedIndexChanged += new System.EventHandler(this.clbProducts_SelectedIndexChanged);
            // 
            // lblUserInfo
            // 
            this.lblUserInfo.AutoSize = true;
            this.lblUserInfo.Location = new System.Drawing.Point(793, 9);
            this.lblUserInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUserInfo.Name = "lblUserInfo";
            this.lblUserInfo.Size = new System.Drawing.Size(94, 15);
            this.lblUserInfo.TabIndex = 1;
            this.lblUserInfo.Text = "Администратор:";
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.label4);
            this.pnlTop.Controls.Add(this.cmbFilterCategory);
            this.pnlTop.Controls.Add(this.cmbSortBy);
            this.pnlTop.Controls.Add(this.btnSearch);
            this.pnlTop.Controls.Add(this.txtSearch);
            this.pnlTop.Location = new System.Drawing.Point(68, 47);
            this.pnlTop.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(387, 68);
            this.pnlTop.TabIndex = 0;
            this.pnlTop.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlTop_Paint);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(180, 35);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 15);
            this.label4.TabIndex = 7;
            this.label4.Text = "Категория:";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // cmbFilterCategory
            // 
            this.cmbFilterCategory.FormattingEnabled = true;
            this.cmbFilterCategory.Location = new System.Drawing.Point(251, 32);
            this.cmbFilterCategory.Name = "cmbFilterCategory";
            this.cmbFilterCategory.Size = new System.Drawing.Size(127, 23);
            this.cmbFilterCategory.TabIndex = 3;
            this.cmbFilterCategory.SelectedIndexChanged += new System.EventHandler(this.cmbFilterCategory_SelectedIndexChanged_1);
            // 
            // cmbSortBy
            // 
            this.cmbSortBy.FormattingEnabled = true;
            this.cmbSortBy.Location = new System.Drawing.Point(30, 32);
            this.cmbSortBy.Name = "cmbSortBy";
            this.cmbSortBy.Size = new System.Drawing.Size(121, 23);
            this.cmbSortBy.TabIndex = 2;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(304, 3);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(74, 23);
            this.btnSearch.TabIndex = 1;
            this.btnSearch.Text = "Найти";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(4, 3);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(279, 23);
            this.txtSearch.TabIndex = 0;
            this.txtSearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSearch_KeyPress);
            // 
            // AdminForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1025, 620);
            this.Controls.Add(this.pnlMain);
            this.Font = new System.Drawing.Font("Comic Sans MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "AdminForm";
            this.Text = "Панель администратора";
            this.Load += new System.EventHandler(this.AdminForm_Load);
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.tableLayoutMain.ResumeLayout(false);
            this.pnlProductsList.ResumeLayout(false);
            this.pnlActions.ResumeLayout(false);
            this.gbSelection.ResumeLayout(false);
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.TableLayoutPanel tableLayoutMain;
        private System.Windows.Forms.Panel pnlProductsList;
        private System.Windows.Forms.FlowLayoutPanel flowProducts;
        private System.Windows.Forms.Panel pnlActions;
        private System.Windows.Forms.Button btnEditProduct;
        private System.Windows.Forms.GroupBox gbSelection;
        private System.Windows.Forms.CheckedListBox clbProducts;
        private System.Windows.Forms.Button btnAddProduct;
        private System.Windows.Forms.Button btnDeleteProduct;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Label lblUserInfo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbSortBy;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbFilterCategory;
    }
}