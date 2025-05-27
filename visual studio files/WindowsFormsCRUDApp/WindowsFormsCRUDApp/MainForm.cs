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
using System.Configuration;

namespace ManagerInventar
{
    public partial class MainForm : Form
    {
        private string connectionString = @"Server=localhost\SQLEXPRESS;Database=DBProiectII;Trusted_Connection=True;Encrypt=False;";
        private DataTable dataTable = new DataTable();
        private SqlDataAdapter dataAdapter;
        private SqlConnection connection;
        private string currentTable = "Clienti"; // Default table
        private Dictionary<string, List<string>> tableColumns = new Dictionary<string, List<string>>();
        private string moot;

        public MainForm()
        {
            InitializeComponent();
            // Initialize the connection
            connection = new SqlConnection(connectionString);
            SetupTableColumns();
            PopulateTableComboBox();
            LoadData();
        }

        private void SetupTableColumns()
        {
            // Define columns for each table
            tableColumns.Add("Clienti", new List<string> { "ID_Client", "Nume", "Email", "Telefon" });
            tableColumns.Add("Produse", new List<string> { "ID_Produs", "NumeProdus", "Pret", "Cantitate" });
            tableColumns.Add("Comenzi", new List<string> { "ID_Comanda", "ID_Client", "DataComanda", "Total" });
        }

        private void PopulateTableComboBox()
        {
            cboTables.Items.Clear();
            foreach (var table in tableColumns.Keys)
            {
                cboTables.Items.Add(table);
            }
            cboTables.SelectedItem = currentTable;
        }

