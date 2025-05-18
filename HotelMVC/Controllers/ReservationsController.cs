using HotelMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HotelMVC.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly HotelContext _context;

        public ReservationsController(HotelContext context)
        {
            _context = context;
        }

        // GET: Reservations
        public async Task<IActionResult> Index(string status, DateTime? dateFrom, DateTime? dateTo)
        {
            var reservations = _context.Reservations
                .Include(r => r.Guest)
                .Include(r => r.Room)
                .AsQueryable();

            // Фильтрация по статусу
            if (!string.IsNullOrEmpty(status))
            {
                reservations = reservations.Where(r => r.Status == status);
            }

            // Фильтрация по дате заезда
            if (dateFrom.HasValue)
            {
                reservations = reservations.Where(r => r.CheckInDate >= dateFrom.Value);
            }

            // Фильтрация по дате выезда
            if (dateTo.HasValue)
            {
                reservations = reservations.Where(r => r.CheckOutDate <= dateTo.Value);
            }

            var statusList = new List<string>
            {
                "Подтверждено",
                "Отменено",
                "Завершено",
                "Ожидание"
            };

            ViewBag.StatusList = new SelectList(statusList.Distinct());

            return View(await reservations.ToListAsync());
        }

        // GET: Reservations/Details/5
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

            return View(reservation);
        }

        // GET: Reservations/Create
        public IActionResult Create()
        {
            ViewData["GuestId"] = new SelectList(_context.Guests, "Id", "FullName");
            ViewData["RoomId"] = new SelectList(_context.Rooms.Where(r => r.IsAvailable), "Id", "Number");
            return View();
        }

        // POST: Reservations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,GuestId,RoomId,CheckInDate,CheckOutDate,Status,TotalAmount")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                reservation.CreatedAt = DateTime.Now;
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GuestId"] = new SelectList(_context.Guests, "Id", "FullName", reservation.GuestId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "Number", reservation.RoomId);
            return View(reservation);
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            ViewData["GuestId"] = new SelectList(_context.Guests, "Id", "FullName", reservation.GuestId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "Number", reservation.RoomId);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GuestId,RoomId,CheckInDate,CheckOutDate,Status,TotalAmount,CreatedAt")] Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
            ViewData["GuestId"] = new SelectList(_context.Guests, "Id", "FullName", reservation.GuestId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "Number", reservation.RoomId);
            return View(reservation);
        }

        // GET: Reservations/Delete/5
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

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
    }
}