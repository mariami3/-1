using System;
using System.Collections.Generic;

namespace proctos.Models;

public partial class Size
{
    public int IdSize { get; set; }

    public int Size1 { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
