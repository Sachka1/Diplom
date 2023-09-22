using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using System.Data.SqlClient;
using System.Data;
using MySql.Data.MySqlClient;
using System.IO.Packaging;
using System.Windows.Input;
using Google.Protobuf.WellKnownTypes;

namespace Forms
{
    class ExcelHelper
    {
        
        public byte[] Generate(VacancyResponse response, string query, string city = "")
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            var package = new ExcelPackage();
            DataBase dataBase = new DataBase();

            // Работа с 1 страинцей - Вакансии
            var vacancies = package.Workbook.Worksheets.Add(query);
            vacancies.Cells["A1"].Value = "Найдено вакансий по запросу: ";
            vacancies.Columns[1].Width = 32;
            vacancies.Cells["B1"].Value = "Название вакансии";
            vacancies.Columns[2].Width = 40;
            vacancies.Cells["C1"].Value = "Опыт работы";
            vacancies.Columns[3].Width = 17;
            vacancies.Cells["D1"].Value = "Город";
            vacancies.Columns[3, 4].Width = 23;
            vacancies.Cells["E1"].Value = "Зарплата от";
            vacancies.Cells["F1"].Value = "Зарплата до";
            vacancies.Columns[5, 6].Width = 13;
            vacancies.Cells["G1"].Value = "Валюта";
            vacancies.Cells["H1"].Value = "Дата публикации";
            vacancies.Columns[8].Width = 25;
            vacancies.Cells["I1"].Value = "Ссылка на вакансию";
            vacancies.Columns[9].Width = 45;

            var querySelect = @$"SELECT * FROM 
                                    vacancy 
                                WHERE LOWER
                                    (name) 
                                LIKE LOWER
                                ('%{query}%')";


            if (query == "программист" || query == "разработчик"
                || query == "developer" || query == "junior")
            {
                querySelect = @"SELECT * FROM 
                                    vacancy 
                                WHERE LOWER 
                                    (name) 
                                LIKE LOWER 
                                    ('%программист%') 
                                OR
                                    ('%разработчик%') 
                                OR
                                    ('%developer%') 
                                OR
                                    ('%junior%')
                                OR
                                    ('%php%') 
                                OR
                                    ('%c#%') 
                                OR
                                    ('%c++%')
                                OR
                                    ('%java%') 
                                OR
                                    ('%javascript%') 
                                OR
                                    ('%python%')";
            }                               

            using var conn = dataBase.OpenConnection();

            MySqlCommand command = new MySqlCommand(querySelect, conn);

            using MySqlDataReader reader = command.ExecuteReader();
            {
                VacancyResponse vac = new VacancyResponse();
                if (reader.HasRows)
                {
                    var row = 2;
                    var column = 1;
                    while (reader.Read()) 
                    {
                        vacancies.Cells[row, column].Value = row - 1;
                        vacancies.Cells[row, column + 1].Value = Convert.ToString(reader.GetValue(1));
                        vacancies.Cells[row, column + 2].Value = Convert.ToString(reader.GetValue(3));
                        vacancies.Cells[row, column + 3].Value = Convert.ToString(reader.GetValue(4));
                        vacancies.Cells[row, column + 4].Value = reader.GetValue(5) != DBNull.Value ?
                                                                 Convert.ToInt32(reader.GetValue(5)) :
                                                                 null;
                        vacancies.Cells[row, column + 5].Value = reader.GetValue(6) != DBNull.Value ?
                                                                 Convert.ToInt32(reader.GetValue(6)) :
                                                                 null;
                        vacancies.Cells[row, column + 6].Value = reader.GetValue(7) != DBNull.Value ?
                                                                 Convert.ToString(reader.GetValue(7)) :
                                                                 null;
                        vacancies.Cells[row, column + 7].Value = Convert.ToString(reader.GetValue(8));
                        vacancies.Cells[row, column + 8].Value = Convert.ToString(reader.GetValue(2));

                        row++;
                    }
                    vacancies.Cells["A1"].Value += $"{row - 2}";
                }
            }
            conn.Close();

