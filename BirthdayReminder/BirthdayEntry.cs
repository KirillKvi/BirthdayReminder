using System;

namespace BirthdayReminder
{
    public class BirthdayEntry
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }

        public BirthdayEntry(int id, string name, DateTime date)
        {
            Id = id;
            Name = name;
            Date = date;
        }

        public int GetAge()
        {
            var today = DateTime.Today;
            var age = today.Year - Date.Year;
            if (Date.Date > today.AddYears(-age)) age--;
            return age;
        }

        public DateTime GetThisYearBirthday()
        {
            return new DateTime(DateTime.Today.Year, Date.Month, Date.Day);
        }

        public bool IsToday()
        {
            var today = DateTime.Today;
            return Date.Month == today.Month && Date.Day == today.Day;
        }

        public bool IsPastThisYear()
        {
            var thisYearBirthday = GetThisYearBirthday();
            return thisYearBirthday < DateTime.Today;
        }

        public int DaysUntilNextBirthday()
        {
            var thisYearBirthday = GetThisYearBirthday();
            var nextBirthday = thisYearBirthday;

            if (thisYearBirthday < DateTime.Today)
            {
                nextBirthday = thisYearBirthday.AddYears(1);
            }

            return (nextBirthday - DateTime.Today).Days;
        }

        public override string ToString()
        {
            return $"ID: {Id} | {Name} ({Date:dd.MM.yyyy}) - {GetAge()} лет.";
        }

    }
}