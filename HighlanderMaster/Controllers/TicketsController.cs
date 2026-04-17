using HighlanderMaster.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HighlanderMaster.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TicketsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Моите билети
        public async Task<IActionResult> MyTickets()
        {
            var userId = _userManager.GetUserId(User);
            var tickets = await _context.Tickets
                .Include(t => t.TripRoute)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.PurchaseDate)
                .ToListAsync();
            return View(tickets);
        }

        // POST: Купи билет
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Buy(int tripRouteId)
        {
            var userId = _userManager.GetUserId(User);

            // Провери дали вече има билет за този маршрут
            var existing = await _context.Tickets
                .FirstOrDefaultAsync(t => t.UserId == userId && t.TripRouteId == tripRouteId);

            if (existing != null)
            {
                TempData["Error"] = "Вече имаш билет за този маршрут!";
                return RedirectToAction("Details", "TripRoutes", new { id = tripRouteId });
            }

            var route = await _context.Routes.FindAsync(tripRouteId);
            if (route == null) return NotFound();

            // Провери дали има свободни места
            if (route.CurrentParticipants >= route.MaxCountPeople)
            {
                TempData["Error"] = "Няма свободни места за този маршрут!";
                return RedirectToAction("Details", "TripRoutes", new { id = tripRouteId });
            }

            var ticket = new Ticket
            {
                UserId = userId,
                TripRouteId = tripRouteId,
                PurchaseDate = DateTime.Now,
                Price = route.Price,
                IsPaid = true
            };

            _context.Tickets.Add(ticket);

            // Намали свободните места
            route.CurrentParticipants++;
            _context.Routes.Update(route);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Билетът е закупен успешно!";
            return RedirectToAction("MyTickets");
        }

        // POST: Откажи билет
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = _userManager.GetUserId(User);
            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (ticket == null) return NotFound();

            // Върни мястото
            var route = await _context.Routes.FindAsync(ticket.TripRouteId);
            if (route != null && route.CurrentParticipants > 0)
            {
                route.CurrentParticipants--;
                _context.Routes.Update(route);
            }

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Билетът е отказан успешно!";
            return RedirectToAction("MyTickets");
        }
    }
}