            // Работа со 2 страницей - Зарплаты
            var report = package.Workbook.Worksheets.Add("Сводка по Заработной Плате");
            report.Cells["A1"].Value = "Зарплата, RUR";
            report.Columns[1].Width = 18;
            report.Cells["B1"].Value = "Количество";
            report.Columns[2].Width = 12;
            report.Cells["A2"].Value = "До 25 000";
            report.Cells["A3"].Value = "25 000 - 50 000";
            report.Cells["A4"].Value = "50 000 - 75 000";
            report.Cells["A5"].Value = "75 000 - 100 000";
            report.Cells["A6"].Value = "От 100 000";

            string queryReport = @$"SELECT COUNT(id) FROM  
                                        vacancy 
                                    WHERE 
                                        salary_from <= 25000 
                                    AND 
                                        currency = 'RUR' 
                                    AND LOWER 
                                        (name)
                                    LIKE LOWER 
                                        ('%{query}%')";
            QueryExecute(report,query, queryReport, 2, 2); // B2

            queryReport = @$"SELECT COUNT(id) FROM 
                                vacancy 
                            WHERE 
                                salary_from > 25000 
                            AND 
                                salary_from <= 50000 
                            AND 
                                currency = 'RUR' 
                            AND LOWER 
                                (name)
                            LIKE LOWER 
                                ('%{query}%')";
            QueryExecute(report, query, queryReport, 3, 2); // B3

            queryReport = @$"SELECT COUNT(id) FROM 
                                vacancy 
                            WHERE 
                                salary_from > 50000 
                            AND 
                                salary_from <= 75000 
                            AND 
                                currency = 'RUR' 
                            AND LOWER 
                                (name)
                            LIKE LOWER 
                                ('%{query}%')"; 
            QueryExecute(report, query, queryReport, 4, 2); // B4

            queryReport = @$"SELECT COUNT(id) FROM 
                                vacancy 
                            WHERE 
                                salary_from > 75000
                            AND 
                                salary_from <= 100000
                            AND 
                                currency = 'RUR' 
                            AND LOWER 
                                (name)
                            LIKE LOWER 
                                ('%{query}%')";
            QueryExecute(report, query, queryReport, 5, 2); // B5

            queryReport = @$"SELECT COUNT(id) FROM 
                                vacancy 
                            WHERE 
                                salary_from > 100000
                            AND 
                                currency = 'RUR'
                            AND LOWER 
                                (name)
                            LIKE LOWER 
                                ('%{query}%')";
            QueryExecute(report, query, queryReport, 6, 2); // B6
            
            queryReport = @$"SELECT AVG(salary_from) FROM 
                                vacancy 
                            WHERE 
                                currency = 'RUR' 
                            AND 
                                salary_from != 0
                            AND LOWER 
                                (name)
                            LIKE LOWER 
                                ('%{query}%')";
            QueryExecute(report, query, queryReport, 8, 2); // B8

            queryReport = @$"SELECT AVG(salary_to) FROM 
                                vacancy 
                            WHERE 
                                currency = 'RUR' 
                            AND 
                                salary_from != 0
                            AND LOWER 
                                (name)
                            LIKE LOWER 
                                ('%{query}%')";
            QueryExecute(report, query, queryReport, 9, 2); // B9
            
            report.Cells["A8"].Value = "Ср. Зарпалата От";
            report.Cells["A9"].Value = "Ср. Зарпалата До";

            // Гистограмма
            var capitalizationChart = report.Drawings.AddChart("Зарплаточки", OfficeOpenXml.Drawing.Chart.eChartType.ColumnClustered);
            capitalizationChart.Title.Text = "Зарплаты";
            capitalizationChart.SetPosition(1, 4, 6, 7);
            capitalizationChart.SetSize(700, 400);
            var capitalizationData = (ExcelChartSerie)(capitalizationChart.Series.Add(report.Cells["B2:B6"], report.Cells["A2:A6"]));
            capitalizationData.Header = "Количество";

