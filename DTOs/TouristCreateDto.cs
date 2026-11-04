using System.ComponentModel.DataAnnotations;

namespace TouristAgencyApp.DTOs
{
    public class TouristCreateDto
    {
        [Required(ErrorMessage = "Имя обязательно.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Имя должно содержать от 2 до 50 символов.")]
        public string tourist_name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Фамилия обязательна.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Фамилия должна содержать от 2 до 50 символов.")]
        public string tourist_surname { get; set; } = string.Empty;

        // Отчество может быть необязательным, поэтому не ставим [Required]
        public string tourist_patronymic { get; set; } = string.Empty;

        [Required(ErrorMessage = "Дата рождения обязательна.")]
        public DateTime tourist_date_of_birth { get; set; }

        [Required(ErrorMessage = "Пол обязателен.")]
        [RegularExpression("Мужской|Женский", ErrorMessage = "Пол должен быть указан как 'Мужской' или 'Женский'.")]
        public string tourist_gender { get; set; } = string.Empty;

        // НОВОЕ ПОЛЕ: Категория туриста
        [Required(ErrorMessage = "Категория туриста обязательна.")]
        [RegularExpression("Отдыхающий|Шоп-тур", ErrorMessage = "Категория должна быть 'Отдыхающий' или 'Шоп-тур'.")]
        public string category { get; set; } = string.Empty;

        // НОВОЕ ПОЛЕ: Наличие визы
        [Required]
        public bool has_visa { get; set; }

        [Required(ErrorMessage = "ID рейса обязателен.")]
        public int flight_id { get; set; }
    }
}