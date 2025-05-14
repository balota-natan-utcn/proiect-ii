using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;

namespace Proiect_II
{
    public partial class Form1 : Form
    {
        // Update the connection string to match your setup
        string connectionString = @"Server=localhost\SQLEXPRESS;Database=Products;Trusted_Connection=True;Encrypt=False;";

        public Form1()
        {
            InitializeComponent();
        }

        private void btnLoadData_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = "SELECT * FROM Produse"; // Replace 'Users' with your table name
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView1.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "INSERT INTO Produse (name, price, in_stock) VALUES (@name, @price, @in_stock)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@name", txtName.Text);
                cmd.Parameters.AddWithValue("@price", float.Parse(txtPrice.Text));
                cmd.Parameters.AddWithValue("@in_stock", int.Parse(txtInStock.Text));

                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Product added.");
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "UPDATE Produse SET name = @name, price = @price, in_stock = @in_stock WHERE id = @id";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@name", txtName.Text);
                cmd.Parameters.AddWithValue("@price", float.Parse(txtPrice.Text));
                cmd.Parameters.AddWithValue("@in_stock", int.Parse(txtInStock.Text));

                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Product updated.");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "DELETE FROM Produse WHERE id = @id";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", int.Parse(txtId.Text));

                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Product deleted.");
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                txtId.Text = row.Cells["id"].Value.ToString();
                txtName.Text = row.Cells["name"].Value.ToString();
                txtPrice.Text = row.Cells["price"].Value.ToString();
                txtInStock.Text = row.Cells["in_stock"].Value.ToString();

                tabControl1.SelectedTab = tabEdit;
            }
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {

        }
    }
}