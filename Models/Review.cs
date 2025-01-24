using System;
using System.Collections.Generic;

namespace proctos.Models;

public partial class Review
{
    public int IdReview { get; set; }

    public string UserName { get; set; } = null!;

    public int Rating { get; set; }

    public string Comment { get; set; } = null!;

    public DateTime? DateCreated { get; set; }

    public int? ProductId { get; set; }

    public virtual Product? Product { get; set; }
}
