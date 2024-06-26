﻿using System.ComponentModel.DataAnnotations;

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
        public string? email { get; set; }
        public ICollection<FoodOrder> FoodOrders { get; set; } // 1 order can contain many food orders

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(PickupDate.Date < DateTime.Now) 
            {
                yield return new ValidationResult("You cannot set the collection date in the past.", new[] { "PickupDate" });
            }
        }
    }
}
