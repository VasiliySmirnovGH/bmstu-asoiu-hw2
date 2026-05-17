using System;
using System.IO;

namespace YourProjectNamespace // Обязательно замените на namespace вашего проекта (например, DevDepApp)
{
    class Program
    {
        static void Main(string[] args)
        {
            // Путь к файлу базы данных SQLite
            string dbPath = Path.Combine(AppContext.BaseDirectory, "developer_office.db");

            // Инициализация менеджера базы данных
            DatabaseManager dbManager = new DatabaseManager(dbPath);

            // Инициализация таблиц и первичных данных, если база пустая
            dbManager.InitializeDatabase();

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n=================== ГЛАВНОЕ МЕНЮ ===================");
                Console.ResetColor();
                Console.WriteLine("1. [Группа В] Просмотр всех разработчиков и отделов");
                Console.WriteLine("2. [Группа А] Добавить нового разработчика");
                Console.WriteLine("3. [Группа А] Удалить разработчика");
                Console.WriteLine("4. [Группа Г] Фильтр разработчиков по отделу");
                Console.WriteLine("5. [Группа Б] Экспорт данных в CSV");
                Console.WriteLine("6. [Группа Б] Импорт данных из CSV");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("7. ОТЧЁТЫ (Fluent Interface)");
                Console.ResetColor();
                Console.WriteLine("0. Выход из программы");
                Console.WriteLine("====================================================");
                Console.Write("Выберите действие: ");

                string mainChoice = Console.ReadLine();
                Console.WriteLine();

                switch (mainChoice)
                {
                    case "1":
                        ShowAllData(dbManager);
                        break;
                    case "2":
                        AddNewDeveloper(dbManager);
                        break;
                    case "3":
                        DeleteDeveloper(dbManager);
                        break;
                    case "4":
                        FilterByDepartment(dbManager);
                        break;
                    case "5":
                        ExportToCsv(dbManager);
                        break;
                    case "6":
                        ImportFromCsv(dbManager);
                        break;
                    case "7":
                        // Вызов меню отчётов
                        ShowReportsMenu(dbManager);
                        break;
                    case "0":
                        Console.WriteLine("Программа завершена. До свидания!");
                        return;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Ошибка: Неверный пункт меню. Попробуйте снова.");
                        Console.ResetColor();
                        break;
                }
            }
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // ПОДМЕНЮ ОБЯЗАТЕЛЬНЫХ ОТЧЁТОВ (Часть 6 методических указаний)
        // ═══════════════════════════════════════════════════════════════════════════
        static void ShowReportsMenu(DatabaseManager db)
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n====================== МЕНЮ ОТЧЁТОВ ======================");
                Console.ResetColor();
                Console.WriteLine("1. Отчёт 1: Список разработчиков с названиями отделов (JOIN)");
                Console.WriteLine("2. Отчёт 2: Количество разработчиков в каждом отделе (COUNT)");
                Console.WriteLine("3. Отчёт 3: Среднее количество коммитов по отделам (AVG)");
                Console.WriteLine("0. Вернуться в главное меню");
                Console.WriteLine("==========================================================");
                Console.Write("Выберите пункт отчёта: ");

                string reportChoice = Console.ReadLine();
                Console.WriteLine();

