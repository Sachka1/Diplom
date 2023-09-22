using Microsoft.Office.Interop.Excel;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Collections.Generic;

namespace Forms
{
    public partial class MainForm : Form
    {
        ApiGraber apiGraber;
        DataBase database;
        ExcelHelper excelHelper;
        string City = "";

        public MainForm()
        {
            InitializeComponent();
            apiGraber = new ApiGraber();
            database = new DataBase();
            excelHelper = new ExcelHelper();

            this.Height = 215;
            checkPassButton.Visible = false;
        }


        private void WriteData_Click(object sender, EventArgs e)
        {
            CheckCity();
            if (City == "-1")
            {
                MessageBox.Show("������� �����, �������� ��� � ������.");
                return;
            }
            var sqlRequest = "SELECT * FROM request WHERE name = @Name";
            var conn = database.OpenConnection();
            MySqlCommand command = conn.CreateCommand();
            command.CommandText = sqlRequest;
            MySqlParameter name = command.Parameters.Add("@Name", MySqlDbType.VarString);
            name.Value = QueryComboBox.Text.ToLower();
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                var reportExcel = new ExcelHelper().Generate(database.DataRead(), QueryComboBox.Text.ToLower(), City);
                File.WriteAllBytes("../../../������_" + QueryComboBox.Text.ToLower() + ".xlsx", reportExcel);
                MessageBox.Show("������ ������� � Excel �����");
            }
            else
            {
                DialogResult result = MessageBox.Show($"�� �������: {QueryComboBox.Text.ToLower()} - ��� ���������� � ���� ������.\n" +
                    "��������� ������ � ���������?\n" +
                    "��� ����� ������ 30-45 ������\n" +
                    "��-�� ����������� ��������� ������ �������� �������� ������ ������ 2000 ��������.\n",
                    "���������",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly);
                if (result == DialogResult.Yes)
                {
                    apiGraber.query = QueryComboBox.Text.ToLower();
                    WriteDataInTable("request", apiGraber.query);
                    var foundedVacancy = new VacancyResponse();
                    var page = 20;
                    for (int i = 0; i < page + 1; i++)
                    {
                        foundedVacancy = apiGraber.GetRequest(i);
                        if (foundedVacancy?.Items.Count != 0)
                        {
                            apiGraber.DataWrite();
                        }
                    }
                    var reportExcel = new ExcelHelper().Generate(database.DataRead(), QueryComboBox.Text.ToLower(), City);
                    File.WriteAllBytes("������_" + QueryComboBox.Text.ToLower() + ".xlsx", reportExcel);
                    MessageBox.Show(
                                    "������ ������� � �������� � Excel �����",
                                    "���������",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
            }
            conn.Close();

            comboBoxQueryUpdate();
        }

        public void WriteDataInTable(string table, string name)
        {
            var sqlQuery = @$"INSERT INTO {table} (
                                name)
                            VALUES (
                                '{name}')";
            var conn = database.OpenConnection();
            MySqlCommand command = conn.CreateCommand();
            command.CommandText = sqlQuery;

            command.ExecuteNonQuery();
            conn.Close();
        }

        private void DeleteData_Click(object sender, EventArgs e)
        {
            //quantytyQu.Text = "���������� ������������ �������: ";
            QueryComboBox.Text = String.Empty;
            string sqlQuery = "DELETE FROM request";
            database.DataDrop(sqlQuery); ;
        }

        private void AdminButton_Click(object sender, EventArgs e)
        {
            this.Height = 250;
            PasswordBox.Visible = true;
            PasswordBox.UseSystemPasswordChar = true;
            checkPassButton.Visible = true;


        }

        private void checkPassButton_Click(object sender, EventArgs e)
        {
            if (PasswordBox.Text == "admin")
            {
                this.Height = 220;
                DeleteData.Visible = true;
                DeleteQuery.Visible = true;
                checkPassButton.Visible = false;
                PasswordBox.Visible = false;
                AdminButton.Visible = false;
            }
            else
            {
                MessageBox.Show(
                    "������ ����� ������",
                    "�������� ������",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            comboBoxQueryUpdate();
            comboBoxCityUpdate();
        }

        public List<string> GetListFromBD(string table)
        {
            List<string> list = new List<string>();
            var sqlRequest = @$"SELECT name FROM {table}";
            var conn = database.OpenConnection();
            MySqlCommand command = conn.CreateCommand();
            command.CommandText = sqlRequest;
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    list.Add(Convert.ToString(reader.GetValue(0)));
                }
            }
            conn.Close();
            return list;
        }

        public void comboBoxQueryUpdate()
        {
            QueryComboBox.Items.Clear();
            QueryComboBox.Sorted = true;
            QueryComboBox.MaxDropDownItems = 3;

            QueryComboBox.Items.AddRange(GetListFromBD("request").ToArray());
        }

        public void comboBoxCityUpdate()
        {
            CitiesComboBox.Items.Clear();
            CitiesComboBox.Sorted = true;
            CitiesComboBox.MaxDropDownItems = 10;

            CitiesComboBox.Items.AddRange(GetListFromBD("tCities").ToArray());
        }

        private void DeleteQuery_Click(object sender, EventArgs e)
        {
            QueryComboBox.Text = String.Empty;
            string sqlQuery = "DELETE FROM vacancy";
            database.DataDrop(sqlQuery);
        }


        private void QueryComboBox_TextChanged(object sender, EventArgs e)
        {
            if (QueryComboBox.Text == String.Empty)
            {
                WriteDataButton.Enabled = false;
            }
            else
            {
                WriteDataButton.Enabled = true;
            }
        }

        public void CheckCity()
        {
            List<string> cities = GetListFromBD("tCities");
            foreach (string city in cities)
            {
                if (city.ToLower() == CitiesComboBox.Text.ToLower())
                {
                    CitiesComboBox.Text = String.Empty;
                    City = "-1";
                    return;
                }
            }
            City = CitiesComboBox.Text;
            return;
        }
    }
}