using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace BirthdayReminder
{
    public class BirthdayManager
    {
        private List<BirthdayEntry> birthdays;
        private string FileName;
        //private const string FileName = "birthdays.json";

        public BirthdayManager()
        {
            birthdays = new List<BirthdayEntry>();
            //LoadFromFile();
        }

        public void AddBirthday(int id, string name, DateTime date)
        {
            var birthday = new BirthdayEntry(id, name, date);
            birthdays.Add(birthday);
            SaveToFile();
        }


        public bool RemoveBirthday(int id)
        {
            var birthday = birthdays.FirstOrDefault(b => b.Id == id);
            if (birthday != null)
            {
                birthdays.Remove(birthday);
                SaveToFile();
                return true;
            }
            return false;
        }

        public bool EditBirthday(int id, string name, DateTime date)
        {
            var birthday = birthdays.FirstOrDefault(b => b.Id == id);
            if (birthday != null)
            {
                birthday.Name = name;
                birthday.Date = date;
                SaveToFile();
                return true;
            }
            return false;
        }

        public List<BirthdayEntry> GetAllBirthdays()
        {
            return birthdays.OrderBy(b => b.GetThisYearBirthday()).ToList();
        }

        public List<BirthdayEntry> GetTodayBirthdays()
        {
            return birthdays.Where(b => b.IsToday()).ToList();
        }

        public List<BirthdayEntry> GetUpcomingBirthdays(int days = 30)
        {
            return birthdays
                .Where(b => b.DaysUntilNextBirthday() <= days)
                .OrderBy(b => b.DaysUntilNextBirthday())
                .ToList();
        }

        public void SaveToFile()
        {
            try
            {
                var json = JsonConvert.SerializeObject(new { Birthdays = birthdays }, Formatting.Indented);
                File.WriteAllText(FileName, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении данных: {ex.Message}");
            }
        }

        public void LoadFromFile()
        {
            if (File.Exists(FileName))
            {
                try
                {
                    var json = File.ReadAllText(FileName);
                    var data = JsonConvert.DeserializeObject<dynamic>(json);

                    birthdays = JsonConvert.DeserializeObject<List<BirthdayEntry>>(data.Birthdays.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при загрузке данных: {ex.Message}");
                }
            }
        }

        public void SetFileName(string nameWithoutExtension)
        {
            var safeName = string.Join("_", nameWithoutExtension.Split(Path.GetInvalidFileNameChars()));
            FileName = Path.Combine(Environment.CurrentDirectory, safeName + ".txt");

        }


    }
}