                switch (reportChoice)
                {
                    case "1":
                        // Отчёт 1: Список записей основной таблицы с названиями из справочника (JOIN, сортировка по имени)
                        new ReportBuilder(db)
                            .SetTitle("Список разработчиков и их отделов (Сортировка по имени)")
                            .SetHeaders("Имя разработчика", "Название отдела")
                            .SetQuery("SELECT d.dev_name, dep.dep_name FROM dev d JOIN dep ON d.dep_id = dep.dep_id ORDER BY d.dev_name ASC")
                            .BuildAndPrint();
                        break;

                    case "2":
                        // Отчёт 2: Количество записей в каждой категории (GROUP BY с COUNT(*))
                        new ReportBuilder(db)
                            .SetTitle("Количество разработчиков по отделам")
                            .SetHeaders("Название отдела", "Кол-во разработчиков")
                            .SetQuery("SELECT dep.dep_name, COUNT(*) AS cnt FROM dev d JOIN dep ON d.dep_id = dep.dep_id GROUP BY dep.dep_name")
                            .BuildAndPrint();
                        break;

                    case "3":
                        // Отчёт 3: Среднее значение числового поля по категориям (GROUP BY с AVG(...), с сортировкой)
                        new ReportBuilder(db)
                            .SetTitle("Среднее число коммитов по отделам (Сортировка по убыванию)")
                            .SetHeaders("Название отдела", "Среднее кол-во коммитов")
                            .SetQuery("SELECT dep.dep_name, AVG(d.commits_count) AS avg_commits FROM dev d JOIN dep ON d.dep_id = dep.dep_id GROUP BY dep.dep_name ORDER BY avg_commits DESC")
                            .BuildAndPrint();
                        break;

                    case "0":
                        return; // Возврат в главное меню

                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Ошибка: Некорректный выбор. Попробуйте еще раз.");
                        Console.ResetColor();
                        break;
                }
            }
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // ЗАГЛУШКИ ДЛЯ ОСТАЛЬНЫХ ФУНКЦИЙ КЛАССА PROGRAM (Свяжите со своим DatabaseManager)
        // ═══════════════════════════════════════════════════════════════════════════

        static void ShowAllData(DatabaseManager db)
        {
            Console.WriteLine("--- Список всех отделов ---");
            var departments = db.GetAllDepartments();
            foreach (var dep in departments)
            {
                Console.WriteLine($" ID: {dep.Id} | Отдел: {dep.Name}");
            }

            Console.WriteLine("\n--- Список всех разработчиков ---");
            var developers = db.GetAllDevelopers();
            foreach (var dev in developers)
            {
                Console.WriteLine($" ID: {dev.Id} | Имя: {dev.Name} | Коммиты: {dev.CommitsCount} | ID отдела: {dev.DepartmentId}");
            }
        }

        static void AddNewDeveloper(DatabaseManager db)
        {
            Console.WriteLine("--- Добавление нового разработчика ---");
            Console.Write("Введите имя: ");
            string name = Console.ReadLine();

            Console.Write("Введите количество коммитов: ");
            int.TryParse(Console.ReadLine(), out int commits);

            Console.Write("Введите ID отдела: ");
            int.TryParse(Console.ReadLine(), out int depId);

            db.AddDeveloper(name, commits, depId);
            Console.WriteLine("Разработчик успешно добавлен!");
        }

        static void DeleteDeveloper(DatabaseManager db)
        {
            Console.WriteLine("--- Удаление разработчика ---");
            Console.Write("Введите ID разработчика для удаления: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                db.DeleteDeveloper(id);
                Console.WriteLine("Запись удалена (если она существовала).");
            }
        }

        static void FilterByDepartment(DatabaseManager db)
        {
            Console.WriteLine("--- Фильтр разработчиков по отделу ---");
            Console.Write("Введите ID отдела: ");
            if (int.TryParse(Console.ReadLine(), out int depId))
            {
                var filtered = db.GetDevelopersByDepartment(depId);
                Console.WriteLine($"Найдено разработчиков: {filtered.Count}");
                foreach (var dev in filtered)
                {
                    Console.WriteLine($"  - {dev.Name} (Коммитов: {dev.CommitsCount})");
                }
            }
        }

        static void ExportToCsv(DatabaseManager db)
        {
            Console.WriteLine("--- Экспорт данных в CSV ---");
            db.ExportCsv();
            Console.WriteLine("Данные успешно экспортированы в файлы CSV.");
        }

        static void ImportFromCsv(DatabaseManager db)
        {
            Console.WriteLine("--- Импорт данных из CSV ---");
            db.ImportCsv();
            Console.WriteLine("Данные успешно импортированы из файлов CSV.");
        }
    }
}