        private void LoadData()
        {
            try
            {
                // Create a persistent connection that will be used by the data adapter
                SqlConnection connection = new SqlConnection(connectionString);

                // Create a query that specifically selects only the columns we want for the current table
                List<string> columns = tableColumns[currentTable];
                string columnList = string.Join(", ", columns);
                string query = $"SELECT {columnList} FROM {currentTable}";

                // Create a new data adapter with the query and connection
                dataAdapter = new SqlDataAdapter(query, connection);

                // Setup the command builder to generate the commands
                SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);

                // Explicitly set the adapter's insert, update, and delete commands
                dataAdapter.InsertCommand = commandBuilder.GetInsertCommand();
                dataAdapter.UpdateCommand = commandBuilder.GetUpdateCommand();
                dataAdapter.DeleteCommand = commandBuilder.GetDeleteCommand();

                // Clear previous data and create a new DataTable
                dataTable = new DataTable();

                // Fill the data table with the data from the specified table
                dataAdapter.Fill(dataTable);

                // Bind the data grid view to the data table
                dataGridView.DataSource = null; // Unbind first to reset
                dataGridView.DataSource = dataTable;

                // Update the fields on the form
                UpdateFormFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateFormFields()
        {
            // Clear the fields panel
            panelFields.Controls.Clear();

            // Get the columns for the current table
            List<string> columns = tableColumns[currentTable];

            int yPos = 10;
            foreach (string column in columns)
            {
                // Create label
                Label label = new Label
                {
                    Text = column + ":",
                    Location = new Point(10, yPos),
                    AutoSize = true
                };
                panelFields.Controls.Add(label);

                // Create textbox
                TextBox textBox = new TextBox
                {
                    Name = "txt" + column,
                    Location = new Point(120, yPos),
                    Width = 200
                };
                panelFields.Controls.Add(textBox);

                yPos += 30;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Create a new row
                DataRow newRow = dataTable.NewRow();

                // Get the data from the form fields
                foreach (string column in tableColumns[currentTable])
                {
                    TextBox textBox = panelFields.Controls["txt" + column] as TextBox;
                    if (textBox != null && !string.IsNullOrWhiteSpace(textBox.Text))
                    {
                        newRow[column] = textBox.Text;
                    }
                }

                // Add the new row to the data table
                dataTable.Rows.Add(newRow);

                // Update the database with explicit connection
                dataAdapter.Update(dataTable);

                // Refresh the grid
                LoadData();

                // Clear the form fields
                ClearFormFields();

                MessageBox.Show("Record added successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding record: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView.CurrentRow != null)
                {
                    // Get the selected row index
                    int rowIndex = dataGridView.CurrentRow.Index;

                    // Get the DataRow that needs to be updated
                    DataRow row = dataTable.Rows[rowIndex];

                    // Update the values from the form fields
                    foreach (string column in tableColumns[currentTable])
                    {
                        TextBox textBox = panelFields.Controls["txt" + column] as TextBox;
                        if (textBox != null && !string.IsNullOrWhiteSpace(textBox.Text))
                        {
                            row[column] = textBox.Text;
                        }
                    }

                    // Update the database with explicit connection
                    dataAdapter.Update(dataTable);

                    // Refresh the grid
                    LoadData();

                    MessageBox.Show("Record updated successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Please select a record to update.", "Selection Required",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating record: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView.CurrentRow != null)
                {
                    // Confirm deletion
                    DialogResult result = MessageBox.Show("Are you sure you want to delete this record?",
                        "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        // Get the selected row index
                        int rowIndex = dataGridView.CurrentRow.Index;

                        // Mark the row for deletion
                        dataTable.Rows[rowIndex].Delete();

                        // Update the database with explicit connection
                        dataAdapter.Update(dataTable);

                        // Refresh the grid
                        LoadData();

                        // Clear the form fields
                        ClearFormFields();

                        MessageBox.Show("Record deleted successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Please select a record to delete.", "Selection Required",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting record: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFormFields();
        }

        private void ClearFormFields()
        {
            foreach (Control control in panelFields.Controls)
            {
                if (control is TextBox)
                {
                    ((TextBox)control).Clear();
                }
            }
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView.CurrentRow != null)
                {
                    DataRowView selectedRow = (DataRowView)dataGridView.CurrentRow.DataBoundItem;

                    foreach (string column in tableColumns[currentTable])
                    {
                        TextBox textBox = panelFields.Controls["txt" + column] as TextBox;
                        if (textBox != null)
                        {
                            textBox.Text = selectedRow[column].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine($"Error in selection changed: {ex.Message}");
            }
        }

        private void cboTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentTable = cboTables.SelectedItem.ToString();
            LoadData();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnSales_Click(object sender, EventArgs e)
        {
            try
            {
                SalesForm salesForm = new SalesForm(4);
                salesForm.ShowDialog();


                // Refresh data when sales form is closed (in case inventory was updated)
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening sales module: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // This method will be called by the designer
        private void InitializeComponent()
        {
            this.cboTables = new System.Windows.Forms.ComboBox();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnSales = new System.Windows.Forms.Button();
            this.panelFields = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // cboTables
            // 
            this.cboTables.FormattingEnabled = true;
            this.cboTables.Location = new System.Drawing.Point(107, 12);
            this.cboTables.Name = "cboTables";
            this.cboTables.Size = new System.Drawing.Size(200, 21);
            this.cboTables.TabIndex = 0;
            this.cboTables.SelectedIndexChanged += new System.EventHandler(this.cboTables_SelectedIndexChanged);
            // 
            // dataGridView
            // 
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Location = new System.Drawing.Point(12, 39);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.Size = new System.Drawing.Size(500, 220);
            this.dataGridView.TabIndex = 1;
            this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(12, 430);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "Adauga";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(93, 430);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnUpdate.TabIndex = 3;
            this.btnUpdate.Text = "Actualizeaza";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(174, 430);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 4;
            this.btnDelete.Text = "Sterge";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(255, 430);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 5;
            this.btnClear.Text = "Elibereaza";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(336, 430);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 6;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnSales
            // 
            this.btnSales.BackColor = System.Drawing.Color.DarkGreen;
            this.btnSales.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSales.ForeColor = System.Drawing.Color.White;
            this.btnSales.Location = new System.Drawing.Point(417, 430);
            this.btnSales.Name = "btnSales";
            this.btnSales.Size = new System.Drawing.Size(95, 23);
            this.btnSales.TabIndex = 7;
            this.btnSales.Text = "Modul Vanzari";
            this.btnSales.UseVisualStyleBackColor = false;
            this.btnSales.Click += new System.EventHandler(this.btnSales_Click);
            // 
            // panelFields
            // 
            this.panelFields.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelFields.Location = new System.Drawing.Point(12, 265);
            this.panelFields.Name = "panelFields";
            this.panelFields.Size = new System.Drawing.Size(500, 159);
            this.panelFields.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Selectie Tabel:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(524, 465);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panelFields);
            this.Controls.Add(this.btnSales);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.cboTables);
            this.Name = "MainForm";
            this.Text = "Manager Inventar";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.ComboBox cboTables;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnSales;
        private System.Windows.Forms.Panel panelFields;
        private System.Windows.Forms.Label label1;
    }
}