using System.ComponentModel.DataAnnotations;

namespace HotelMVC.Models
{
    public class Guest
    {
        public int Id { get; set; }

        [Display(Name = "ФИО")]
        [Required(ErrorMessage = "Поле 'ФИО' обязательно для заполнения")]
        public string FullName { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Некорректный email адрес")]
        public string Email { get; set; }

        [Display(Name = "Телефон")]
        [Phone(ErrorMessage = "Некорректный номер телефона")]
        public string Phone { get; set; }

        [Display(Name = "Паспортные данные")]
        [Required(ErrorMessage = "Поле 'Паспортные данные' обязательно для заполнения")]
        public string PassportData { get; set; }

        [Display(Name = "Дата регистрации")]
        [DataType(DataType.Date)]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
    }
}