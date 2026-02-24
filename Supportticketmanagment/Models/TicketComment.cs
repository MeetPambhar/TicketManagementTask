using System;
using System.Collections.Generic;

namespace Supportticketmanagment.Models;

public partial class TicketComment
{
    public int Id { get; set; }

    public int? TicketId { get; set; }

    public int? UserId { get; set; }

    public string Comment { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual Ticket? Ticket { get; set; }

    public virtual User? User { get; set; }
}
