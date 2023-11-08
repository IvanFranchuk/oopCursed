using oopCursed.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oopCursed.code
{
    class Warehouse
    {
        public int WarehouseId { get; set; }
        public string Location { get; set; }
        public List<Product> ProductsInWarehouse { get; set; }
    }
}
