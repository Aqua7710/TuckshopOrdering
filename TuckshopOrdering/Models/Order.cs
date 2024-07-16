using System.ComponentModel.DataAnnotations;

namespace TuckshopOrdering.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime PickupDate { get; set; }
        [StringLength(50)]
        public string studentName { get; set; }
        [Range(1, 34)]
        public float roomNumber { get; set; }
        public string Status { get; set; } = "Pending"; // default status
        [EmailAddress]
        public string? email { get; set; }
        public string orderComplete { get; set; }
        [StringLength(100)]
        public string? note { get; set; }
        public ICollection<FoodOrder> FoodOrders { get; set; } // 1 order can contain many food orders
    }
}
