using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelMVC.Models;
using Microsoft.AspNetCore.Authorization;

namespace HotelMVC.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        private readonly HotelContext _context;

        public ReservationsController(HotelContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string status, DateTime? dateFrom, DateTime? dateTo)
        {
            var reservations = _context.Reservations
                .Include(r => r.Guest)
                .Include(r => r.Room)
                .AsQueryable();

            // Для обычных пользователей показываем только их бронирования
            if (!User.IsInRole("Admin"))
            {
                var currentUsername = User.Identity.Name;
                var currentGuest = await _context.Guests
                    .FirstOrDefaultAsync(g => g.Email == currentUsername);

                if (currentGuest != null)
                {
                    reservations = reservations.Where(r => r.GuestId == currentGuest.Id);
                }
            }

            if (!string.IsNullOrEmpty(status))
            {
                reservations = reservations.Where(r => r.Status == status);
            }

            if (dateFrom.HasValue)
            {
                reservations = reservations.Where(r => r.CheckInDate >= dateFrom.Value);
            }

            if (dateTo.HasValue)
            {
                reservations = reservations.Where(r => r.CheckOutDate <= dateTo.Value);
            }

            ViewBag.StatusList = new SelectList(new[]
            {
                "Ожидание",
                "Подтверждено",
                "Отменено",
                "Завершено"
            });

            return View(await reservations.ToListAsync());
        }

        // GET: Reservations/Create
        [Authorize]
        public IActionResult Create(int? roomId = null)
        {
            var reservation = new Reservation
            {
                CheckInDate = DateTime.Today,
                CheckOutDate = DateTime.Today.AddDays(1)
            };

            if (roomId.HasValue)
            {
                var room = _context.Rooms.Find(roomId.Value);
                if (room != null && room.IsAvailable)
                {
                    reservation.RoomId = room.Id;
                }
            }

            PrepareViewBagData(reservation);
            return View(reservation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([FromForm] Reservation reservation)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Если не админ, устанавливаем текущего пользователя как гостя
                    if (!User.IsInRole("Admin"))
                    {
                        var currentUser = User.Identity.Name;
                        var guest = await _context.Guests.FirstOrDefaultAsync(g => g.Email == currentUser);

                        if (guest == null)
                        {
                            ModelState.AddModelError("", "Необходимо создать профиль гостя");
                            PrepareViewBagData(reservation);
                            return View(reservation);
                        }

                        reservation.GuestId = guest.Id;
                    }

                    // Проверяем даты
                    if (reservation.CheckInDate < DateTime.Today)
                    {
                        ModelState.AddModelError("CheckInDate", "Дата заезда не может быть в прошлом");
                        PrepareViewBagData(reservation);
                        return View(reservation);
                    }

                    if (reservation.CheckOutDate <= reservation.CheckInDate)
                    {
                        ModelState.AddModelError("CheckOutDate", "Дата выезда должна быть позже даты заезда");
                        PrepareViewBagData(reservation);
                        return View(reservation);
                    }

                    // Проверяем доступность номера
                    var room = await _context.Rooms.FindAsync(reservation.RoomId);
                    if (room == null || !room.IsAvailable)
                    {
                        ModelState.AddModelError("RoomId", "Выбранный номер недоступен");
                        PrepareViewBagData(reservation);
                        return View(reservation);
                    }

                    // Устанавливаем остальные поля
                    reservation.Status = "Ожидание";
                    reservation.CreatedAt = DateTime.Now;
                    reservation.TotalAmount = room.PricePerNight * (decimal)(reservation.CheckOutDate - reservation.CheckInDate).TotalDays;

                    _context.Add(reservation);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Бронирование успешно создано";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Произошла ошибка при создании бронирования: " + ex.Message);
            }

            PrepareViewBagData(reservation);
            return View(reservation);
        }

        private void PrepareViewBagData(Reservation reservation)
        {
            if (User.IsInRole("Admin"))
            {
                ViewBag.GuestId = new SelectList(_context.Guests, "Id", "FullName", reservation.GuestId);
            }
            else
            {
                var currentUser = User.Identity.Name + "@mail.ru";
                var guest = _context.Guests.FirstOrDefault(g => g.Email == currentUser);
                if (guest != null)
                {
                    reservation.GuestId = guest.Id;
                }
            }
            ViewBag.RoomId = new SelectList(_context.Rooms.Where(r => r.IsAvailable), "Id", "Number", reservation.RoomId);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GuestId,RoomId,CheckInDate,CheckOutDate,Status,TotalAmount")] Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Обновляем сумму при изменении дат
                    var room = await _context.Rooms.FindAsync(reservation.RoomId);
                    var days = (reservation.CheckOutDate - reservation.CheckInDate).Days;
                    reservation.TotalAmount = room.PricePerNight * days;

                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.Id))
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

            ViewBag.GuestId = new SelectList(_context.Guests, "Id", "FullName", reservation.GuestId);
            ViewBag.RoomId = new SelectList(_context.Rooms, "Id", "Number", reservation.RoomId);
            return View(reservation);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Guest)
                .Include(r => r.Room)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            // Проверяем права доступа
            if (!User.IsInRole("Admin"))
            {
                var currentUsername = User.Identity.Name;
                var currentGuest = await _context.Guests
                    .FirstOrDefaultAsync(g => g.Email == currentUsername);

                if (currentGuest == null || reservation.GuestId != currentGuest.Id)
                {
                    return Forbid();
                }
            }

            return View(reservation);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Guest)
                .Include(r => r.Room)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Confirm(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            reservation.Status = "Подтверждено";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Метод для пользователей для отмены их бронирований
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            // Проверяем права доступа
            if (!User.IsInRole("Admin"))
            {
                var currentUsername = User.Identity.Name;
                var currentGuest = await _context.Guests
                    .FirstOrDefaultAsync(g => g.Email == currentUsername);

                if (currentGuest == null || reservation.GuestId != currentGuest.Id)
                {
                    return Forbid();
                }
            }

            reservation.Status = "Отменено";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
    }
}