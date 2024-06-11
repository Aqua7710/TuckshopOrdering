namespace TuckshopOrdering.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime PickupDate { get; set; }
        public ICollection<FoodOrder> FoodOrders { get; set; } // 1 order can contain many food orders
    }
}
