using System;
using System.Globalization;
using System.Linq;
using System.IO;


namespace BirthdayReminder
{
    public class Program
    {
        private static BirthdayManager manager = new BirthdayManager();
        private static string lastUsedFileName = "was";
        private static int selectedIndex = 0;
        private static string[] menuItems = {
            "Показать сегодняшние дни рождения",
            "Показать ближайшие дни рождения",
            "Показать все дни рождения",
            "Добавить день рождения",
            "Редактировать день рождения",
            "Удалить день рождения",
            "Сохранить в файл",
            "Загрузить из файла",
            "Выход"
        };

        private static bool IsValidName(string name)
        {
            return !string.IsNullOrWhiteSpace(name) &&
                   name.All(c => char.IsLetter(c) || c == ' ');
        }


        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            manager.SetFileName("was");
            manager.LoadFromFile();
            ShowMainMenu();
        }

        private static void ShowMainMenu()
        {
            ConsoleKey key;
            do
            {
                Console.Clear();
                Console.WriteLine("                      ┌───────────────────────────────────────────────┐");
                Console.WriteLine("                      │                  Поздравлятор                 │");
                Console.WriteLine("                      ├───────────────────────────────────────────────┤");

                for (int i = 0; i < menuItems.Length; i++)
                {
                    Console.Write("                      │");

                    if (i == selectedIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("  > ");
                        Console.Write($"{menuItems[i].PadRight(35)}< ");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write("    ");
                        Console.Write(menuItems[i].PadRight(37));
                    }

                    Console.WriteLine("      │");

                    if (i < menuItems.Length - 1)
                    {
                        Console.WriteLine("                      ├───────────────────────────────────────────────┤");
                    }
                }

                Console.WriteLine("                      └───────────────────────────────────────────────┘");
                Console.WriteLine("\nИспользуйте стрелки ↑ ↓ для выбора, Enter для подтверждения");

                key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = (selectedIndex > 0) ? selectedIndex - 1 : menuItems.Length - 1;
                        break;
                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex < menuItems.Length - 1) ? selectedIndex + 1 : 0;
                        break;
                    case ConsoleKey.Enter:
                        ExecuteSelectedOption();
                        break;
                }

            } while (key != ConsoleKey.Escape);
        }

        private static void ExecuteSelectedOption()
        {
            switch (selectedIndex)
            {
                case 0:
                    ShowTodayBirthdays();
                    break;
                case 1:
                    ShowUpcomingBirthdays();
                    break;
                case 2:
                    ShowAllBirthdays();
                    break;
                case 3:
                    AddBirthday();
                    break;
                case 4:
                    EditBirthday();
                    break;
                case 5:
                    RemoveBirthday();
                    break;
                case 6: 
                    SaveData(); 
                    break;
                case 7: 
                    LoadData(); 
                    break;
                case 8:
                    manager.SetFileName(lastUsedFileName);
                    manager.SaveToFile();

                    manager.SetFileName("was");
                    manager.SaveToFile();

                    Environment.Exit(0);
                    break;

            }

            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        private static void ShowTodayBirthdays()
        {
            var todayBirthdays = manager.GetTodayBirthdays();

            Console.WriteLine("\n┌────────────────────────────────────────────────────────┐");
            Console.WriteLine("│                 Сегодняшние дни рождения               │");
            Console.WriteLine("├─────┬──────────────────────┬────────────┬──────────────┤");
            Console.WriteLine("│ ID  │         Имя          │    Дата    │   Возраст    │");
            Console.WriteLine("├─────┼──────────────────────┼────────────┼──────────────┤");

            if (!todayBirthdays.Any())
            {
                Console.WriteLine("│                                                        │");
                Console.WriteLine("│               Сегодня нет дней рождения                │");
                Console.WriteLine("│                                                        │");
                Console.WriteLine("└────────────────────────────────────────────────────────┘");
                return;
            }
            else
            {
                foreach (var b in todayBirthdays)
                {
                    string id = b.Id.ToString().PadLeft(2).PadRight(4);
                    string name = b.Name.Length > 20 ? b.Name.Substring(0, 17) + "…" : b.Name.PadRight(20);
                    string date = b.Date.ToString("dd.MM.yyyy").PadRight(10);
                    string age = $"{b.GetAge()} лет".PadRight(12);

                    Console.WriteLine($"│ {id}│ {name} │ {date} │ {age} │");
                }
            }

            Console.WriteLine("└─────┴──────────────────────┴────────────┴──────────────┘");
        }


        private static void ShowUpcomingBirthdays()
        {
            var upcoming = manager.GetUpcomingBirthdays();

            Console.WriteLine("\n┌────────────────────────────────────────────────────────┐");
            Console.WriteLine("│  Ближайшие дни рождения (в течение 30 дней)            │");
            Console.WriteLine("├─────┬──────────────────────┬────────────┬──────────────┤");
            Console.WriteLine("│ ID  │         Имя          │    Дата    │   Возраст    │");
            Console.WriteLine("├─────┼──────────────────────┼────────────┼──────────────┤");

            if (!upcoming.Any())
            {
                Console.WriteLine("│                                                        │");
                Console.WriteLine("│        В ближайшие 30 дней нет дней рождения           │");
                Console.WriteLine("│                                                        │");
                Console.WriteLine("└────────────────────────────────────────────────────────┘");
                return;
            }

            foreach (var b in upcoming)
            {
                string id = b.Id.ToString().PadLeft(2).PadRight(4);
                string name = b.Name.Length > 20 ? b.Name.Substring(0, 17) + "…" : b.Name.PadRight(20);//удалю
                string date = b.Date.ToString("dd.MM.yyyy").PadRight(10);
                string age = $"{b.GetAge()} лет".PadRight(12);

                Console.WriteLine($"│ {id}│ {name} │ {date} │ {age} │");
            }

            Console.WriteLine("└─────┴──────────────────────┴────────────┴──────────────┘");
        }



        private static int ShowAllBirthdays()
        {
            var allBirthdays = manager.GetAllBirthdays();

            Console.WriteLine("\n┌────────────────────────────────────────────────────────┐");
            Console.WriteLine("│                   Все дни рождения                     │");
            Console.WriteLine("├─────┬──────────────────────┬────────────┬──────────────┤");
            Console.WriteLine("│ ID  │         Имя          │    Дата    │   Возраст    │");
            Console.WriteLine("├─────┼──────────────────────┼────────────┼──────────────┤");

            if (!allBirthdays.Any())
            {
                Console.WriteLine("│                                                        │");
                Console.WriteLine("│               Список дней рождения пуст                │");
                Console.WriteLine("│                                                        │");
                Console.WriteLine("└────────────────────────────────────────────────────────┘");
                return 0;
            }

            foreach (var b in allBirthdays)
            {
                string id = b.Id.ToString().PadLeft(2).PadRight(4);
                string name = b.Name.Length > 20 ? b.Name.Substring(0, 17) + "…" : b.Name.PadRight(20);//удалю
                string date = b.Date.ToString("dd.MM.yyyy").PadRight(10);
                string age = $"{b.GetAge()} лет".PadRight(12);

                Console.WriteLine($"│ {id}│ {name} │ {date} │ {age} │");
            }

            Console.WriteLine("└─────┴──────────────────────┴────────────┴──────────────┘");
            return 1;
        }



        private static void AddBirthday()
        {
            Console.WriteLine("\n=== Добавление дня рождения ===");
            Console.Write("ID: ");
            int id;
            while (!int.TryParse(Console.ReadLine(), out id) || id <= 0 || id >= 1000 || manager.GetAllBirthdays().Any(b => b.Id == id))
            {
                Console.WriteLine("ID должен быть уникальным, положительным и <1000  . Попробуйте другой:");
                Console.Write("ID: ");
            }


            Console.Write("Имя: ");
            var name = Console.ReadLine();

            while (!IsValidName(name) || name.Length > 20)
            {
                Console.WriteLine("Имя должно содержать только буквы и не превышать 20 символов. Попробуйте снова.");
                Console.Write("Имя: ");
                name = Console.ReadLine();
            }


            DateTime date;
            while (true)
            {
                Console.Write("Дата рождения (дд.мм.гггг): ");
                if (DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                {
                    int age = DateTime.Today.Year - date.Year;
                    if (date > DateTime.Today)
                    {
                        Console.WriteLine("Дата рождения не может быть в будущем.");
                        continue;
                    }
                    if (date > DateTime.Today.AddYears(-age)) age--;
                    if (age < 0)
                    {
                        Console.WriteLine("Возраст не может быть отрицательным.");
                        continue;
                    }
                    if (age > 99)
                    {
                        Console.WriteLine("Возраст не может быть больше 99 лет.");
                        continue;
                    }

                    break;
                }
                Console.WriteLine("Неверный формат даты. Попробуйте снова.");
            }

            manager.AddBirthday(id, name, date);
            Console.WriteLine("День рождения успешно добавлен!");
        }


        private static void EditBirthday()
        {
            var allBirthdays = manager.GetAllBirthdays();

            Console.WriteLine("\n┌────────────────────────────────────────────────────────┐");
            Console.WriteLine("│               Редактирование дня рождения              │");
            Console.WriteLine("├─────┬──────────────────────┬────────────┬──────────────┤");
            Console.WriteLine("│ ID  │         Имя          │    Дата    │   Возраст    │");
            Console.WriteLine("├─────┼──────────────────────┼────────────┼──────────────┤");

            if (!allBirthdays.Any())
            {
                Console.WriteLine("│                                                        │");
                Console.WriteLine("│               Список дней рождения пуст                │");
                Console.WriteLine("│                                                        │");
                Console.WriteLine("└────────────────────────────────────────────────────────┘");
                return;
            }

            foreach (var b in allBirthdays)
            {
                string idStr = b.Id.ToString().PadLeft(2).PadRight(4);
                string nameStr = b.Name.Length > 20 ? b.Name.Substring(0, 17) + "…" : b.Name.PadRight(20);//удалю
                string dateStr = b.Date.ToString("dd.MM.yyyy").PadRight(10);
                string age = $"{b.GetAge()} лет".PadRight(12);

                Console.WriteLine($"│ {idStr}│ {nameStr} │ {dateStr} │ {age} │");
            }

            Console.WriteLine("└─────┴──────────────────────┴────────────┴──────────────┘");

            Console.Write("Введите ID дня рождения для редактирования: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Неверный формат ID.");
                return;
            }

            if (!manager.GetAllBirthdays().Any(b => b.Id == id))
            {
                Console.WriteLine($"Запись с ID {id} не найдена.");
                return;
            }

            Console.Write("Новое имя: ");
            var name = Console.ReadLine();
            while (!IsValidName(name) || name.Length > 20)
            {
                Console.WriteLine("Имя должно содержать только буквы и быть не длиннее 20 символов. Попробуйте снова.");
                Console.Write("Новое имя: ");
                name = Console.ReadLine();
            }

            DateTime date;
            while (true)
            {
                Console.Write("Новая дата рождения (дд.мм.гггг): ");
                if (DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                {
                    int age = DateTime.Today.Year - date.Year;
                    if (date > DateTime.Today)
                    {
                        Console.WriteLine("Дата рождения не может быть в будущем.");
                        continue;
                    }
                    if (date > DateTime.Today.AddYears(-age)) age--;
                    if (age < 0 || age > 99)
                    {
                        Console.WriteLine("Возраст должен быть от 0 до 99 лет.");
                        continue;
                    }

                    break;
                }
                Console.WriteLine("Неверный формат даты. Попробуйте снова.");
            }

            if (manager.EditBirthday(id, name, date))
                Console.WriteLine("День рождения успешно отредактирован!");
        }



        private static void RemoveBirthday()
        {
            var allBirthdays = manager.GetAllBirthdays();

            Console.WriteLine("\n┌────────────────────────────────────────────────────────┐");
            Console.WriteLine("│                Удаление дня рождения                   │");
            Console.WriteLine("├─────┬──────────────────────┬────────────┬──────────────┤");
            Console.WriteLine("│ ID  │         Имя          │    Дата    │   Возраст    │");
            Console.WriteLine("├─────┼──────────────────────┼────────────┼──────────────┤");

            if (!allBirthdays.Any())
            {
                Console.WriteLine("│                                                        │");
                Console.WriteLine("│               Список дней рождения пуст                │");
                Console.WriteLine("│                                                        │");
                Console.WriteLine("└────────────────────────────────────────────────────────┘");
                return;
            }

            foreach (var b in allBirthdays)
            {
                string idStr = b.Id.ToString().PadLeft(2).PadRight(4);
                string name = b.Name.Length > 20 ? b.Name.Substring(0, 17) + "…" : b.Name.PadRight(20);//удалю
                string date = b.Date.ToString("dd.MM.yyyy").PadRight(10);
                string age = $"{b.GetAge()} лет".PadRight(12);

                Console.WriteLine($"│ {idStr}│ {name} │ {date} │ {age} │");
            }

            Console.WriteLine("└─────┴──────────────────────┴────────────┴──────────────┘");

            Console.Write("Введите ID дня рождения для удаления: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Неверный формат ID.");
                return;
            }

            if (manager.RemoveBirthday(id))
            {
                Console.WriteLine("День рождения успешно удален!");
            }
            else
            {
                Console.WriteLine("Запись с указанным ID не найдена.");
            }
        }
        private static void SaveData()
        {
            Console.Write("Введите имя файла для сохранения (без .txt): ");
            var name = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Имя файла не может быть пустым.");
                return;
            }

            manager.SetFileName(name);
            lastUsedFileName = name;
            manager.SaveToFile();
            Console.WriteLine("Данные успешно сохранены!");
        }



        private static void LoadData()
        {
            Console.Write("Введите имя файла для загрузки (без .txt): ");
            var name = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Имя файла не может быть пустым.");
                return;
            }

            manager.SetFileName(name);
            manager.LoadFromFile();
            Console.WriteLine("Данные успешно загружены!");
        }



    }
}