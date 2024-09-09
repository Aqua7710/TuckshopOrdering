using System.ComponentModel.DataAnnotations;

namespace TuckshopOrdering.Models
{
    public class Order
    {
        public int OrderID { get; set; } // primary key 
        public DateTime OrderDate { get; set; } // order date property (the date that the order is made)
        public DateTime PickupDate { get; set; } // pickup date property (the date that the order is being collected on)
        [StringLength(maximumLength: 35, MinimumLength = 1)] // maximum characters for student name is 35 and minimum characters is 1
        public string studentName { get; set; } // student name property 
        [Range(1, 36)] // as the classes at the school only range from rm1 - rm36, the limit is in place so that the user does not enter a classroom that does not exist
        public float roomNumber { get; set; } // room number property 
        public string Status { get; set; } = "Pending"; // default status is "pending". Status property represents the status of the orders completion 
        [EmailAddress] // property type
        public string? email { get; set; } // email property (not required)
        public string orderComplete { get; set; } // order complete property represents whether or not the order has been made and the customer has collected it
        [StringLength(100)] // character limit of 100 for the note property 
        public string? note { get; set; } // note property (not required)
        public ICollection<FoodOrder> FoodOrders { get; set; } // 1 order can contain many food orders
    }
}
