using System.ComponentModel.DataAnnotations;

namespace Supportticketmanagment.DTOs
{
    public class CreateTicketDto
    {
        [Required]
        [MinLength(5)]
        public string Title { get; set; } = null!;

        [Required]
        [MinLength(10)]
        public string Description { get; set; } = null!;

        [Required]
        public string Priority { get; set; } = null!; // LOW, MEDIUM, HIGH
    }

    public class UpdateStatusDto
    {
        [Required]
        public string Status { get; set; } = null!; // OPEN, IN_PROGRESS, RESOLVED, CLOSED
    }

    public class AssignTicketDto
    {
        [Required]
        public int UserId { get; set; }
    }

    public class TicketResponseDto
    {
        public int _id { get; set; }
        public string title { get; set; } = null!;
        public string description { get; set; } = null!;
        public string status { get; set; } = null!;
        public string priority { get; set; } = null!;

        public UserMiniDto createdBy { get; set; } = null!;
        public UserMiniDto? assignedTo { get; set; }

        public List<CommentMiniDto> comments { get; set; } = new();
        public List<StatusLogMiniDto> statusLogs { get; set; } = new();

        public DateTime? createdAt { get; set; }
    }

    public class UserMiniDto
    {
        public string name { get; set; } = null!;
        public string role { get; set; } = null!;
    }

    public class CommentMiniDto
    {
        public string authorName { get; set; } = null!;
        public string authorRole { get; set; } = null!;
        public string comment { get; set; } = null!;
        public DateTime? createdAt { get; set; }
    }

    public class StatusLogMiniDto
    {
        public string oldStatus { get; set; } = null!;
        public string newStatus { get; set; } = null!;
        public UserMiniDto changedBy { get; set; } = null!;
        public DateTime? changedAt { get; set; }
    }
}
