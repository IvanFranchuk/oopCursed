using System;
using System.Collections.Generic;
using System.Drawing;

namespace oopCursed.DB;

public partial class Product
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public float? Price { get; set; }
    public string? Type { get; set; }
    public int? Quantity { get; set; }
    public string? Charecter { get; set; }
    //public System.Windows.Media.Brush MarkColor { get; set; }

    public DateTime? ManufactureDate { get; set; }
    public int? ShelfLife { get; set; }
    public int? WarehouseId { get; set; } 
    public virtual Warehouse? Warehouse { get; set; }

    public Product() { }
   public Product(int id, string name, float pricePerUnit, DateTime manufactureDate, string type,  int quantity,  int shelfLife, String charecter)
    {
        Id = id;
        Name = name;
        Price = pricePerUnit;
        ManufactureDate = manufactureDate;
        Type = type;
        Quantity = quantity;
        ShelfLife = shelfLife;
        Charecter = charecter;
    }

    public Product(Product other)
    {
        Id = other.Id;
        Name = other.Name;
        Price = other.Price;
        ManufactureDate = other.ManufactureDate;
        Type = other.Type;
        Quantity = other.Quantity;
        ShelfLife = other.ShelfLife;
        Charecter = other.Charecter;
    }

    public override string ToString()
    {
        return $"{Name}, Type: {Type}, Quantity: {Quantity}, Manufacture Date: {ManufactureDate.ToString()}, Shelf Life: {ShelfLife} days, Price per Unit: ${Price}";
    }
}
