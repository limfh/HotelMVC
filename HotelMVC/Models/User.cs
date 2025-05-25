using System.ComponentModel.DataAnnotations;

namespace HotelMVC.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "������� ��� ������������")]
        [Display(Name = "��� ������������")]
        public string Username { get; set; }

        [Required(ErrorMessage = "������� ������")]
        [Display(Name = "������")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "������ ������ ��������� ������� 6 ��������")]
        public string Password { get; set; }

        [Required(ErrorMessage = "������� ����")]
        [Display(Name = "����")]
        public string Role { get; set; } = "User";
    }
}