using System;
using System.Collections.Generic;

namespace proctos.Models;

public partial class Order
{
    public int IdOrder { get; set; }

    public int? ProductId { get; set; }

    public int? UsersId { get; set; }

    public decimal SumOfOrder { get; set; }

    public DateOnly DateOfOrder { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Product? Product { get; set; }

    public virtual User? Users { get; set; }
}
