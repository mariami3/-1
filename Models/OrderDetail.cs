using System;
using System.Collections.Generic;

namespace proctos.Models;

public partial class OrderDetail
{
    public int IdOrderDetail { get; set; }

    public int? ProductId { get; set; }

    public int? OrderId { get; set; }

    public int CountOfProduct { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }
}
