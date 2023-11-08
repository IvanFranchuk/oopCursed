using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oopCursed.code1
{
    class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal PricePerUnit { get; set; }
        public string Type { get; set; }
        public int Quantity { get; set; }
        public DateTime ManufactureDate { get; set; }
        public int ShelfLife { get; set; }


        public Product() { }

        public Product(string name, string type, decimal pricePerUnit, int quantity, DateTime manufactureDate, int shelfLife)
        {
            Name = name;
            Type = type;
            Quantity = quantity;
            ManufactureDate = manufactureDate;
            ShelfLife = shelfLife;
            PricePerUnit = pricePerUnit;
        }

        public Product(Product other)
        {
            Name = other.Name;
            Type = other.Type;
            Quantity = other.Quantity;
            ManufactureDate = other.ManufactureDate;
            ShelfLife = other.ShelfLife;
            PricePerUnit = other.PricePerUnit;
        }

        public override string ToString()
        {
            return $"{Name}, Type: {Type}, Quantity: {Quantity}, Manufacture Date: {ManufactureDate.ToShortDateString()}, Shelf Life: {ShelfLife} days, Price per Unit: ${PricePerUnit}";
        }

  
    }
}
