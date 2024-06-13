namespace TuckshopOrdering.Models
{
    public class FoodOrder
    {
        public int FoodOrderID { get; set; }
        public int MenuID { get; set; }
        public int OrderID { get; set; }
        public int quantity { get; set; }
        public string studentName { get; set; }
        public int roomNumber { get; set; }
        public Menu Menu { get; set; }
        public Order Order { get; set; } // many food orders can belong to 1 order

    }
}
