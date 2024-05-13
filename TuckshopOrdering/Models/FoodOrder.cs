namespace TuckshopOrdering.Models
{
    public class FoodOrder
    {
        public int FoodOrderID { get; set; }
        public int MenuID { get; set; }
        public int quantity { get; set; }
        public int CustomerID { get; set; }

        public Menu Menu { get; set; }
        public Customer Customer { get; set; }
    }
}
