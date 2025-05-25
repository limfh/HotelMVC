using Microsoft.AspNetCore.Mvc;
using HotelMVC.Models;

namespace HotelMVC.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var viewModel = new ErrorViewModel
            {
                StatusCode = statusCode,
                Title = GetErrorTitle(statusCode),
                Message = GetErrorMessage(statusCode)
            };

            return View("Error", viewModel);
        }

        private string GetErrorTitle(int statusCode)
        {
            return statusCode switch
            {
                404 => "�������� �� �������",
                403 => "������ ��������",
                401 => "��������� �����������",
                _ => "��������� ������"
            };
        }

        private string GetErrorMessage(int statusCode)
        {
            return statusCode switch
            {
                404 => "������������� �������� �� ����������.",
                403 => "� ��� ��� ���� ��� ������� � ���� ��������.",
                401 => "��� ������� � ���� �������� ���������� ����� � �������.",
                _ => "��� ��������� ������ ������� ��������� ������."
            };
        }
    }
}