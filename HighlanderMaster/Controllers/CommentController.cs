using HighlanderMaster.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HighlanderMaster.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CommentsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int tripRouteId, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return RedirectToAction("Details", "TripRoutes", new { id = tripRouteId });
            }

            var user = await _userManager.GetUserAsync(User);

            var comment = new Comment
            {
                UserId = user.Id,
                UserName = user.UserName,
                TripRouteId = tripRouteId,
                Text = text,
                CreatedAt = DateTime.Now,
                Deleted = false
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "TripRoutes", new { id = tripRouteId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int tripRouteId)
        {
            var userId = _userManager.GetUserId(User);
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null) return NotFound();

            if (User.IsInRole("Admin") || comment.UserId == userId)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", "TripRoutes", new { id = tripRouteId });
        }
    }
}