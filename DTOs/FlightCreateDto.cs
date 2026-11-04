using System.ComponentModel.DataAnnotations;

namespace TouristAgencyApp.DTOs
{
    public class FlightCreateDto
    {
        [Required(ErrorMessage = "Номер рейса обязателен.")]
        [MaxLength(50)]
        public string flight_number { get; set; } = string.Empty;

        [Required]
        public DateTime flight_date { get; set; }

        [Required(ErrorMessage = "Тип самолета обязателен.")]
        [MaxLength(100)]
        public string aircraft_type { get; set; } = string.Empty;
    }
}