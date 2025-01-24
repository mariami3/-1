using System;
using System.Collections.Generic;

namespace proctos.Models;

public partial class CategoryOfProduct
{
    public int IdCategoryOfProduct { get; set; }

    public string NameCategoryOfProduct { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
