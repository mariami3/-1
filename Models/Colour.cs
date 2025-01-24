using System;
using System.Collections.Generic;

namespace proctos.Models;

public partial class Colour
{
    public int IdColour { get; set; }

    public string ColourName { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
