using System;
using System.Collections.Generic;

namespace proctos.Models;

public partial class Material
{
    public int IdMaterial { get; set; }

    public string Material1 { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
