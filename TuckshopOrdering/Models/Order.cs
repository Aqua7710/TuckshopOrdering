namespace TuckshopOrdering.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime PickupDate { get; set; }
        public string studentName { get; set; }
        public float roomNumber { get; set; }
        public string Status { get; set; } = "Pending"; // default status
        public ICollection<FoodOrder> FoodOrders { get; set; } // 1 order can contain many food orders
    }
}
