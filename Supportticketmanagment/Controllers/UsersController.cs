using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Supportticketmanagment.Models;
using Supportticketmanagment.DTOs;

namespace Supportticketmanagment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "MANAGER")]
    public class UsersController : ControllerBase
    {
        private readonly SupportTicketContext _db;

        public UsersController(SupportTicketContext context)
        {
            _db = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto dto)
        {
            if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
            {
                return BadRequest("Email already exists.");
            }

            var role = await _db.Roles.FirstOrDefaultAsync(r => r.Name == dto.Role.ToUpper());
            if (role == null) return BadRequest("Invalid role.");

            var newUser = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RoleId = role.Id
            };

            _db.Users.Add(newUser);
            await _db.SaveChangesAsync();

            return Ok(new UserResponseDto
            {
                _id = newUser.Id,
                name = newUser.Name,
                email = newUser.Email,
                role = role.Name,
                createdAt = newUser.CreatedAt
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _db.Users
                .Include(u => u.Role)
                .Select(u => new UserResponseDto
                {
                    _id = u.Id,
                    name = u.Name,
                    email = u.Email,
                    role = u.Role.Name,
                    createdAt = u.CreatedAt
                })
                .ToListAsync();

            return Ok(users);
        }
    }
}
