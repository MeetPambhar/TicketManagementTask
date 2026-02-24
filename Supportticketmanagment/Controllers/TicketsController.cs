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
    public class TicketsController : ControllerBase
    {
        private readonly SupportTicketContext _db;

        public TicketsController(SupportTicketContext context)
        {
            _db = context;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        private string GetUserRole() => User.FindFirstValue(ClaimTypes.Role)!;

        [HttpPost]
        [Authorize(Roles = "USER,MANAGER")]
        public async Task<IActionResult> Create(CreateTicketDto dto)
        {
            var user = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == GetUserId());
            if (user == null) return Unauthorized();

            var ticket = new Ticket
            {
                Title = dto.Title,
                Description = dto.Description,
                Priority = dto.Priority.ToUpper(),
                Status = "OPEN",
                CreatedBy = user.Id
            };

            _db.Tickets.Add(ticket);
            await _db.SaveChangesAsync();

            return Ok(new TicketResponseDto
            {
                _id = ticket.Id,
                title = ticket.Title,
                description = ticket.Description,
                status = ticket.Status!,
                priority = ticket.Priority!,
                createdBy = new UserMiniDto { name = user.Name, role = user.Role.Name },
                createdAt = ticket.CreatedAt
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetUserId();
            var role = GetUserRole();

            var query = _db.Tickets
                .Include(t => t.CreatedByNavigation).ThenInclude(u => u.Role)
                .Include(t => t.AssignedToNavigation).ThenInclude(u => u.Role)
                .Include(t => t.TicketComments).ThenInclude(c => c.User).ThenInclude(u => u.Role)
                .Include(t => t.TicketStatusLogs).ThenInclude(l => l.ChangedByNavigation).ThenInclude(u => u.Role)
                .AsQueryable();

            if (role == "SUPPORT")
            {
                query = query.Where(t => t.AssignedTo == userId);
            }
            else if (role == "USER")
            {
                query = query.Where(t => t.CreatedBy == userId);
            }

            var tickets = await query.Select(t => new TicketResponseDto
            {
                _id = t.Id,
                title = t.Title,
                description = t.Description,
                status = t.Status ?? "OPEN",
                priority = t.Priority ?? "MEDIUM",
                createdBy = new UserMiniDto
                {
                    name = t.CreatedByNavigation.Name,
                    role = t.CreatedByNavigation.Role.Name
                },
                assignedTo = t.AssignedToNavigation != null ? new UserMiniDto
                {
                    name = t.AssignedToNavigation.Name,
                    role = t.AssignedToNavigation.Role.Name
                } : null,
                comments = t.TicketComments.Select(c => new CommentMiniDto
                {
                    authorName = c.User != null ? c.User.Name : "User",
                    authorRole = c.User != null ? c.User.Role.Name : "USER",
                    comment = c.Comment,
                    createdAt = c.CreatedAt
                }).ToList(),
                statusLogs = t.TicketStatusLogs.Select(l => new StatusLogMiniDto
                {
                    oldStatus = l.OldStatus,
                    newStatus = l.NewStatus,
                    changedBy = new UserMiniDto
                    {
                        name = l.ChangedByNavigation != null ? l.ChangedByNavigation.Name : "System",
                        role = l.ChangedByNavigation != null ? l.ChangedByNavigation.Role.Name : "MANAGER"
                    },
                    changedAt = l.ChangedAt
                }).ToList(),
                createdAt = t.CreatedAt
            }).ToListAsync();

            return Ok(tickets);
        }

        [HttpPatch("{id}/assign")]
        [Authorize(Roles = "MANAGER,SUPPORT")]
        public async Task<IActionResult> Assign(int id, AssignTicketDto dto)
        {
            var ticket = await _db.Tickets.FindAsync(id);
            if (ticket == null) return NotFound();

            var target = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == dto.UserId);
            if (target == null || target.Role.Name == "USER")
            {
                return BadRequest("Cannot assign tickets to common users.");
            }

            ticket.AssignedTo = dto.UserId;
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "MANAGER,SUPPORT")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateStatusDto dto)
        {
            var ticket = await _db.Tickets.FindAsync(id);
            if (ticket == null) return NotFound();

            var oldStatus = ticket.Status ?? "OPEN";
            var newStatus = dto.Status.ToUpper();

            if (oldStatus == "OPEN" && newStatus == "IN_PROGRESS" ||
                oldStatus == "IN_PROGRESS" && newStatus == "RESOLVED" ||
                oldStatus == "RESOLVED" && newStatus == "CLOSED")
            {
                ticket.Status = newStatus;

                _db.TicketStatusLogs.Add(new TicketStatusLog
                {
                    TicketId = ticket.Id,
                    OldStatus = oldStatus,
                    NewStatus = newStatus,
                    ChangedBy = GetUserId()
                });

                await _db.SaveChangesAsync();
                return NoContent();
            }

            return BadRequest("Invalid status transition.");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "MANAGER")]
        public async Task<IActionResult> Delete(int id)
        {
            var ticket = await _db.Tickets.FindAsync(id);
            if (ticket == null) return NotFound();

            _db.Tickets.Remove(ticket);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        }
    }

