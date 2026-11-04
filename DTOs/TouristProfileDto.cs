using TouristAgencyApp.Models;

namespace TouristAgencyApp.DTOs
{
    public class TouristProfileDto
    {
        // Основные данные туриста
        public int tourist_id { get; set; }
        public string tourist_name { get; set; } = string.Empty;
        public string tourist_surname { get; set; } = string.Empty;
        public string tourist_patronymic { get; set; } = string.Empty;
        public DateTime tourist_date_of_birth { get; set; }
        public string tourist_gender { get; set; } = string.Empty;

        // НОВЫЕ ПОЛЯ ИЗ ЗАДАНИЯ
        public string category { get; set; } = string.Empty;
        public bool has_visa { get; set; }

        public int flight_id { get; set; }

        // Связанные данные из других таблиц
        public Passport? Passport { get; set; }
        public List<TouristChild> Children { get; set; } = new List<TouristChild>();
        public List<TouristCargo> Cargo { get; set; } = new List<TouristCargo>();
    }
}