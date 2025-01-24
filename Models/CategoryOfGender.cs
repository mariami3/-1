using System;
using System.Collections.Generic;

namespace proctos.Models;

public partial class CategoryOfGender
{
    public int IdCategoryOfGender { get; set; }

    public string NameCategoryOfGender { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
