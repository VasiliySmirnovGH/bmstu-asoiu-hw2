using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace YourProjectNamespace // Замените YourProjectNamespace на пространство имён вашего проекта
{
    public class ReportBuilder
    {
        private readonly DatabaseManager _dbManager;
        private string _title = "Отчёт";
        private string _sqlQuery = string.Empty;
        private List<string> _headers = new List<string>();

        public ReportBuilder(DatabaseManager dbManager)
        {
            _dbManager = dbManager;
        }

        // Установка заголовка (возвращает this для цепочки вызовов)
        public ReportBuilder SetTitle(string title)
        {
            _title = title;
            return this;
        }

        // Установка SQL-запроса (возвращает this)
        public ReportBuilder SetQuery(string sqlQuery)
        {
            _sqlQuery = sqlQuery;
            return this;
        }

        // Настройка заголовков колонок таблицы (возвращает this)
        public ReportBuilder SetHeaders(params string[] headers)
        {
            _headers = new List<string>(headers);
            return this;
        }

        // Терминальный метод: выполняет запрос и выводит данные в консоль
        public void BuildAndPrint()
        {
            if (string.IsNullOrEmpty(_sqlQuery))
            {
                Console.WriteLine("Ошибка: SQL-запрос не задан.");
                return;
            }

            // Красивая рамка заголовка
            Console.WriteLine(new string('=', 60));
            Console.WriteLine($" │ {_title.ToUpper()}");
            Console.WriteLine(new string('=', 60));

            // Вывод шапки таблицы
            if (_headers.Count > 0)
            {
                Console.WriteLine(" │ " + string.Join("\t│ ", _headers));
                Console.WriteLine(new string('-', 60));
            }

            try
            {
                // Получаем подключение. Если в вашем DatabaseManager метод называется иначе
                // (например, CreateConnection), замените GetConnection() на ваше название.
                using (var connection = _dbManager.GetConnection())
                {
                    connection.Open();
                    using (var command = new SqliteCommand(_sqlQuery, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            int rowCount = 0;
                            while (reader.Read())
                            {
                                var rowValues = new List<string>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    // Обработка возможных NULL значений в базе данных
                                    var val = reader.GetValue(i);
                                    rowValues.Add(val != DBNull.Value ? val.ToString() : "NULL");
                                }
                                Console.WriteLine(" │ " + string.Join("\t│ ", rowValues));
                                rowCount++;
                            }

                            Console.WriteLine(new string('-', 60));
                            Console.WriteLine($" │ Итого строк: {rowCount}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($" [ОШИБКА БД]: {ex.Message}");
            }

            Console.WriteLine(new string('=', 60));
            Console.WriteLine();
        }
    }
}
