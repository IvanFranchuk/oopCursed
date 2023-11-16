using System.Collections.Generic;

namespace oopCursed.DB;

public partial class Warehouse
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Location { get; set; }

    public int? Userid { get; set; }

    public virtual User? User { get; set; }
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
