using System.ComponentModel.DataAnnotations;

namespace Supportticketmanagment.DTOs
{
    public class CreateUserDto
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = null!;

        [Required]
        public string Role { get; set; } = null!; // MANAGER, SUPPORT, USER
    }

    public class UserResponseDto
    {
        public int _id { get; set; }
        public string name { get; set; } = null!;
        public string email { get; set; } = null!;
        public string role { get; set; } = null!;
        public DateTime? createdAt { get; set; }
    }
}
