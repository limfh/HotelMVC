using System.ComponentModel.DataAnnotations;

namespace HotelMVC.Models
{
    public class Guest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле 'ФИО' обязательно для заполнения")]
        [Display(Name = "ФИО")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Поле 'Email' обязательно для заполнения")]
        [EmailAddress(ErrorMessage = "Некорректный email адрес")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле 'Телефон' обязательно для заполнения")]
        [Phone(ErrorMessage = "Некорректный номер телефона")]
        [Display(Name = "Телефон")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Поле 'Паспортные данные' обязательно для заполнения")]
        [Display(Name = "Паспортные данные")]
        public string PassportData { get; set; }

        [Display(Name = "Дата регистрации")]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        // Навигационное свойство для связи с бронированиями
        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}