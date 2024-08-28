using System.ComponentModel.DataAnnotations;

namespace TuckshopOrdering.Models
{
    public class Order
    {
        public int OrderID { get; set; } // primary key 
        public DateTime OrderDate { get; set; } // order date property (the date that the order is made)
        public DateTime PickupDate { get; set; } // pickup date property (the date that the order is being collected on)
        [StringLength(50)] // character limit of 50 for the student name property 
        public string studentName { get; set; } // student name property 
        [Range(1, 34)] // as the classes at the school only range from rm1 - rm34, the limit is in place so that the user does not enter a classroom that does not exist
        public float roomNumber { get; set; } // room number property 
        public string Status { get; set; } = "Pending"; // default status is "pending". Status property represents the status of the orders completion 
        [EmailAddress] // property type
        public string? email { get; set; } // email property (not required)
        public string orderComplete { get; set; } // order complete property represents whether or not the order has been made and the customer has collected it
        [StringLength(100)] // character limit of 100 for the note property 
        public string? note { get; set; } // note property for orders w
        public ICollection<FoodOrder> FoodOrders { get; set; } // 1 order can contain many food orders
    }
}
