using System.ComponentModel.DataAnnotations;

namespace HotelMVC.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        [Display(Name = "Гость")]
        public int GuestId { get; set; }
        public Guest Guest { get; set; }

        [Display(Name = "Номер")]
        public int RoomId { get; set; }
        public Room Room { get; set; }

        [Display(Name = "Дата заезда")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Поле 'Дата заезда' обязательно для заполнения")]
        public DateTime CheckInDate { get; set; }

        [Display(Name = "Дата выезда")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Поле 'Дата выезда' обязательно для заполнения")]
        public DateTime CheckOutDate { get; set; }

        [Display(Name = "Статус")]
        public string Status { get; set; } = "Подтверждено";

        [Display(Name = "Сумма оплаты")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Дата создания")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}