using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

namespace ToDoList
{
    public partial class Form1 : Form
    {
        public static string connectionStrig = @"Data Source=NOUR\SQLEXPRESS;Initial Catalog=ToDoList;Integrated Security=True";
        SqlConnection conn = new SqlConnection(connectionStrig);
        SqlCommand sqlcmd;
        string todostring, datestring;
        List<string> arrs = new List<string>();
        List<string> arrd = new List<string>();
        public void UserData()
        {
            string name = nameTextBox.Text;
            string pass = passwordTextBox.Text;
            string datacheck = "SELECT * FROM todoTable WHERE Name = @nama AND Pass = @passa";
            sqlcmd = new SqlCommand(datacheck, conn);
            sqlcmd.Parameters.AddWithValue("@nama", name);
            sqlcmd.Parameters.AddWithValue("@passa", pass);
            sqlcmd.ExecuteNonQuery();


        }
        public void accountDetails()
        {
            string name = nameTextBox.Text;
            string pass = passwordTextBox.Text;
            string data = todoTextBox.Text;
            string deadline = deadLineTextBox.Text;
            string insertString = "INSERT INTO todoTable (Name , Pass , ToDo , Date) VALUES (@nama , @passa , @todo , @deadline)";
            sqlcmd = new SqlCommand(insertString, conn);
            sqlcmd.Parameters.AddWithValue("@nama", name);
            sqlcmd.Parameters.AddWithValue("@passa", pass);
            sqlcmd.Parameters.AddWithValue("@todo", data);
            sqlcmd.Parameters.AddWithValue("@deadline", deadline);
            sqlcmd.ExecuteNonQuery();
        }
        public bool ValidData()
        {
            Regex reg = new Regex(@"^\d{1,2}/\d{1,2}$");
            if (!string.IsNullOrWhiteSpace(nameTextBox.Text) && !string.IsNullOrWhiteSpace(passwordTextBox.Text) && !string.IsNullOrWhiteSpace(todoTextBox.Text) && !string.IsNullOrWhiteSpace(deadLineTextBox.Text) && reg.IsMatch(deadLineTextBox.Text))
                return true;
            return false;
        }
        public void add_account()
        {

            if (ValidData())
            {
                try
                {

                    accountDetails();
                    MessageBox.Show("Account successfully created");
                }
                catch
                {
                    MessageBox.Show("Error occured, No account was created");
                }
            }
            else
            {
                MessageBox.Show("Enter a valid name & pass & at data for the todo list ");
            }


        }
        public void removeChecked()
        {
            todostring = "";
            datestring = "";
            foreach (int idx in checkedListBox1.CheckedIndices)
            {
                arrs[idx] = null;
                arrd[idx] = null;
            }
            for (int i = 0; i < arrs.Count; i++)
            {
                if (arrs[i] != null && i != 0)
                {
                    todostring += "," + arrs[i];
                    datestring += "," + arrd[i];
                }
                if (i == 0)
                {
                    todostring += arrs[i];
                    datestring += arrd[i];
                }

            }
        }
        public void updatedb()
        {
            string insertString = "UPDATE  todoTable SET ToDo=@Data , Date = @Date WHERE  Name = @txtboxName AND Pass = @txtboxPass ";
            sqlcmd = new SqlCommand(insertString, conn);
            sqlcmd.Parameters.AddWithValue("@txtboxName", nameTextBox.Text);
            sqlcmd.Parameters.AddWithValue("@txtboxPass", passwordTextBox.Text);
            sqlcmd.Parameters.AddWithValue("@Data", todostring);
            sqlcmd.Parameters.AddWithValue("@Date", datestring);
            sqlcmd.ExecuteNonQuery();
        }
        public void fetchUserData()
        {
            SqlDataReader rdr = null;
            rdr = sqlcmd.ExecuteReader();

            if (rdr.Read())
            {
                todostring = (string)rdr["ToDo"];
                datestring = (string)rdr["Date"];

                string[] tmpar = todostring.Split(',');
                foreach (var word in tmpar)
                {
                    arrs.Add(word);
                }
                string[] tmpd = datestring.Split(',');
                foreach (var word in tmpd)
                {
                    arrd.Add(word);
                }

                for (int i = 0; i < arrs.Count; i++)
                {

                    checkedListBox1.Items.Add(arrs[i] + " " + arrd[i]);
                }
            }


            if (rdr != null)
                rdr.Close();
        }

        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {



        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void addButton_Click(object sender, EventArgs e)
        {
            try
            {
                conn.Open();
            }
            catch
            {
                MessageBox.Show("Error connecting to DB");
            }
            add_account();


            if (conn != null)
                conn.Close();

        }


        private void doneButton_Click(object sender, EventArgs e)
        {
            try
            {
                conn.Open();
            }
            catch
            {
                MessageBox.Show("Error connecting to DB");
            }
            removeChecked();
            updatedb();
            checkedListBox1.Items.Clear();
            arrs.Clear();
            arrd.Clear();
            UserData();
            fetchUserData();
            if (conn != null)
                conn.Close();

        }

        private void showButton_Click(object sender, EventArgs e)
        {
            checkedListBox1.Items.Clear();
            arrs.Clear();
            arrd.Clear();
            try
            {
                conn.Open();
            }
            catch
            {
                MessageBox.Show("Error connecting to DB");
            }
            try
            {
                UserData();
                if (sqlcmd.ExecuteScalar() == null)
                    MessageBox.Show("Account not found");
                else
                {
                    fetchUserData();

                }
            }
            catch
            {
                MessageBox.Show("error executing query");
            }
            if (conn != null)
                conn.Close();
        }

        private void deadLineTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            string data = todoTextBox.Text;
            string deadline = deadLineTextBox.Text;
            Regex reg = new Regex(@"^\d{1,2}/\d{1,2}$");
            if (!reg.IsMatch(deadLineTextBox.Text))
            {
                MessageBox.Show("enter date in format of dd/mm");

            }
            else
            {


                if (!string.IsNullOrWhiteSpace(data) && !string.IsNullOrWhiteSpace(deadline))
                {
                    try
                    {
                        conn.Open();
                    }
                    catch
                    {
                        MessageBox.Show("Error connecting to DB");
                    }
                    todostring += "," + data;
                    datestring += "," + deadline;
                    arrs.Clear();
                    arrd.Clear();
                    updatedb();
                }
                else
                {
                    MessageBox.Show("Enter a valid data & deadline");
                }
                checkedListBox1.Items.Clear();
                arrs.Clear();
                arrd.Clear();
                UserData();
                fetchUserData();
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }
    }
}
