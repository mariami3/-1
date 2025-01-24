using System;
using System.Collections.Generic;

namespace proctos.Models;

public partial class Product
{
    public int IdProduct { get; set; }

    public string NameProduct { get; set; } = null!;

    public decimal Price { get; set; }

    public string? ImageOfProduct { get; set; }

    public int? CategoryOfProductId { get; set; }

    public int? CategoryOfGenderId { get; set; }

    public int? SizeId { get; set; }

    public int? UsersId { get; set; }

    public int? ColourId { get; set; }

    public int? MaterialId { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual CategoryOfGender? CategoryOfGender { get; set; }

    public virtual CategoryOfProduct? CategoryOfProduct { get; set; }

    public virtual Colour? Colour { get; set; }

    public virtual Material? Material { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual Size? Size { get; set; }

    public virtual User? Users { get; set; }
}
