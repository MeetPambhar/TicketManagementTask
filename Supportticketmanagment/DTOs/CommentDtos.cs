using System.ComponentModel.DataAnnotations;

namespace Supportticketmanagment.DTOs
{
    public class CreateCommentDto
    {
        [Required]
        public string Comment { get; set; } = null!;
    }

    public class CommentResponseDto
    {
        public int _id { get; set; }
        public int ticketId { get; set; }
        public int userId { get; set; }
        public string userName { get; set; } = null!;
        public string comment { get; set; } = null!;
        public DateTime? createdAt { get; set; }
    }
}
