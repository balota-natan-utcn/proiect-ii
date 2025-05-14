namespace Proiect_II
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            dataGridView1 = new DataGridView();
            btnLoadData = new Button();
            tabControl1 = new TabControl();
            tabView = new TabPage();
            tabEdit = new TabPage();
            btnDelete = new Button();
            btnUpdate = new Button();
            btnAdd = new Button();
            txtInStock = new TextBox();
            txtPrice = new TextBox();
            lblInStock = new Label();
            lblPrice = new Label();
            txtName = new TextBox();
            lblName = new Label();
            txtId = new TextBox();
            lblId = new Label();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            tabControl1.SuspendLayout();
            tabView.SuspendLayout();
            tabEdit.SuspendLayout();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(6, 6);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(647, 386);
            dataGridView1.TabIndex = 0;
            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
            // 
            // btnLoadData
            // 
            btnLoadData.Location = new Point(659, 369);
            btnLoadData.Name = "btnLoadData";
            btnLoadData.Size = new Size(103, 23);
            btnLoadData.TabIndex = 1;
            btnLoadData.Text = "load products";
            btnLoadData.UseVisualStyleBackColor = true;
            btnLoadData.Click += btnLoadData_Click;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabView);
            tabControl1.Controls.Add(tabEdit);
            tabControl1.Location = new Point(12, 12);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(776, 426);
            tabControl1.TabIndex = 2;
            // 
            // tabView
            // 
            tabView.Controls.Add(dataGridView1);
            tabView.Controls.Add(btnLoadData);
            tabView.Location = new Point(4, 24);
            tabView.Name = "tabView";
            tabView.Padding = new Padding(3);
            tabView.Size = new Size(768, 398);
            tabView.TabIndex = 0;
            tabView.Text = "View Data";
            tabView.UseVisualStyleBackColor = true;
            // 
            // tabEdit
            // 
            tabEdit.Controls.Add(btnDelete);
            tabEdit.Controls.Add(btnUpdate);
            tabEdit.Controls.Add(btnAdd);
            tabEdit.Controls.Add(txtInStock);
            tabEdit.Controls.Add(txtPrice);
            tabEdit.Controls.Add(lblInStock);
            tabEdit.Controls.Add(lblPrice);
            tabEdit.Controls.Add(txtName);
            tabEdit.Controls.Add(lblName);
            tabEdit.Controls.Add(txtId);
            tabEdit.Controls.Add(lblId);
            tabEdit.Location = new Point(4, 24);
            tabEdit.Name = "tabEdit";
            tabEdit.Padding = new Padding(3);
            tabEdit.Size = new Size(768, 398);
            tabEdit.TabIndex = 1;
            tabEdit.Text = "Edit Data";
            tabEdit.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(224, 157);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(75, 23);
            btnDelete.TabIndex = 10;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // btnUpdate
            // 
            btnUpdate.Location = new Point(118, 157);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(75, 23);
            btnUpdate.TabIndex = 10;
            btnUpdate.Text = "Update";
            btnUpdate.UseVisualStyleBackColor = true;
            btnUpdate.Click += btnUpdate_Click;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(6, 157);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(75, 23);
            btnAdd.TabIndex = 10;
            btnAdd.Text = "Add";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // txtInStock
            // 
            txtInStock.Location = new Point(467, 39);
            txtInStock.Name = "txtInStock";
            txtInStock.Size = new Size(100, 23);
            txtInStock.TabIndex = 9;
            // 
            // txtPrice
            // 
            txtPrice.Location = new Point(310, 39);
            txtPrice.Name = "txtPrice";
            txtPrice.Size = new Size(100, 23);
            txtPrice.TabIndex = 8;
            // 
            // lblInStock
            // 
            lblInStock.AutoSize = true;
            lblInStock.Location = new Point(494, 21);
            lblInStock.Name = "lblInStock";
            lblInStock.Size = new Size(52, 15);
            lblInStock.TabIndex = 7;
            lblInStock.Text = "In Stock:";
            // 
            // lblPrice
            // 
            lblPrice.AutoSize = true;
            lblPrice.Location = new Point(342, 21);
            lblPrice.Name = "lblPrice";
            lblPrice.Size = new Size(33, 15);
            lblPrice.TabIndex = 6;
            lblPrice.Text = "Price";
            // 
            // txtName
            // 
            txtName.Location = new Point(153, 39);
            txtName.Name = "txtName";
            txtName.Size = new Size(100, 23);
            txtName.TabIndex = 5;
            txtName.TextChanged += txtName_TextChanged;
            // 
            // lblName
            // 
            lblName.AutoSize = true;
            lblName.Location = new Point(185, 21);
            lblName.Name = "lblName";
            lblName.Size = new Size(39, 15);
            lblName.TabIndex = 4;
            lblName.Text = "Name";
            // 
            // txtId
            // 
            txtId.BorderStyle = BorderStyle.FixedSingle;
            txtId.Location = new Point(6, 39);
            txtId.Name = "txtId";
            txtId.ReadOnly = true;
            txtId.Size = new Size(100, 23);
            txtId.TabIndex = 3;
            // 
            // lblId
            // 
            lblId.AutoSize = true;
            lblId.Location = new Point(28, 21);
            lblId.Name = "lblId";
            lblId.Size = new Size(18, 15);
            lblId.TabIndex = 2;
            lblId.Text = "ID";
            lblId.Click += label1_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tabControl1);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            tabControl1.ResumeLayout(false);
            tabView.ResumeLayout(false);
            tabEdit.ResumeLayout(false);
            tabEdit.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dataGridView1;
        private Button btnLoadData;
        private TabControl tabControl1;
        private TabPage tabView;
        private TabPage tabEdit;
        private Label lblId;
        private TextBox txtName;
        private Label lblName;
        private TextBox txtId;
        private Button btnDelete;
        private Button btnUpdate;
        private Button btnAdd;
        private TextBox txtInStock;
        private TextBox txtPrice;
        private Label lblInStock;
        private Label lblPrice;
    }
}
