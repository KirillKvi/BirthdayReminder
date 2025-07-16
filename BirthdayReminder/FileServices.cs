using System;
using System.IO;
using Newtonsoft.Json;

namespace BirthdayReminder
{
    public static class FileServices
    {
        public static void SaveToFile(string fileName, object data)
        {
            try
            {
                // Используем Newtonsoft.Json вместо System.Text.Json
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(fileName, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении данных: {ex.Message}");
            }
        }

        public static T LoadFromFile<T>(string fileName) where T : new()
        {
            if (File.Exists(fileName))
            {
                try
                {
                    // Используем Newtonsoft.Json вместо System.Text.Json
                    string json = File.ReadAllText(fileName);
                    return JsonConvert.DeserializeObject<T>(json);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при загрузке данных: {ex.Message}");
                }
            }
            return new T();
        }
    }
}