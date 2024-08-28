using System.ComponentModel.DataAnnotations;

namespace TuckshopOrdering.Models
{
    public class FoodOrder
    {
        public int FoodOrderID { get; set; } // primary key
        public int MenuID { get; set; } // foreign key (1 menu item can belong to many food orders)
        public int OrderID { get; set; } // foreign key (1 order ID can belong to many food orders)
        [Range(1, 39)] // the range of quantity for each menu item (the user must have at least a quantity of 1 and a max of 39)
        public int quantity { get; set; } // quantity property 
        public Menu Menu { get; set; } // 1 menu item can belong to many food orders 
        public Order Order { get; set; } // many food orders can belong to 1 order

    }
}
