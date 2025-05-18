using System.ComponentModel.DataAnnotations;

namespace HotelMVC.Models
{
    public class Room
    {
        public int Id { get; set; }

        [Display(Name = "Номер")]
        [Required(ErrorMessage = "Поле 'Номер' обязательно для заполнения")]
        public string Number { get; set; }

        [Display(Name = "Тип")]
        [Required(ErrorMessage = "Поле 'Тип' обязательно для заполнения")]
        public string Type { get; set; }

        [Display(Name = "Вместимость")]
        [Range(1, 10, ErrorMessage = "Вместимость должна быть от 1 до 10")]
        public int Capacity { get; set; }

        [Display(Name = "Цена за ночь")]
        [Range(0, 100000, ErrorMessage = "Цена должна быть положительной")]
        public decimal PricePerNight { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Доступен")]
        public bool IsAvailable { get; set; } = true;
    }
}