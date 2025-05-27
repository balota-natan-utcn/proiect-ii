using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace ManagerInventar
{
    public partial class SalesForm : Form
    {
        private string connectionString = @"Server=localhost\SQLEXPRESS;Database=DBProiectII;Trusted_Connection=True;Encrypt=False;";
        private DatabaseHelper dbHelper;
        private DataTable clientsTable;
        private DataTable productsTable;
        private DataTable cartItems;
        private decimal totalAmount = 0;

        public SalesForm(int i)
        {
            InitializeComponent();
            dbHelper = new DatabaseHelper(connectionString);
            InitializeCartTable();
            LoadClients();
            LoadProducts();
            UpdateTotalLabel();
        }

        private void InitializeCartTable()
        {
            cartItems = new DataTable();
            cartItems.Columns.Add("ID_Produs", typeof(int));
            cartItems.Columns.Add("NumeProdus", typeof(string));
            cartItems.Columns.Add("Pret", typeof(decimal));
            cartItems.Columns.Add("Cantitate", typeof(int));
            cartItems.Columns.Add("Subtotal", typeof(decimal));

            dataGridViewCart.DataSource = cartItems;
        }

        private void LoadClients()
        {
            try
            {
                clientsTable = dbHelper.GetTableData("Clienti");
                cboClients.DisplayMember = "Nume";
                cboClients.ValueMember = "ID_Client";
                cboClients.DataSource = clientsTable;
                cboClients.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading clients: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProducts()
        {
            try
            {
                productsTable = dbHelper.GetTableData("Produse");
                cboProducts.DisplayMember = "NumeProdus";
                cboProducts.ValueMember = "ID_Produs";
                cboProducts.DataSource = productsTable;
                cboProducts.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cboProducts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboProducts.SelectedValue != null)
            {
                DataRowView selectedProduct = (DataRowView)cboProducts.SelectedItem;
                if (selectedProduct != null)
                {
                    txtPrice.Text = selectedProduct["Pret"].ToString();
                    int availableStock = Convert.ToInt32(selectedProduct["Cantitate"]);
                    lblStock.Text = $"Stoc Disponibil: {availableStock}";
                    numQuantity.Maximum = availableStock;
                    numQuantity.Value = 1;
                }
            }
        }

        private void btnAddToCart_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboProducts.SelectedValue == null)
                {
                    MessageBox.Show("Please select a product.", "Selection Required",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (numQuantity.Value <= 0)
                {
                    MessageBox.Show("Please enter a valid quantity.", "Invalid Quantity",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int productId = Convert.ToInt32(cboProducts.SelectedValue);
                string productName = cboProducts.Text;
                decimal price = Convert.ToDecimal(txtPrice.Text);
                int quantity = Convert.ToInt32(numQuantity.Value);
                decimal subtotal = price * quantity;

                //Check if product already exists in cart
                DataRow existingRow = cartItems.AsEnumerable()
                    .FirstOrDefault(row => row.Field<int>("ID_Produs") == productId);

                if (existingRow != null)
                {
                    //update existing item
                    int newQuantity = existingRow.Field<int>("Cantitate") + quantity;
                    existingRow["Cantitate"] = newQuantity;
                    existingRow["Subtotal"] = price * newQuantity;
                }
                else
                {
                    //add new item
                    DataRow newRow = cartItems.NewRow();
                    newRow["ID_Produs"] = productId;
                    newRow["NumeProdus"] = productName;
                    newRow["Pret"] = price;
                    newRow["Cantitate"] = quantity;
                    newRow["Subtotal"] = subtotal;
                    cartItems.Rows.Add(newRow);
                }

                UpdateTotalAmount();
                ClearProductSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding to cart: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRemoveFromCart_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewCart.SelectedRows.Count > 0)
                {
                    int rowIndex = dataGridViewCart.SelectedRows[0].Index;
                    cartItems.Rows[rowIndex].Delete();
                    UpdateTotalAmount();
                }
                else
                {
                    MessageBox.Show("Please select an item to remove.", "Selection Required",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error removing from cart: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClearCart_Click(object sender, EventArgs e)
        {
            cartItems.Clear();
            UpdateTotalAmount();
        }

        private void btnProcessSale_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboClients.SelectedValue == null)
                {
                    MessageBox.Show("Please select a client.", "Selection Required",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cartItems.Rows.Count == 0)
                {
                    MessageBox.Show("Cart is empty. Please add items to cart.", "Empty Cart",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show(
                    $"Process sale for {cboClients.Text}?\nTotal Amount: {totalAmount:C}",
                    "Confirm Sale", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    ProcessSaleTransaction();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing sale: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ProcessSaleTransaction()
        {
            int savedOrderId;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    //insert order into Comenzi table
                    string orderQuery = @"
                        INSERT INTO Comenzi (ID_Client, DataComanda, Total) 
                        VALUES (@ClientId, @OrderDate, @Total);
                        SELECT SCOPE_IDENTITY();";

                    SqlCommand orderCommand = new SqlCommand(orderQuery, connection, transaction);
                    orderCommand.Parameters.AddWithValue("@ClientId", cboClients.SelectedValue);
                    orderCommand.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                    orderCommand.Parameters.AddWithValue("@Total", totalAmount);

                    int orderId = Convert.ToInt32(orderCommand.ExecuteScalar());
                    savedOrderId = Convert.ToInt32(orderCommand.ExecuteScalar());

                    //update product quantities
                    foreach (DataRow item in cartItems.Rows)
                    {
                        int productId = Convert.ToInt32(item["ID_Produs"]);
                        int quantitySold = Convert.ToInt32(item["Cantitate"]);

                        string updateQuery = @"
                            UPDATE Produse 
                            SET Cantitate = Cantitate - @QuantitySold 
                            WHERE ID_Produs = @ProductId";

                        SqlCommand updateCommand = new SqlCommand(updateQuery, connection, transaction);
                        updateCommand.Parameters.AddWithValue("@QuantitySold", quantitySold);
                        updateCommand.Parameters.AddWithValue("@ProductId", productId);
                        updateCommand.ExecuteNonQuery();
                    }

                    transaction.Commit();

                    MessageBox.Show($"Sale processed successfully!\nOrder ID: {orderId}", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception($"Transaction failed: {ex.Message}");
                }
            }

            GenerateInvoicePdf(savedOrderId);

            //clear the form
            ClearSaleForm();
            LoadProducts(); //refresh products to show updated stock
        }

        private void UpdateTotalAmount()
        {
            totalAmount = 0;
            foreach (DataRow row in cartItems.Rows)
            {
                if (row.RowState != DataRowState.Deleted)
                {
                    totalAmount += Convert.ToDecimal(row["Subtotal"]);
                }
            }
            UpdateTotalLabel();
        }

        private void UpdateTotalLabel()
        {
            lblTotal.Text = $"Total: {totalAmount:C}";
        }

        private void ClearProductSelection()
        {
            cboProducts.SelectedIndex = -1;
            txtPrice.Clear();
            numQuantity.Value = 1;
            lblStock.Text = "Available Stock: 0";
        }

        private void ClearSaleForm()
        {
            cboClients.SelectedIndex = -1;
            ClearProductSelection();
            cartItems.Clear();
            UpdateTotalAmount();
        }

        private void GenerateInvoicePdf(int orderId)
        {
            string filePath = $"Factura_{orderId}.pdf";

            Document doc = new Document(PageSize.A4, 50, 50, 25, 25);
            PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
            doc.Open();

            //fonts
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
            var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            var regularFont = FontFactory.GetFont(FontFactory.HELVETICA, 11);

            //title
            Paragraph title = new Paragraph("Factura Fiscală", titleFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 20f
            };
            doc.Add(title);

            //client info
            PdfPTable clientInfo = new PdfPTable(2);
            clientInfo.WidthPercentage = 100;
            clientInfo.SetWidths(new float[] { 1f, 2f });

            clientInfo.AddCell(new Phrase("Client:", boldFont));
            clientInfo.AddCell(new Phrase(cboClients.Text, regularFont));
            clientInfo.AddCell(new Phrase("Data:", boldFont));
            clientInfo.AddCell(new Phrase(DateTime.Now.ToString("dd.MM.yyyy"), regularFont));
            clientInfo.AddCell(new Phrase("ID Comandă:", boldFont));
            clientInfo.AddCell(new Phrase(orderId.ToString(), regularFont));
            clientInfo.AddCell(new Phrase("Total:", boldFont));
            clientInfo.AddCell(new Phrase(string.Format("{0:C}", totalAmount), regularFont));

            clientInfo.SpacingAfter = 20f;
            doc.Add(clientInfo);

            //products table
            PdfPTable table = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 1f, 3f, 1f, 1f, 1f });

            //headers
            string[] headers = { "ID", "Produs", "Cantitate", "Preț unitar", "Subtotal" };
            foreach (var header in headers)
            {
                PdfPCell cell = new PdfPCell(new Phrase(header, boldFont));
                cell.BackgroundColor = new BaseColor(230, 230, 250);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);
            }

            //cartItems products
            foreach (DataRow item in cartItems.Rows)
            {
                int quantity = Convert.ToInt32(item["Cantitate"]);
                decimal unitPrice = Convert.ToDecimal(item["Pret"]);
                decimal subtotal = Convert.ToDecimal(item["Subtotal"]);

                table.AddCell(new Phrase(item["ID_Produs"].ToString(), regularFont));
                table.AddCell(new Phrase(item["NumeProdus"].ToString(), regularFont));
                table.AddCell(new Phrase(quantity.ToString(), regularFont));
                table.AddCell(new Phrase($"{unitPrice:C}", regularFont));
                table.AddCell(new Phrase($"{subtotal:C}", regularFont));
            }

            table.SpacingAfter = 20f;
            doc.Add(table);

            //total
            Paragraph totalPar = new Paragraph($"TOTAL DE PLATĂ: {totalAmount:C}", boldFont)
            {
                Alignment = Element.ALIGN_RIGHT
            };
            doc.Add(totalPar);

            doc.Close();

            MessageBox.Show($"Factura a fost generată: {filePath}", "PDF Generat",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            System.Diagnostics.Process.Start(filePath);
        }

        private void InitializeComponent()
        {
            this.groupBoxClient = new System.Windows.Forms.GroupBox();
            this.cboClients = new System.Windows.Forms.ComboBox();
            this.lblClient = new System.Windows.Forms.Label();
            this.groupBoxProducts = new System.Windows.Forms.GroupBox();
            this.lblStock = new System.Windows.Forms.Label();
            this.btnAddToCart = new System.Windows.Forms.Button();
            this.numQuantity = new System.Windows.Forms.NumericUpDown();
            this.txtPrice = new System.Windows.Forms.TextBox();
            this.cboProducts = new System.Windows.Forms.ComboBox();
            this.lblQuantity = new System.Windows.Forms.Label();
            this.lblPrice = new System.Windows.Forms.Label();
            this.lblProduct = new System.Windows.Forms.Label();
            this.groupBoxCart = new System.Windows.Forms.GroupBox();
            this.btnClearCart = new System.Windows.Forms.Button();
            this.btnRemoveFromCart = new System.Windows.Forms.Button();
            this.dataGridViewCart = new System.Windows.Forms.DataGridView();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnProcessSale = new System.Windows.Forms.Button();
            this.groupBoxClient.SuspendLayout();
            this.groupBoxProducts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numQuantity)).BeginInit();
            this.groupBoxCart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCart)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxClient
            // 
            this.groupBoxClient.Controls.Add(this.cboClients);
            this.groupBoxClient.Controls.Add(this.lblClient);
            this.groupBoxClient.Location = new System.Drawing.Point(12, 12);
            this.groupBoxClient.Name = "groupBoxClient";
            this.groupBoxClient.Size = new System.Drawing.Size(300, 60);
            this.groupBoxClient.TabIndex = 0;
            this.groupBoxClient.TabStop = false;
            this.groupBoxClient.Text = "Selectie Client";
            this.groupBoxClient.Enter += new System.EventHandler(this.groupBoxClient_Enter);
            // 
            // cboClients
            // 
            this.cboClients.FormattingEnabled = true;
            this.cboClients.Location = new System.Drawing.Point(60, 25);
            this.cboClients.Name = "cboClients";
            this.cboClients.Size = new System.Drawing.Size(200, 21);
            this.cboClients.TabIndex = 1;
            // 
            // lblClient
            // 
            this.lblClient.AutoSize = true;
            this.lblClient.Location = new System.Drawing.Point(15, 28);
            this.lblClient.Name = "lblClient";
            this.lblClient.Size = new System.Drawing.Size(36, 13);
            this.lblClient.TabIndex = 0;
            this.lblClient.Text = "Client:";
            // 
            // groupBoxProducts
            // 
            this.groupBoxProducts.Controls.Add(this.lblStock);
            this.groupBoxProducts.Controls.Add(this.btnAddToCart);
            this.groupBoxProducts.Controls.Add(this.numQuantity);
            this.groupBoxProducts.Controls.Add(this.txtPrice);
            this.groupBoxProducts.Controls.Add(this.cboProducts);
            this.groupBoxProducts.Controls.Add(this.lblQuantity);
            this.groupBoxProducts.Controls.Add(this.lblPrice);
            this.groupBoxProducts.Controls.Add(this.lblProduct);
            this.groupBoxProducts.Location = new System.Drawing.Point(330, 12);
            this.groupBoxProducts.Name = "groupBoxProducts";
            this.groupBoxProducts.Size = new System.Drawing.Size(350, 140);
            this.groupBoxProducts.TabIndex = 1;
            this.groupBoxProducts.TabStop = false;
            this.groupBoxProducts.Text = "Selectie Produs";
            // 
            // lblStock
            // 
            this.lblStock.AutoSize = true;
            this.lblStock.ForeColor = System.Drawing.Color.Blue;
            this.lblStock.Location = new System.Drawing.Point(15, 85);
            this.lblStock.Name = "lblStock";
            this.lblStock.Size = new System.Drawing.Size(89, 13);
            this.lblStock.TabIndex = 7;
            this.lblStock.Text = "Stoc Disponibil: 0";
            // 
            // btnAddToCart
            // 
            this.btnAddToCart.Location = new System.Drawing.Point(250, 105);
            this.btnAddToCart.Name = "btnAddToCart";
            this.btnAddToCart.Size = new System.Drawing.Size(90, 25);
            this.btnAddToCart.TabIndex = 6;
            this.btnAddToCart.Text = "Adauga in Cos";
            this.btnAddToCart.UseVisualStyleBackColor = true;
            this.btnAddToCart.Click += new System.EventHandler(this.btnAddToCart_Click);
            // 
            // numQuantity
            // 
            this.numQuantity.Location = new System.Drawing.Point(260, 55);
            this.numQuantity.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numQuantity.Name = "numQuantity";
            this.numQuantity.Size = new System.Drawing.Size(80, 20);
            this.numQuantity.TabIndex = 5;
            this.numQuantity.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // txtPrice
            // 
            this.txtPrice.Location = new System.Drawing.Point(60, 55);
            this.txtPrice.Name = "txtPrice";
            this.txtPrice.ReadOnly = true;
            this.txtPrice.Size = new System.Drawing.Size(120, 20);
            this.txtPrice.TabIndex = 4;
            // 
            // cboProducts
            // 
            this.cboProducts.FormattingEnabled = true;
            this.cboProducts.Location = new System.Drawing.Point(60, 25);
            this.cboProducts.Name = "cboProducts";
            this.cboProducts.Size = new System.Drawing.Size(280, 21);
            this.cboProducts.TabIndex = 3;
            this.cboProducts.SelectedIndexChanged += new System.EventHandler(this.cboProducts_SelectedIndexChanged);
            // 
            // lblQuantity
            // 
            this.lblQuantity.AutoSize = true;
            this.lblQuantity.Location = new System.Drawing.Point(200, 58);
            this.lblQuantity.Name = "lblQuantity";
            this.lblQuantity.Size = new System.Drawing.Size(52, 13);
            this.lblQuantity.TabIndex = 2;
            this.lblQuantity.Text = "Cantitate:";
            // 
            // lblPrice
            // 
            this.lblPrice.AutoSize = true;
            this.lblPrice.Location = new System.Drawing.Point(15, 58);
            this.lblPrice.Name = "lblPrice";
            this.lblPrice.Size = new System.Drawing.Size(29, 13);
            this.lblPrice.TabIndex = 1;
            this.lblPrice.Text = "Pret:";
            // 
            // lblProduct
            // 
            this.lblProduct.AutoSize = true;
            this.lblProduct.Location = new System.Drawing.Point(15, 28);
            this.lblProduct.Name = "lblProduct";
            this.lblProduct.Size = new System.Drawing.Size(43, 13);
            this.lblProduct.TabIndex = 0;
            this.lblProduct.Text = "Produs:";
            // 
            // groupBoxCart
            // 
            this.groupBoxCart.Controls.Add(this.btnClearCart);
            this.groupBoxCart.Controls.Add(this.btnRemoveFromCart);
            this.groupBoxCart.Controls.Add(this.dataGridViewCart);
            this.groupBoxCart.Location = new System.Drawing.Point(12, 158);
            this.groupBoxCart.Name = "groupBoxCart";
            this.groupBoxCart.Size = new System.Drawing.Size(668, 200);
            this.groupBoxCart.TabIndex = 2;
            this.groupBoxCart.TabStop = false;
            this.groupBoxCart.Text = "Shopping Cart";
            // 
            // btnClearCart
            // 
            this.btnClearCart.Location = new System.Drawing.Point(590, 50);
            this.btnClearCart.Name = "btnClearCart";
            this.btnClearCart.Size = new System.Drawing.Size(70, 34);
            this.btnClearCart.TabIndex = 2;
            this.btnClearCart.Text = "Elibereaza Cos";
            this.btnClearCart.UseVisualStyleBackColor = true;
            this.btnClearCart.Click += new System.EventHandler(this.btnClearCart_Click);
            // 
            // btnRemoveFromCart
            // 
            this.btnRemoveFromCart.Location = new System.Drawing.Point(590, 19);
            this.btnRemoveFromCart.Name = "btnRemoveFromCart";
            this.btnRemoveFromCart.Size = new System.Drawing.Size(70, 25);
            this.btnRemoveFromCart.TabIndex = 1;
            this.btnRemoveFromCart.Text = "Sterge";
            this.btnRemoveFromCart.UseVisualStyleBackColor = true;
            this.btnRemoveFromCart.Click += new System.EventHandler(this.btnRemoveFromCart_Click);
            // 
            // dataGridViewCart
            // 
            this.dataGridViewCart.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewCart.Location = new System.Drawing.Point(19, 19);
            this.dataGridViewCart.Name = "dataGridViewCart";
            this.dataGridViewCart.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewCart.Size = new System.Drawing.Size(565, 175);
            this.dataGridViewCart.TabIndex = 0;
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.Location = new System.Drawing.Point(450, 370);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(94, 20);
            this.lblTotal.TabIndex = 3;
            this.lblTotal.Text = "Total: 0.00";
            // 
            // btnProcessSale
            // 
            this.btnProcessSale.BackColor = System.Drawing.Color.Green;
            this.btnProcessSale.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnProcessSale.ForeColor = System.Drawing.Color.White;
            this.btnProcessSale.Location = new System.Drawing.Point(580, 365);
            this.btnProcessSale.Name = "btnProcessSale";
            this.btnProcessSale.Size = new System.Drawing.Size(100, 30);
            this.btnProcessSale.TabIndex = 4;
            this.btnProcessSale.Text = "Procesare";
            this.btnProcessSale.UseVisualStyleBackColor = false;
            this.btnProcessSale.Click += new System.EventHandler(this.btnProcessSale_Click);
            // 
            // SalesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(692, 407);
            this.Controls.Add(this.btnProcessSale);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.groupBoxCart);
            this.Controls.Add(this.groupBoxProducts);
            this.Controls.Add(this.groupBoxClient);
            this.Name = "SalesForm";
            this.Text = "Modul Vanzari";
            this.groupBoxClient.ResumeLayout(false);
            this.groupBoxClient.PerformLayout();
            this.groupBoxProducts.ResumeLayout(false);
            this.groupBoxProducts.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numQuantity)).EndInit();
            this.groupBoxCart.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.GroupBox groupBoxClient;
        private System.Windows.Forms.ComboBox cboClients;
        private System.Windows.Forms.Label lblClient;
        private System.Windows.Forms.GroupBox groupBoxProducts;
        private System.Windows.Forms.Label lblStock;
        private System.Windows.Forms.Button btnAddToCart;
        private System.Windows.Forms.NumericUpDown numQuantity;
        private System.Windows.Forms.TextBox txtPrice;
        private System.Windows.Forms.ComboBox cboProducts;
        private System.Windows.Forms.Label lblQuantity;
        private System.Windows.Forms.Label lblPrice;
        private System.Windows.Forms.Label lblProduct;
        private System.Windows.Forms.GroupBox groupBoxCart;
        private System.Windows.Forms.Button btnClearCart;
        private System.Windows.Forms.Button btnRemoveFromCart;
        private System.Windows.Forms.DataGridView dataGridViewCart;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Button btnProcessSale;
    }
}
