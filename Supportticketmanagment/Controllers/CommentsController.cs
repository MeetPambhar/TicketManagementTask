using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Supportticketmanagment.Models;
using Supportticketmanagment.DTOs;
using System.Security.Claims;

namespace Supportticketmanagment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly SupportTicketContext _db;

        public CommentsController(SupportTicketContext context)
        {
            _db = context;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        private string GetUserRole() => User.FindFirstValue(ClaimTypes.Role)!;

        [HttpPost("ticket/{ticketId}")]
        public async Task<IActionResult> Add(int ticketId, CreateCommentDto dto)
        {
            var ticket = await _db.Tickets.FindAsync(ticketId);
            if (ticket == null) return NotFound();

            var userId = GetUserId();
            var role = GetUserRole();

            bool canComment = role == "MANAGER" ||
                              (role == "SUPPORT" && ticket.AssignedTo == userId) ||
                              (role == "USER" && ticket.CreatedBy == userId);

            if (!canComment) return Forbid();

            var comment = new TicketComment
            {
                TicketId = ticketId,
                UserId = userId,
                Comment = dto.Comment
            };

            _db.TicketComments.Add(comment);
            await _db.SaveChangesAsync();

            return Ok(new CommentResponseDto
            {
                _id = comment.Id,
                ticketId = ticketId,
                userId = userId,
                userName = User.FindFirstValue(ClaimTypes.Name) ?? "User",
                comment = comment.Comment,
                createdAt = comment.CreatedAt
            });
        }

        [HttpGet("ticket/{ticketId}")]
        public async Task<IActionResult> List(int ticketId)
        {
            var ticket = await _db.Tickets.FindAsync(ticketId);
            if (ticket == null) return NotFound();

            var userId = GetUserId();
            var role = GetUserRole();

            bool canView = role == "MANAGER" ||
                           (role == "SUPPORT" && ticket.AssignedTo == userId) ||
                           (role == "USER" && ticket.CreatedBy == userId);

            if (!canView) return Forbid();

            var items = await _db.TicketComments
                .Where(c => c.TicketId == ticketId)
                .Include(c => c.User)
                .Select(c => new CommentResponseDto
                {
                    _id = c.Id,
                    ticketId = c.TicketId ?? ticketId,
                    userId = c.UserId ?? 0,
                    userName = c.User != null ? c.User.Name : "User",
                    comment = c.Comment,
                    createdAt = c.CreatedAt
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(int id, CreateCommentDto dto)
        {
            var item = await _db.TicketComments.FindAsync(id);
            if (item == null) return NotFound();

            if (GetUserRole() != "MANAGER" && item.UserId != GetUserId())
            {
                return Forbid();
            }

            item.Comment = dto.Comment;
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.TicketComments.FindAsync(id);
            if (item == null) return NotFound();

            if (GetUserRole() != "MANAGER" && item.UserId != GetUserId())
            {
                return Forbid();
            }

            _db.TicketComments.Remove(item);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
