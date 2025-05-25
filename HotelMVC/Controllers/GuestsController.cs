using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelMVC.Models;
using Microsoft.AspNetCore.Authorization;

namespace HotelMVC.Controllers
{
    public class GuestsController : Controller
    {
        private readonly HotelContext _context;

        public GuestsController(HotelContext context)
        {
            _context = context;
        }

        // Список гостей доступен только администраторам
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Guests.ToListAsync());
        }

        // Создание гостя доступно всем авторизованным пользователям
        [Authorize]
        public IActionResult Create()
        {
            // Проверяем, есть ли уже профиль гостя у пользователя
            var currentUsername = User.Identity.Name;
            var existingGuest = _context.Guests.FirstOrDefault(g => g.Email == currentUsername + "@mail.ru");

            if (existingGuest != null)
            {
                TempData["Message"] = "У вас уже есть профиль гостя";
                return RedirectToAction("Index", "Reservations");
            }

            var guest = new Guest
            {
                Email = currentUsername + "@mail.ru",
                RegistrationDate = DateTime.Now
            };

            return View(guest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("FullName,Phone,PassportData,Email")] Guest guest)
        {
            if (ModelState.IsValid)
            {
                // Email уже установлен через скрытое поле в форме
                guest.RegistrationDate = DateTime.Now;
                
                _context.Add(guest);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Профиль гостя успешно создан!";
                
                if (User.IsInRole("Admin"))
                    return RedirectToAction(nameof(Index));
                else
                    return RedirectToAction("Index", "Reservations");
            }

            // Если дошли до сюда, что-то пошло не так
            ModelState.AddModelError("", "Пожалуйста, проверьте введенные данные");
            return View(guest);
        }

        // Остальные действия доступны только администраторам
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var guest = await _context.Guests.FindAsync(id);
            if (guest == null)
            {
                return NotFound();
            }
            return View(guest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Email,Phone,PassportData,RegistrationDate")] Guest guest)
        {
            if (id != guest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(guest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GuestExists(guest.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(guest);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var guest = await _context.Guests
                .FirstOrDefaultAsync(m => m.Id == id);
            if (guest == null)
            {
                return NotFound();
            }

            return View(guest);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var guest = await _context.Guests.FindAsync(id);
            if (guest != null)
            {
                _context.Guests.Remove(guest);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool GuestExists(int id)
        {
            return _context.Guests.Any(e => e.Id == id);
        }
    }
}