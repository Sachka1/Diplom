using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;

namespace Forms
{
    class ApiGraber
    {
        private string url = "https://api.hh.ru/vacancies"; // URL API hh.ru
        public string query = "";
       
        private string resultJson;
        private VacancyResponse response = new VacancyResponse();
        DataBase dataBase = new DataBase();
        public ApiGraber()
        {

        }

        public VacancyResponse? GetRequest(int page)
        {
            try
            {
                // Создаем объект WebClient для выполнения GET-запроса
                using (WebClient webClient = new WebClient())
                {
                    // Устанавливаем кодировку UTF-8 для корректного считывания результата
                    webClient.Encoding = System.Text.Encoding.UTF8;

                    // Устанавливаем User-Agent заголовок
                    webClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");

                    // Выполняем GET-запрос и получаем результат в виде строки
                    resultJson = webClient.DownloadString($"{url}?text={query}&page={page}&per_page=100");
                }

                // Десериализуем JSON-строку в объекты C#
                return response = JsonConvert.DeserializeObject<VacancyResponse>(resultJson);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public void DataWrite()
        {
            if (response != null)
            {
                DataBase dataBaseHelper = new DataBase();
                dataBaseHelper.DataWrite(response, query);
            }
        }
    }

    // Классы для десериализации JSON
    public class VacancyResponse
    {
        public int Found { get; set; }
        public List <Vacancy> Items { get; set; } = new List<Vacancy>();
    }

    public class Vacancy
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public Exp_Data? Experience { get; set; } = new Exp_Data();
        public Area_Data? Area { get; set; } = new Area_Data();
        public SalaryData? Salary { get; set; } = new SalaryData();
        public string Published_at { get; set; }
        public Vacancy () { }
        public Vacancy(MySqlDataReader reader) 
        {

            Id = Convert.ToString(reader.GetValue(0));
            Name = Convert.ToString(reader.GetValue(1));
            Url = Convert.ToString(reader.GetValue(2));
            Experience.Name = Convert.ToString(reader.GetValue(3));
            Area.Name = Convert.ToString(reader.GetValue(4));
            
            Salary.From = reader.GetValue(5) != DBNull.Value?
                Convert.ToInt32(reader.GetValue(5)):
                null;
            Salary.To = reader.GetValue(6) != DBNull.Value?
                Convert.ToInt32(reader.GetValue(6)):
                null;
            Salary.Currency = reader.GetValue(7) != DBNull.Value ?
                Convert.ToString(reader.GetValue(7)) :
                null; 
            Published_at = Convert.ToString(reader.GetValue(8));
        }
    }
    public class Exp_Data
    {
        public string? id { get; set; }
        public string? Name { get; set; }

    }
    public class Area_Data
    {
        public int? id { get; set; }
        public string? Name { get; set; }

    }
    public class SalaryData
    {
        public int? From { get; set; }
        public int? To { get; set; }
        public string? Currency { get; set; }
    }
}