            // Работа с третей страинцей - Города
            var cities = package.Workbook.Worksheets.Add("Сводка по городам");
            cities.Cells["A1"].Value = "Города России";
            cities.Columns[1].Width = 16;
            cities.Cells["B1"].Value = "Количество вакансий в этих городах";
            cities.Columns[2].Width = 34;
            cities.Cells["A2"].Value = "Москва";
            cities.Cells["A3"].Value = "Санкт-Петербург";
            cities.Cells["A4"].Value = "Екатеринбург";
            cities.Cells["A5"].Value = "Казань";
            cities.Cells["A6"].Value = "Новосибирск";
            cities.Cells["A7"].Value = "Владивосток";
            cities.Cells["A8"].Value = "Краснодар";
            cities.Cells["A9"].Value = "Ярославль";
            cities.Cells["A10"].Value = "Ижевск";
            cities.Cells["A11"].Value = "Воронеж";
            cities.Cells["A12"].Value = "Другие города";

            string queryCities = @$"SELECT COUNT(id) FROM 
                                        vacancy 
                                    WHERE 
                                        area_id = 'Москва'
                                    AND LOWER 
                                        (name)
                                    LIKE LOWER 
                                        ('%{query}%')";
            QueryExecute(cities, query, queryCities, 2, 2); // B2

            queryCities = @$"SELECT COUNT(id) FROM 
                                vacancy
                            WHERE 
                                area_id = 'Санкт-Петербург'
                            AND LOWER 
                                (name)
                            LIKE LOWER 
                                 ('%{query}%')";
            QueryExecute(cities, query, queryCities, 3, 2); // B3

            queryCities = @$"SELECT COUNT(id) FROM 
                                vacancy 
                            WHERE 
                                area_id = 'Екатеринбург'
                            AND LOWER 
                                (name)
                            LIKE LOWER 
                                 ('%{query}%')";
            QueryExecute(cities, query, queryCities, 4, 2); // B4

            queryCities = @$"SELECT COUNT(id) FROM 
                                vacancy 
                            WHERE 
                                area_id = 'Казань'
                            AND LOWER 
                                (name)
                            LIKE LOWER 
                                 ('%{query}%')";
            QueryExecute(cities, query, queryCities, 5, 2); // B5

            queryCities = @$"SELECT COUNT(id) FROM 
                                vacancy 
                            WHERE 
                                area_id = 'Новосибирск'
                            AND LOWER 
                                (name)
                            LIKE LOWER 
                                 ('%{query}%')";
            QueryExecute(cities, query, queryCities, 6, 2); // B6
            
            queryCities = @$"SELECT COUNT(id) FROM
                                vacancy 
                            WHERE 
                                area_id = 'Владивосток'
                            AND LOWER 
                                (name)
                            LIKE LOWER 
                                 ('%{query}%')";
            QueryExecute(cities, query, queryCities, 7, 2); // B7

            queryCities = @$"SELECT COUNT(id) FROM 
                                vacancy 
                            WHERE 
                                area_id = 'Краснодар'
                            AND LOWER 
                                (name)
                            LIKE LOWER 
                                 ('%{query}%')";
            QueryExecute(cities, query, queryCities, 8, 2); // B8

            queryCities = @$"SELECT COUNT(id) FROM
                                vacancy 
                            WHERE 
                                area_id = 'Ярославль'
                            AND LOWER 
                                (name)
                            LIKE LOWER 
                                 ('%{query}%')";
            QueryExecute(cities, query, queryCities, 9, 2); // B9

            queryCities = @$"SELECT COUNT(id) FROM 
                                vacancy 
                            WHERE 
                                area_id = 'Ижевск'
                            AND LOWER 
                                (name)
                            LIKE LOWER 
                                 ('%{query}%')";
            QueryExecute(cities, query, queryCities, 10, 2); // B10

