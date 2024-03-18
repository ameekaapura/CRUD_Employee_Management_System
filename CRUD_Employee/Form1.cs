using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CRUD_Employee
{
    public partial class form1 : Form
    {
        SqlConnection con;
        SqlCommand cmd;

        public form1()
        {
            InitializeComponent();
            con = new SqlConnection("Data Source=LENOVO\\MSSQLSERVER01;Initial Catalog=Employee_db;Integrated Security=True");
            cmd = new SqlCommand();
            cmd.Connection = con;
        }

        // INSERT BUTTON
        private void insertBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(employee_id.Text) ||
                    string.IsNullOrEmpty(first_name.Text) ||
                    string.IsNullOrEmpty(last_name.Text) ||
                    string.IsNullOrEmpty(address.Text) ||
                    string.IsNullOrEmpty(phone_number.Text))
                {
                    MessageBox.Show("Please fill in any fields.");
                    return;
                }

                con.Open();

                SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Employee WHERE Employee_Id = @Employee_Id", con);
                checkCmd.Parameters.AddWithValue("@Employee_Id", int.Parse(employee_id.Text));
                int existingRecords = (int)checkCmd.ExecuteScalar();

                if (existingRecords > 0)
                {
                    MessageBox.Show("Same Employee Id already exists. Please choose a different Employee Id.");
                    return;
                }
                DialogResult result = MessageBox.Show("Are you sure you want to insert this employee?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO Employee(Employee_Id, First_Name, Last_Name, Address, Phone_Number) VALUES (@Employee_Id,@First_Name,@Last_Name,@Address,@Phone_Number)", con);

                    cmd.Parameters.AddWithValue("@Employee_Id", int.Parse(employee_id.Text));
                    cmd.Parameters.AddWithValue("@First_Name", (first_name.Text));
                    cmd.Parameters.AddWithValue("@Last_Name", (last_name.Text));
                    cmd.Parameters.AddWithValue("@Address", (address.Text));
                    cmd.Parameters.AddWithValue("@Phone_Number", (phone_number.Text));

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Successfully Inserted!");
                }
                else
                {
                    MessageBox.Show("Insert operation canceled.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured: " + ex.Message);
            }
            finally
            {
                con.Close();
                ClearTextBoxes();
                RefreshDataGridView();
            }
        }

        // UPDATE BUTTON
        private void updateBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(employee_id.Text) ||
                    string.IsNullOrEmpty(first_name.Text) ||
                    string.IsNullOrEmpty(last_name.Text) ||
                    string.IsNullOrEmpty(address.Text) ||
                    string.IsNullOrEmpty(phone_number.Text))
                {
                    MessageBox.Show("Please fill in any fields.");
                    return;
                }

                con.Open();

                SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Employee WHERE Employee_Id = @Employee_Id", con);
                checkCmd.Parameters.AddWithValue("@Employee_Id", int.Parse(employee_id.Text));
                int existingRecords = (int)checkCmd.ExecuteScalar();

                if (existingRecords == 0)
                {
                    if (MessageBox.Show("Employee with the specified Employee ID does not exist. Do you want to insert it?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        SqlCommand cmd = new SqlCommand("INSERT INTO Employee(Employee_Id, First_Name, Last_Name, Address, Phone_Number) VALUES (@Employee_Id,@First_Name,@Last_Name,@Address,@Phone_Number)", con);

                        cmd.Parameters.AddWithValue("@Employee_Id", int.Parse(employee_id.Text));
                        cmd.Parameters.AddWithValue("@First_Name", (first_name.Text));
                        cmd.Parameters.AddWithValue("@Last_Name", (last_name.Text));
                        cmd.Parameters.AddWithValue("@Address", (address.Text));
                        cmd.Parameters.AddWithValue("@Phone_Number", (phone_number.Text));

                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Successfully Inserted!");
                    }
                }
                else
                {
                    SqlCommand cmd = new SqlCommand("UPDATE Employee SET First_Name = COALESCE(@First_Name, First_Name), Last_Name = COALESCE(@Last_Name, Last_Name), Address = COALESCE(@Address, Address), Phone_Number = COALESCE(@Phone_Number, Phone_Number) WHERE Employee_Id = @Employee_Id", con);

                    cmd.Parameters.AddWithValue("@Employee_Id", int.Parse(employee_id.Text));
                    cmd.Parameters.AddWithValue("@First_Name", string.IsNullOrEmpty(first_name.Text) ? DBNull.Value : (object)first_name.Text);
                    cmd.Parameters.AddWithValue("@Last_Name", string.IsNullOrEmpty(last_name.Text) ? DBNull.Value : (object)last_name.Text);
                    cmd.Parameters.AddWithValue("@Address", string.IsNullOrEmpty(address.Text) ? DBNull.Value : (object)address.Text);
                    cmd.Parameters.AddWithValue("@Phone_Number", string.IsNullOrEmpty(phone_number.Text) ? DBNull.Value : (object)phone_number.Text);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Successfully Updated!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                ClearTextBoxes();
                RefreshDataGridView();
            }
        }

        // DELETE BUTTON
        private void deleteBtn_Click(object sender, EventArgs e)
        {

            try
            {
                    if (string.IsNullOrEmpty(employee_id.Text) ||
                    string.IsNullOrEmpty(first_name.Text) ||
                    string.IsNullOrEmpty(last_name.Text) ||
                    string.IsNullOrEmpty(address.Text) ||
                    string.IsNullOrEmpty(phone_number.Text))
                    {
                        MessageBox.Show("Please fill in all fields.");
                        return;
                    }
                
                    con.Open();

                    SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Employee WHERE Employee_Id = @Employee_Id", con);
                    checkCmd.Parameters.AddWithValue("@Employee_Id", int.Parse(employee_id.Text));
                    int existingRecords = (int)checkCmd.ExecuteScalar();

                    if (existingRecords == 0)
                    {
                        MessageBox.Show("Employee with the specified Employee Id does not exist.");
                        return;
                    }
                    if (MessageBox.Show("Are you sure you want to delete this record?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                    SqlCommand cmd = new SqlCommand("DELETE Employee WHERE Employee_Id = @Employee_Id", con);

                    cmd.Parameters.AddWithValue("@Employee_Id", int.Parse(employee_id.Text));

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Successfully Deleted!");
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured: " + ex.Message);
            }
            finally
            {
                con.Close();
                ClearTextBoxes();
                RefreshDataGridView();
            }
        }

        // SHOW ALL BUTTON
        private void showBtn_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM Employee", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView.DataSource = dt;

                findBar.Text = "";

                ClearTextBoxes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured:" + ex.Message);
            }
            finally
            {
                con.Close();
            }

        }

        // FIND BUTTON
        private void findBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(findBar.Text))
            {
                MessageBox.Show("Please enter search criteria.");
                return;
            }

            try
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.Text;

                dataGridView.DataSource = null;

                if (int.TryParse(findBar.Text, out int employeeId))
                {
                    cmd.CommandText = "SELECT * FROM Employee WHERE Employee_Id = @SearchText";
                    cmd.Parameters.AddWithValue("@SearchText", employeeId);
                }
                else
                {
                    cmd.CommandText = "SELECT * FROM Employee WHERE First_Name = @SearchText OR Last_Name = @SearchText OR Address = @SearchText OR Phone_Number = @SearchText";
                    cmd.Parameters.AddWithValue("@SearchText", findBar.Text);
                }

                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                con.Close();

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    employee_id.Text = row["Employee_Id"].ToString();
                    first_name.Text = row["First_Name"].ToString();
                    last_name.Text = row["Last_Name"].ToString();
                    address.Text = row["Address"].ToString();
                    phone_number.Text = row["Phone_Number"].ToString();

                    dataGridView.DataSource = dt;
                }
                else
                {
                    MessageBox.Show("No matching records found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured:" + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }

        }

        // CLEAR ENTRY
        private void clearBtn_Click(object sender, EventArgs e)
        {
            ClearTextBoxes();
        }

        private void form1_Load(object sender, EventArgs e)
        {
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM Employee", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView.DataSource = dt;

                findBar.Text = "";

                ClearTextBoxes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured:" + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        // REFRESH DATA GRID VIEW
        private void RefreshDataGridView()
        {
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM Employee", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView.DataSource = dt;

                findBar.Text = "";

            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred:" + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }

        }

        // CLEAR TEXT BOXES
        private void ClearTextBoxes()
        {
            employee_id.Text = "";
            first_name.Text = "";
            last_name.Text = "";
            address.Text = "";
            phone_number.Text = "";
        }

      
    }
}
