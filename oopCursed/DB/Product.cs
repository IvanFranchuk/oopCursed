using System;

namespace oopCursed.DB;

public partial class Product
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? Price { get; set; }

    public string? Type { get; set; }

    public int? Quantity { get; set; }

    public DateTime? ManufactureDate { get; set; }

    public DateTime? ShelfLife { get; set; }

    public int? WarehouseId { get; set; }

    public string? Character { get; set; }

    public virtual Warehouse? Warehouse { get; set; }

    public Product() { }
    public Product(int id, string name, int pricePerUnit, DateTime manufactureDate, string type, int quantity, DateTime? shelfLife, String character)
    {
        Id = id;
        Name = name;
        Price = pricePerUnit;
        ManufactureDate = manufactureDate;
        Type = type;
        Quantity = quantity;
        ShelfLife = shelfLife;
        Character = character;
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
        Character = other.Character;
    }

    public override string ToString()
    {
        return $"{Name}, Type: {Type}, Quantity: {Quantity}, Manufacture Date: {ManufactureDate.ToString()}, Shelf Life: {ShelfLife.ToString()} days, Price per Unit: ${Price}";
    }
}