            queryCities = @$"SELECT COUNT(id) FROM 
                                vacancy 
                            WHERE 
                                area_id = 'Воронеж'
                            AND LOWER 
                                (name)
                            LIKE LOWER 
                                 ('%{query}%')";
            QueryExecute(cities, query, queryCities, 11, 2); // B11

            queryCities = @$"SELECT COUNT(id) FROM 
                                vacancy 
                            WHERE
                                area_id != 'Москва' 
                            AND
                                area_id != 'Санкт-Петербург'  
                            AND
                                area_id != 'Екатеринбург'  
                            AND
                                area_id != 'Казань'  
                            AND
                                area_id != 'Новосибирск'  
                            AND
                                area_id != 'Владивосток'  
                            AND
                                area_id != 'Краснодар'  
                            AND
                                area_id != 'Ярославль'  
                            AND
                                area_id != 'Ижевск'  
                            AND
                                area_id != 'Воронеж'
                            AND
                                area_id != 'Минск'  
                            AND
                                area_id != 'Алматы'  
                            AND
                                area_id != 'Астана'
                            AND LOWER 
                                (name)
                            LIKE LOWER 
                                 ('%{query}%')
                            AND
                                (currency IS NULL
                            OR
                                currency = 'RUR')";
            if (city != null)
                queryCities = @$"SELECT COUNT(id) FROM 
                                vacancy 
                            WHERE
                                area_id != 'Москва' 
                            AND
                                area_id != 'Санкт-Петербург'  
                            AND
                                area_id != 'Екатеринбург'  
                            AND
                                area_id != 'Казань'  
                            AND
                                area_id != 'Новосибирск'  
                            AND
                                area_id != 'Владивосток'  
                            AND
                                area_id != 'Краснодар'  
                            AND
                                area_id != 'Ярославль'  
                            AND
                                area_id != 'Ижевск'  
                            AND
                                area_id != 'Воронеж'
                            AND
                                area_id != 'Минск'  
                            AND
                                area_id != 'Алматы'  
                            AND
                                area_id != 'Астана'
                            AND
                                area_id != '{city}'
                            AND LOWER 
                                (name)
                            LIKE LOWER 
                                 ('%{query}%')
                            AND
                                (currency IS NULL
                            OR
                                currency = 'RUR')";
            QueryExecute(cities, query, queryCities, 12, 2); // B12

            cities.Cells["A15"].Value = "Процентное соотношение вакансий по городам";
            cities.Columns[1].Width = 45;
            cities.Cells["A16"].Value = "Москва";
            var count = 0;
            for(int i = 2; i < 13; i++)
                count += Convert.ToInt32(cities.Cells[i,2].Value);
            cities.Cells["B16"].Value = Convert.ToDouble(cities.Cells["B2"].Value) / count * 100;
            

            capitalizationChart = cities.Drawings.AddChart("Города", OfficeOpenXml.Drawing.Chart.eChartType.Doughnut);
            capitalizationChart.Title.Text = "Города";
            capitalizationChart.SetPosition(1, 4, 6, 7);
            capitalizationChart.SetSize(550, 400);
            if (city != null)
            {
                cities.Cells["A13"].Value = city;
                queryCities = @$"SELECT COUNT(id) FROM 
                                vacancy 
                            WHERE 
                                area_id = '{city}'
                            AND LOWER 
                                (name)
                            LIKE LOWER 
                                 ('%{query}%')";
                QueryExecute(cities, query, queryCities, 13, 2); // B13

                cities.Cells["A17"].Value = city;
                cities.Cells["B17"].Value = Convert.ToDouble(cities.Cells["B13"].Value) / count * 100;
                capitalizationData = (ExcelChartSerie)(capitalizationChart.Series.Add(cities.Cells["B2:B13"], cities.Cells["A2:A13"]));
            }
            else 
            {
                cities.Cells["A17"].Value = "Другие города";
                cities.Cells["B17"].Value = Convert.ToDouble(cities.Cells["B12"].Value) / count * 100;
                capitalizationData = (ExcelChartSerie)(capitalizationChart.Series.Add(cities.Cells["B2:B12"], cities.Cells["A2:A12"])); 
            }
            capitalizationData.Header = "Количество";

