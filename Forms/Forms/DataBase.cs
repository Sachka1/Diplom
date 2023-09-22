using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Forms
{
    class DataBase
    {
        public VacancyResponse vacancies { get; set; }

        public DataBase() 
        { 
            vacancies = new VacancyResponse();
        }
        public MySqlConnection GetDBConnection()
        {
            string host = "localhost";
            int port = 3306;
            string database = "ozzi";
            string username = "root";
            string password = "root";

            return GetDBConnection(host, port, database, username, password);
        }

        public MySqlConnection GetDBConnection(string host, int port, string database, string username, string password)
        {
            var connString =
                "Server=" + host
                + ";Database=" + database
                + ";port=" + port
                + ";User Id=" + username
                + ";password=" + password;

            var conn = new MySqlConnection(connString);

            return conn;
        }

        public MySqlConnection OpenConnection()
        {
            var conn = GetDBConnection();

            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex);
            }

            return conn;
        }

        public MySqlConnection CloseConnection()
        {
            var conn = GetDBConnection();

            try
            {
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex);
            }

            return conn;
        }

        public void DataWrite(VacancyResponse response, string query)
        {
            try
            {
                foreach (Vacancy vacancy in response.Items)
                {
                    // работа с sql

                    var sqlQuery1 = @"INSERT INTO vacancy 
                                    (id, name, url, experience, area_id, salary_from, salary_to, currency, published) 
                                    SELECT * FROM 
                                        (SELECT 
                                            @Id,
                                            @Name, 
                                            @Url, 
                                            @ExperienceName AS experience,                                     
                                            @AreaName AS area_id, 
                                            @SalaryFrom AS salary_from, 
                                            @SalaryTo AS salary_to,
                                            @SalaryCurrency AS currency, 
                                            @published AS published) 
                                         AS tmp
                                            WHERE NOT EXISTS 
                                                (SELECT id FROM vacancy WHERE id = @Id)";

                    var conn = OpenConnection();
                    
                    MySqlCommand command = conn.CreateCommand();
                    command.CommandText = sqlQuery1;

                    MySqlParameter id = command.Parameters.Add("@Id", MySqlDbType.Int64);
                    id.Value = vacancy.Id;
                    
                    MySqlParameter name = command.Parameters.Add("@Name", MySqlDbType.VarString);
                    name.Value = vacancy.Name;

                    MySqlParameter url = command.Parameters.Add("@Url", MySqlDbType.VarChar);
                    url.Value = vacancy.Url;

                    MySqlParameter experience = command.Parameters.Add("@ExperienceName", MySqlDbType.VarChar);
                    experience.Value = vacancy.Experience?.Name;

                    MySqlParameter area_id = command.Parameters.Add("@AreaName", MySqlDbType.VarChar);
                    area_id.Value = vacancy.Area?.Name;

                    MySqlParameter Salary_from = command.Parameters.Add("@SalaryFrom", MySqlDbType.VarChar);
                    Salary_from.Value = vacancy.Salary?.From;

                    MySqlParameter Salary_to = command.Parameters.Add("@SalaryTo", MySqlDbType.VarChar);
                    Salary_to.Value = vacancy.Salary?.To;

                    MySqlParameter Salary_cur = command.Parameters.Add("@SalaryCurrency", MySqlDbType.VarChar);
                    Salary_cur.Value = vacancy.Salary?.Currency;

                    MySqlParameter published = command.Parameters.Add("@published", MySqlDbType.DateTime);
                    DateTime dateTime = DateTime.Parse(vacancy.Published_at);
                    published.Value = dateTime.ToString("yyyy-MM-dd HH:mm:ss");

                    command.ExecuteNonQuery();

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public VacancyResponse DataRead()
        {
            var conn = OpenConnection();
            var sqlQuery = $"select * from vacancy";
            using MySqlCommand command = new MySqlCommand(sqlQuery, conn);
            using MySqlDataReader sqlDataReader = command.ExecuteReader();
            {
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read()) // построчно считываем данные
                    {
                        var vacancy = new Vacancy(sqlDataReader);
                        vacancies.Items.Add(vacancy);
                    }
                }
                else
                {
                    MessageBox.Show("Не удалось получить результаты.");
                    vacancies.Items = null;
                }
            }
            conn.Close();
            return vacancies;
        }
        public void DataDrop(string sqlQuery)
        {
            var conn = OpenConnection();

            try
            {
                MySqlCommand command = new MySqlCommand(sqlQuery, conn);
                if (command.ExecuteNonQuery() > 0)
                    MessageBox.Show("Данные успешно удалены");
                conn.Close();
                conn.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
