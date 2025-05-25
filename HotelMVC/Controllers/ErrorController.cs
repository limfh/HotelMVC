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
                404 => "Страница не найдена",
                403 => "Доступ запрещен",
                401 => "Требуется авторизация",
                _ => "Произошла ошибка"
            };
        }

        private string GetErrorMessage(int statusCode)
        {
            return statusCode switch
            {
                404 => "Запрашиваемая страница не существует.",
                403 => "У вас нет прав для доступа к этой странице.",
                401 => "Для доступа к этой странице необходимо войти в систему.",
                _ => "При обработке вашего запроса произошла ошибка."
            };
        }
    }
}