            // Работа с 4 страницей - Опыт работы
            var experience = package.Workbook.Worksheets.Add("Сводка по Опыту работы");
            experience.Cells["A1"].Value = "Опыт работы";
            experience.Columns[1].Width = 16;
            experience.Cells["B1"].Value = "Количество вакансий";
            experience.Columns[2].Width = 22;
            experience.Cells["A2"].Value = "Нет опыта";
            experience.Cells["A3"].Value = "От 1 года до 3 лет";
            experience.Cells["A4"].Value = "От 3 до 6 лет";
            experience.Cells["A5"].Value = "Более 6 лет";

            string queryExperience = @$"SELECT COUNT(id) FROM 
                                            vacancy 
                                        WHERE 
                                            experience = 'Нет опыта'
                                        AND LOWER 
                                            (name)
                                        LIKE LOWER 
                                            ('%{query}%')";
            QueryExecute(experience, query, queryExperience, 2, 2); // B2

            queryExperience = @$"SELECT COUNT(id) FROM 
                                    vacancy 
                                WHERE 
                                    experience = 'От 1 года до 3 лет'
                                AND LOWER 
                                    (name)
                                LIKE LOWER 
                                    ('%{query}%')";
            QueryExecute(experience, query, queryExperience, 3, 2); // B3

            queryExperience = @$"SELECT COUNT(id) FROM 
                                    vacancy 
                                WHERE 
                                    experience = 'От 3 до 6 лет'
                                AND LOWER 
                                    (name)
                                LIKE LOWER 
                                    ('%{query}%')";
            QueryExecute(experience, query, queryExperience, 4, 2); // B4

            queryExperience = @$"SELECT COUNT(id) FROM 
                                    vacancy 
                                WHERE 
                                    experience = 'Более 6 лет'
                                AND LOWER 
                                    (name)
                                LIKE LOWER 
                                    ('%{query}%')";
            QueryExecute(experience, query, queryExperience, 5, 2); // B5

            experience.Cells["A7"].Value = "Процентное соотношение вакансий для специалистов без опыта";
            experience.Columns[1].Width = 62;

            count = 0;
            for (int i = 2; i < 6; i++)
                count += Convert.ToInt32(experience.Cells[i, 2].Value);
            experience.Cells["B7"].Value = Convert.ToDouble(experience.Cells["B2"].Value) / count * 100;

            capitalizationChart = experience.Drawings.AddChart("Опыт работы", OfficeOpenXml.Drawing.Chart.eChartType.BarStacked);
            capitalizationChart.Title.Text = "Опыт работы";
            capitalizationChart.SetPosition(1, 4, 6, 7);
            capitalizationChart.SetSize(700, 400);
            capitalizationData = (ExcelChartSerie)(capitalizationChart.Series.Add(experience.Cells["B2:B5"], experience.Cells["A2:A5"]));
            capitalizationData.Header = "Количество";

            return package.GetAsByteArray();
        }

        public void QueryExecute(ExcelWorksheet excelWorksheet,string query, string sqlQuery, int row, int collumn)
        {
            DataBase dataBase = new DataBase();
            var conn = dataBase.OpenConnection();
            MySqlCommand command = new MySqlCommand(sqlQuery, conn); ;
            MySqlParameter qu = command.Parameters.Add("@query", MySqlDbType.VarString);
            qu.Value = query;
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                reader.Read();
                excelWorksheet.Cells[row, collumn].Value = reader.GetValue(0);
            }
            conn = dataBase.CloseConnection();
        }
    }
}
